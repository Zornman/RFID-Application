using System.Data.SqlClient;

/// <summary>
/// Summary description for clsConnection
/// </summary>
namespace Sonrai.Mobile.BusinessLogic
{
    public class Connection
    {
        string strConnection = string.Empty;
        public Connection()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        
        public SqlConnection getSonraiConnection()
        {
            strConnection = "server=192.168.254.3,10070;database=SonraiMobile;uid=sa;pwd=a@www.comm1;Connect Timeout=30";
            SqlConnection con = new SqlConnection(strConnection);
            return con;
        }
    }
}
