using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NTrace.Services;
using NTrace.Tests.Data;

namespace NTrace.Tests
{
  [TestClass]
  public class DefaultTraceServiceTests
  {
    protected DefaultTraceManagementService ManagementService
    {
      get;
      set;
    }

    protected TraceOptions TraceOptions
    {
      get;
      private set;
    }

    protected DefaultTraceService TraceService
    {
      get;
      set;
    }

    protected Mock<ITracer> Target
    {
      get;
      set;
    }

    protected List<string> ActualMessages
    {
      get;
      private set;
    }

    protected TraceCategories ActualTraceCategories
    {
      get;
      set;
    }

    protected TraceDataParent Data
    {
      get;
      private set;
    }

    protected string GetActualMessages()
    {
      StringBuilder Result = new StringBuilder();

      foreach (string message in this.ActualMessages)
      {
        Result.AppendLine(message);
      }

      return Result.ToString();
    }

    [TestInitialize]
    public void Init()
    {
      this.ActualMessages = new List<string>();
      this.ManagementService = new DefaultTraceManagementService();
      this.TraceOptions = new TraceOptions();
      this.TraceService = new DefaultTraceService(this.ManagementService, this.TraceOptions);

      this.Target = new Mock<ITracer>();
      this.Target.Setup(t => t.Error(It.IsAny<string>())).Callback<string>(message => this.ActualMessages.Add(message));
      this.Target.Setup(t => t.Warn(It.IsAny<string>())).Callback<string>(message => this.ActualMessages.Add(message));
      this.Target.Setup(t => t.Info(It.IsAny<string>(), It.IsAny<TraceCategories>())).Callback<string, TraceCategories>((message, categories) => { this.ActualMessages.Add(message); this.ActualTraceCategories = categories; });

      this.ManagementService.AddTracer(this.Target.Object);

      this.Data = new TraceDataParent()
      {
        Details = new TraceDataChild1()
        {
          Child = new TraceDataChild1()
          {
            Child = null,
            Data = new byte[] { 1, 3, 5, 8, 13, 21, 34, 55, 89 },
            IsActive = true,
            Items = null,
            Name = "Child #2"
          },
          Data = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128 },
          IsActive = false,
          Items = new List<TraceDataChild2>()
          {
            new TraceDataChild2()
            {
              Index = 0,
              Timestamp = new DateTime(2018, 10, 5, 12, 00, 00, DateTimeKind.Local),
              UserName = "User Data #1",
              UserData = new System.Security.SecureString()
            },
            new TraceDataChild2()
            {
              Index = 1,
              Timestamp = new DateTime(2018, 10, 5, 12, 01, 00, DateTimeKind.Local),
              UserName = "User Data #2",
              UserData = null
            },
            new TraceDataChild2()
            {
              Index = 2,
              Timestamp = new DateTime(2018, 10, 5, 12, 02, 00, DateTimeKind.Local),
              UserName = String.Empty,
              UserData = new System.Security.SecureString()
            }
          },
          Name = "Child #1"
        },
        iValue = 123,
        lValue = 1234567890344466768L,
        Password = "top secret"
      };
    }

    [TestMethod]
    public void Error_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.None;
      string message = $"Error Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Error(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Warn_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.None;
      string message = $"Warn Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Warn(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Application_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Application;
      string message = $"Application Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Application(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Connection_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Connection;
      string message = $"Connection Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Connection(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Data_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Data;
      string message = $"Data Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();
      this.TraceOptions.InspectionDepth = 5;

      // act
      this.TraceService.Data(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Data_Full_Complex_Object_By_Plain_Text_Serializer_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Data;
      object data = this.Data;
      string sName = "data";
      string expected = $"{sName}:{Environment.NewLine}" +
                        $"  Details:{Environment.NewLine}" +
                        $"    Name: \"Child #1\"{Environment.NewLine}" +
                        $"    IsActive: False{Environment.NewLine}" +
                        $"    Items: [3]{Environment.NewLine}" +
                        $"      [000]:{Environment.NewLine}" +
                        $"        Index: 0{Environment.NewLine}" +
                        $"        UserName: \"User Data #1\"{Environment.NewLine}" +
                        $"        UserData: <secret>{Environment.NewLine}" +
                        $"        Timestamp: 2018-10-05 12:00:00.000{Environment.NewLine}" +
                        $"      [001]:{Environment.NewLine}" +
                        $"        Index: 1{Environment.NewLine}" +
                        $"        UserName: \"User Data #2\"{Environment.NewLine}" +
                        $"        UserData: <secret>{Environment.NewLine}" +
                        $"        Timestamp: 2018-10-05 12:01:00.000{Environment.NewLine}" +
                        $"      [002]:{Environment.NewLine}" +
                        $"        Index: 2{Environment.NewLine}" +
                        $"        UserName: \"\"{Environment.NewLine}" +
                        $"        UserData: <secret>{Environment.NewLine}" +
                        $"        Timestamp: 2018-10-05 12:02:00.000{Environment.NewLine}" +
                        $"    Data: [9]{Environment.NewLine}" +
                        $"      [000]: 0{Environment.NewLine}" +
                        $"      [001]: 1{Environment.NewLine}" +
                        $"      [002]: 2{Environment.NewLine}" +
                        $"      [003]: 4{Environment.NewLine}" +
                        $"      [004]: 8{Environment.NewLine}" +
                        $"      [005]: 16{Environment.NewLine}" +
                        $"      [006]: 32{Environment.NewLine}" +
                        $"      [007]: 64{Environment.NewLine}" +
                        $"      [008]: 128{Environment.NewLine}" +
                        $"  iValue: 123{Environment.NewLine}" +
                        $"  lValue: 1234567890344466768{Environment.NewLine}" +
                        $"  Password: <secret>{Environment.NewLine}";
      this.ActualMessages.Clear();
      this.TraceOptions.InspectionDepth = 5;

      // act
      this.TraceService.Data(sName, data);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Data_Limited_Depth_Complex_Object_By_Plain_Text_Serializer_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Data;
      object data = this.Data;
      string sName = "data";
      string expected = $"{sName}:{Environment.NewLine}" +
                        $"  Details:{Environment.NewLine}" +
                        $@"    Name: ""Child #1""{Environment.NewLine}" +
                        $"    IsActive: False{Environment.NewLine}" +
                        $"    Items: [3]{Environment.NewLine}" +
                        $"      [000]: <object>{Environment.NewLine}" +
                        $"      [001]: <object>{Environment.NewLine}" +
                        $"      [002]: <object>{Environment.NewLine}" +
                        $"    Data: [9]{Environment.NewLine}" +
                        $"      [000]: 0{Environment.NewLine}" +
                        $"      [001]: 1{Environment.NewLine}" +
                        $"      [002]: 2{Environment.NewLine}" +
                        $"      [003]: 4{Environment.NewLine}" +
                        $"      [004]: 8{Environment.NewLine}" +
                        $"      [005]: 16{Environment.NewLine}" +
                        $"      [006]: 32{Environment.NewLine}" +
                        $"      [007]: 64{Environment.NewLine}" +
                        $"      [008]: 128{Environment.NewLine}" +
                        $"  iValue: 123{Environment.NewLine}" +
                        $"  lValue: 1234567890344466768{Environment.NewLine}" +
                        $"  Password: <secret>{Environment.NewLine}";
      this.ActualMessages.Clear();
      this.TraceOptions.InspectionDepth = 3;

      // act
      this.TraceService.Data(sName, data);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Debug_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Debug;
      string message = $"Debug Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Debug(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Method_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Method;
      string message = $"Method Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Info(message, TraceCategories.Method);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Query_Message_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Query;
      string message = $"Query Message {Guid.NewGuid().ToString()}";
      string expected = $"{message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      // act
      this.TraceService.Query(message);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Indented_Application_Message_For_Level_1_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Application;
      string message = $"Indented Message {Guid.NewGuid().ToString()}";
      string expected = $"  {message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      this.TraceService.BeginMethod();

      try
      {
        // act
        this.TraceService.Application(message);

        // assert
        Assert.AreEqual(expected, GetActualMessages());
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }

    [TestMethod]
    public void Indented_Application_Messages_For_Nested_Levels_Are_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Application;
      string message1 = $"Indented Message #1 {Guid.NewGuid().ToString()}";
      string expected1 = $"  {message1}{Environment.NewLine}";
      string message2 = $"Indented Message #2 {Guid.NewGuid().ToString()}";
      string expected2 = $"    {message2}{Environment.NewLine}";

      this.TraceService.BeginMethod();

      try
      {
        this.TraceService.BeginMethod();

        try
        {
          // arrange
          this.ActualMessages.Clear();

          // act
          this.TraceService.Application(message2);

          // assert
          Assert.AreEqual(expected2, GetActualMessages());
        }
        finally
        {
          this.TraceService.EndMethod();
        }

        // arrange
        this.ActualMessages.Clear();

        // act
        this.TraceService.Application(message1);

        // assert
        Assert.AreEqual(expected1, GetActualMessages());
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }

    [TestMethod]
    public void Indented_Begin_Method_Message_With_Time_Tracking_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Application | TraceCategories.Method;
      this.TraceService.Options.UseMethodDurations = true;
      string message = $"Indented Message {Guid.NewGuid().ToString()}";
      string expected = $"  +----------------------------------------------------------------------------------+{Environment.NewLine}" +
                        $"  | Indented_Begin_Method_Message_With_Time_Tracking_Is_Written >                    |{Environment.NewLine}" +
                        $"  +----------------------------------------------------------------------------------+{Environment.NewLine}" +
                        $"  {message}{Environment.NewLine}";

      this.ActualMessages.Clear();

      this.TraceService.BeginMethod();

      try
      {
        // act
        this.TraceService.Application(message);

        // assert
        Assert.AreEqual(expected, GetActualMessages());
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }

    [TestMethod]
    public void Indented_Begin_Method_Message_Without_Time_Tracking_Is_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Application | TraceCategories.Method;
      this.TraceService.Options.UseMethodDurations = false;
      string message = $"Indented Message {Guid.NewGuid().ToString()}";
      string expected = $"  +----------------------------------------------------------------------------------+{Environment.NewLine}" +
                        $"  | Indented_Begin_Method_Message_Without_Time_Tracking_Is_Written                   |{Environment.NewLine}" +
                        $"  +----------------------------------------------------------------------------------+{Environment.NewLine}" +
                        $"  {message}{Environment.NewLine}";
      this.ActualMessages.Clear();

      this.TraceService.BeginMethod();

      try
      {
        // act
        this.TraceService.Application(message);

        // assert
        Assert.AreEqual(expected, GetActualMessages());
      }
      finally
      {
        this.TraceService.EndMethod();
      }
    }

    [TestMethod]
    public void Indented_End_Method_Message_Without_Time_Tracking_Is_Not_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Application | TraceCategories.Method;
      this.TraceService.Options.UseMethodDurations = false;
      string expected = $"  +----------------------------------------------------------------------------------+{Environment.NewLine}" +
                        $"  | Indented_End_Method_Message_Without_Time_Tracking_Is_Not_Written                 |{Environment.NewLine}" +
                        $"  +----------------------------------------------------------------------------------+{Environment.NewLine}";
      this.ActualMessages.Clear();

      this.TraceService.BeginMethod();

      // act
      this.TraceService.EndMethod();

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Application_Message_Is_Not_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.Data;
      this.ActualMessages.Clear();
      string expected = String.Empty;

      // act
      this.TraceService.Application($"Application Message {Guid.NewGuid().ToString()}");

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }

    [TestMethod]
    public void Info_Message_Is_Not_Written()
    {
      // arrange
      this.ManagementService.Categories = TraceCategories.None;
      this.ActualMessages.Clear();
      string expected = String.Empty;

      // act
      this.TraceService.Info($"Info Message {Guid.NewGuid().ToString()}", TraceCategories.All);

      // assert
      Assert.AreEqual(expected, GetActualMessages());
    }
  }
}
