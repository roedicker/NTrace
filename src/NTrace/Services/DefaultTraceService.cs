using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using NTrace.Serializers;

namespace NTrace.Services
{
  /// <summary>
  /// Defines the default trace service
  /// </summary>
  public class DefaultTraceService : ITraceService, IDisposable
  {
    /// <summary>
    /// Gets the management service
    /// </summary>
    protected ITraceManagementService ManagementService
    {
      get;
    }

    /// <summary>
    /// Gets the object serializer
    /// </summary>
    internal ITraceObjectSerializer Serializer
    {
      get;
    }

    /// <summary>
    /// Gets the trace options
    /// </summary>
    public TraceOptions Options
    {
      get;
    }

    /// <summary>
    /// Gets the indention level
    /// </summary>
    protected int IndentLevel
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the stack of used timestamps
    /// </summary>
    protected Stack<DateTime> Timestamps
    {
      get;
    }

    /// <summary>
    /// Gets an indicator whether the trace has been ended or not
    /// </summary>
    protected bool IsEnded
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the queue of asynchronous messages
    /// </summary>
    public Queue<Message> Messages
    {
      get;
    }

    /// <summary>
    /// Gets the manual reset-event for stopping the asynchronous message process
    /// </summary>
    private ManualResetEvent ProcessMessagesStopEvent
    {
      get;
    }

    /// <summary>
    /// Gets or sets the asynchronous message worker
    /// </summary>
    private Task MessageWorker
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new instance of the default trace service
    /// </summary>
    /// <param name="managementService">Management service</param>
    public DefaultTraceService(ITraceManagementService managementService) : this(managementService, new TraceOptions())
    {
      // not used
    }

    /// <summary>
    /// Creates a new instance of the default trace service with given options
    /// </summary>
    /// <param name="managementService">Management service</param>
    /// <param name="options">Options to use for this trace service</param>
    public DefaultTraceService(ITraceManagementService managementService, TraceOptions options)
    {
      this.ManagementService = managementService ?? throw new ArgumentNullException(nameof(managementService));
      this.Options = options ?? throw new ArgumentNullException(nameof(options));
      this.IndentLevel = 0;
      this.IsEnded = false;
      this.Messages = new Queue<Message>();
      this.Serializer = new PlainTextTraceObjectSerializer(this.Options);
      this.Timestamps = new Stack<DateTime>();
      this.ProcessMessagesStopEvent = new ManualResetEvent(false);
      this.MessageWorker = StartMessageWorker();
    }

    /// <summary>
    /// Manages the disposal of this instance
    /// </summary>
    /// <param name="disposing">Indicator whether disposing has to be performed or not</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!_Disposed)
      {
        if (disposing)
        {
          EndWrite();
        }

        _Disposed = true;
      }
    }

    /// <summary>
    /// Disposes this instance of the default trace-service
    /// </summary>
    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Increases the indent level
    /// </summary>
    private void PushIndentLevel()
    {
      if (this.Options.UseIndention)
      {
        this.IndentLevel++;
      }
    }

    /// <summary>
    /// Decreases the indent level
    /// </summary>
    private void PopIndentLevel()
    {
      if (this.Options.UseIndention && this.IndentLevel > 0)
      {
        this.IndentLevel--;
      }
    }

    /// <summary>
    /// Pushes a new timestamp
    /// </summary>
    /// <param name="timestamp">Timestamp to push on the stack</param>
    private void PushTimestamp(DateTime timestamp)
    {
      this.Timestamps.Push(timestamp);
    }

    /// <summary>
    /// Pulls the latest timestamp from the stack
    /// </summary>
    /// <returns>Pulled timestamp or current timestamp as UTC if stack is empty</returns>
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

    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Error(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      string sMessageText = GetIndentedMessage(message);

      // enqueue message if there any asynchronous tracers have been added
      if (this.ManagementService.HasAsynchronousTracers)
      {
        EnqueueMessage(new Message(TraceType.Error, sMessageText));
      }

      // write message directly for all synchronous tracers
      this.ManagementService.Tracers.Where(tracer => tracer is not IAsyncTracer).ToList().ForEach(tracer => tracer.Error(sMessageText));
    }

    /// <summary>
    /// Warites named data as an error
    /// </summary>
    /// <param name="name">Name of the data to write</param>
    /// <param name="data">Data to write</param>
    public void Error(string name, object data)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      // trace each line of serialzed data separately
      foreach (string sLine in this.Serializer.Serialize(name, data).Split(new[] { Environment.NewLine }, StringSplitOptions.None))
      {
        if (!String.IsNullOrEmpty(sLine))
        {
          Error(sLine);
        }
      }
    }

    /// <summary>
    /// Writes an exception as an error
    /// </summary>
    /// <param name="exception">Exception to write</param>
    public void Error(Exception exception)
    {
      if (exception == null)
      {
        throw new ArgumentNullException(nameof(exception));
      }

      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Error(GetIndentedMessage(exception.GetMessageStackString()));
    }

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Warn(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      string sMessageText = GetIndentedMessage(message);

      // enqueue message if there any asynchronous tracers have been added
      if (this.ManagementService.HasAsynchronousTracers)
      {
        EnqueueMessage(new Message(TraceType.Warning, sMessageText));
      }

      // write message directly for all synchronous tracers
      this.ManagementService.Tracers.Where(tracer => tracer is not IAsyncTracer).ToList().ForEach(tracer => tracer.Warn(sMessageText));
    }

    /// <summary>
    /// Writes named data as warning
    /// </summary>
    /// <param name="name">Name of data to write</param>
    /// <param name="data">Data to write</param>
    public void Warn(string name, object data)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      // trace each line of serialzed data separately
      foreach (string sLine in this.Serializer.Serialize(name, data).Split(new[] { Environment.NewLine }, StringSplitOptions.None))
      {
        if (!String.IsNullOrEmpty(sLine))
        {
          Warn(sLine);
        }
      }
    }

    /// <summary>
    /// Warites an information message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="categories">Optional. Specifies the category for this message. Default value is <see cref="TraceCategories.Debug"/>.</param>
    public void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      if (IsValidCategory(categories))
      {
        string sMessageText = GetIndentedMessage(message);

        // enqueue message if there any asynchronous tracers have been added
        if (this.ManagementService.HasAsynchronousTracers)
        {
          EnqueueMessage(new Message(TraceType.Information, sMessageText, categories));
        }

        // write message directly for all synchronous tracers
        this.ManagementService.Tracers.Where(tracer => tracer is not IAsyncTracer).ToList().ForEach(tracer => tracer.Info(sMessageText, categories));
      }
    }

    /// <summary>
    /// Writes a named data as information
    /// </summary>
    /// <param name="name"></param>
    /// <param name="data"></param>
    /// <param name="categories"></param>
    public void Info(string name, object data, TraceCategories categories = TraceCategories.Debug)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      // trace each line of serialzed data separately
      foreach (string sLine in this.Serializer.Serialize(name, data).Split(new[] { Environment.NewLine }, StringSplitOptions.None))
      {
        if (!String.IsNullOrEmpty(sLine))
        {
          Info(sLine, categories);
        }
      }
    }

    /// <summary>
    /// Writes an application message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Application(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Info(message, TraceCategories.Application);
    }

    /// <summary>
    /// Write the begin of a method
    /// </summary>
    /// <param name="memberName">Optional. Name of the method. Uses the original name if omitted.</param>
    public void BeginMethod([CallerMemberName] string memberName = "")
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

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

    /// <summary>
    /// Write the end of a method
    /// </summary>
    /// <param name="memberName">Optional. Name of the method. Uses the original name if omitted.</param>
    public void EndMethod([CallerMemberName] string memberName = "")
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

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

    /// <summary>
    /// Write a connection message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Connection(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Info(message, TraceCategories.Connection);
    }

    /// <summary>
    /// Writes a query message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Query(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Info(message, TraceCategories.Query);
    }

    /// <summary>
    /// Writes a data message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Data(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Info(message, TraceCategories.Data);
    }

    /// <summary>
    /// Writes a named data object
    /// </summary>
    /// <param name="name">Name of the data to write</param>
    /// <param name="data">Data to write</param>
    public void Data(string name, object data)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Info(name, data, TraceCategories.Data);
    }

    /// <summary>
    /// Writes a debug message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Debug(string message)
    {
      if (this.IsEnded)
      {
        throw new InvalidOperationException("Trace has been ended");
      }

      Info(message, TraceCategories.Debug);
    }

    /// <summary>
    /// Signals end of writing messages
    /// </summary>
    public void EndWrite()
    {
      if (!this.IsEnded)
      {
        // stop processing asynchronous messages
        StopMessageWorker();

        // wait for completion of message worker - if not already aborted due to process termination
        if (!this.MessageWorker.IsCompleted)
        {
          this.MessageWorker.Wait();
        }

        // signal all tracers to end write messages
        this.ManagementService.Tracers.ToList().ForEach(tracer => tracer.EndWrite());

        this.IsEnded = true;
      }
    }

    /// <summary>
    /// Gets on indicator whether a given category is valid based on the management service settings or not
    /// </summary>
    /// <param name="category">Category to check</param>
    /// <returns>Indicator whether given category is valid or not</returns>
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
        message ??= String.Empty;

        return message.PadLeft(message.Length + this.IndentLevel * this.Options.IndentionWidth, _IndentCharacter);
      }
    }

    /// <summary>
    /// Generates the begin of a method block
    /// </summary>
    /// <param name="memberName">Name of the method</param>
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
      Info(String.Format(CultureInfo.InvariantCulture, "{0} {1} {0}", _MethodBlockLeftRightFrame, memberName.PadRight(_MethodBlockTopBottomFrame.Length - 2 * _MethodBlockLeftRightFrame.Length - 2, ' ')), TraceCategories.Method);
      Info(_MethodBlockTopBottomFrame, TraceCategories.Method);
    }

    /// <summary>
    /// Generates the end of a method block
    /// </summary>
    /// <param name="memberName">Name of the method</param>
    /// <param name="duration">duration of method execution</param>
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
        Info(String.Format(CultureInfo.InvariantCulture, "{0} {1} {0}", _MethodBlockLeftRightFrame, sDisplayText.PadRight(_MethodBlockTopBottomFrame.Length - 2 * _MethodBlockLeftRightFrame.Length - 2, ' ')), TraceCategories.Method);
        Info(_MethodBlockTopBottomFrame, TraceCategories.Method);
      }
    }

    /// <summary>
    /// Enqueues a asynchronous message
    /// </summary>
    /// <param name="message">Messgae to add to the queue</param>
    private void EnqueueMessage(Message message)
    {
      if (message == null)
      {
        throw new ArgumentNullException(nameof(message));
      }

      this.Messages.Enqueue(message);
    }

    /// <summary>
    /// Dequeues a message
    /// </summary>
    /// <returns>Next message from the queue</returns>
    private Message DequeueMessage()
    {
      return this.Messages.Dequeue();
    }

    /// <summary>
    /// Starts the backgound worker for processing the messages asynchronously
    /// </summary>
    /// <returns>The task of the background worker</returns>
    private Task StartMessageWorker()
    {
      if (this.MessageWorker == null || this.MessageWorker.IsCompleted)
      {
        this.ProcessMessagesStopEvent.Reset();
        this.MessageWorker = Task.Run(() => ProcessMessages(this));
      }

      return this.MessageWorker;
    }

    /// <summary>
    /// Stops the background worker for asynchronous messages
    /// </summary>
    private void StopMessageWorker()
    {
      if (this.MessageWorker != null && !this.MessageWorker.IsCompleted)
      {
        this.ProcessMessagesStopEvent.Set();
      }
    }

    /// <summary>
    /// Processes asynchronous messages
    /// </summary>
    private static void ProcessMessages(DefaultTraceService service)
    {
      ManualResetEvent[] aoStartedEvents = new ManualResetEvent[]
                                               {
                                                 service.ProcessMessagesStopEvent
                                               };

      while (true)
      {
        try
        {
          // process all messages in queue
          while (service.Messages.Any())
          {
            Message oMessage = service.DequeueMessage();

            switch (oMessage.MessageType)
            {
              case TraceType.Information:
                service.ManagementService.Tracers.Where(tracer => tracer is IAsyncTracer).ToList().ForEach(tracer => tracer.Info(oMessage.Text, oMessage.Categories));
                break;

              case TraceType.Warning:
                service.ManagementService.Tracers.Where(tracer => tracer is IAsyncTracer).ToList().ForEach(tracer => tracer.Warn(oMessage.Text));
                break;

              case TraceType.Error:
                service.ManagementService.Tracers.Where(tracer => tracer is IAsyncTracer).ToList().ForEach(tracer => tracer.Error(oMessage.Text));
                break;

              default:
                throw new NotSupportedException($@"Trace type ""{oMessage.MessageType}"" for asynchronouse message is not supported");
            }
          }

          WaitHandle.WaitAny(aoStartedEvents, _QueueIdleTime);

          if (service.ProcessMessagesStopEvent.WaitOne(0) && !service.Messages.Any())
          {
            break;
          }
        }
        catch
        {
          WaitHandle.WaitAny(aoStartedEvents, _QueueIdleTime);
        }
      }
    }

    private static TimeSpan _QueueIdleTime = TimeSpan.FromMilliseconds(1000);

    private readonly char _IndentCharacter = ' ';
    private readonly string _MethodBlockTopBottomFrame = "+----------------------------------------------------------------------------------+";
    private readonly string _MethodBlockLeftRightFrame = "|";
    private readonly string _MethodBeginBlockIndicator = " >";
    private readonly string _MethodEndBlockIndicator = "< ";

    private bool _Disposed;
  }
}
