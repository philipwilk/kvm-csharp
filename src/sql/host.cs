using MySql.Data.MySqlClient;

namespace Main
{
  partial class sql
  {
    public static MySqlDataReader get_host(Guid uuid, MySqlConnection conn)
    {
      string sql_str = String.Format("SELECT * FROM hosts WHERE Uuid = '{0}';", uuid);
      MySqlCommand get_host = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_host.ExecuteReader();
      return res;
    }
    public static bool host_table_exists(Guid uuid, MySqlConnection conn)
    {
      MySqlDataReader res = sql.get_host(uuid, conn);
      bool val = res.Read();
      res.Close();
      return val;
    }
  }
}