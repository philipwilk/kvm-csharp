using MySql.Data.MySqlClient;

namespace Main
{

  partial class sql
  {
    public int create_vm(virtual_machine vm)
    {
      string sql_str;
      if (vm.template_id == null)
      {
        sql_str = String.Format("INSERT INTO vms (Uuid, FriendlyName, Memory, Vcpus, Arch) VALUES ('{0}', '{1}', {2}, {3}, '{4}');", vm.id, vm.friendly_name, vm.memory, vm.vcpus, vm.arch);
      }
      else
      {
        sql_str = String.Format("INSERT INTO vms (Uuid, FriendlyName, Memory, Vcpus, Template, Arch) VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}');", vm.id, vm.friendly_name, vm.memory, vm.vcpus, vm.template_id, vm.arch);
      }

      MySqlCommand create_vm = new MySqlCommand(sql_str, conn);
      int res = create_vm.ExecuteNonQuery();

      return res;
    }

    public MySqlDataReader get_vm(MySqlConnection conn, Guid id)
    {
      string sql_str = String.Format("SELECT * FROM vms WHERE Uuid = '{0}'", id);
      MySqlCommand get_vm = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_vm.ExecuteReader();
      return res;
    }

    public MySqlDataReader get_vm(MySqlConnection conn, string name)
    {
      string sql_str = String.Format("SELECT * FROM vm_templates WHERE FriendlyName = '{0}'", name);
      MySqlCommand get_vm = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_vm.ExecuteReader();
      return res;
    }

    public MySqlDataReader get_vms(MySqlConnection conn)
    {
      string sql_str = String.Format("SELECT * FROM vms");
      MySqlCommand get_vms = new MySqlCommand(sql_str, conn);
      MySqlDataReader res = get_vms.ExecuteReader();
      return res;
    }

    public void remove_vm(Guid vm_id)
    {
      // stub
    }
  }
}