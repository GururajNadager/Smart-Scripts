using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Text;

namespace SmartScript.Generators
{
    public class CreateTable
    {
        #region Private Fields

        private readonly StringBuilder builder;
        private readonly Database database;
        private readonly ScriptingOptions options;
        private readonly string schema;
        private readonly string tableName;

        #endregion Private Fields

        #region Public Constructors

        public override string ToString()
        {
            return builder.ToString();
        }

        public CreateTable(Database database, string schema, string tableName)
        {
            builder = new StringBuilder();
            this.database = database;
            this.schema = schema;
            this.tableName = tableName;
            options = new ScriptingOptions
            {
                SchemaQualify = true,
                EnforceScriptingOptions = true,
                NoCollation = true,
                NoExecuteAs = true,
                NoIndexPartitioningSchemes = true
            };
        }

        #endregion Public Constructors



        #region Public Methods

        public CreateTable IncludeDefaultConstraints()
        {
            builder.CreateDefaultKeyConstraintHeaderText();
            var scriptTable = GetTable();
            for (int i = 0; i < scriptTable.Columns.Count; i++)
            {
                if (scriptTable.Columns[i].DefaultConstraint != null)
                {
                    AddDefaultItem(scriptTable.Columns[i].DefaultConstraint);
                }
            }
            builder.NewLine();
            return this;
        }

        public CreateTable IncludeForeignKey()
        {
            builder.CreateForeignKeyConstraintHeaderText();
            Table scriptTable = GetTable();
            for (int i = 0; i < scriptTable.ForeignKeys.Count; i++)
            {
                builder.AppendLine(ScriptHelper.KeyIsNull(scriptTable.ForeignKeys[i].Name));

                builder.Begin();
                var foreignScripts = scriptTable.ForeignKeys[i].Script(options);
                foreach (string script in foreignScripts)
                    builder.AppendLine(script);

                builder.AddEndGo();
            }
            builder.NewLine();
            return this;
        }

        public CreateTable IncludePrimaryKey()
        {
            builder.CreatePrimaryKeyConstraintHeaderText();
            Table scriptTable = GetTable();
            for (int i = 0; i < scriptTable.Indexes.Count; i++)
            {
                var indexItem = scriptTable.Indexes[i];
                if (indexItem.IndexKeyType == IndexKeyType.DriPrimaryKey)
                {
                    AddIndexItem(indexItem);
                }
            }
            builder.NewLine();
            return this;
        }

        public CreateTable IncludeTable()
        {
            TableIfNotExists(database.Name);
            Table tbl = GetTable();
            var createScripts = tbl.Script(options);
            foreach (string script in createScripts)
                builder.AppendLine(script);

            builder.RemoveAnsiQuotedIdentifier();
            builder.AddEndGo();
            builder.NewLine();
            return this;
        }

        public CreateTable IncludeUniqueKey()
        {
            builder.CreateUniqueKeyConstraintHeaderText();
            var scriptTable = GetTable();
            for (int i = 0; i < scriptTable.Indexes.Count; i++)
            {
                var indexItem = scriptTable.Indexes[i];
                if (indexItem.IndexKeyType == IndexKeyType.DriUniqueKey)
                {
                    AddIndexItem(indexItem);
                }
            }
            builder.NewLine();
            return this;
        }

        public void WriteToFile(string fileName)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.Write(builder.ToString());
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void AddDefaultItem(DefaultConstraint defaultItem)
        {
            builder.AppendLine(ScriptHelper.KeyIsNull(defaultItem.Name));
            builder.Begin();
            var foreignScripts = defaultItem.Script();
            foreach (string script in foreignScripts)
                builder.AppendLine(script);

            builder.AddEndGo();
            builder.NewLine();
        }

        private void AddIndexItem(Index indexItem)
        {
            builder.AppendLine(ScriptHelper.KeyIsNull(indexItem.Name));

            builder.Begin();
            builder.Append(string.Format("ALTER TABLE [{0}].[{1}] ADD CONSTRAINT [{2}] {4} {3}", schema,
                                                                                        tableName,
                                                                                        indexItem.Name,
                                                                                        indexItem.IsClustered ? "CLUSTERED" : "NONCLUSTERED",
                                                                                        indexItem.IndexKeyType == IndexKeyType.DriPrimaryKey ? "PRIMARY KEY" : "UNIQUE"));
            builder.Append("(");
            for (int j = 0; j < indexItem.IndexedColumns.Count; j++)
            {
                builder.Append(string.Format("[{0}] {1}", indexItem.IndexedColumns[j].Name
                                                   , indexItem.IndexedColumns[j].Descending ? "DESC" : "ASC"));
            }
            builder.AppendLine(")");

            builder.AddEndGo();
            builder.NewLine();
        }

        private Table GetTable()
        {
            return database.Tables[tableName, schema];
        }

        private void TableIfNotExists(string dbName)
        {
            builder.UseDb(dbName);
            builder.Go();
            builder.AppendLine("IF NOT EXISTS (");
            builder.AppendLine("                 SELECT TABLE_NAME");
            builder.AppendLine("                 FROM " + dbName + ".INFORMATION_SCHEMA.TABLES WITH (NOLOCK)");
            builder.AppendLine(string.Format("                 WHERE TABLE_SCHEMA = '{0}'", schema));
            builder.AppendLine(string.Format("                 AND TABLE_NAME = N'{0}'", tableName));
            builder.AppendLine("               )");
            builder.Begin();
        }

        #endregion Private Methods
    }
}