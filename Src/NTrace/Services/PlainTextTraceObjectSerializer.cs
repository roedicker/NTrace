using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

using NTrace.Attributes;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("NTrace.Tests")]

namespace NTrace.Services
{
  /// <summary>
  /// Defines the plain-text object serializer
  /// </summary>
  internal class PlainTextTraceObjectSerializer : ITraceObjectSerializer
  {
    /// <summary>
    /// Gets the trace options
    /// </summary>
    protected TraceOptions Options
    {
      get;
    }

    /// <summary>
    /// Creates a new instance of the plain-text object serializer
    /// </summary>
    /// <param name="options">Trace options</param>
    public PlainTextTraceObjectSerializer(TraceOptions options)
    {
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Serializes a named data object
    /// </summary>
    /// <param name="name">Name of the data to serialize</param>
    /// <param name="data">Data to serialize</param>
    /// <param name="maxDepth">Maximum depth of serialization</param>
    /// <returns>Serialized data</returns>
    public string Serialize(string name, object data, int? maxDepth = null)
    {
      StringBuilder Result = new StringBuilder();

      if (String.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      SerializeMember(Result, name, data, 0, maxDepth ?? this.Options.InspectionDepth);

      return Result.ToString();
    }

    /// <summary>
    /// Gets the properties of an object
    /// </summary>
    /// <param name="member">Object to analyze</param>
    /// <returns>Array of property information</returns>
    private PropertyInfo[] GetProperties(object member)
    {
      if (member == null)
      {
        throw new ArgumentNullException(nameof(member));
      }

      return member.GetType().GetProperties();
    }

    /// <summary>
    /// Gets an indicator whether a data-type is a basic one
    /// </summary>
    /// <param name="type">Data-type to analyze</param>
    /// <returns>Indicator whether a given data-type is abasic data-type or not</returns>
    private bool IsBasicType(Type type)
    {
      return _BasicTypes.Contains(type?.FullName);
    }

    /// <summary>
    /// Gets an indicator whether a property is secutity relevant or not
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="type">Data-type of the property</param>
    /// <returns>Indicator whether a property is security relevant or not</returns>
    private bool IsSecurityRelevantMember(string name, Type type)
    {
      if (name == null)
      {
        throw new ArgumentNullException(nameof(name));
      }

      if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(name, "password", CompareOptions.IgnoreCase) >= 0 ||
         CultureInfo.CurrentCulture.CompareInfo.IndexOf(name, "passphrase", CompareOptions.IgnoreCase) >= 0 ||
         CultureInfo.CurrentCulture.CompareInfo.IndexOf(name, "pwd", CompareOptions.IgnoreCase) >= 0 ||
         type == typeof(SecureString))
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Serializes a member
    /// </summary>
    /// <param name="builder">Builder for this serialization</param>
    /// <param name="name">Name of the member</param>
    /// <param name="value">Value of the member</param>
    /// <param name="level">Depth level</param>
    /// <param name="maxLevel">Maximum depth level</param>
    private void SerializeMember(StringBuilder builder, string name, object? value, int level, int maxLevel)
    {
      if (builder == null)
      {
        throw new ArgumentNullException(nameof(builder));
      }

      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      // skip member if tracing is prohibited
      if (value != null && value.GetType().GetCustomAttributes(typeof(DoNotTrace), true).FirstOrDefault() is DoNotTrace)
      {
        return;
      }

      if (value != null && IsSecurityRelevantMember(name, value.GetType()))
      {
        builder.Append(FormatValue(name, "<secret>", level));
      }
      else if (value == null || IsBasicType(value.GetType()) || value is Enum)
      {
        builder.Append(FormatValue(name, value, level));
      }
      else if (value is IEnumerable)
      {
        SerializeEnumeration(builder, name, value as IEnumerable, level, maxLevel);
      }
      else
      {
        if (level >= maxLevel)
        {
          builder.Append(FormatValue(name, "<object>", level));
        }
        else
        {
          builder.Append(FormatName(name, level));
          level++;

          foreach (PropertyInfo oInfo in GetProperties(value))
          {
            // skip member if tracing is prohibited
            if (oInfo.GetCustomAttributes(typeof(DoNotTrace), true).FirstOrDefault() is DoNotTrace)
            {
              continue;
            }

            // skip member if it is an indexed property
            if (oInfo.GetIndexParameters().Length > 0)
            {
              continue;
            }

            // write property value based on type
            if (IsSecurityRelevantMember(oInfo.Name, oInfo.PropertyType))
            {
              builder.Append(FormatValue(oInfo.Name, "<secret>", level));
            }
            else if (IsBasicType(oInfo.PropertyType))
            {
              builder.Append(FormatValue(oInfo.Name, oInfo.GetValue(value), level));
            }
            else
            {
              object oValue = oInfo.GetValue(value);

              if (oValue is IEnumerable)
              {
                SerializeEnumeration(builder, oInfo.Name, oValue as IEnumerable, level, maxLevel);
              }
              else
              {
                SerializeMember(builder, oInfo.Name, oValue, level, maxLevel);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Serialzes an enumeration
    /// </summary>
    /// <param name="builder">Builder for this serialization</param>
    /// <param name="name">Name of the enumeration</param>
    /// <param name="enumeration">Enumeration value</param>
    /// <param name="level">Depth level</param>
    /// <param name="maxLevel">Maximum depth level</param>
    private void SerializeEnumeration(StringBuilder builder, string name, IEnumerable? enumeration, int level, int maxLevel)
    {
      if (builder == null)
      {
        throw new ArgumentNullException(nameof(builder));
      }

      if (enumeration == null)
      {
        builder.Append(FormatValue(name, null, level));
      }
      else
      {
        int iIndex = 0;
        int iCount = 0;

        // iterate enumeration to determine number of objects
        foreach (object item in enumeration)
        {
          iCount++;
        }

        int iIndention = GetIndention(level);

        if (iIndention < _IndentCharacter.Length)
        {
          builder.Append($"{name}: [{iCount}]{Environment.NewLine}");
        }
        else
        {
          builder.Append($"{_IndentCharacter.PadLeft(iIndention)}{name}: [{iCount}]{Environment.NewLine}");
        }

        // indent items
        level++;

        if (level <= maxLevel)
        {
          foreach (object oMember in enumeration)
          {
            // iterate through the first 1000 items only
            if (iIndex > 999)
            {
              break;
            }

            string sIndex = $"[{iIndex++:000}]";
            SerializeMember(builder, sIndex, oMember, level, maxLevel);
          }
        }
      }
    }

    /// <summary>
    /// Formats a name by its depth level
    /// </summary>
    /// <param name="name">Name to format</param>
    /// <param name="level">Depth level</param>
    /// <returns>Formatted name by given depth level</returns>
    private string FormatName(string name, int level)
    {
      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      int iIndention = GetIndention(level);

      if (iIndention < _IndentCharacter.Length)
      {
        return $"{name}:{Environment.NewLine}";
      }
      else
      {
        return $"{_IndentCharacter.PadLeft(iIndention)}{name}:{Environment.NewLine}";
      }
    }
    private string FormatValue(string name, string? value, int level)
    {
      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      if (value == null)
      {
        value = "<null>";
      }
      else if (string.IsNullOrEmpty(value))
      {
        value = "<empty>";
      }

      int iIndention = GetIndention(level);

      if (iIndention < _IndentCharacter.Length)
      {
        return $"{name}: {value}{Environment.NewLine}";
      }
      else
      {
        return $"{_IndentCharacter.PadLeft(iIndention)}{name}: {value}{Environment.NewLine}";
      }
    }

    /// <summary>
    /// Formats a property value by its name and depth level
    /// </summary>
    /// <param name="name">Name of the property</param>
    /// <param name="value">Property value</param>
    /// <param name="level">Depth level</param>
    /// <returns>Formatted property value by given name and depth level</returns>
    private string FormatValue(string name, object? value, int level)
    {
      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      if (value == null)
      {
        return FormatValue(name, value as string, level);
      }
      else
      {
        if (value is DateTime time)
        {
          return FormatValue(name, time.ToIsoDateTimeString(true), level);
        }
        else if (value is string)
        {
          string sValue = Convert.ToString(value, CultureInfo.InvariantCulture);
          bool bContainsDoubleQuotes = sValue.Contains("\"");
          bool bContainsSingleQuotes = sValue.Contains("'");

          if (bContainsSingleQuotes && bContainsDoubleQuotes)
          {
            return FormatValue(name, sValue, level);
          }
          else if (bContainsDoubleQuotes)
          {
            return FormatValue(name, $@"'{sValue}'", level);
          }
          else
          {
            return FormatValue(name, $@"""{sValue}""", level);
          }
        }
        else
        {
          return FormatValue(name, Convert.ToString(value, CultureInfo.InvariantCulture), level);
        }
      }
    }

    /// <summary>
    /// Gets the current idention
    /// </summary>
    /// <param name="level">Depth level</param>
    /// <returns>Current indention by options and given depth level</returns>
    private int GetIndention(int level)
    {
      return level * this.Options.IndentionWidth;
    }

    private readonly string _IndentCharacter = " ";
    private readonly string[] _BasicTypes = new string[] {"System.Boolean", "System.Byte", "System.SByte", "System.Char", "System.Decimal",
                                                          "System.Double", "System.Single", "System.Int32", "System.UInt32", "System.Int64",
                                                          "System.UInt64", "System.Int16", "System.UInt16", "System.String",
                                                          "System.DateTime"};
  }
}
