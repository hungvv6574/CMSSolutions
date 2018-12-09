using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using CMSSolutions.Data;
using CMSSolutions.GTools.Common.Models;
using KeyType = CMSSolutions.GTools.Common.Models.KeyType;

namespace CMSSolutions.GTools.Common.Data
{
    public abstract class BaseProvider
    {
        #region Public Properties

        public abstract string DbProviderName { get; }

        protected ConnectionDetails ConnectionDetails { get; set; }

        /// <summary>
        /// Used in T-SQL queries for escaping spaces and reserved words
        /// </summary>
        protected virtual string SpaceEscapeStart { get; set; }

        protected virtual string SpaceEscapeEnd { get; set; }

        #endregion Public Properties

        protected string SpaceEscape(string value)
        {
            return string.Concat(SpaceEscapeStart, value, SpaceEscapeEnd);
        }

        #region Constructor

        public BaseProvider(ConnectionDetails connectionDetails)
        {
            this.ConnectionDetails = connectionDetails;
            SpaceEscapeStart = "[";
            SpaceEscapeEnd = "]";
        }

        #endregion Constructor

        #region Table Methods

        public virtual IEnumerable<string> TableNames
        {
            get
            {
                using (DbConnection connection = CreateDbConnection(DbProviderName, ConnectionDetails.ConnectionString))
                {
                    string[] restrictions = new string[4];
                    restrictions[3] = "Base Table";

                    connection.Open();
                    DataTable schema = connection.GetSchema("Tables", restrictions);
                    connection.Close();

                    List<string> tableNames = new List<string>();
                    foreach (DataRow row in schema.Rows)
                    {
                        tableNames.Add(row.Field<string>("TABLE_NAME"));
                    }
                    return tableNames;
                }
            }
        }

        #endregion Table Methods

        #region Field Methods

        public virtual IEnumerable<string> GetFieldNames(string tableName)
        {
            using (DbConnection connection = CreateDbConnection(DbProviderName, ConnectionDetails.ConnectionString))
            {
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format(Models.Constants.Data.CMD_SELECT_INFO_SCHEMA_COLUMN_NAMES, tableName);
                    List<string> columns = new List<string>();

                    connection.Open();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader.GetString(0));
                        }
                    }
                    connection.Close();
                    return columns;
                }
            }
        }

        public virtual FieldCollection GetFields(string tableName)
        {
            const string CMD_COLUMN_INFO_FORMAT =
@"SELECT COLUMN_NAME, COLUMN_DEFAULT, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '{0}'";

            const string CMD_IS_PRIMARY_KEY_FORMAT =
@"SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_NAME), 'IsPrimaryKey') = 1
AND TABLE_NAME = '{0}'";

            var list = new FieldCollection();

            using (var connection = CreateDbConnection(DbProviderName, ConnectionDetails.ConnectionString))
            {
                try
                {
                    var foreignKeyColumns = GetForeignKeyInfo(tableName);

                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format(CMD_COLUMN_INFO_FORMAT, tableName);

                        using (var reader = command.ExecuteReader())
                        {
                            Field columnInfo = null;

                            while (reader.Read())
                            {
                                columnInfo = new Field();

                                if (!reader.IsDBNull(0))
                                { columnInfo.Name = reader.GetString(0); }

                                if (!reader.IsDBNull(1))
                                { columnInfo.DefaultValue = reader.GetString(1); }
                                else
                                { columnInfo.DefaultValue = string.Empty; }

                                if (foreignKeyColumns.Contains(columnInfo.Name))
                                {
                                    columnInfo.KeyType = KeyType.ForeignKey;
                                }

                                try
                                {
                                    columnInfo.Type = GetScaffolderFieldType(reader.GetString(2));
                                }
                                catch (ArgumentNullException)
                                {
                                    columnInfo.Type = FieldType.Object;
                                }
                                catch (ArgumentException)
                                {
                                    columnInfo.Type = FieldType.Object;
                                }

                                if (!reader.IsDBNull(3))
                                { columnInfo.MaxLength = reader.GetInt32(3); }

                                if (!reader.IsDBNull(4))
                                {
                                    if (reader.GetString(4).ToUpperInvariant().Equals("NO"))
                                    { columnInfo.IsRequired = true; }
                                    else
                                    { columnInfo.IsRequired = false; }
                                }

                                list.Add(columnInfo);
                            }
                        }
                    }
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    { connection.Close(); }
                }

                #region Primary Keys

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(CMD_IS_PRIMARY_KEY_FORMAT, tableName);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string pkColumn = reader.GetString(0);
                            Field match = list[pkColumn];
                            if (match != null)
                            {
                                match.KeyType = KeyType.PrimaryKey;
                            }
                        }
                    }
                    connection.Close();
                }

                #endregion Primary Keys
            }

            return list;
        }

        #region Old

        //public virtual FieldCollection GetFields(string tableName)
        //{
        //    using (DbConnection connection = CreateDbConnection(DbProviderName, ConnectionDetails.ConnectionString))
        //    {
        //        using (DbCommand command = connection.CreateCommand())
        //        {
        //            command.CommandType = CommandType.Text;
        //            command.CommandText = string.Format(Constants.Data.CMD_SELECT_INFO_SCHEMA_COLUMNS, tableName);
        //            FieldCollection fields = new FieldCollection();

        //            connection.Open();
        //            using (DbDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    Field field = new Field();
        //                    field.Name = reader.GetString(0);
        //                    if (!reader.IsDBNull(1))
        //                    { field.Ordinal = reader.GetInt32(1); }
        //                    if (!reader.IsDBNull(2))
        //                    { field.Type = GetScaffolderFieldType(reader.GetString(2)); }
        //                    if (!reader.IsDBNull(3))
        //                    { field.IsRequired = reader.GetString(3) == "YES"; }
        //                    if (!reader.IsDBNull(4))
        //                    { field.MaxLength = reader.GetInt32(4); }
        //                    fields.Add(field);
        //                }
        //            }
        //            connection.Close();

        //            try
        //            {
        //                command.CommandText = string.Format(Constants.Data.CMD_IS_PRIMARY_KEY_FORMAT, tableName);

        //                var foreignKeyColumns = GetForeignKeyInfo(tableName);

        //                connection.Open();
        //                using (DbDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        string pkColumn = reader.GetString(0);
        //                        Field match = fields.SingleOrDefault(f => f.Name == pkColumn);
        //                        if (match != null)
        //                        {
        //                            match.IsPrimaryKey = true;
        //                        }
        //                    }
        //                }

        //                connection.Close();
        //            }
        //            catch (Exception x)
        //            {
        //                TraceService.Instance.WriteConcat(TraceEvent.Error, "Error: Could not get primary key info - ", x.Message);
        //                if (connection.State != ConnectionState.Closed)
        //                {
        //                    connection.Close();
        //                }
        //            }

        //            return fields;
        //        }
        //    }
        //}

        #endregion Old

        public virtual ForeignKeyInfoCollection GetForeignKeyInfo(string tableName)
        {
            using (var connection = CreateDbConnection(DbProviderName, ConnectionDetails.ConnectionString))
            {
                const string query =
@"SELECT FK_Table = FK.TABLE_NAME,
    FK_Column = CU.COLUMN_NAME,
	PK_Table = PK.TABLE_NAME,
    PK_Column = PT.COLUMN_NAME,
	Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN
(
	SELECT i1.TABLE_NAME, i2.COLUMN_NAME
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
	INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON
		i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
	WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
) PT ON PT.TABLE_NAME = PK.TABLE_NAME
WHERE FK.TABLE_NAME = '{0}'
ORDER BY 1,2,3,4";

                var foreignKeyData = new ForeignKeyInfoCollection();

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = string.Format(query, tableName);
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreignKeyData.Add(new ForeignKeyInfo(
                                reader.GetString(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                string.Empty,
                                reader.GetString(4)));
                        }
                    }
                }
                connection.Close();
                return foreignKeyData;
            }
        }

        #endregion Field Methods

        #region Public Static Methods

        public static DbConnection CreateDbConnection(string providerName, string connectionString)
        {
            // Assume failure.
            DbConnection connection = null;

            // Create the DbProviderFactory and DbConnection.
            if (connectionString != null)
            {
                try
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionString;
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    if (connection != null)
                    {
                        connection = null;
                    }
                    Console.WriteLine(ex.Message);
                }
            }

            // Return the connection.
            return connection;
        }

        #endregion Public Static Methods

        #region Field Conversion Methods

        public abstract FieldType GetScaffolderFieldType(string providerFieldType);

        public abstract string GetDataProviderFieldType(FieldType fieldType);

        #endregion Field Conversion Methods
    }
}