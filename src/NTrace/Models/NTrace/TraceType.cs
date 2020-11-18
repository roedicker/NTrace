using System;
using System.ComponentModel.DataAnnotations;

namespace NTrace
{
  /// <summary>
  /// Defines the type of a trace output.
  /// </summary>
  internal enum TraceType
  {
    /// <summary>
    /// Information messages.
    /// </summary>
    [Display(Name = "INF")]
    Information = 0,

    /// <summary>
    /// Warning messages.
    /// </summary>
    [Display(Name = "WNG")]
    Warning = 1,

    /// <summary>
    /// Error messages.
    /// </summary>
    [Display(Name = "ERR")]
    Error = 2
  }
}
