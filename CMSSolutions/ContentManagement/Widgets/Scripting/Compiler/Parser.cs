﻿using System.Collections.Generic;
using CMSSolutions.ContentManagement.Widgets.Scripting.Ast;

namespace CMSSolutions.ContentManagement.Widgets.Scripting.Compiler
{
    public class Parser
    {
        private readonly string expression;
        private readonly Lexer lexer;
        private bool parsingMethodCall;

        public Parser(string expression)
        {
            this.expression = expression;
            lexer = new Lexer(new Tokenizer(this.expression));
        }

        public AbstractSyntaxTree Parse()
        {
            var node = ParseExpression();
            if (lexer.Token().Kind != TokenKind.Eof)
            {
                node = UnexpectedTokenError();
            }
            return new AbstractSyntaxTree { Root = node };
        }

        private AstNode ParseExpression()
        {
            return ParseKeywordLogicalExpression();
        }

        private AstNode ParseKeywordLogicalExpression()
        {
            var expr = ParseKeywordNotExpression();

        again:
            var token = IsMatch(TokenKind.Or, TokenKind.And);
            if (token != null)
            {
                var right = ParseKeywordNotExpression();

                expr = new BinaryAstNode(expr, token, right);
                goto again;
            }

            return expr;
        }

        private AstNode ParseKeywordNotExpression()
        {
            var token = IsMatch(TokenKind.Not);
            if (token != null)
            {
                var expr = ParseKeywordNotExpression();

                return new UnaryAstNode(token, expr);
            }

            return ParseLogicalOrExpression();
        }

        private AstNode ParseLogicalOrExpression()
        {
            var expr = ParseLogicalAndExpression();

        again:
            var token = IsMatch(TokenKind.OrSign);
            if (token != null)
            {
                var right = ParseLogicalAndExpression();

                expr = new BinaryAstNode(expr, token, right);
                goto again;
            }

            return expr;
        }

        private AstNode ParseLogicalAndExpression()
        {
            var expr = ParseEqualityExpression();

        again:
            var token = IsMatch(TokenKind.AndSign);
            if (token != null)
            {
                var right = ParseEqualityExpression();

                expr = new BinaryAstNode(expr, token, right);
                goto again;
            }

            return expr;
        }

        private AstNode ParseEqualityExpression()
        {
            var expr = ParseComparisonExpression();

            var token = IsMatch(TokenKind.EqualEqual, TokenKind.NotEqual);
            if (token != null)
            {
                var right = ParseEqualityExpression();

                expr = new BinaryAstNode(expr, token, right);
            }

            return expr;
        }

        private AstNode ParseComparisonExpression()
        {
            var expr = ParseAdditiveExpression();

            var token =
                IsMatch(TokenKind.LessThan, TokenKind.LessThanEqual) ??
                IsMatch(TokenKind.GreaterThan, TokenKind.GreaterThanEqual);
            if (token != null)
            {
                var right = ParseComparisonExpression();

                expr = new BinaryAstNode(expr, token, right);
            }

            return expr;
        }

        private AstNode ParseAdditiveExpression()
        {
            var expr = ParseMultiplicativeExpression();

        again:
            var token = IsMatch(TokenKind.Plus, TokenKind.Minus);
            if (token != null)
            {
                var right = ParseMultiplicativeExpression();

                expr = new BinaryAstNode(expr, token, right);
                goto again;
            }

            return expr;
        }

        private AstNode ParseMultiplicativeExpression()
        {
            var expr = ParseUnaryExpression();

        again:
            var token = IsMatch(TokenKind.Mul, TokenKind.Div);
            if (token != null)
            {
                var right = ParseUnaryExpression();

                expr = new BinaryAstNode(expr, token, right);
                goto again;
            }

            return expr;
        }

        private AstNode ParseUnaryExpression()
        {
            var token = IsMatch(TokenKind.NotSign);
            if (token != null)
            {
                var expr = ParseUnaryExpression();

                return new UnaryAstNode(token, expr);
            }

            return ParsePrimaryExpression();
        }

        private AstNode ParsePrimaryExpression()
        {
            var token = lexer.Token();
            switch (lexer.Token().Kind)
            {
                case TokenKind.NullLiteral:
                case TokenKind.True:
                case TokenKind.False:
                case TokenKind.SingleQuotedStringLiteral:
                case TokenKind.StringLiteral:
                case TokenKind.Integer:
                    return ProduceConstant(token);

                case TokenKind.OpenParen:
                    return ParseParenthesizedExpression();

                case TokenKind.Identifier:
                    return ParseMethodCallExpression();

                default:
                    return UnexpectedTokenError();
            }
        }

        private AstNode ParseIndentifier(Token identifier)
        {
            return new MethodCallAstNode(identifier, new List<AstNode>());
        }

        private AstNode ParseParenthesizedExpression()
        {
            // '('
            lexer.NextToken();

            var expr = ParseExpression();

            // ')'
            if (IsMatch(TokenKind.CloseParen) == null)
            {
                return ExpectedTokenError(TokenKind.CloseParen);
            }

            return expr;
        }

        private AstNode ParseMethodCallExpression()
        {
            var target = lexer.Token();
            lexer.NextToken();

            bool isParenthesizedCall = (IsMatch(TokenKind.OpenParen) != null);

            // This is to avoid parsing method calls within method calls that have no
            // parenthesis (language ambiguity)
            if (!isParenthesizedCall && parsingMethodCall)
            {
                return ParseIndentifier(target);
            }

            // Detect tokens that can't be a function argument start token
            if (!IsValidMethodArgumentToken(isParenthesizedCall))
            {
                return ParseIndentifier(target);
            }

            parsingMethodCall = true;
            try
            {
                var arguments = new List<AstNode>();
                while (true)
                {
                    // Special case: we might reach the end of the token stream
                    if (lexer.Token().Kind == TokenKind.Eof)
                        break;

                    // Special case: we must support "foo()"
                    if (isParenthesizedCall && lexer.Token().Kind == TokenKind.CloseParen)
                        break;

                    // Special case: for non parenthized calls, some tokens mark the end of the call
                    if (!isParenthesizedCall)
                    {
                        bool endOfMethodCall = false;
                        switch (lexer.Token().Kind)
                        {
                            case TokenKind.And:
                            case TokenKind.Or:
                            case TokenKind.Not:
                                endOfMethodCall = true;
                                break;
                        }
                        if (endOfMethodCall)
                            break;
                    }

                    var argument = ParseExpression();
                    arguments.Add(argument);

                    if (IsMatch(TokenKind.Comma) == null)
                        break;
                }

                if (isParenthesizedCall)
                {
                    // ')'
                    if (IsMatch(TokenKind.CloseParen) == null)
                    {
                        return ExpectedTokenError(TokenKind.CloseParen);
                    }
                }

                return new MethodCallAstNode(target, arguments);
            }
            finally
            {
                parsingMethodCall = false;
            }
        }

        private bool IsValidMethodArgumentToken(bool isParenthesizedCall)
        {
            switch (lexer.Token().Kind)
            {
                case TokenKind.OpenParen:
                case TokenKind.StringLiteral:
                case TokenKind.SingleQuotedStringLiteral:
                case TokenKind.Identifier:
                case TokenKind.Integer:
                case TokenKind.NotSign:
                case TokenKind.NullLiteral:
                case TokenKind.Minus:
                case TokenKind.Plus:
                case TokenKind.True:
                case TokenKind.False:
                    return true;

                case TokenKind.CloseParen:
                    return isParenthesizedCall;

                default:
                    return false;
            }
        }

        private AstNode ProduceConstant(Token token)
        {
            lexer.NextToken();
            return new ConstantAstNode(token);
        }

        private AstNode UnexpectedTokenError()
        {
            var token = lexer.Token();
            lexer.NextToken();
            return new ErrorAstNode(token, string.Format("Unexpected token in primary expression ({0})", token));
        }

        private AstNode ExpectedTokenError(TokenKind tokenKind)
        {
            var token = lexer.Token();
            return new ErrorAstNode(token, string.Format("Expected token {0}", tokenKind));
        }

        private Token IsMatch(TokenKind kind)
        {
            var token = lexer.Token();
            if (token.Kind == kind)
            {
                lexer.NextToken();
                return token;
            }
            return null;
        }

        private Token IsMatch(TokenKind kind, TokenKind kind2)
        {
            var token = lexer.Token();
            if (token.Kind == kind || token.Kind == kind2)
            {
                lexer.NextToken();
                return token;
            }
            return null;
        }
    }
}