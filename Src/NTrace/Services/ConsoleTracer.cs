using System;

namespace NTrace.Services
{
  public class ConsoleTracer : ITracer
  {
    public void EndWrite()
    {
      // not used
    }

    public void Error(string message)
    {
      Write(message, TraceCategories.All, TraceType.Error);
    }

    public void Warn(string message)
    {
      Write(message, TraceCategories.All, TraceType.Warning);
    }

    public void Info(string message, TraceCategories category = TraceCategories.Debug)
    {
      Write(message, category, TraceType.Information);
    }

    internal void Write(string message, TraceCategories category, TraceType type)
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
