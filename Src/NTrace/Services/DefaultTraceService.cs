using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NTrace.Services
{
  public class DefaultTraceService : ITraceService
  {
    protected ITraceManagementService ManagementService
    {
      get;
    }

    internal ITraceObjectSerializer Serializer
    {
      get;
    }

    public TraceOptions Options
    {
      get;
    }

    protected int IndentLevel
    {
      get;
      private set;
    }

    protected Stack<DateTime> Timestamps
    {
      get;
    }

    public DefaultTraceService(ITraceManagementService managementService) : this(managementService, new TraceOptions())
    {
      // not used
    }

    public DefaultTraceService(ITraceManagementService managementService, TraceOptions options)
    {
      this.ManagementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
      this.IndentLevel = 0;
      this.Serializer = new PlainTextTraceObjectSerializer(this.Options);
      this.Timestamps = new Stack<DateTime>();
    }

    ~DefaultTraceService()
    {
      EndWrite();
    }

    private void PushIndentLevel()
    {
      if (this.Options.UseIndention)
      {
        this.IndentLevel++;
      }
    }

    private void PopIndentLevel()
    {
      if (this.Options.UseIndention && this.IndentLevel > 0)
      {
        this.IndentLevel--;
      }
    }

    private void PushTimestamp(DateTime timestamp)
    {
      this.Timestamps.Push(timestamp);
    }

    private DateTime PopTimestamp()
    {
      if (this.Timestamps.Any())
      {
        return this.Timestamps.Pop();
      }
      else
      {
        return DateTime.UtcNow;
      }
    }

    public void Error(string message)
    {
      this.ManagementService.Tracers.ToList().ForEach(tracer => tracer.Error(GetIndentedMessage(message)));
    }

    public void Error(string name, object data)
    {
      // trace each line of serialzed data separately
      foreach (string sLine in this.Serializer.Serialize(name, data).Split(new[] { Environment.NewLine }, StringSplitOptions.None))
      {
        if (!String.IsNullOrEmpty(sLine))
        {
          Error(sLine);
        }
      }
    }

    public void Error(Exception exception)
    {
      this.ManagementService.Tracers.ToList().ForEach(tracer => tracer.Error(GetIndentedMessage(exception.GetMessageStackString())));
    }

    public void Warn(string message)
    {
      this.ManagementService.Tracers.ToList().ForEach(tracer => tracer.Warn(GetIndentedMessage(message)));
    }

    public void Warn(string name, object data)
    {
      // trace each line of serialzed data separately
      foreach (string sLine in this.Serializer.Serialize(name, data).Split(new[] { Environment.NewLine }, StringSplitOptions.None))
      {
        if (!String.IsNullOrEmpty(sLine))
        {
          Error(sLine);
        }
      }
    }

    public void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      if (IsValidCategory(categories))
      {
        message = GetIndentedMessage(message);
        this.ManagementService.Tracers.ToList().ForEach(tracer => tracer.Info(message, categories));
      }
    }

    public void Info(string name, object data, TraceCategories categories = TraceCategories.Debug)
    {
      // trace each line of serialzed data separately
      foreach (string sLine in this.Serializer.Serialize(name, data).Split(new[] { Environment.NewLine }, StringSplitOptions.None))
      {
        if (!String.IsNullOrEmpty(sLine))
        {
          Info(sLine, categories);
        }
      }
    }

    public void Application(string message)
    {
      Info(message, TraceCategories.Application);
    }

    public void BeginMethod([CallerMemberName] string memberName = "")
    {
      PushIndentLevel();

      if (this.Options.UseMethodBlocks)
      {
        TraceBeginMethodBlock(memberName);
      }

      if (this.Options.UseMethodDurations)
      {
        PushTimestamp(DateTime.UtcNow);
      }
    }

    public void EndMethod([CallerMemberName] string memberName = "")
    {
      DateTime dtNow = DateTime.UtcNow;
      DateTime dtEndMethod;

      if (this.Options.UseMethodDurations)
      {
        dtEndMethod = PopTimestamp();
      }
      else
      {
        dtEndMethod = dtNow;
      }

      if (this.Options.UseMethodBlocks)
      {
        TraceEndMethodBlock(memberName, dtNow.Subtract(dtEndMethod));
      }

      PopIndentLevel();
    }

    public void Connection(string message)
    {
      Info(message, TraceCategories.Connection);
    }

    public void Query(string message)
    {
      Info(message, TraceCategories.Query);
    }

    public void Data(string message)
    {
      Info(message, TraceCategories.Data);
    }

    public void Data(string name, object data)
    {
      Info(name, data, TraceCategories.Data);
    }

    public void Debug(string message)
    {
      Info(message, TraceCategories.Debug);
    }

    public void EndWrite()
    {
      this.ManagementService.Tracers.ToList().ForEach(tracer => tracer.EndWrite());
    }

    private bool IsValidCategory(TraceCategories category)
    {
      return (this.ManagementService.Categories & category) > 0;
    }

    private string GetIndentedMessage(string message)
    {
      if (this.IndentLevel == 0)
      {
        return message;
      }
      else
      {
        message = message ?? String.Empty;

        return message.PadLeft(message.Length + this.IndentLevel * this.Options.IndentionWidth, _IndentCharacter);
      }
    }

    private void TraceBeginMethodBlock(string memberName)
    {
      if (String.IsNullOrWhiteSpace(memberName))
      {
        memberName = "n/a";
      }

      if (this.Options.UseMethodDurations)
      {
        memberName += _MethodBeginBlockIndicator;
      }

      Info(_MethodBlockTopBottomFrame, TraceCategories.Method);
      Info(String.Format("{0} {1} {0}", _MethodBlockLeftRightFrame, memberName.PadRight(_MethodBlockTopBottomFrame.Length - 2 * _MethodBlockLeftRightFrame.Length - 2, ' ')), TraceCategories.Method);
      Info(_MethodBlockTopBottomFrame, TraceCategories.Method);
    }

    private void TraceEndMethodBlock(string memberName, TimeSpan duration)
    {
      if (this.Options.UseMethodDurations)
      {
        if (String.IsNullOrWhiteSpace(memberName))
        {
          memberName = "n/a";
        }

        memberName = _MethodEndBlockIndicator + memberName;
        string sDisplayText = $"{memberName} [{Math.Floor(duration.TotalHours)}:{duration.Minutes:00}:{duration.Seconds:00}.{duration.Milliseconds:000}]";

        Info(_MethodBlockTopBottomFrame, TraceCategories.Method);
        Info(String.Format("{0} {1} {0}", _MethodBlockLeftRightFrame, sDisplayText.PadRight(_MethodBlockTopBottomFrame.Length - 2 * _MethodBlockLeftRightFrame.Length - 2, ' ')), TraceCategories.Method);
        Info(_MethodBlockTopBottomFrame, TraceCategories.Method);
      }
    }

    private readonly char _IndentCharacter = ' ';
    private readonly string _MethodBlockTopBottomFrame = "+----------------------------------------------------------------------------------+";
    private readonly string _MethodBlockLeftRightFrame = "|";
    private readonly string _MethodBeginBlockIndicator = " >";
    private readonly string _MethodEndBlockIndicator = "< ";
  }
}
