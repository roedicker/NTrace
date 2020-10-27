using System;

using NLog;

namespace NTrace.Adapters
{
  public class NLogAdapter : ITracer
  {
    public ILogger Logger
    {
      get;
    }

    public NLogAdapter(ILogger logger)
    {
      this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void EndWrite()
    {
      // not supported by NLog
    }

    public void Error(string message)
    {
      this.Logger.Error(message);
    }

    public void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      if ((categories & TraceCategories.Debug) == TraceCategories.Debug)
      {
        this.Logger.Debug(message);
      }
      else
      {
        this.Logger.Info(message);
      }
    }

    public void Warn(string message)
    {
      this.Logger.Warn(message);
    }
  }
}
