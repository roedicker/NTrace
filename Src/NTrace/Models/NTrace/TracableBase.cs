using System;

namespace NTrace
{
  public abstract class TraceableBase : ITraceable
  {
    public virtual ITraceService TraceService
    {
      get;
      set;
    }

    protected TraceableBase()
    {
      this.TraceService = null;
    }

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
