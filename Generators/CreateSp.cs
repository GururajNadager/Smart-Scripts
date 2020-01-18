using Microsoft.SqlServer.Management.Smo;
using System.IO;
using System.Text;

namespace SmartScript.Generators
{
    public class CreateSp
    {
        private readonly StringBuilder builder;
        private readonly Database database;
        private readonly string spName;
        private readonly string schemaName;
        private readonly ScriptingOptions options;

        public CreateSp(Database database, string spName, string schemaName)
        {
            builder = new StringBuilder();
            this.database = database;
            this.spName = spName;
            this.schemaName = schemaName;
            options = new ScriptingOptions
            {
                SchemaQualify = true,
                EnforceScriptingOptions = true,
                NoCollation = true,
                NoExecuteAs = true,
                NoIndexPartitioningSchemes = true
            };
            builder.UseDb(database.Name);
            builder.Go();
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        public CreateSp IncludeSp()
        {
            builder.AppendLine(ScriptHelper.KeyIsNull(spName, "P"));
            builder.Begin();
            builder.AppendLine(string.Format(" EXEC ('CREATE PROCEDURE [{0}].[{1}] AS ')", schemaName, spName));
            builder.AddEndGo();
            var createScripts = database.StoredProcedures[spName, schemaName].Script(options);
            foreach (string script in createScripts)
                builder.AppendLine(script);

            builder.RemoveAnsiQuotedIdentifier();

            return this;
        }

        public void WriteToFile(string fileName)
        {
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.Write(builder.ToString());
            }
        }
    }
}