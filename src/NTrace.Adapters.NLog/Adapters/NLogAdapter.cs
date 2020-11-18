using System;

using NLog;

namespace NTrace.Adapters
{
  /// <summary>
  /// Defines the NTrace adapter for using NLog
  /// </summary>
  public class NLogAdapter : ITracer
  {
    /// <summary>
    /// Gets the Logger for NLog
    /// </summary>
    public ILogger Logger
    {
      get;
    }

    /// <summary>
    /// Creates a new instance of the NTrace adapter for NLog
    /// </summary>
    /// <param name="logger">NLog logger reference</param>
    public NLogAdapter(ILogger logger)
    {
      this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Indicates end of writing messages
    /// </summary>
    public void EndWrite()
    {
      // not supported by NLog
    }

    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Error(string message)
    {
      this.Logger.Error(message);
    }

    /// <summary>
    /// Writes an information message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="categories">Category for message</param>
    public void Info(string message, TraceCategories categories = TraceCategories.Debug)
    {
      if ((categories & TraceCategories.Debug) == TraceCategories.Debug)
      {
        this.Logger.Debug(message);
      }
      else
      {
        this.Logger.Info(message);
      }
    }

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Warn(string message)
    {
      this.Logger.Warn(message);
    }
  }
}
