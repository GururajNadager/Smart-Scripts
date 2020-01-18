using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Text;
using SM = SmartScript.Models;

namespace SmartScript.Generators
{
    public class DropTable
    {
        #region Private Fields

        private readonly StringBuilder builder;
        private readonly Database database;
        private readonly SM.Table scriptTable;

        #endregion Private Fields

        #region Public Constructors

        public DropTable(Database database, SM.Table scriptTable)
        {
            builder = new StringBuilder();
            this.database = database;
            this.scriptTable = scriptTable;

            builder.UseDb(database.Name);
            builder.Go();
        }

        #endregion Public Constructors

        #region Public Methods

        public DropTable IncludeDefaultConstraints()
        {
            builder.DropDefaultKeyConstraintHeaderText();
            var table = GetTable();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].DefaultConstraint != null)
                {
                    AddDefaultItem(table.Columns[i].DefaultConstraint);
                }
            }
            builder.NewLine();
            return this;
        }

        public DropTable IncludeForeignKey()
        {
            builder.DropForeignKeyConstraintHeaderText();
            var table = GetTable();
            for (int i = 0; i < table.ForeignKeys.Count; i++)
            {
                AddIndexItem(table.ForeignKeys[i].Name);
            }
            builder.NewLine();
            return this;
        }

        public DropTable IncludePrimaryKey()
        {
            builder.DropPrimaryKeyConstraintHeaderText();
            Table table = GetTable();
            for (int i = 0; i < table.Indexes.Count; i++)
            {
                var indexItem = table.Indexes[i];
                if (indexItem.IndexKeyType == IndexKeyType.DriPrimaryKey)
                {
                    AddIndexItem(indexItem.Name);
                }
            }
            builder.NewLine();
            return this;
        }

        public DropTable IncludeTable()
        {
            builder.DropTableHeaderText();
            TableIfExists(database.Name);
            builder.Begin();
            builder.AppendLine(string.Format(" DROP TABLE [{0}].[{1}]", scriptTable.Schema, scriptTable.Name));
            builder.AddEndGo();
            builder.NewLine();
            return this;
        }

        public DropTable IncludeUniqueKey()
        {
            builder.DropUniqueKeyConstraintHeaderText();
            var table = GetTable();
            for (int i = 0; i < table.Indexes.Count; i++)
            {
                var indexItem = table.Indexes[i];
                if (indexItem.IndexKeyType == IndexKeyType.DriUniqueKey)
                {
                    AddIndexItem(indexItem.Name);
                }
            }
            builder.NewLine();
            return this;
        }

        public override string ToString()
        {
            return builder.ToString();
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

        private void AddIndexItem(string keyName)
        {
            builder.AppendLine(ScriptHelper.KeyIsNotNull(keyName));

            builder.Begin();
            builder.AppendLine(string.Format("ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]", scriptTable.Schema,
                                                                                        scriptTable.Name,
                                                                                        keyName
                                                                                       ));

            builder.AddEndGo();
            builder.NewLine();
        }

        private Table GetTable()
        {
            return database.Tables[scriptTable.Name, scriptTable.Schema];
        }

        private void TableIfExists(string dbName)
        {
            builder.AppendLine("IF" +
                " EXISTS (");
            builder.AppendLine("                 SELECT TABLE_NAME");
            builder.AppendLine("                 FROM " + dbName + ".INFORMATION_SCHEMA.TABLES WITH (NOLOCK)");
            builder.AppendLine(string.Format("                 WHERE TABLE_SCHEMA = '{0}'", scriptTable.Schema));
            builder.AppendLine(string.Format("                 AND TABLE_NAME = N'{0}'", scriptTable.Name));
            builder.AppendLine("               )");
        }

        #endregion Private Methods
    }
}