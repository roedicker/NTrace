using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTrace.Services
{
  /// <summary>
  /// Defines the default trace management service
  /// </summary>
  public class DefaultTraceManagementService : ITraceManagementService
  {
    /// <summary>
    /// Gets a list of all registered tracers
    /// </summary>
    public IEnumerable<ITracer> Tracers
    {
      get
      {
        return _Tracers;
      }
    }

    /// <summary>
    /// Gets the assembly information
    /// </summary>
    internal AssemblyInfo AssemblyInfo
    {
      get;
    }

    /// <summary>
    /// Gets the computer name
    /// </summary>
    public string ComputerName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the indicator whether process identifiers shall be traced or not
    /// </summary>
    public bool LogProcessId
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the originator name
    /// </summary>
    public string OriginatorName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the origin name
    /// </summary>
    public string OriginName
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the trace categories
    /// </summary>
    public TraceCategories Categories
    {
      get;
      set;
    }

    /// <summary>
    /// Creates a new instance of the default trace management service
    /// </summary>
    public DefaultTraceManagementService()
    {
      _Tracers = new List<ITracer>();

      this.AssemblyInfo = new AssemblyInfo(Assembly.GetExecutingAssembly());
      this.ComputerName = Environment.MachineName;
      this.LogProcessId = false;
      this.OriginatorName = Environment.UserName;
      this.OriginName = this.AssemblyInfo.Name;
      this.Categories = TraceCategories.Application;
    }

    /// <summary>
    /// Adds a tracer
    /// </summary>
    /// <param name="tracer">Tracer to add</param>
    public void AddTracer(ITracer tracer)
    {
      if (tracer == null)
      {
        throw new ArgumentNullException(nameof(tracer));
      }

      if (!this.Tracers.Contains(tracer))
      {
        _Tracers.Add(tracer);
      }
    }

    /// <summary>
    /// Clears all tracers
    /// </summary>
    public void ClearTracers()
    {
      _Tracers.Clear();
    }

    /// <summary>
    /// Removes a tracer
    /// </summary>
    /// <param name="tracer">Tracer to remove</param>
    public void RemoveTracer(ITracer tracer)
    {
      if (this.Tracers.Contains(tracer))
      {
        _Tracers.Remove(tracer);
      }
    }

    private readonly List<ITracer> _Tracers;
  }
}
