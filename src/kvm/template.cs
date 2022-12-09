namespace Main
{
  public class template_virtual_machine
  {
    public ulong memory { get; set; }// bytes
    public uint vcpus { get; set; }
    public string friendly_name { get; set; }
    public Guid id { get; }

    public template_virtual_machine(ulong _memory, uint _vcpus)
    {
      create_class(_memory, _vcpus);
      friendly_name = String.Format("virtual machine with {0} mebibytes and {1} vcpus.", _memory, vcpus);
      id = Guid.NewGuid();
    }

    public template_virtual_machine(ulong _memory, uint _vcpus, string _name)
    {
      create_class(_memory, _vcpus);
      friendly_name = _name;
      id = Guid.NewGuid();
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