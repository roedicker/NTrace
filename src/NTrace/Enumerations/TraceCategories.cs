using System;

namespace NTrace
{
  /// <summary>
  /// Defines categories of traces to be written.
  /// </summary>
  [Flags]
  public enum TraceCategories : byte
  {
    /// <summary>
    /// No messages shall be written, but errors.
    /// </summary>
    None = 0,

    /// <summary>
    /// Tracing application related messages such as starting, pausing, resuming, finishing.
    /// </summary>
    Application = 1,

    /// <summary>
    /// Tracing Connection related messages such as establishing a connection to a database, performing a FTP connect or a request of a web service.
    /// </summary>
    Connection = 2,

    /// <summary>
    /// Tracing calls of class methods.
    /// </summary>
    Method = 4,

    /// <summary>
    /// Tracing application data.
    /// </summary>
    Data = 8,

    /// <summary>
    /// Tracing of queries.
    /// </summary>
    /// <remarks>
    /// </remarks>
    Query = 16,

    /// <summary>
    /// Tracing all debug output.
    /// </summary>
    Debug = 128,

    /// <summary>
    /// Tracing all output
    /// </summary>
    All = 255
  }
}
