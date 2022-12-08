using MySql.Data.MySqlClient;

namespace Main
{

  partial class sql
  {
    public void create_vm(Guid template_id)
    {
      // stub
      // create_vm(template mem, template vcpus)
      // set template id
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