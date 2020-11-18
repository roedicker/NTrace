using System;

namespace NTrace
{
  /// <summary>
  /// Interface of a tracer
  /// </summary>
  public interface ITracer
  {
    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message"></param>
    void Error(string message);

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message"></param>
    void Warn(string message);

    /// <summary>
    /// Writes an informational message of a specific category
    /// </summary>
    /// <param name="message"></param>
    /// <param name="categories">Optional. Trace categories of this message. Default value is <see cref="TraceCategories.Debug"/>.</param>
    void Info(string message, TraceCategories categories = TraceCategories.Debug);

    /// <summary>
    /// Signals end of writing trace messages
    /// </summary>
    void EndWrite();
  }
}
