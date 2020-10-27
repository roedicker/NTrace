using System;
using System.Globalization;

namespace NTrace
{
  internal static class Number
  {
    public static bool IsNumeric(object Expression)
    {
      return Double.TryParse(Convert.ToString(Expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _);
    }
  }
}
