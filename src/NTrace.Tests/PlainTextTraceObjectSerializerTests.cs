using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NTrace.Services;
using NTrace.Tests.Data;

namespace NTrace.Tests
{
  [TestClass]
  public class PlainTextTraceObjectSerializerTests
  {
    protected TraceDataParent Data
    {
      get;
      private set;
    }

    protected TraceOptions TraceOptions
    {
      get;
      private set;
    }

    internal PlainTextTraceObjectSerializer Target
    {
      get;
      private set;
    }

    [TestInitialize]
    public void Init()
    {
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
        lValue = 1234567890344466768L
      };

      this.TraceOptions = new TraceOptions();
      this.Target = new PlainTextTraceObjectSerializer(this.TraceOptions);
    }

    [TestMethod]
    public void Simple_Null_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      object oData = null;
      string expected = $"{sName}: <null>{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, oData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_Empty_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      string sData = String.Empty;
      string expected = $@"{sName}: """"{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);


      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_DateTime_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      DateTime dtData = new DateTime(2018, 10, 10, 12, 00, 00, DateTimeKind.Utc);
      string expected = $"{sName}: 2018-10-10 12:00:00.000{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, dtData);


      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_String_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      string sData = "This is a test example";
      string expected = $@"{sName}: ""{sData}""{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Double_Quoted_String_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      string sData = "This is a \"double quoted test example\"";
      string expected = $@"{sName}: '{sData}'{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Single_Quoted_String_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      string sData = "This is a 'single quoted test example'";
      string expected = $@"{sName}: ""{sData}""{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Mixed_Quoted_String_Data_Is_Serialized()
    {
      // arrange
      string sName = "data";
      string sData = "This is a 'mixed \"quoted test\" example'";
      string expected = $@"{sName}: {sData}{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_Password_Data_Is_Serialized()
    {
      // arrange
      string sName = "passphrase";
      string sData = "This is a test example";
      string expected = $@"{sName}: <secret>{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_Null_Password_Data_Is_Serialized()
    {
      // arrange
      string sName = "passphrase";
      string sData = null;
      string expected = $@"{sName}: <null>{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, sData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_Empty_SecureString_Data_Is_Serialized()
    {
      // arrange
      string sName = "MySecret";
      SecureString oData = new SecureString();
      string expected = $@"{sName}: <secret>{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, oData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Simple_Null_SecureString_Data_Is_Serialized()
    {
      // arrange
      string sName = "MySecret";
      SecureString oData = null;
      string expected = $@"{sName}: <null>{Environment.NewLine}";

      // act
      string actual = this.Target.Serialize(sName, oData);

      // assert
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Complex_Data_Is_Serialized()
    {
      // arrange
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
      this.TraceOptions.InspectionDepth = 5;

      // act
      string actual = this.Target.Serialize(sName, this.Data);

      // assert
      Assert.AreEqual(expected, actual);
    }
  }
}
