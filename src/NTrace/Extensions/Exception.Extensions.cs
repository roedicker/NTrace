using System;
using System.Collections.Generic;
using System.Text;

namespace NTrace
{
  internal static class ExceptionExtensions
  {
    public static string GetMessageStackString(this Exception exception, bool includeStackTrace = false, string delimeter = "\n")
    {
      StringBuilder sbResult = new StringBuilder();
      Exception oException = exception;

      while (oException != null)
      {
        if (sbResult.Length > 0 && !sbResult.ToString().EndsWith(delimeter, StringComparison.InvariantCulture))
        {
          sbResult.Append(delimeter);
        }

        // add exception message if not an aggregated exception
        if (oException as AggregateException == null)
        {
          sbResult.Append(oException.Message);
        }

        oException = oException.InnerException;
      }

      if (includeStackTrace)
      {
        sbResult.Append($"\n\n{exception.StackTrace}"); // NOSONAR
      }

      return sbResult.ToString();
    }

    public static IEnumerable<string> GetMessageStackStrings(this Exception exception, bool includeStackTrace = false)
    {
      List<string> Result = new List<string>();
      Exception oException = exception;

      while (oException != null)
      {
        // add exception message if not an aggregated exception
        if (oException as AggregateException == null)
        {
          Result.Add(oException.Message);
        }

        oException = oException.InnerException;
      }

      if (includeStackTrace)
      {
        Result.Add(exception.StackTrace); // NOSONAR
      }

      return Result;
    }
  }
}
