using System;
using System.Globalization;

namespace NTrace
{
  /// <summary>
  /// Provides analyzing methods for numerics
  /// </summary>
  internal static class Number
  {
    /// <summary>
    /// Gets an indicator wether a value is a numeric or not
    /// </summary>
    /// <param name="value">Value to analyze</param>
    /// <returns>Indicator whether a value is numeric or not</returns>
    public static bool IsNumeric(object value)
    {
      return Double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _);
    }
  }
}
