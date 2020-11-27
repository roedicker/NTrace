using System;

namespace NTrace.Tracers
{
  /// <summary>
  /// Defines the asynchronous file-tracer
  /// </summary>
  public class AsyncFileTracer: FileTracer, ITracer, IAsyncTracer
  {
    /// <summary>
    /// Creates a new instance of an asynchronous file-tracer
    /// </summary>
    public AsyncFileTracer(): base()
    {
      // not used
    }
  }
}
