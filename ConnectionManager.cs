using Microsoft.SqlServer.Management.Common;
using SmartScript.Models;

namespace SmartScript
{
    public static class ConnectionManager
    {
        #region Public Methods

        public static ServerConnection GetConnection(DatabaseServer datasbaseServer)
        {
            return new ServerConnection(datasbaseServer.Server)
            {
                LoginSecure = false,
                Login = datasbaseServer.UserName,
                Password = datasbaseServer.Password
            };
        }

        #endregion Public Methods
    }
}