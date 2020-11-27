using System;

namespace NTrace.Tracers
{
  /// <summary>
  /// Defines the asynchronous delegate tracer
  /// </summary>
  public class AsyncDelegateTracer : DelegateTracer, ITracer, IAsyncTracer
  {
    /// <summary>
    /// Creates a new instance of an asynchronous delegate tracer
    /// </summary>
    /// <param name="errorAction">Action for error messages</param>
    /// <param name="warnAction">Action for warning messages</param>
    /// <param name="infoAction">Action for information messages</param>
    public AsyncDelegateTracer(TraceAction errorAction, TraceAction warnAction, TraceCategorizedAction infoAction) : base(errorAction, warnAction, infoAction)
    {
      // not used
    }
  }
}
