namespace NTrace
{
  /// <summary>
  /// Interface for a traceable class
  /// </summary>
  public interface ITraceable
  {
    /// <summary>
    /// Gets or sets the trace service of a class
    /// </summary>
    ITraceService? TraceService
    {
      get;
      set;
    }

    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message">Messag to write</param>
    void Error(string message);

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message">Message to write</param>
    void Warn(string message);

    /// <summary>
    /// Writes an information message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="categories">Optional. Categories for this message. Default value is <see cref="TraceCategories.Debug"/>.</param>
    void Info(string message, TraceCategories categories = TraceCategories.Debug);
  }
}
