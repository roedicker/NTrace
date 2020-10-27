using System;

namespace NTrace.Services
{
  public delegate void TraceAction (string message);
  public delegate void TraceCategorizedAction(string message, TraceCategories categories);

  public class DelegateTracer : ITracer
  {
    public TraceAction ErrorTraceAction
    {
      get;
    }

    public TraceAction WarnTraceAction
    {
      get;
    }

    public TraceCategorizedAction InfoTraceAction
    {
      get;
    }

    public DelegateTracer(TraceAction errorAction, TraceAction warnAction, TraceCategorizedAction infoAction)
    {
      this.ErrorTraceAction = errorAction;
      this.WarnTraceAction = warnAction;
      this.InfoTraceAction = infoAction;
    }

    public void EndWrite()
    {
      // not used
    }

    public void Error(string message)
    {
      this.ErrorTraceAction?.Invoke(message);
    }

    public void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      this.InfoTraceAction?.Invoke(message, categories);
    }

    public void Warn(string message)
    {
      this.WarnTraceAction?.Invoke(message);
    }
  }
}
