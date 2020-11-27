using System;

using NLog;

namespace NTrace.Adapters
{
  /// <summary>
  /// Defines the asynchronous NTrace adapter for using NLog
  /// </summary>
  public class NLogAsyncAdapter : NLogAdapter, ITracer, IAsyncTracer
  {
    /// <summary>
    /// Creates a new instance of the asynchronous NTrace adapter for NLog
    /// </summary>
    /// <param name="logger">NLog logger reference</param>
    public NLogAsyncAdapter(ILogger logger) : base(logger)
    {
      // not used;
    }
  }
}
