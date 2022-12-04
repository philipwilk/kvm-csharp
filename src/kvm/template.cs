namespace Main
{
  public class template_virtual_machine
  {
    protected uint memory { get; set; }// bytes
    protected uint vcpus { get; set; }
    protected string name { get; set; }
    protected Guid id { get; }

    public template_virtual_machine(uint _memory, uint _vcpus)
    {
      create_class(_memory, _vcpus);
      name = String.Format("virtual machine with {0} mebibytes and {1} vcpus.", _memory, vcpus);
      id = new Guid();
    }

    public template_virtual_machine(uint _memory, uint _vcpus, string _name)
    {
      create_class(_memory, _vcpus);
      name = _name;
      id = new Guid();
    }

    protected void create_class(uint _memory, uint _vcpus)
    {
      if (memory < 1)
      {
        throw new less_than_one_memory(); // input memory value as mebibytes, then convert to bytes
      }
      memory = _memory * 1048576; // bytes to mebis
      if (vcpus < 1)
      {
        throw new less_than_one_vcpu();
      }
      vcpus = _vcpus;
    }
  }
}