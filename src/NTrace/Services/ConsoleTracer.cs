using System;

namespace NTrace.Services
{
  /// <summary>
  /// Defines the console tracer
  /// </summary>
  public class ConsoleTracer : ITracer
  {
    /// <summary>
    /// Signals the end of writing traces
    /// </summary>
    public void EndWrite()
    {
      // not used
    }

    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Error(string message)
    {
      Write(message, TraceType.Error);
    }

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message">Message to write</param>
    public void Warn(string message)
    {
      Write(message, TraceType.Warning);
    }

    /// <summary>
    /// Writes an information message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="category">Optional. Categories for this message, Default value is <see cref="TraceCategories.Debug"/>.</param>
    public void Info(string message, TraceCategories category = TraceCategories.Debug)
    {
      Write(message, TraceType.Information);
    }

    /// <summary>
    /// Writes a message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="type">Trace type of this message</param>
    /// 
    internal void Write(string message, TraceType type)
    {
      try
      {
        Console.WriteLine($"{DateTime.Now.ToIsoDateTimeString()} {type.GetDisplayName()} {message}");
      }
      catch (Exception ex)
      {
        // can't display errors anywhere - try again to write exception
        try
        {
          Console.WriteLine($"Failed to write to console. {ex.GetMessageStackStrings()}");
        }
        catch
        {
          // ignore additional exceptions
        }
      }
    }
  }
}
