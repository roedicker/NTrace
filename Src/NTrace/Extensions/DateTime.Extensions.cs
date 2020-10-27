using System;

namespace NTrace
{
  internal static class DateTimeExtensions
  {
    /// <summary>
    /// Creates a string equal to the referred date value in ISO format.
    /// </summary>
    /// <param name="date">Date value to convert to string. Use Now() to use current date.</param>
    /// <returns>The referred date as ISO formatted string.</returns>
    /// <remarks>
    /// Official ISO format for dates is: &lt;YYYY&gt;-&lt;MM&gt;-&lt;DD&gt;
    /// <p>
    ///   <ul>
    ///     <li>YYYY - Year (4 digits)</li>
    ///     <li>MM - Month of year with leading 0</li>
    ///     <li>DD - Day of month with leading 0</li>
    ///   </ul>
    /// </p>
    /// </remarks>
    /// <example>
    /// Assumed: Call of this method at Friday, January the 13th of 2006.
    /// 
    /// <code>
    /// string sResult;
    /// 
    /// sResult = DateTime.Now.GetIsoDateString();
    /// </code>
    /// 
    /// The value of sResult is set to "2006-01-13".
    /// </example>
    public static string ToIsoDateString(this DateTime date)
    {
      return String.Format("{0:yyyy}-{0:MM}-{0:dd}", date);
    }


    /// <summary>
    /// Creates a string equal to the referred date value in W3CDTF format.
    /// </summary>
    /// <param name="date">Date value to convert to string. Use Now() to use current date.</param>
    /// <returns>The referred date as W3CDTF formatted string.</returns>
    /// <remarks>
    /// The result is equal to the ISO formatted string.
    /// </remarks>
    public static string ToW3CdtfDateString(this DateTime date)
    {
      return ToIsoDateString(date);
    }

    /// <summary>
    /// Creates a string equal to the referred date and time value in ISO format.
    /// </summary>
    /// <param name="date">Date and time value to convert to string. Use Now() to use current date and time.</param>
    /// <param name="useMilliseconds">Optional. Indicator whether milliseconds have to be used or not. Default value is <strong>false</strong>.</param>
    /// <returns>The referred date and time as ISO formatted string</returns>
    /// <remarks>
    /// Official ISO format for date/time combination is: &lt;YYYY&gt;-&lt;MM&gt;-&lt;DD&gt;&nbsp;&lt;hh&gt;:&lt;mm&gt;:&lt;ss&gt;
    /// <p>
    ///   <ul>
    ///     <li>YYYY - Year (4 digits)</li>
    ///     <li>MM - Month of year with leading 0</li>
    ///     <li>DD - Day of month with leading 0</li>
    ///     <li>hh - Hour (24h) with leading 0</li>
    ///     <li>mm - Minute with leading 0</li>
    ///     <li>ss - Second with leading 0</li>
    ///   </ul>
    /// </p>
    /// </remarks>
    /// <example>
    /// Assumed: Call of this method at Friday, January the 13th of 2006 at 10:46 and 18 seconds.
    /// 
    /// <code>
    /// string sResult;
    /// 
    /// sResult = DateTime.Now.GetIsoDateTimeString();
    /// </code>
    /// 
    /// The value of sResult is set to "2006-01-13 10:46:18".
    /// </example>
    public static string ToIsoDateTimeString(this DateTime date, bool useMilliseconds = false)
    {
      if (useMilliseconds)
      {
        return String.Format("{0:yyyy}-{0:MM}-{0:dd} {0:HH}:{0:mm}:{0:ss}.{0:fff}", date);
      }
      else
      {
        return String.Format("{0:yyyy}-{0:MM}-{0:dd} {0:HH}:{0:mm}:{0:ss}", date);
      }
    }

    /// <summary>
    /// Creates a string equal to the referred date and time value in W3CDTF format.
    /// </summary>
    /// <param name="dateTime">Local date and time value to convert to string. Use Now() to use current local date and time.</param>
    /// <param name="useUtcDesignator">Optional. Indicator whether using the UTC designator or not. Default value is <strong>true</strong></param>
    /// <param name="useMilliseconds">Optional. Indicator whether milliseconds have to be used or not. Default value is <strong>false</strong>.</param>
    /// <returns>The referred local date and time as W3CDTF formatted string.</returns>
    /// <remarks>
    /// Official W3CDTF format for date/time combination is: &lt;YYYY&gt;-&lt;MM&gt;-&lt;DD&gt;T&lt;hh&gt;:&lt;mm&gt;:&lt;ss&gt;[TZD]
    /// <p>
    ///   <ul>
    ///     <li>YYYY - Year (4 digits)</li>
    ///     <li>MM   - Month of year with leading 0</li>
    ///     <li>DD   - Day of month with leading 0</li>
    ///     <li>hh   - Hour (24h) with leading 0</li>
    ///     <li>mm   - Minute with leading 0</li>
    ///     <li>ss   - Second with leading 0</li>
    ///     <li>TZD  - Timezone designator (Z or +hh:mm or -hh:mm)</li>
    ///   </ul>
    /// </p>
    /// </remarks>
    public static string ToW3cdtfDateTimeString(this DateTime date, bool useUtcDesignator = true, bool useMilliseconds = false)
    {
      string Result;
      DateTime dtUtc = date.ToUniversalTime();
      int iTimeZoneDifference = date.Subtract(dtUtc).Hours;

      if (iTimeZoneDifference == 0)
      {
        if (useMilliseconds)
        {
          Result = String.Format("{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}.{0:fff}{1}", date, useUtcDesignator ? "Z" : "+00:00");
        }
        else
        {
          Result = String.Format("{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}{1}", date, useUtcDesignator ? "Z" : "+00:00");
        }
      }
      else
      {
        if (iTimeZoneDifference > 0)
        {
          if (useMilliseconds)
          {
            Result = String.Format("{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}.{0:fff}+{1:d02}:00", date, iTimeZoneDifference);
          }
          else
          {
            Result = String.Format("{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}+{1:d02}:00", date, iTimeZoneDifference);
          }

        }
        else
        {
          if (useMilliseconds)
          {
            Result = String.Format("{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}.{0:fff}{1:d02}:00", date, iTimeZoneDifference);
          }
          else
          {
            Result = String.Format("{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}{1:d02}:00", date, iTimeZoneDifference);
          }
        }
      }

      return Result;
    }

    /// <summary>
    /// Creates a string equal to the referred date (time) value in ISO format.
    /// </summary>
    /// <param name="date">Date (time part) value to convert to string. Use DateTime.Now to use current time.</param>
    /// <returns>The referred time as ISO formatted string.</returns>
    /// <remarks>
    /// Official ISO format for time is: &lt;hh&gt;:&lt;mm&gt;:&lt;ss&gt;
    /// <p>
    ///   <ul>
    ///     <li>hh - Hour (24h) with leading 0</li>
    ///     <li>mm - Minute with leading 0</li>
    ///     <li>ss - Second with leading 0</li>
    ///   </ul>
    /// </p>
    /// </remarks>
    /// <example>
    /// Assumed: Call of this method at Friday, January the 13th of 2006 at 10:46 and 18 seconds.
    /// 
    /// <code>
    /// string sResult;
    /// 
    /// sResult = DateTime.Now.GetIsoTimeString();
    /// </code>
    /// 
    /// The value of sResult is set to "10:46:18".
    /// </example>
    public static string ToIsoTimeString(this DateTime date, bool useMilliseconds = false)
    {
      if (useMilliseconds)
      {
        return String.Format("{0:HH}:{0:mm}:{0:ss}.fff", date);
      }
      else
      {
        return String.Format("{0:HH}:{0:mm}:{0:ss}", date);
      }
    }
  }
}
