using MySql.Data.MySqlClient;

namespace Main
{
  partial class sql
  {
    public int create_template(template_virtual_machine template)
    {
      string arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
      string sql_str = String.Format("INSERT INTO vm_templates (Uuid, FriendlyName, Memory, Vcpus, Arch) VALUES ('{0}', '{1}', {2}, {3}, '{4}');", template.id, template.friendly_name, template.memory, template.vcpus, arch);

      MySqlCommand create_template = new MySqlCommand(sql_str, conn);
      int res = create_template.ExecuteNonQuery();

      return res;

    }

    public void remove_template(Guid template_id)
    {
      // stub
    }

    public void get_templates(Guid template_id)
    {

    }
  }
}
