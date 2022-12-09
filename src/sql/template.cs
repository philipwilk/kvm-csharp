using MySql.Data.MySqlClient;

namespace Main
{
  partial class sql
  {
    public int create_template(template_virtual_machine template)
    {
      string sql_str = String.Format("INSERT INTO vm_templates (Uuid, FriendlyName, Memory, Vcpus, Arch) VALUES ('{0}', '{1}', {2}, {3}, '{4}');", template.id, template.friendly_name, template.memory, template.vcpus, template.arch);

      MySqlCommand create_template = new MySqlCommand(sql_str, conn);
      int res = create_template.ExecuteNonQuery();

      return res;
    }

    public void remove_template(Guid template_id)
    {
      // stub
    }

    public static MySqlDataReader get_templates(MySqlConnection conn)
    {
      string sql_str = String.Format("SELECT * FROM vm_templates");
      MySqlCommand get_templates = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_templates.ExecuteReader();
      return res;
    }

    public static MySqlDataReader get_template(MySqlConnection conn, Guid uuid)
    {
      string sql_str = String.Format("SELECT * FROM vm_templates WHERE Uuid = '{0}'", uuid);
      MySqlCommand get_templates = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_templates.ExecuteReader();
      return res;
    }
    public static MySqlDataReader get_template(MySqlConnection conn, string name)
    {
      string sql_str = String.Format("SELECT * FROM vm_templates WHERE FriendlyName = '{0}'", name);
      MySqlCommand get_templates = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_templates.ExecuteReader();
      return res;
    }
  }
}
