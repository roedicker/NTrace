using System;
using System.Security;

namespace NTrace.Tests.Data
{
  public class TraceDataParent
  {
    public TraceDataChild1 Details
    {
      get;
      set;
    }

    public int iValue
    {
      get;
      set;
    }

    public long lValue
    {
      get;
      set;
    }

    public string Password
    {
      get;
      set;
    }
  }
}
