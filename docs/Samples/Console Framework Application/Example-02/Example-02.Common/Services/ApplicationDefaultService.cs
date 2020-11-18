using System;

using NTrace;

using Example_02.Core;

namespace Example_02.Common.Services
{
  public class ApplicationDefaultService : IApplicationService
  {
    protected ITraceService TraceService
    {
      get;
    }

    public ApplicationDefaultService(ITraceService traceService)
    {
      this.TraceService = traceService ?? throw new ArgumentNullException(nameof(traceService));
    }

    public void BeginApplication(string[] args)
    {
      this.TraceService.BeginMethod();

      try
      {
        this.TraceService.Info("Application has been started", TraceCategories.Method);
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }

    public void ExecuteApplication()
    {
      this.TraceService.BeginMethod();

      try
      {
        this.TraceService.Info("This is an info message", TraceCategories.Method);
        this.TraceService.Warn("This is a warning message");
        this.TraceService.Error("This is an error message");
        this.TraceService.Info("This is a debug message");

        // raise exception to be catched in the main program
        throw new Exception("This is an example exception");
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }

    public void EndApplication()
    {
      this.TraceService.BeginMethod();

      try
      {
        this.TraceService.Info("Application has been ended", TraceCategories.Method);
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }
  }
}
