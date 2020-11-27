using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NTrace.Services;
using NTrace.Tracers;

namespace NTrace.Tests
{
  [TestClass]
  public class DelegateTracerTests
  {
    [TestMethod]
    public void Async_Error_Message_Should_Be_Written()
    {
      string expected = String.Empty;
      string actual = String.Empty;

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();

      using (DefaultTraceService oTraceService = new DefaultTraceService(oManagementService))
      {
        AsyncDelegateTracer target = new AsyncDelegateTracer((message) => actual = message, null, null);
        oManagementService.AddTracer(target);

        string sMessage = $"This is unit test error message {Guid.NewGuid()}";
        expected = sMessage;

        oTraceService.Error(sMessage);
        oTraceService.EndWrite();
      }

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Error_Message_Should_Be_Written()
    {
      string expected = String.Empty;
      string actual = String.Empty;

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();
      DefaultTraceService oTraceService = new DefaultTraceService(oManagementService);
      DelegateTracer target = new DelegateTracer((message) => actual = message, null, null);
      oManagementService.AddTracer(target);

      string sMessage = $"This is unit test error message {Guid.NewGuid()}";
      expected = sMessage;

      oTraceService.Error(sMessage);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Async_Error_Message_Should_Not_Be_Written_For_Non_Set_Delegate()
    {
      string expected = String.Empty;
      string actual = $"Initial message {Guid.NewGuid()}";

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();

      using (DefaultTraceService oTraceService = new DefaultTraceService(oManagementService))
      {
        AsyncDelegateTracer target = new AsyncDelegateTracer(null, (message) => actual = message, (message, category) => actual = message);
        oManagementService.AddTracer(target);

        string sMessage = $"This is unit test error message {Guid.NewGuid()}";
        expected = actual;

        oTraceService.Error(sMessage);
        oTraceService.EndWrite();
      }

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Error_Message_Should_Not_Be_Written_For_Non_Set_Delegate()
    {
      string expected = String.Empty;
      string actual = $"Initial message {Guid.NewGuid()}";

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();
      DefaultTraceService oTraceService = new DefaultTraceService(oManagementService);
      DelegateTracer target = new DelegateTracer(null, (message) => actual = message, (message, category) => actual = message);
      oManagementService.AddTracer(target);

      string sMessage = $"This is unit test error message {Guid.NewGuid()}";
      expected = actual;

      oTraceService.Error(sMessage);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Async_Warning_Message_Should_Be_Written()
    {
      string expected = String.Empty;
      string actual = String.Empty;

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();

      using (DefaultTraceService oTraceService = new DefaultTraceService(oManagementService))
      {
        AsyncDelegateTracer target = new AsyncDelegateTracer(null, (message) => actual = message, null);
        oManagementService.AddTracer(target);

        string sMessage = $"This is unit test warning message {Guid.NewGuid()}";
        expected = sMessage;

        oTraceService.Warn(sMessage);
        oTraceService.EndWrite();
      }

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Warning_Message_Should_Be_Written()
    {
      string expected = String.Empty;
      string actual = String.Empty;

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();
      DefaultTraceService oTraceService = new DefaultTraceService(oManagementService);
      DelegateTracer target = new DelegateTracer(null, (message) => actual = message, null);
      oManagementService.AddTracer(target);

      string sMessage = $"This is unit test warning message {Guid.NewGuid()}";
      expected = sMessage;

      oTraceService.Warn(sMessage);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Async_Warning_Message_Should_Not_Be_Written_For_Non_Set_Delegate()
    {
      string expected = String.Empty;
      string actual = $"Initial message {Guid.NewGuid()}";

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();

      using (DefaultTraceService oTraceService = new DefaultTraceService(oManagementService))
      {
        AsyncDelegateTracer target = new AsyncDelegateTracer((message) => actual = message, null, (message, category) => actual = message);
        oManagementService.AddTracer(target);

        string sMessage = $"This is unit test warning message {Guid.NewGuid()}";
        expected = actual;

        oTraceService.Warn(sMessage);
        oTraceService.EndWrite();
      }

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Warning_Message_Should_Not_Be_Written_For_Non_Set_Delegate()
    {
      string expected = String.Empty;
      string actual = $"Initial message {Guid.NewGuid()}";

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();
      DefaultTraceService oTraceService = new DefaultTraceService(oManagementService);
      DelegateTracer target = new DelegateTracer((message) => actual = message, null, (message, category) => actual = message);
      oManagementService.AddTracer(target);

      string sMessage = $"This is unit test warning message {Guid.NewGuid()}";
      expected = actual;

      oTraceService.Warn(sMessage);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Async_Info_Message_Should_Be_Written()
    {
      string expected = String.Empty;
      string actual = String.Empty;

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();

      using (DefaultTraceService oTraceService = new DefaultTraceService(oManagementService))
      {
        AsyncDelegateTracer target = new AsyncDelegateTracer(null, null, (message, category) => actual = message);
        oManagementService.AddTracer(target);
        oManagementService.Categories = TraceCategories.Application;

        string sMessage = $"This is unit test info message {Guid.NewGuid()}";
        expected = sMessage;

        oTraceService.Info(sMessage, TraceCategories.Application);
        oTraceService.EndWrite();
      }

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Info_Message_Should_Be_Written()
    {
      string expected = String.Empty;
      string actual = String.Empty;

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();
      DefaultTraceService oTraceService = new DefaultTraceService(oManagementService);
      DelegateTracer target = new DelegateTracer(null, null, (message, category) => actual = message);
      oManagementService.AddTracer(target);
      oManagementService.Categories = TraceCategories.Application;

      string sMessage = $"This is unit test info message {Guid.NewGuid()}";
      expected = sMessage;

      oTraceService.Info(sMessage, TraceCategories.Application);

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Async_Info_Message_Should_Not_Be_Written_For_Non_Set_Delegate()
    {
      string expected = String.Empty;
      string actual = $"Initial message {Guid.NewGuid()}";

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();

      using (DefaultTraceService oTraceService = new DefaultTraceService(oManagementService))
      {
        AsyncDelegateTracer target = new AsyncDelegateTracer((message) => actual = message, (message) => actual = message, null);
        oManagementService.AddTracer(target);

        string sMessage = $"This is unit test info message {Guid.NewGuid()}";
        expected = actual;

        oTraceService.Info(sMessage);
        oTraceService.EndWrite();
      }

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Info_Message_Should_Not_Be_Written_For_Non_Set_Delegate()
    {
      string expected = String.Empty;
      string actual = $"Initial message {Guid.NewGuid()}";

      DefaultTraceManagementService oManagementService = new DefaultTraceManagementService();
      DefaultTraceService oTraceService = new DefaultTraceService(oManagementService);
      DelegateTracer target = new DelegateTracer((message) => actual = message, (message) => actual = message, null);
      oManagementService.AddTracer(target);

      string sMessage = $"This is unit test info message {Guid.NewGuid()}";
      expected = actual;

      oTraceService.Info(sMessage);

      Assert.AreEqual(expected, actual);
    }
  }
}
