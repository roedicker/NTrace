using System;

namespace NTrace.Tracers
{
  /// <summary>
  /// Defines the asynchronous console tracer
  /// </summary>
  public class AsyncConsoleTracer: ConsoleTracer, ITracer, IAsyncTracer
  {
    /// <summary>
    /// Creates a new instance of an asynchronous console tracer
    /// </summary>
    public AsyncConsoleTracer(): base()
    {
      // not used
    }
  }
}
