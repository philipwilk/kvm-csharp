namespace Main
{
  class commands
  {
    public void run_command(string[] raw_args)
    {
      var raw_arg_q = new Queue<String>();
      foreach (var raw_command in raw_args)
      {
        raw_arg_q.Enqueue(raw_command);
      }

      string command = raw_arg_q.Dequeue();

      switch (command.ToLower())
      {
        case "vm":
          {
            return;
          }
        case "template":
          {
            return;
          }
        case "info":
          {
            return;
          }
        case "vdisks":
          {
            return;
          }
        case "vdevs":
          {
            return;
          }
        case "hosts":
          {
            var x = new command_hosts(raw_arg_q);
            return;
          }
        default: return;
      }
    }
  }
  abstract class run
  {
    protected virtual string command_name { get; }
    public Queue<String> args { get; set; }

    public run(string _command_name, Queue<String> _args)
    {
      command_name = _command_name;
      args = _args;
    }

    public abstract void execute();
  }
}

