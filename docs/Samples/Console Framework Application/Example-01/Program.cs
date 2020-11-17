using System;

using NTrace;
using NTrace.Services;

namespace Example_01
{
  public static class Program
  {
    private static ITraceManagementService TraceManagementService;
    private static ITraceService TraceService;

    public static void Main(string[] args)
    {
      BeginApplication();

      ExecuteApplication();

      EndApplication();
    }

    private static void BeginApplication()
    {
      // initialize trace services
      TraceManagementService = new DefaultTraceManagementService();
      TraceService = new DefaultTraceService(TraceManagementService);

      // add tracers (e.g. console tracer)
      TraceManagementService.AddTracer(new ConsoleTracer());

      // configure tracing (e.g. trace all kind of messages)
      TraceManagementService.Categories = TraceCategories.All;
    }

    private static void EndApplication()
    {
      // signal end of tracing
      TraceService.EndWrite();
    }

    private static void ExecuteApplication()
    {
      TraceService.BeginMethod();

      try
      {
        TraceService.Info("This is the execution of the application");
      }
      finally
      {
        TraceService.EndMethod();
      }
    }
  }
}
