﻿using System.Collections.Generic;
using CMSSolutions.ContentManagement.Widgets.Scripting.Compiler;

namespace CMSSolutions.ContentManagement.Widgets.Scripting.Ast
{
    public class MethodCallAstNode : AstNode, IAstNodeWithToken
    {
        private readonly Token token;
        private readonly IList<AstNode> arguments;

        public MethodCallAstNode(Token token, IList<AstNode> arguments)
        {
            this.token = token;
            this.arguments = arguments;
        }

        public Token Target { get { return token; } }

        public IList<AstNode> Arguments { get { return arguments; } }

        public Token Token { get { return token; } }

        public override IEnumerable<AstNode> Children
        {
            get { return arguments; }
        }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.VisitMethodCall(this);
        }
    }
}