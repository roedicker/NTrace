using System;
using System.Collections.Generic;

namespace NTrace
{
  public interface ITraceManagementService
  {
    /// <summary>
    /// Gets a enumeration of all registered tracers
    /// </summary>
    IEnumerable<ITracer> Tracers
    {
      get;
    }

    /// <summary>
    /// Gets or sets the categories that shall be allowed to be written
    /// </summary>
    TraceCategories Categories
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the origin of the trace message.
    /// </summary>
    /// <value>Source of the trace message.</value>
    /// <remarks>Value should be a significant denotation (e.g. short name of the executing assembly)</remarks>
    string OriginName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the user account under that the trace messages will be created.
    /// </summary>
    /// <value>Originator writing a trace message.</value>
    string OriginatorName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the name of the machine where trace messages will be written.
    /// </summary>
    string ComputerName
    {
      get;
    }

    /// <summary>
    /// Gets or sets the indicator for writing the process identifier for each trace message.
    /// </summary>
    /// <value>Indicator for writing the process identifier for each trace message</value>
    /// <returns>Current indicator for writing the process identifier for each trace message</returns>
    /// <remarks>The default value is <strong>false</strong>.</remarks>
    bool LogProcessId
    {
      get;
      set;
    }

    /// <summary>
    /// Adds a trace to the tracer collection
    /// </summary>
    /// <param name="tracer">Trace to be added to the collection</param>
    void AddTracer(ITracer tracer);

    /// <summary>
    /// Clears the tracer collection
    /// </summary>
    void ClearTracers();

    /// <summary>
    /// Removes a tracer from the tracer collection
    /// </summary>
    /// <param name="tracer">Tracer to remove from the tracer collection</param>
    void RemoveTracer(ITracer tracer);
  }
}
