namespace Main
{

  class kvm
  {
    public static void Main()
    {
      List<String> modifiers = new List<String>();
      List<log> notices = logging.get_parsed_preflights(log.Severity.Info, modifiers);
      if (notices.Count == 0)
      {
        Console.WriteLine("No notices from pfcs to display");
      }
      else
      {
        foreach (log i in notices)
        {
          Console.WriteLine("{0}, {1}", i.get_desc(), i.get_id());
        }
      }
    }
  }
}