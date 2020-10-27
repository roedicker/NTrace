namespace NTrace
{
  public interface ITraceable
  {
    ITraceService TraceService
    {
      get;
      set;
    }

    void Error(string message);
    void Warn(string message);
    void Info(string message, TraceCategories categories = TraceCategories.Debug);
  }
}
