using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTrace.Services
{
  public class DefaultTraceManagementService: ITraceManagementService
  {
    public IEnumerable<ITracer> Tracers
    {
      get
      {
        return this._Tracers;
      }
    }

    internal AssemblyInfo AssemblyInfo
    {
      get;
    }

    public string ComputerName
    {
      get;
      set;
    }

    public bool LogProcessId
    {
      get;
      set;
    }

    public string OriginatorName
    {
      get;
      set;
    }

    public string OriginName
    {
      get;
      set;
    }

    public TraceCategories Categories
    {
      get;
      set;
    }

    public DefaultTraceManagementService()
    {
      this.AssemblyInfo = new AssemblyInfo(Assembly.GetExecutingAssembly());
      this.ComputerName = Environment.MachineName;
      this.LogProcessId = false;
      this.OriginatorName = Environment.UserName;
      this.OriginName = this.AssemblyInfo.Name;
      this._Tracers = new List<ITracer>();
      this.Categories = TraceCategories.Application;
    }

    public void AddTracer(ITracer tracer)
    {
      if(tracer == null)
      {
        throw new ArgumentNullException(nameof(tracer));
      }

      if (!this.Tracers.Any(t => t == tracer))
      {
        // add tracer
        this._Tracers.Add(tracer);
      }
    }

    public void ClearTracers()
    {
      this._Tracers.Clear();
    }

    public void RemoveTracer(ITracer tracer)
    {
      if (this.Tracers.Any(t => t == tracer))
      {
        // remove tracer
        this._Tracers.Remove(tracer);
      }
    }

    private readonly List<ITracer> _Tracers;
  }
}
