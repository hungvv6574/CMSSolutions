using System.Data;
using CMSSolutions.Extensions;
using CMSSolutions.GTools.Common;
using CMSSolutions.GTools.Common.Data;
using CMSSolutions.GTools.Common.Models;

namespace CMSSolutions.GTools.Sql
{
    public class SqlProvider : BaseProvider
    {
        public override string DbProviderName
        {
            get { return "System.Data.SqlClient"; }
        }

        public SqlProvider(ConnectionDetails connectionDetails)
            : base(connectionDetails)
        {
        }

        public override FieldType GetScaffolderFieldType(string providerFieldType)
        {
            return AppContext.SqlDbTypeConverter.GetScaffolderFieldType(EnumExtensions.ToEnum<SqlDbType>(providerFieldType, true));
        }

        public override string GetDataProviderFieldType(FieldType fieldType)
        {
            return AppContext.SqlDbTypeConverter.GetDataProviderFieldType(fieldType).ToString();
        }
    }
}