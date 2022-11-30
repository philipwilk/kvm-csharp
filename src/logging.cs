namespace Main
{
  class logging
  {
    static List<log> get_preflights(int kvm_fd)
    {
      List<log> results = new List<log>();

      // Check for kvm basic api presence
      results.Add(preFlights.has_kvm_extension(kvm_fd));

      return results;
    }

    public static List<log> parse_logs_by_severity(List<log> logs, log.Severity level, List<String> modifiers)
    {
      List<log> notices = new List<log>();
      foreach (log log in logs)
      {
        // Only add tuple to notice if level <= the desired level and isnt being manually filtered out
        if (log.get_level() <= level && !(modifiers.Contains("-" + log.get_id())) && !(modifiers.Contains("-all") && !(modifiers.Contains("+" + log.get_id()))))
        {
          notices.Add(log);
        }
      }
      return notices;
    }

    public static List<log> get_parsed_preflights(log.Severity level, List<String> modifiers)
    {
      return parse_logs_by_severity(get_preflights(kvm.get_kvm_fd()), level, modifiers);
    }
  }
}