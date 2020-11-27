using System;

namespace NTrace
{
  /// <summary>
  /// Defines the trace options of a tracer
  /// </summary>
  public class TraceOptions
  {
    /// <summary>
    /// Gets or sets an indicator whether indention shall be used or not for trace messages of called sub-routines (using &quot;BeginMethod()&quot; and &quot;EndMethod()&quot;).
    /// </summary>
    public bool UseIndention
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the indention width of trace messages for called sub-routines (using &quot;BeginMethod()&quot; and &quot;EndMethod()&quot;)
    /// </summary>
    public int IndentionWidth
    {
      get
      {
        return _IndentionWidth;
      }

      set
      {
        if (value < 0)
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Indention width must not be negative");
        }

        _IndentionWidth = value;
      }
    }

    /// <summary>
    /// Gets or sets an indicator whether called methods shall be traced by using &quot;BeginMethod()&quot; and &quot;EndMethod()&quot; or not
    /// </summary>
    public bool UseMethodBlocks
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets an indicator whether runtime-statistics of called methods shall be traced by using &quot;BeginMethod()&quot; and &quot;EndMethod()&quot; or not
    /// </summary>
    public bool UseMethodDurations
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the inspection depth
    /// </summary>
    public int InspectionDepth
    {
      get
      {
        return _DefaultInspectionDepth;
      }
      set
      {
        if (value < 0)
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Inspection depth must not be negative");
        }

        if (value > _MaxInspectionDepth)
        {
          throw new ArgumentOutOfRangeException(nameof(value), $"Inspection depth must not be greater than {_MaxInspectionDepth}");
        }

        _DefaultInspectionDepth = value;
      }
    }

    /// <summary>
    /// Creates a new instance of trace options
    /// </summary>
    public TraceOptions()
    {
      _DefaultInspectionDepth = 2;
      _IndentionWidth = 2;

      this.UseIndention = true;
      this.UseMethodBlocks = true;
      this.UseMethodDurations = false;
    }

    private int _IndentionWidth;
    private int _DefaultInspectionDepth;

    private readonly int _MaxInspectionDepth = 5;
  }
}
