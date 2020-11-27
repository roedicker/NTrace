using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NTrace
{
  internal static class EnumExtensions
  {
    public static string GetDisplayName(this Enum enumValue)
    {
      MemberInfo oInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

      if(oInfo == null)
      {
        return enumValue.ToString();
      }
      else
      {
        Attribute oAttribute = oInfo.GetCustomAttribute(typeof(DisplayAttribute), false);

        if(oAttribute == null)
        {
          return enumValue.ToString();
        }
        else
        {
          return ((DisplayAttribute)oAttribute).GetName();
        }
      }
    }
  }
}
