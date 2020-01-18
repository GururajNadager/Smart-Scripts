using System.IO;
using System.Text;

namespace SmartScript.Generators
{
    public class DropSp
    {
        #region Private Fields

        private readonly StringBuilder builder;

        private readonly string dbName;
        private readonly string schemaName;
        private readonly string spName;

        #endregion Private Fields

        #region Public Constructors

        public DropSp(string dbName, string spName, string schemaName)
        {
            builder = new StringBuilder();
            this.spName = spName;
            this.schemaName = schemaName;
            this.dbName = dbName;
        }

        #endregion Public Constructors

        #region Public Methods

        public DropSp IncludeSp()
        {
            builder.UseDb(dbName);
            builder.Go();
            SpIfExists();
            builder.Begin();
            builder.AppendLine(string.Format("DROP PROCEDURE [{0}].[{1}]", schemaName, spName));
            builder.AddEndGo();
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

        private void SpIfExists()
        {
            builder.AppendLine("IF EXISTS (");
            builder.AppendLine("                 SELECT 1");
            builder.AppendLine("                 FROM " + dbName + ".sys.procedures WITH (NOLOCK)");
            builder.AppendLine(string.Format("                 WHERE Name = '{0}'", spName));
            builder.AppendLine("               )");
        }

        #endregion Private Methods
    }
}