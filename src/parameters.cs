using System.Text.RegularExpressions;

namespace Main
{
  class param
  {
    private static Regex reg_equals = new Regex("^--[A-z]+=[^ \t\n\r\f\v]+");
    private static Regex reg_space = new Regex("^--[A-z]+");
    private static Regex reg_boolean = new Regex("^-[A-z]+");
    public enum key_format
    {
      Equals,
      Space,
      Boolean
    }

    // When a new parameter is necessary, add it here...
    public enum parameters
    {
      NoPreflights,
      UserLogSeverityLevel,
      LogFilters,
      sqlUser,
      sqlPassword
    }

    // And, add a case for the parameter - whether it is a boolean parameter that is true or false (true) or has a user defined value (false) 
    private static bool param_is_bool_type(param.parameters id)
    {
      switch (id)
      {
        case param.parameters.NoPreflights:
          {
            return true;
          }
        case param.parameters.UserLogSeverityLevel:
          {
            return false;
          }
        case param.parameters.LogFilters:
          {
            return false;
          }
        case param.parameters.sqlPassword:
          {
            return false;
          }
        case param.parameters.sqlUser:
          {
            return false;
          }
        default: return false;
      }
    }

    public static IDictionary<param.parameters, String> get_parameters(string[] raw_args)
    {
      var raw_arg_q = new Queue<String>();
      foreach (var raw_parameter in raw_args)
      {
        raw_arg_q.Enqueue(raw_parameter);
      }

      var arg_dict = new Dictionary<param.parameters, String>();
      while (raw_arg_q.Count > 0)
      {
        var front = raw_arg_q.Dequeue();
        param.key_format key_format;

        if (reg_equals.IsMatch(front))
        {
          key_format = param.key_format.Equals;
        }
        else if (reg_space.IsMatch(front))
        {
          key_format = param.key_format.Space;
        }
        else if (reg_boolean.IsMatch(front))
        {
          key_format = param.key_format.Boolean;
        }
        else
        {
          throw new ArgumentException(String.Format("Invalid parameter format {0}", front));
        }
        var equals_val = "";
        var parsed_front_key = "";
        if (key_format == param.key_format.Equals)
        {
          var vals = front.Split("=");
          equals_val = vals[1];
          parsed_front_key = vals[0];
        }
        else
        {
          parsed_front_key = front;
        }
        parsed_front_key = parsed_front_key.Replace("-", "");

        // convert front to a parameter or error the program 
        param.parameters front_key;
        if (!Enum.TryParse<param.parameters>(parsed_front_key, true, out front_key))
        {
          throw new ArgumentException(String.Format("Invalid parameter {0}", parsed_front_key));
        }

        if (key_format == key_format.Space && !param_is_bool_type(front_key) && raw_arg_q.Count < 1
        | raw_arg_q.Count > 0 && reg_equals.IsMatch(raw_arg_q.Peek()) | reg_space.IsMatch(raw_arg_q.Peek()) | reg_boolean.IsMatch(raw_arg_q.Peek()))
        {
          throw new ArgumentException(String.Format("Space formatted parameter without value: {0}", parsed_front_key));
        }
        else if (key_format == param.key_format.Space && param_is_bool_type(front_key))
        {
          if (raw_arg_q.Count > 0 && !reg_equals.IsMatch(raw_arg_q.Peek()) && !reg_space.IsMatch(raw_arg_q.Peek()) && !reg_boolean.IsMatch(raw_arg_q.Peek()))
          {
            equals_val = raw_arg_q.Dequeue();
            if (equals_val.ToLower() == "true")
            {
              equals_val = "True";
            }
            else if (equals_val.ToLower() == "false")
            {
              equals_val = "False";
            }
            else
            {
              throw new ArgumentException(String.Format("Use of equals formatted binary parameter with invalid value: {0}", parsed_front_key));
            }
          }
          else
          {
            equals_val = "True";
          }
        }
        else if (key_format == param.key_format.Space)
        {
          equals_val = raw_arg_q.Dequeue();
        }

        if (key_format == param.key_format.Boolean && !param_is_bool_type(front_key))
        {
          throw new ArgumentException(String.Format("Use of key+value parameter as boolean parameter: {0}", parsed_front_key));
        }
        if (key_format == param.key_format.Equals && param_is_bool_type(front_key))
        {
          if (equals_val.ToLower() == "true")
          {
            equals_val = "True";
          }
          else if (equals_val.ToLower() == "false")
          {
            equals_val = "False";
          }
        }
        if (key_format == param.key_format.Boolean)
        {
          equals_val = "True";
        }

        arg_dict.Add(front_key, equals_val);
      }
      return arg_dict;
    }
  }
}
