using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

using NTrace.Attributes;

namespace NTrace.Services
{
  internal class PlainTextTraceObjectSerializer : ITraceObjectSerializer
  {
    protected TraceOptions Options
    {
      get;
    }

    public PlainTextTraceObjectSerializer(TraceOptions options)
    {
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

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

    private PropertyInfo[] GetProperties(object member)
    {
      if (member == null)
      {
        throw new ArgumentNullException(nameof(member));
      }

      return member.GetType().GetProperties();
    }

    private bool IsBasicType(Type type)
    {
      return _BasicTypes.Contains(type?.FullName);
    }

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

    private void SerializeMember(StringBuilder builder, string name, object value, int level, int maxLevel)
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
              if (oInfo.GetValue(value) is IEnumerable)
              {
                SerializeEnumeration(builder, oInfo.Name, oInfo.GetValue(value) as IEnumerable, level, maxLevel);
              }
              else
              {
                SerializeMember(builder, oInfo.Name, oInfo.GetValue(value), level, maxLevel);
              }
            }
          }
        }
      }
    }

    private void SerializeEnumeration(StringBuilder builder, string name, IEnumerable enumeration, int level, int maxLevel)
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
    private string FormatValue(string name, string value, int level)
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

    private string FormatValue(string name, object value, int level)
    {
      if (String.IsNullOrEmpty(name))
      {
        throw new ArgumentNullException(nameof(name));
      }

      if (value == null)
      {
        return FormatValue(name, (string)value, level);
      }
      else
      {
        if (value is DateTime time)
        {
          return FormatValue(name, time.ToIsoDateTimeString(true), level);
        }
        else if (value is String)
        {
          string sValue = Convert.ToString(value);
          bool bContainsDoubleQuotes = sValue.Contains("\"");
          bool bContainsSingleQuotes = sValue.Contains("'");

          if (bContainsSingleQuotes && bContainsDoubleQuotes)
          {
            return FormatValue(name, sValue, level);
          }
          else if(bContainsDoubleQuotes)
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
          return FormatValue(name, Convert.ToString(value), level);
        }
      }
    }

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
