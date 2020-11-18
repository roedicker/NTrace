using System;
using System.Security;

namespace NTrace.Tests.Data
{
  public class TraceDataChild2
  {
    public int Index
    {
      get;
      set;
    }

    public string UserName
    {
      get;
      set;
    }

    public SecureString UserData
    {
      get;
      set;
    }

    public DateTime Timestamp
    {
      get;
      set;
    }
  }
}
