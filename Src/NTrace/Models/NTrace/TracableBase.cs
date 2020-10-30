using System;

namespace NTrace
{
  /// <summary>
  /// Base class of a traceable class
  /// </summary>
  public abstract class TraceableBase : ITraceable
  {
    /// <summary>
    /// Gets or sets the trace service
    /// </summary>
    public virtual ITraceService? TraceService
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new instance of the base traceable class
    /// </summary>
    protected TraceableBase()
    {
      this.TraceService = null;
    }

    /// <summary>
    /// Creates a new instance of the base traceable class with initial trace service
    /// </summary>
    /// <param name="traceService">Trace service</param>
    protected TraceableBase(ITraceService traceService)
    {
      this.TraceService = traceService ?? throw new ArgumentNullException(nameof(traceService));
    }

    /// <summary>
    /// Write error message to all related tracers.
    /// </summary>
    /// <param name="message">Message to write</param>
    public virtual void Error(string message)
    {
      this.TraceService?.Error(message);
    }

    /// <summary>
    /// Write error warning to all related tracers.
    /// </summary>
    /// <param name="message">Message to write</param>
    public virtual void Warn(string message)
    {
      this.TraceService?.Warn(message);
    }

    /// <summary>
    /// Write information message to all related tracers.
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="categories">Optional. Trace category of this message. Default value is <see cref="TraceCategories.Debug"/></param>
    public virtual void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      this.TraceService?.Info(message, categories);
    }
  }
}
