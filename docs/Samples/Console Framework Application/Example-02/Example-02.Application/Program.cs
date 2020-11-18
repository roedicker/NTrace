using System;

using Autofac;

using NLog;

using NTrace;
using NTrace.Adapters;
using NTrace.Services;

using Example_02.Common.Services;
using Example_02.Core;

namespace Example_02
{
  internal static class Program
  {
    static private IContainer IocContainer;

    internal static void Main(string[] args)
    {
      try
      {
        SetupDependencies();
        SetupTraceManagement();

        IApplicationService Application = IocContainer.Resolve<IApplicationService>();
        ITraceService TraceService = IocContainer.Resolve<ITraceService>();

        try
        {
          Application.BeginApplication(args);
          Application.ExecuteApplication();
        }
        catch (Exception ex)
        {
          TraceService.Error(ex.Message);
        }
        finally
        {
          try
          {
            Application.EndApplication();
          }
          finally
          {
            TraceService.EndWrite();
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private static void SetupDependencies()
    {
      ContainerBuilder oBuilder = new ContainerBuilder();

      RegisterTypes(oBuilder);

      IocContainer = oBuilder.Build();
    }

    private static void RegisterTypes(ContainerBuilder builder)
    {
      if (builder == null)
      {
        throw new ArgumentNullException(nameof(builder));
      }

      builder.RegisterType<DefaultTraceManagementService>().As<ITraceManagementService>().SingleInstance();
      builder.RegisterType<DefaultTraceService>().As<ITraceService>().SingleInstance();
      builder.RegisterType<ApplicationDefaultService>().As<IApplicationService>().SingleInstance();
    }

    private static void SetupTraceManagement()
    {
      ITraceManagementService oManagementService = IocContainer.Resolve<ITraceManagementService>();

      // add NLog tracer - configuration is done in app.config
      oManagementService.AddTracer(new NLogAdapter(LogManager.GetCurrentClassLogger()));

      // allow all types of messages to be written
      oManagementService.Categories = TraceCategories.All;
    }
  }
}
