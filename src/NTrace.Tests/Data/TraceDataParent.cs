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

    public int IntegerValue
    {
      get;
      set;
    }

    public long LongValue
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
