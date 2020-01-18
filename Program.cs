using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using SmartScript.Generators;
using System;
using System.IO;
using System.Reflection;
using SM = SmartScript.Models;

namespace SmartScript
{
    internal class Program
    {
        #region Private Properties

        private static string FilePath
        {
            get
            {
                return Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName).FullName +
                        "\\" + "datasource.json";
            }
        }

        private static string Header
        {
            get
            {
                return "*****************************************************************************" + Environment.NewLine +
                       "*************************SMART SCRIPT GENERATOR *****************************" + Environment.NewLine +
                       "*****************************************************************************";
            }
        }

        private static string Finish
        {
            get
            {
                return
                       "*************************SMART SCRIPT COMPLETED *****************************";
            }
        }

        #endregion Private Properties

        #region Private Methods

        private static string GetOutputPath(string folder, string name)
        {
            return folder + "\\" + name + ".sql";
        }

        private static void Main(string[] args)
        {
            Console.WriteLine(Header);

            SM.DatabaseServer datasource = JsonConvert.DeserializeObject<SM.DatabaseServer>(System.IO.File.ReadAllText(FilePath));

            Server server = new Server(ConnectionManager.GetConnection(datasource));
            server.ConnectionContext.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
            server.ConnectionContext.Connect();

            var database = server.Databases[datasource.Database];

            foreach (var tableItem in datasource.Tables)
            {
                CreateTable table = new CreateTable(database, tableItem.Schema, tableItem.Name);
                DropTable dropTable = new DropTable(database, tableItem);

                table.IncludeTable()
                     .IncludePrimaryKey()
                     .IncludeForeignKey()
                     .IncludeUniqueKey()
                     .IncludeDefaultConstraints()
                     .WriteToFile(GetOutputPath(datasource.OutputFolder, "table_" + tableItem.Name));

                dropTable.IncludeForeignKey()
                    .IncludePrimaryKey()
                    .IncludeUniqueKey()
                    .IncludeDefaultConstraints()
                    .IncludeTable()
                    .WriteToFile(GetOutputPath(datasource.RollbackOutputFolder, "ROLLBACK_table_" + tableItem.Name));

                Console.WriteLine(string.Format("[{0}].[{1}] Completed.", tableItem.Schema, tableItem.Name));
            }

            foreach (var sp in datasource.StoredProcedures)
            {
                CreateSp createSp = new CreateSp(database, sp.Name, sp.Schema);
                createSp.IncludeSp().WriteToFile(GetOutputPath(datasource.OutputFolder, "sp_" + sp));
                DropSp dropSp = new DropSp(database.Name, sp.Name, sp.Schema);
                dropSp.IncludeSp().WriteToFile(GetOutputPath(datasource.RollbackOutputFolder, "ROLLBACK_sp_" + sp));
            }

            server.ConnectionContext.Disconnect();

            Console.WriteLine(Finish);
            Console.ReadLine();
        }

        #endregion Private Methods
    }
}