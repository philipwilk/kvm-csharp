namespace Main
{
  public class template_virtual_machine
  {
    public ulong memory { get; set; }// bytes
    public uint vcpus { get; set; }
    public string friendly_name { get; set; }
    public string arch { get; set; }
    public Guid id { get; }

    public template_virtual_machine(ulong _memory, uint _vcpus)
    {
      create_class(_memory, _vcpus);
      arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
      friendly_name = String.Format("virtual machine with {0} mebibytes and {1} vcpus", _memory, vcpus);
      id = Guid.NewGuid();
    }

    public template_virtual_machine(ulong _memory, uint _vcpus, string _name)
    {
      create_class(_memory, _vcpus);
      arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
      friendly_name = _name;
      id = Guid.NewGuid();
    }
    public template_virtual_machine(template_virtual_machine template)
    {
      memory = template.memory;
      vcpus = template.vcpus;
      friendly_name = template.friendly_name;
      arch = template.arch;
      id = Guid.NewGuid();
    }

    public template_virtual_machine(template_virtual_machine template, string name)
    {
      memory = template.memory;
      vcpus = template.vcpus;
      friendly_name = name;
      arch = template.arch;
      id = Guid.NewGuid();
    }


    public template_virtual_machine(Guid uuid, ulong _memory, uint _vcpus, string _name, string _arch)
    {
      memory = _memory;
      vcpus = _vcpus;
      arch = _arch;
      friendly_name = _name;
      id = uuid;
    }

    protected void create_class(ulong _memory, uint _vcpus)
    {
      if (_memory < 1)
      {
        throw new less_than_one_memory(); // input memory value as mebibytes, then convert to bytes
      }
      memory = _memory * 1048576; // bytes to mebis
      if (_vcpus < 1)
      {
        throw new less_than_one_vcpu();
      }
      vcpus = _vcpus;
    }
  }
}