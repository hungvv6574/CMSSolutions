using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CMSSolutions.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.ControlForms;
using CMSSolutions.GTools.Common.Configuration;
using CMSSolutions.GTools.Common.Data;
using CMSSolutions.GTools.Common.Models;
using CMSSolutions.Reflection;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.GTools.Common
{
    [Export(typeof(IGeneratorPlugin))]
    public class MvcCornerstoneGenerator : IGeneratorPlugin
    {
        private readonly CMSSolutionsSettingsControl settingsControl;
        private readonly SystemTypeConverter systemTypeConverter = new SystemTypeConverter();

        public MvcCornerstoneGenerator()
        {
            settingsControl = new CMSSolutionsSettingsControl();
        }

        #region IGeneratorPlugin Members

        public string Name
        {
            get { return "CMSSolutions"; }
        }

        public IGeneratorSettings Settings
        {
            get { return settingsControl; }
        }

        public void Run(BaseProvider dataProvider, ConfigFile config)
        {
            var index = 0;
            foreach (string tableName in config.SelectedTables)
            {
                if (settingsControl.GenerateDomainEntities)
                {
                    GenerateDomainEntity(tableName, dataProvider);
                }

                if (settingsControl.GenerateViewModels)
                {
                    GenerateViewModel(tableName, dataProvider);
                }

                if (settingsControl.GenerateServices)
                {
                    GenerateService(tableName, dataProvider);
                }

                if (settingsControl.GenerateControllers)
                {
                    GenerateController(tableName, dataProvider);
                }

                GeneratePermission(tableName);

                GenerateMenus(tableName, index);
                index++;
            }
        }

        #endregion

        private string GetTableName(string tableName)
        {
            var baseTableName = tableName;
            var index = baseTableName.LastIndexOf('_');
            if (index > -1)
            {
                index += 1;
                baseTableName = baseTableName.Substring(index, baseTableName.Length - index);
            }

            return baseTableName;
        }

        private void GenerateController(string tableName, BaseProvider dataProvider)
        {
            string singularizedName = GetTableName(tableName);
            string pluralizedName = singularizedName.Pluralize();
            var buildUrl = pluralizedName.ToSlugUrl();
            buildUrl = "admin/" + buildUrl;

            var @namespace = new CodeNamespace(settingsControl.ProjectName + ".Controllers");
            @namespace.Imports.Add(new CodeNamespaceImport("System"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.Globalization"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.Web.Mvc"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.Mvc"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.Themes"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.UI.ControlForms"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Entities"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Models"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Services"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.UI.Navigation"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.Routing"));

            var fields = dataProvider.GetFields(tableName);

            string className = singularizedName + "Controller";
            var @class = new CodeTypeDeclaration(className) { IsClass = true, Attributes = MemberAttributes.Public };
            @class.BaseTypes.Add(new CodeTypeReference
            {
                BaseType = string.Format("BaseController")
            });
            @class.CustomAttributes.Add(new CodeAttributeDeclaration("Authorize"));
            @class.CustomAttributes.Add(new CodeAttributeDeclaration("Themed", new CodeAttributeArgument { Name = "IsDashboard", Value = new CodePrimitiveExpression(true) }));
            @namespace.Types.Add(@class);

            // Fields
            var service = new CodeMemberField(string.Format("readonly I{0}Service", singularizedName), "service")
            {
                Attributes = MemberAttributes.Private
            };
            @class.Members.Add(service);

            // Constructor
            var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
            constructor.Parameters.Add(new CodeParameterDeclarationExpression("IWorkContextAccessor", "workContextAccessor"));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(string.Format("I{0}Service", singularizedName), "service"));
            constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("workContextAccessor"));
            constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "service"), new CodeArgumentReferenceExpression("service")));
            constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "TableName"), new CodePrimitiveExpression("tbl" + singularizedName)));
            @class.Members.Add(constructor);

            #region Index

            var indexMethod = new CodeMemberMethod
            {
                Name = "Index",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference("ActionResult")
            };

            indexMethod.CustomAttributes.Add(new CodeAttributeDeclaration("Url", new CodeAttributeArgument(new CodePrimitiveExpression(buildUrl))));

            @class.Members.Add(indexMethod);

            if (settingsControl.GenerateBreadcrumbs)
            {
                indexMethod.Statements.Add(GenerateIndexActionBreadcrumbs(singularizedName));
            }

            var indexMethodResult = new CodeVariableDeclarationStatement("var", "result",
                new CodeObjectCreateExpression(new CodeTypeReference
                {
                    BaseType = string.Format("ControlGridFormResult`1[{0}]", singularizedName),
                    Options = CodeTypeReferenceOptions.GenericTypeParameter
                }));
            indexMethod.Statements.Add(indexMethodResult);
            indexMethod.Statements.Add(new CodeVariableDeclarationStatement("var", "siteSettings", new CodeSnippetExpression("WorkContext.Resolve<SiteSettings>()")));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.Title"), new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Management " + singularizedName) })));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.CssClass"), new CodePrimitiveExpression("table table-bordered table-striped")));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.UpdateActionName"), new CodePrimitiveExpression("Update")));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.IsAjaxSupported"), new CodePrimitiveExpression(true)));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.DefaultPageSize"), new CodeSnippetExpression("siteSettings.DefaultPageSize")));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.EnablePaginate"), new CodePrimitiveExpression(true)));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.FetchAjaxSource"), new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Get" + tableName.Pluralize())));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.GridWrapperStartHtml"), new CodeSnippetExpression("Constants.Grid.GridWrapperStartHtml")));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.GridWrapperEndHtml"), new CodeSnippetExpression("Constants.Grid.GridWrapperEndHtml")));
            indexMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.ClientId"), new CodeSnippetExpression("TableName")));

            foreach (var field in fields)
            {
                CodeExpression addColumn = new CodeMethodInvokeExpression(
                    new CodeVariableReferenceExpression("result"), "AddColumn", new CodeExpression[] { new CodeSnippetExpression("x => x." + field.Name) });
                indexMethod.Statements.Add(addColumn);
            }

            CodeExpression addAction = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddAction");
            addAction = new CodeMethodInvokeExpression(addAction, "HasText", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Create") }) });
            addAction = new CodeMethodInvokeExpression(addAction, "HasUrl", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Url.Action", new CodeExpression[] { new CodePrimitiveExpression("Edit"), new CodeSnippetExpression("new { id = 0 }") }) });
            addAction = new CodeMethodInvokeExpression(addAction, "HasButtonStyle", new CodeExpression[] { new CodeSnippetExpression("ButtonStyle.Primary") });
            addAction = new CodeMethodInvokeExpression(addAction, "HasBoxButton", new CodeExpression[] { new CodeSnippetExpression("false") });
            addAction = new CodeMethodInvokeExpression(addAction, "HasCssClass", new CodeExpression[] { new CodeSnippetExpression("Constants.RowLeft") });
            addAction = new CodeMethodInvokeExpression(addAction, "HasRow", new CodeExpression[] { new CodeSnippetExpression("true") });
            indexMethod.Statements.Add(addAction);

            CodeExpression addEditRowAction = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddRowAction");
            addEditRowAction = new CodeMethodInvokeExpression(addEditRowAction, "HasText", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Edit") }) });
            addEditRowAction = new CodeMethodInvokeExpression(addEditRowAction, "HasUrl", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("x => Url"), "Action", new CodeExpression[] { new CodePrimitiveExpression("Edit"), new CodeSnippetExpression("new { id = x.Id }") }) });
            addEditRowAction = new CodeMethodInvokeExpression(addEditRowAction, "HasButtonStyle", new CodeExpression[] { new CodeSnippetExpression("ButtonStyle.Default") });
            addEditRowAction = new CodeMethodInvokeExpression(addEditRowAction, "HasButtonSize", new CodeExpression[] { new CodeSnippetExpression("ButtonSize.ExtraSmall") });
            indexMethod.Statements.Add(addEditRowAction);

            CodeExpression addDeleteRowAction = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddRowAction", new CodePrimitiveExpression(true));
            addDeleteRowAction = new CodeMethodInvokeExpression(addDeleteRowAction, "HasText", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Delete") }) });
            addDeleteRowAction = new CodeMethodInvokeExpression(addDeleteRowAction, "HasName", new CodeExpression[] { new CodePrimitiveExpression("Delete") });
            addDeleteRowAction = new CodeMethodInvokeExpression(addDeleteRowAction, "HasValue", new CodeExpression[] { new CodeSnippetExpression("x => Convert.ToString(x.Id)") });
            addDeleteRowAction = new CodeMethodInvokeExpression(addDeleteRowAction, "HasButtonStyle", new CodeExpression[] { new CodeSnippetExpression("ButtonStyle.Danger") });
            addDeleteRowAction = new CodeMethodInvokeExpression(addDeleteRowAction, "HasButtonSize", new CodeExpression[] { new CodeSnippetExpression("ButtonSize.ExtraSmall") });
            addDeleteRowAction = new CodeMethodInvokeExpression(addDeleteRowAction, "HasConfirmMessage", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeSnippetExpression("Constants.Messages.ConfirmDeleteRecord")) });
            indexMethod.Statements.Add(addDeleteRowAction);

            indexMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddReloadEvent", new CodeExpression[] { new CodePrimitiveExpression("UPDATE_ENTITY_COMPLETE") }));
            indexMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddReloadEvent", new CodeExpression[] { new CodePrimitiveExpression("DELETE_ENTITY_COMPLETE") }));

            indexMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result")));

            #endregion Index

            #region Get Data

            var getDataMethod = new CodeMemberMethod
            {
                Name = "Get" + tableName.Pluralize(),
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                ReturnType = new CodeTypeReference
                {
                    BaseType = string.Format("ControlGridAjaxData`1[{0}]", singularizedName),
                    Options = CodeTypeReferenceOptions.GenericTypeParameter
                }
            };

            getDataMethod.Parameters.Add(new CodeParameterDeclarationExpression("ControlGridFormRequest", "options"));

            getDataMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(int), "totals"));
            getDataMethod.Statements.Add(new CodeVariableDeclarationStatement("var", "items", new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(), "service.GetRecords", new CodeExpression[]
                {
                    new CodeArgumentReferenceExpression("options"),
                    new CodeVariableReferenceExpression("out totals")
                })));
            getDataMethod.Statements.Add(new CodeVariableDeclarationStatement("var", "result", new CodeObjectCreateExpression(new CodeTypeReference
            {
                BaseType = string.Format("ControlGridAjaxData`1[{0}]", singularizedName),
                Options = CodeTypeReferenceOptions.GenericTypeParameter
            }, new CodeExpression[]
            {
                new CodeVariableReferenceExpression("items"),
                new CodeVariableReferenceExpression("totals")
            })));
            getDataMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result")));

            @class.Members.Add(getDataMethod);

            #endregion Get Data

            #region Edit

            var editMethod = new CodeMemberMethod
            {
                Name = "Edit",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference("ActionResult")
            };
            editMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "id"));
            editMethod.CustomAttributes.Add(new CodeAttributeDeclaration(
                "Url",
                new CodeAttributeArgument(new CodePrimitiveExpression(buildUrl + "/edit/{id}"))));

            if (settingsControl.GenerateBreadcrumbs)
            {
                editMethod.Statements.Add(GenerateEditActionBreadcrumbs(singularizedName));
            }

            var editModel = new CodeVariableDeclarationStatement("var", "model",
            new CodeObjectCreateExpression(new CodeTypeReference
            {
                BaseType = singularizedName + "Model",
                Options = CodeTypeReferenceOptions.GenericTypeParameter,
            }));
                    
            editMethod.Statements.Add(editModel);
            editMethod.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("id > 0"), new CodeSnippetStatement(@"				 model = this.service.GetById(id);")));

            var editMethodResult = new CodeVariableDeclarationStatement("var", "result",
            new CodeObjectCreateExpression(new CodeTypeReference
            {
                BaseType = string.Format("ControlFormResult`1[{0}]", singularizedName + "Model"),
                Options = CodeTypeReferenceOptions.GenericTypeParameter,
            }, new CodeVariableReferenceExpression("model")));
            editMethod.Statements.Add(editMethodResult);

            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.Title"), new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Edit " + singularizedName) })));
            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.FormMethod"), new CodeSnippetExpression("FormMethod.Post")));
            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.UpdateActionName"), new CodePrimitiveExpression("Update")));
            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.ShowCancelButton"), new CodeSnippetExpression("false")));
            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.ShowBoxHeader"), new CodeSnippetExpression("false")));
            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.FormWrapperStartHtml"), new CodeSnippetExpression("Constants.Form.FormWrapperStartHtml")));
            editMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("result.FormWrapperEndHtml"), new CodeSnippetExpression("Constants.Form.FormWrapperEndHtml")));

            CodeExpression addActionClear = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddAction");
            addActionClear = new CodeMethodInvokeExpression(addActionClear, "HasText", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Clear") }) });
            addActionClear = new CodeMethodInvokeExpression(addActionClear, "HasUrl", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Url.Action", new CodeExpression[] { new CodePrimitiveExpression("Edit"), new CodeSnippetExpression("RouteData.Values.Merge(new { id = 0 })") }) });
            addActionClear = new CodeMethodInvokeExpression(addActionClear, "HasButtonStyle", new CodeExpression[] { new CodeSnippetExpression("ButtonStyle.Success") });
            editMethod.Statements.Add(addActionClear);

            CodeExpression addActionBack = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("result"), "AddAction");
            addActionBack = new CodeMethodInvokeExpression(addActionBack, "HasText", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "T", new CodeExpression[] { new CodePrimitiveExpression("Back") }) });
            addActionBack = new CodeMethodInvokeExpression(addActionBack, "HasUrl", new CodeExpression[] { new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Url.Action", new CodeExpression[] { new CodePrimitiveExpression("Index") }) });
            addActionBack = new CodeMethodInvokeExpression(addActionBack, "HasButtonStyle", new CodeExpression[] { new CodeSnippetExpression("ButtonStyle.Danger") });
            editMethod.Statements.Add(addActionBack);

            editMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result")));

            @class.Members.Add(editMethod);

            #endregion Edit

            #region Update

            var updateMethod = new CodeMemberMethod
            {
                Name = "Update",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference("ActionResult")
            };

            updateMethod.CustomAttributes.Add(new CodeAttributeDeclaration("HttpPost"));
            updateMethod.CustomAttributes.Add(new CodeAttributeDeclaration("FormButton", new CodeAttributeArgument(new CodePrimitiveExpression("Save"))));
            updateMethod.CustomAttributes.Add(new CodeAttributeDeclaration("ValidateInput", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
            updateMethod.CustomAttributes.Add(new CodeAttributeDeclaration("Url", new CodeAttributeArgument(new CodePrimitiveExpression(buildUrl + "/update"))));
            updateMethod.Parameters.Add(new CodeParameterDeclarationExpression(singularizedName + "Model", "model"));

            updateMethod.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("!ModelState.IsValid"), new CodeSnippetStatement(@"				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));")));

            updateMethod.Statements.Add(new CodeVariableDeclarationStatement(singularizedName, "item"));
            updateMethod.Statements.Add(new CodeConditionStatement(new CodeSnippetExpression("model.Id == 0"),
                new CodeStatement[] //true statements (if)
                {
                    new CodeAssignStatement(new CodeVariableReferenceExpression("item"), new CodeObjectCreateExpression(singularizedName))
                },
                new CodeStatement[] //false statements (else)
                {
                    new CodeAssignStatement(
                        new CodeVariableReferenceExpression("item"),
                        new CodeMethodInvokeExpression(
                            new CodeVariableReferenceExpression("service"), "GetById", new CodeArgumentReferenceExpression("model.Id")))
                }));

            foreach (var field in fields)
            {
                if (field.KeyType == KeyType.PrimaryKey)
                {
                    continue;
                }

                updateMethod.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("item." + field.Name), new CodeVariableReferenceExpression("model." + field.Name)));
            }

            updateMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("service"), "Save", new CodeVariableReferenceExpression("item")));

            var returnStatement = new CodeSnippetStatement(@"			return new AjaxResult().NotifyMessage(""UPDATE_ENTITY_COMPLETE"");");
            updateMethod.Statements.Add(returnStatement);

            @class.Members.Add(updateMethod);

            #endregion Update

            #region Delete

            var deleteMethod = new CodeMemberMethod
            {
                Name = "Delete",
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                ReturnType = new CodeTypeReference("ActionResult")
            };

            deleteMethod.CustomAttributes.Add(new CodeAttributeDeclaration("ActionName", new CodeAttributeArgument(new CodePrimitiveExpression("Update"))));
            deleteMethod.CustomAttributes.Add(new CodeAttributeDeclaration("FormButton", new CodeAttributeArgument(new CodePrimitiveExpression("Delete"))));
            deleteMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "id"));

            var deleteModel = new CodeVariableDeclarationStatement("var", "model", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("service"), "GetById", new CodeArgumentReferenceExpression("id")));
            deleteMethod.Statements.Add(deleteModel);
            deleteMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("service"), "Delete", new CodeVariableReferenceExpression("model")));

            var deleteMethodReturnStatement = new CodeSnippetStatement(@"			return new AjaxResult().NotifyMessage(""DELETE_ENTITY_COMPLETE"");");
            deleteMethod.Statements.Add(deleteMethodReturnStatement);

            @class.Members.Add(deleteMethod);

            #endregion Delete

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(@namespace);
            GenerateCSharpCode(compileUnit, Path.Combine(settingsControl.OutputDirectory, "Controllers\\", className + ".cs"));
        }

        private void GenerateDomainEntity(string tableName, BaseProvider dataProvider)
        {
            var baseTableName = GetTableName(tableName);
            var fields = dataProvider.GetFields(tableName).OrderBy(x => x.Ordinal);
            var primaryKey = fields.FirstOrDefault(x => x.KeyType == KeyType.PrimaryKey);
            Type primaryKeyType;
            if (primaryKey != null)
            {
                primaryKeyType = systemTypeConverter.GetDataProviderFieldType(primaryKey.Type);
            }
            else
            {
                primaryKeyType = typeof(int);
            }

            var @namespace = new CodeNamespace(settingsControl.ProjectName + ".Entities");
            @namespace.Imports.Add(new CodeNamespaceImport("System"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Data"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Data.Entity"));

            if (settingsControl.IncludeSerializableAttributes)
            {
                @namespace.Imports.Add(new CodeNamespaceImport("System.Runtime.Serialization"));
            }

            string className = baseTableName;
            var @class = new CodeTypeDeclaration(className);
            @class.IsClass = true;
            @class.Attributes = MemberAttributes.Public;
            @class.BaseTypes.Add(new CodeTypeReference
            {
                BaseType = string.Format("BaseEntity`1[{0}]", primaryKeyType),
                Options = CodeTypeReferenceOptions.GenericTypeParameter
            });

            if (settingsControl.IncludeSerializableAttributes)
            {
                @class.CustomAttributes.Add(new CodeAttributeDeclaration("DataContract"));
            }

            foreach (var field in fields)
            {
                if (field.KeyType == KeyType.PrimaryKey)
                {
                    continue;
                }
                var type = systemTypeConverter.GetDataProviderFieldType(field.Type);

                if (!field.IsRequired)
                {
                    if (type != typeof(string))
                    {
                        type = type.ToNullable();
                    }
                }

                var property = CreateProperty(type, field.Name);
                if (settingsControl.IncludeSerializableAttributes)
                {
                    property.CustomAttributes.Add(new CodeAttributeDeclaration("DataMember"));
                }
                property.CustomAttributes.Add(new CodeAttributeDeclaration("DisplayName", new CodeAttributeArgument(new CodePrimitiveExpression(field.Name))));
                @class.Members.Add(property);
            }

            //foreach (var foreignKeyField in fields.Where(x => x.KeyType == KeyType.ForeignKey))
            //{
            //    //TODO: How to add navigation properties?

            //    //var type = ??
            //    //var property = CreateProperty(type, field.Name, isVirtual: true);
            //}

            @namespace.Types.Add(@class);

            @class = new CodeTypeDeclaration(className + "Mapping");
            @class.IsClass = true;
            @class.Attributes = MemberAttributes.Public;
            @class.BaseTypes.Add(new CodeTypeReference
            {
                BaseType = string.Format("EntityTypeConfiguration`1[{0}]", className),
                Options = CodeTypeReferenceOptions.GenericTypeParameter
            });
            @class.BaseTypes.Add(new CodeTypeReference
            {
                BaseType = "IEntityTypeConfiguration"
            });

            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;

            constructor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "ToTable", new CodeExpression[] { new CodePrimitiveExpression(tableName) }));

            foreach (var field in fields)
            {
                if (field.KeyType == KeyType.PrimaryKey)
                {
                    CodeExpression statement = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "HasKey", new CodeExpression[] { new CodeSnippetExpression("m => m." + field.Name) });
                    constructor.Statements.Add(statement);
                }
                else
                {
                    CodeExpression statement = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Property", new CodeExpression[] { new CodeSnippetExpression("m => m." + field.Name) });

                    // Check data type and field length to add other
                    if (field.IsRequired)
                    {
                        statement = new CodeMethodInvokeExpression(statement, "IsRequired");
                    }

                    if (field.Type == FieldType.String && field.MaxLength != -1)
                    {
                        statement = new CodeMethodInvokeExpression(statement, "HasMaxLength", new CodeExpression[] { new CodePrimitiveExpression(field.MaxLength) });
                    }

                    constructor.Statements.Add(statement);
                }
            }

            @class.Members.Add(constructor);
            @namespace.Types.Add(@class);

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(@namespace);
            GenerateCSharpCode(compileUnit, Path.Combine(settingsControl.OutputDirectory, "Entities\\", className + ".cs"));
        }

        private void GenerateService(string tableName, BaseProvider dataProvider)
        {
            string singularizedName = GetTableName(tableName);
            var @namespace = new CodeNamespace(settingsControl.ProjectName + ".Services");
            @namespace.Imports.Add(new CodeNamespaceImport("System"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            @namespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Caching"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Entities"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Events"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Services"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Entities"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Data"));

            string className = singularizedName + "Service";
            var fields = dataProvider.GetFields(tableName);

            var primaryKey = fields.First(x => x.KeyType == KeyType.PrimaryKey);
            var primaryKeyType = systemTypeConverter.GetDataProviderFieldType(primaryKey.Type);

            #region Interface

            var @interface = new CodeTypeDeclaration("I" + singularizedName + "Service")
            {
                Attributes = MemberAttributes.Public,
                IsInterface = true
            };

            @interface.BaseTypes.Add(new CodeTypeReference
            {
                BaseType = string.Format(
                    "IGenericService`2[{0}, {1}]",
                    singularizedName,
                    primaryKeyType)
            });

            @interface.BaseTypes.Add("IDependency");

            @namespace.Types.Add(@interface);

            #endregion Interface

            #region Class

            var @class = new CodeTypeDeclaration(singularizedName + "Service")
            {
                Attributes = MemberAttributes.Public,
                IsClass = true
            };

            @class.BaseTypes.Add(new CodeTypeReference
            {
                BaseType = string.Format("GenericService`2[{0}, {1}]",
                    singularizedName,
                    primaryKeyType)
            });

            @class.BaseTypes.Add("I" + singularizedName + "Service");

            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            constructor.Parameters.Add(new CodeParameterDeclarationExpression("IEventBus", "eventBus"));
            constructor.Parameters.Add(new CodeParameterDeclarationExpression
            {
                Type = new CodeTypeReference
                {
                    BaseType = string.Format("IRepository`2[{0}, {1}]", singularizedName, primaryKeyType),
                    Options = CodeTypeReferenceOptions.GenericTypeParameter
                },
                Name = "repository"
            });
            constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("repository"));
            constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("eventBus"));
            @class.Members.Add(constructor);
            @namespace.Types.Add(@class);

            #endregion Class

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(@namespace);
            GenerateCSharpCode(compileUnit, Path.Combine(settingsControl.OutputDirectory, "Services\\", className + ".cs"));
        }

        private void GenerateViewModel(string tableName, BaseProvider dataProvider)
        {
            string baseTableName = GetTableName(tableName);
            var fields = dataProvider.GetFields(tableName).OrderBy(x => x.Ordinal);

            var @namespace = new CodeNamespace(settingsControl.ProjectName + ".Models");
            @namespace.Imports.Add(new CodeNamespaceImport("System"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.UI.ControlForms"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Entities"));

            string className = baseTableName + "Model";
            var @class = new CodeTypeDeclaration(className);
            @class.IsClass = true;
            @class.Attributes = MemberAttributes.Public;

            foreach (var field in fields)
            {
                var type = systemTypeConverter.GetDataProviderFieldType(field.Type);

                if (!field.IsRequired)
                {
                    if (type != typeof(string))
                    {
                        type = type.ToNullable();
                    }
                }

                var property = CreateProperty(type, field.Name);

                if (field.KeyType == KeyType.PrimaryKey)
                {
                    property.CustomAttributes.Add(CreateControlHiddenAttribute(field));
                    @class.Members.Add(property);
                    continue;
                }

                switch (field.Type)
                {
                    case FieldType.Byte:
                    case FieldType.Int16:
                    case FieldType.Int32:
                    case FieldType.Int64:
                    case FieldType.SByte:
                    case FieldType.UInt16:
                    case FieldType.UInt32:
                    case FieldType.UInt64:
                    case FieldType.Currency:
                    case FieldType.Decimal:
                    case FieldType.Double:
                    case FieldType.Single:
                        {
                            property.CustomAttributes.Add(CreateControlNumericAttribute(field));
                        }
                        break;

                    case FieldType.Boolean:
                    case FieldType.Choice:
                    case FieldType.MultiChoice:
                    case FieldType.Lookup:
                    case FieldType.MultiLookup:
                        {
                            property.CustomAttributes.Add(CreateControlChoiceAttribute(field));
                        }; break;

                    case FieldType.Date:
                    case FieldType.DateTime:
                        {
                            property.CustomAttributes.Add(CreateControlDateTimeAttribute(field));
                        }; break;

                    case FieldType.Char:
                    case FieldType.String:
                    case FieldType.RichText:
                        {
                            property.CustomAttributes.Add(CreateControlTextAttribute(field));
                        }; break;
                }

                @class.Members.Add(property);
            }

            @class.Members.Add(CreateImplicitConversionOperatorOverload(baseTableName, fields));
            @namespace.Types.Add(@class);

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(@namespace);
            GenerateCSharpCode(compileUnit, Path.Combine(settingsControl.OutputDirectory, "Models\\", className + ".cs"));
        }

        private void GenerateMenus(string tableName, int index)
        {
            string baseTableName = GetTableName(tableName);
            var @namespace = new CodeNamespace(settingsControl.ProjectName + ".Menus");
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Localization"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.UI.Navigation"));
            @namespace.Imports.Add(new CodeNamespaceImport(settingsControl.ProjectName + ".Permissions"));

            string className = baseTableName + "NavigationProvider";
            var @class = new CodeTypeDeclaration(className);
            @class.IsClass = true;
            @class.Attributes = MemberAttributes.Public;
            @class.BaseTypes.Add("INavigationProvider");

            var property = CreateProperty(typeof(Localizer), "T");
            @class.Members.Add(property);

            var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };
            constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "T"), new CodeArgumentReferenceExpression("NullLocalizer.Instance")));
            @class.Members.Add(constructor);

            #region GetNavigation

            var getNavigationMethod = new CodeMemberMethod();
            getNavigationMethod.Name = "GetNavigation";
            getNavigationMethod.Attributes = MemberAttributes.Public;
            getNavigationMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(NavigationBuilder), "builder"));
            getNavigationMethod.Statements.Add(new CodeSnippetExpression(string.Format("builder.Add(T(\"{0} Management\"), \"{1}\", b => b.Action(\"Index\", \"{0}\", new {{ area = \"\" }}).IconCssClass(\"fa-circle\").Permission({0}Permissions.Manager{0}))", baseTableName, index)));
            @class.Members.Add(getNavigationMethod);

            #endregion GetNavigation

            @namespace.Types.Add(@class);

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(@namespace);
            GenerateCSharpCode(compileUnit, Path.Combine(settingsControl.OutputDirectory, "Menus\\", className + ".cs"));
        }

        private void GeneratePermission(string tableName)
        {
            string baseTableName = GetTableName(tableName);
            var @namespace = new CodeNamespace(settingsControl.ProjectName + ".Permissions");
            @namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            @namespace.Imports.Add(new CodeNamespaceImport("CMSSolutions.Web.Security.Permissions"));

            string className = baseTableName + "Permissions";
            var @class = new CodeTypeDeclaration(className);
            @class.IsClass = true;
            @class.Attributes = MemberAttributes.Public;
            @class.BaseTypes.Add("IPermissionProvider");

            var property = new StringBuilder();
            property.AppendFormat("        public static readonly Permission Manager{0} = new Permission", baseTableName);
            property.AppendLine();
            property.Append("        {");
            property.AppendLine();
            property.AppendFormat("            Name = \"Manager{0}\", ", baseTableName);
            property.AppendLine();
            property.Append("            Category = \"Management\", ");
            property.AppendLine();
            property.AppendFormat("            Description = \"Manager {0}\", ", baseTableName);
            property.AppendLine();
            property.Append("        };");
            property.AppendLine();
            @class.Members.Add(new CodeSnippetTypeMember(property.ToString()));

            #region GetPermissions

            var getNavigationMethod = new CodeMemberMethod();
            getNavigationMethod.Name = "GetPermissions";
            getNavigationMethod.Attributes = MemberAttributes.Public;
            getNavigationMethod.ReturnType = new CodeTypeReference()
            {
                BaseType = "IEnumerable<Permission>",
                Options = CodeTypeReferenceOptions.GenericTypeParameter
            };

            getNavigationMethod.Statements.Add(new CodeSnippetExpression(string.Format("yield return Manager{0}", baseTableName)));

            @class.Members.Add(getNavigationMethod);

            #endregion GetPermissions

            @namespace.Types.Add(@class);

            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(@namespace);
            GenerateCSharpCode(compileUnit, Path.Combine(settingsControl.OutputDirectory, "Permissions\\", className + ".cs"));
        }

        #region ControlForms Attributes

        private static CodeAttributeDeclaration CreateControlChoiceAttribute(Field field)
        {
            var attribute = new CodeAttributeDeclaration("ControlChoice");
            switch (field.Type)
            {
                case FieldType.Boolean:
                    {
                        attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("ControlChoice.CheckBox")));
                    }
                    break;

                case FieldType.Choice:
                    {
                        attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("ControlChoice.RadioButtonList")));
                    }
                    break;

                case FieldType.MultiChoice:
                    {
                        attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("ControlChoice.CheckBoxList")));
                    }
                    break;

                case FieldType.Lookup:
                    {
                        attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("ControlChoice.DropDownList")));
                    }
                    break;

                case FieldType.MultiLookup:
                    {
                        attribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeReferenceExpression("ControlChoice.CheckBoxList")));
                    }
                    break;
            }

            attribute.Arguments.Add(new CodeAttributeArgument("LabelText", new CodePrimitiveExpression(field.Name.SpacePascal())));
            attribute.Arguments.Add(new CodeAttributeArgument("Required", new CodePrimitiveExpression(field.IsRequired)));
            attribute.Arguments.Add(new CodeAttributeArgument("ContainerCssClass", new CodeTypeReferenceExpression("Constants.ContainerCssClassCol3")));
            attribute.Arguments.Add(new CodeAttributeArgument("ContainerRowIndex", new CodePrimitiveExpression(field.Ordinal)));

            return attribute;
        }

        private static CodeAttributeDeclaration CreateControlDateTimeAttribute(Field field)
        {
            return new CodeAttributeDeclaration(
                "ControlDatePicker",
                new CodeAttributeArgument("LabelText", new CodePrimitiveExpression(field.Name.SpacePascal())),
                new CodeAttributeArgument("Required", new CodePrimitiveExpression(field.IsRequired)),
                new CodeAttributeArgument("ContainerCssClass", new CodeTypeReferenceExpression("Constants.ContainerCssClassCol3")),
                new CodeAttributeArgument("ContainerRowIndex", new CodePrimitiveExpression(field.Ordinal)));
        }

        private static CodeAttributeDeclaration CreateControlHiddenAttribute(Field field)
        {
            return new CodeAttributeDeclaration("ControlHidden");
        }

        private static CodeAttributeDeclaration CreateControlNumericAttribute(Field field)
        {
            return new CodeAttributeDeclaration(
                "ControlNumeric",
                new CodeAttributeArgument("LabelText", new CodePrimitiveExpression(field.Name.SpacePascal())),
                new CodeAttributeArgument("Required", new CodePrimitiveExpression(field.IsRequired)),
                new CodeAttributeArgument("ContainerCssClass", new CodeTypeReferenceExpression("Constants.ContainerCssClassCol3")),
                new CodeAttributeArgument("ContainerRowIndex", new CodePrimitiveExpression(field.Ordinal)));
        }

        private static CodeAttributeDeclaration CreateControlTextAttribute(Field field)
        {
            if (field.Type == FieldType.RichText || field.MaxLength == -1)
            {
                return new CodeAttributeDeclaration(
                    "ControlText",
                    new CodeAttributeArgument("Type", new CodeTypeReferenceExpression("ControlText.RichText")),
                    new CodeAttributeArgument("Required", new CodePrimitiveExpression(field.IsRequired)),
                    new CodeAttributeArgument("ContainerCssClass", new CodeTypeReferenceExpression("Constants.ContainerCssClassCol12")),
                    new CodeAttributeArgument("ContainerRowIndex", new CodePrimitiveExpression(field.Ordinal)));
            }

            if (field.Type == FieldType.Char)
            {
                return new CodeAttributeDeclaration(
                    "ControlText",
                    new CodeAttributeArgument("Type", new CodeTypeReferenceExpression("ControlText.TextBox")),
                    new CodeAttributeArgument("Required", new CodePrimitiveExpression(field.IsRequired)),
                    new CodeAttributeArgument("MaxLength", new CodePrimitiveExpression(1)),
                    new CodeAttributeArgument("ContainerCssClass", new CodeTypeReferenceExpression("Constants.ContainerCssClassCol6")),
                    new CodeAttributeArgument("ContainerRowIndex", new CodePrimitiveExpression(field.Ordinal)));
            }

            return new CodeAttributeDeclaration(
                "ControlText",
                new CodeAttributeArgument("Type", new CodeTypeReferenceExpression("ControlText.TextBox")),
                new CodeAttributeArgument("Required", new CodePrimitiveExpression(field.IsRequired)),
                new CodeAttributeArgument("MaxLength", new CodePrimitiveExpression(field.MaxLength)),
                new CodeAttributeArgument("ContainerCssClass", new CodeTypeReferenceExpression("Constants.ContainerCssClassCol6")),
                new CodeAttributeArgument("ContainerRowIndex", new CodePrimitiveExpression(field.Ordinal)));
        }

        #endregion ControlForms Attributes

        #region Breadcrumbs

        private static CodeSnippetStatement GenerateCreateActionBreadcrumbs(string entityName)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"            WorkContext.Breadcrumbs.Add(new Breadcrumb {{ Text = T(""{0}""), Url = ""#"" }});", entityName);
            sb.AppendLine();
            sb.AppendFormat(@"            WorkContext.Breadcrumbs.Add(new Breadcrumb {{ Text = T(""Create {0}""), Url = Url.Action(""Index"") }});", entityName.SpacePascal().Pluralize());

            return new CodeSnippetStatement(sb.ToString());
        }

        private static CodeSnippetStatement GenerateEditActionBreadcrumbs(string entityName)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"            WorkContext.Breadcrumbs.Add(new Breadcrumb {{ Text = T(""{0}""), Url = ""#"" }});", entityName);
            sb.AppendLine();
            sb.AppendFormat(@"            WorkContext.Breadcrumbs.Add(new Breadcrumb {{ Text = T(""{0}""), Url = Url.Action(""Index"") }});", entityName.SpacePascal().Pluralize());

            return new CodeSnippetStatement(sb.ToString());
        }

        private static CodeSnippetStatement GenerateIndexActionBreadcrumbs(string entityName)
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"            WorkContext.Breadcrumbs.Add(new Breadcrumb {{ Text = T(""{0}""), Url = ""#"" }});", entityName.SpacePascal().Pluralize());

            return new CodeSnippetStatement(sb.ToString());
        }

        #endregion Breadcrumbs

        #region Miscellaneous

        private static CodeSnippetTypeMember CreateImplicitConversionOperatorOverload(string entityName, IEnumerable<Field> fields)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("        public static implicit operator {0}Model({0} entity)", entityName);
            sb.AppendLine();
            sb.AppendLine("        {");
            sb.AppendFormat("            return new {0}Model", entityName);
            sb.AppendLine();
            sb.AppendLine("            {");

            var orderedFields = fields.OrderBy(x => x.Ordinal);
            int count = fields.Count();
            for (int i = 0; i < count; i++)
            {
                var field = orderedFields.ElementAt(i);
                sb.AppendFormat("                {0} = entity.{0}", field.Name);

                if (i != count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            sb.AppendLine();
            sb.AppendLine("            };");
            sb.AppendLine("        }");

            return new CodeSnippetTypeMember(sb.ToString());
        }

        private static CodeMemberField CreateProperty(Type type, string name, bool isVirtual = false)
        {
            var property = new CodeMemberField
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = name + " { get; set; }",
                Type = new CodeTypeReference(type),
            };

            if (!isVirtual)
            {
                property.Attributes |= MemberAttributes.Final;
            }

            return property;
        }

        private static CodeVariableDeclarationStatement CreateVariable(Type type, string name)
        {
            var typeReference = new CodeTypeReference(type);
            return new CodeVariableDeclarationStatement(typeReference, name)
            {
                InitExpression = new CodeObjectCreateExpression
                {
                    CreateType = typeReference
                }
            };
        }

        #endregion

        public static void GenerateCSharpCode(CodeCompileUnit compileUnit, string fileName)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions { BracingStyle = "C" };

            string directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            provider.GenerateCodeFromCompileUnit(compileUnit, stringWriter, options);
            stringWriter.Close();

            // Fix auto property
            sb.Replace("{ get; set; };", "{ get; set; }");

            // remove <auto-generated>
            var str = sb.ToString();
            var indexOfComments = str.IndexOf("namespace ", StringComparison.Ordinal);
            if (indexOfComments > 0)
            {
                str = str.Remove(0, indexOfComments);
            }

            File.WriteAllText(fileName, str);
        }
    }
}