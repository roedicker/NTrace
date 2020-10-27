using System;
using System.IO;

namespace NTrace.Services
{
  /// <summary>
  /// Tracer class for writing trace messages into a file on the file system.
  /// </summary>
  public class FileTracer : ITracer
  {
    public ITraceService TraceService
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the lock object for this service
    /// </summary>
    protected object LockObject
    {
      get;
    }

    /// <summary>
    /// Gets the stream writer for writing the file log.
    /// </summary>
    protected StreamWriter StreamWriter
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets or sets the path of the trace file
    /// </summary>
    public string FilePath
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the current name of the trace file.
    /// </summary>
    public string FileName
    {
      get;
      private set;
    }
    /// <summary>
    /// Gets or sets the indicator for appending to an existing trace file.
    /// </summary>
    public bool Append
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the indicator for automatic flushing the buffer to the log file.
    /// </summary>
    public bool AutoFlush
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the indicator for sharing the current trace file with other processes.
    /// </summary>
    public bool Share
    {
      get;
      set;
    }

    /// <summary>
    /// Checks if a stream for the current trace file exists, otherwise create a new stream writer.
    /// </summary>
    /// <returns><strong>true</strong> if stream exists or was created successfully, otherwise <strong>false</strong>.</returns>
    protected bool CheckStream()
    {
      bool Result = true;

      // check for new filename (changed due to ongoing time)
      DateTime dtFileName = DateTime.Now;
      string sPath = String.IsNullOrWhiteSpace(this.FilePath) ? "." : this.FilePath;
      string sFileName = Path.Combine(sPath, $"{dtFileName.ToIsoDateString()}.{ dtFileName.Hour:d2}.log");

      if (this.FileName != sFileName)
      {
        CloseStream();
        this.FileName = sFileName;
      }

      // open stream if not already done
      if (this.StreamWriter == null)
      {
        try
        {
          // create output directory if not already exists
          if (!Directory.Exists(Path.GetDirectoryName(this.FileName)))
          {
            Directory.CreateDirectory(Path.GetDirectoryName(this.FileName));
          }

          // create new text file stream writer
          this.StreamWriter = new StreamWriter(this.FileName, this.Append)
          {
            AutoFlush = AutoFlush
          };
        }
        catch
        {
          Result = false;
        }
      }

      return Result;
    }

    /// <summary>
    /// Closes the current stream of the trace file.
    /// </summary>
    protected void CloseStream()
    {
      lock (this.LockObject)
      {
        try
        {
          if (this.StreamWriter != null)
          {
            // flush (implicitly) the stream and close file access
            this.StreamWriter.Close();
          }
        }
        finally
        {
          this.StreamWriter = null;
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the file tracer class.
    /// </summary>
    public FileTracer()
    {
      this.LockObject = new object();
      this.Append = true;
      this.AutoFlush = false;
      this.Share = false;
    }

    /// <summary>
    /// Reintializes the current instance of the file tracer class.
    /// </summary>
    ~FileTracer()
    {
      CloseStream();
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
        if (CheckStream())
        {
          this.StreamWriter.WriteLine($"{DateTime.Now.ToIsoDateTimeString()} {type.GetDisplayName()} {message}");

          // file can be shared if the stream is closed only
          if (this.Share)
          {
            CloseStream();
          }
        }
      }
      catch (Exception ex)
      {
        try
        {
          // can't write into file - bring exception to console
          Console.WriteLine($"Failed to write trace to file. {ex.GetMessageStackString()}");
        }
        catch
        {
          // ignore any additional exceptions
        }
      }
    }

    /// <summary>
    /// Finishes writing into current trace file.
    /// </summary>
    public void EndWrite()
    {
      CloseStream();
    }
  }
}
