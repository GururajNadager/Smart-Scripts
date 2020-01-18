using System.Collections.Generic;

namespace SmartScript.Models
{
    public class DatabaseServer
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public List<Table> Tables { get; set; }
        public List<Sp> StoredProcedures { get; set; }

        public string OutputFolder { get; set; }

        public string RollbackOutputFolder { get; set; }
    }
}