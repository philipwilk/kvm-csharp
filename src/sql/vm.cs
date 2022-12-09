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

    public void create_vm(ulong memory, short vcpus)
    {
      // stub
      // template id = null
    }

    public void remove_vm(Guid vm_id)
    {
      // stub
    }
  }
}