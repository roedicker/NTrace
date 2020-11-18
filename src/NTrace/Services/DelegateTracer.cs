using System;

namespace NTrace.Services
{
  /// <summary>
  /// Signature of a trace action
  /// </summary>
  /// <param name="message">Message to process</param>
  public delegate void TraceAction(string message);

  /// <summary>
  /// Signature of a trace category action
  /// </summary>
  /// <param name="message">Message to process</param>
  /// <param name="categories">Categories of message</param>
  public delegate void TraceCategorizedAction(string message, TraceCategories categories);

  /// <summary>
  /// Defines a delegate tracer
  /// </summary>
  public class DelegateTracer : ITracer
  {
    /// <summary>
    /// Gets the trace action for error messages
    /// </summary>
    public TraceAction ErrorTraceAction
    {
      get;
    }

    /// <summary>
    /// Gets the trace action for warning messages
    /// </summary>
    public TraceAction WarnTraceAction
    {
      get;
    }

    /// <summary>
    /// Gets the trace action for informatio nmessages
    /// </summary>
    public TraceCategorizedAction InfoTraceAction
    {
      get;
    }

    /// <summary>
    /// Creates a new instance of a delegate tracer
    /// </summary>
    /// <param name="errorAction">Action for error messages</param>
    /// <param name="warnAction">Action for warning messages</param>
    /// <param name="infoAction">Action for information messages</param>
    public DelegateTracer(TraceAction errorAction, TraceAction warnAction, TraceCategorizedAction infoAction)
    {
      this.ErrorTraceAction = errorAction;
      this.WarnTraceAction = warnAction;
      this.InfoTraceAction = infoAction;
    }

    /// <summary>
    /// Signals end of writing messages
    /// </summary>
    public void EndWrite()
    {
      // not used
    }

    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Error(string message)
    {
      this.ErrorTraceAction?.Invoke(message);
    }

    /// <summary>
    /// Writes an information message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="categories">Categories of message</param>
    public void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      this.InfoTraceAction?.Invoke(message, categories);
    }

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Warn(string message)
    {
      this.WarnTraceAction?.Invoke(message);
    }
  }
}
