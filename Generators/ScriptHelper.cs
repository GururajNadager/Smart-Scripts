using System.Text;

namespace SmartScript.Generators
{
    public static class ScriptHelper
    {
        #region Public Methods

        public static StringBuilder AddEndGo(this StringBuilder sb)
        {
            return sb.End().Go();
        }

        public static StringBuilder Begin(this StringBuilder sb)
        {
            sb.AppendLine("BEGIN");
            return sb;
        }

        public static StringBuilder CreateDefaultKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                          --DEFAULT KEY CONSTRAINTS                            ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder DropDefaultKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                          --DROP DEFAULT KEY CONSTRAINTS                       ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder CreateForeignKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- CREATE FOREIGN KEYS                              ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder DropForeignKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- DROP FOREIGN KEYS                                ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder CreatePrimaryKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- CREATE PRIMARY KEY                               ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder DropPrimaryKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- DROP PRIMARY KEY                                 ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder CreateUniqueKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- CREATE UNIQUE KEYS                               ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder DropUniqueKeyConstraintHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- DROP UNIQUE KEYS                                 ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder DropTableHeaderText(this StringBuilder sb)
        {
            sb.AppendLine("-- ============================================================================================");
            sb.AppendLine("                                           -- DROP TABLE                                       ");
            sb.AppendLine("-- ============================================================================================");
            return sb;
        }

        public static StringBuilder End(this StringBuilder sb)
        {
            sb.AppendLine("END");
            return sb;
        }

        public static StringBuilder Go(this StringBuilder sb)
        {
            sb.AppendLine("GO");
            return sb;
        }

        public static string KeyIsNotNull(string key)
        {
            return string.Format("IF OBJECT_ID (N'{0}') IS  NOT NULL", key);
        }

        public static string KeyIsNotNull(string key,string type)
        {
            return string.Format("IF OBJECT_ID ('{0}','{1}') IS  NOT NULL", key, type);
        }

        public static string KeyIsNull(string key, string type)
        {
            return string.Format("IF OBJECT_ID ('{0}','{1}') IS NULL", key, type);
        }

        public static string KeyIsNull(string key)
        {
            return string.Format("IF OBJECT_ID (N'{0}') IS  NULL", key);
        }

        public static StringBuilder NewLine(this StringBuilder sb)
        {
            sb.AppendLine(string.Empty);
            return sb;
        }

        public static StringBuilder RemoveAnsiQuotedIdentifier(this StringBuilder sb)
        {
            sb.Replace("SET ANSI_NULLS ON", string.Empty);
            sb.Replace("SET QUOTED_IDENTIFIER ON", string.Empty);
            return sb;
        }

        public static StringBuilder UseDb(this StringBuilder sb, string dbName)
        {
            sb.AppendLine("USE[" + dbName + "]");
            return sb;
        }

        #endregion Public Methods
    }
}