using System;
using System.Collections.Generic;

using NTrace.Attributes;

namespace NTrace.Tests.Data
{
  public class TraceDataChild1
  {
    public string Name
    {
      get;
      set;
    }

    public bool IsActive
    {
      get;
      set;
    }

    public List<TraceDataChild2> Items
    {
      get;
      set;
    }

    [DoNotTrace()]
    public TraceDataChild1 Child
    {
      get;
      set;
    }

#pragma warning disable CA1819 // Properties should not return arrays
    public byte[] Data
#pragma warning restore CA1819 // Properties should not return arrays
    {
      get;
      set;
    }
  }
}
