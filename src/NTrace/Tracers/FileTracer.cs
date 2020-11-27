using System;
using System.IO;

namespace NTrace.Tracers
{
  /// <summary>
  /// Defines the synchronous file-tracer.
  /// </summary>
  public class FileTracer : ITracer
  {
    /// <summary>
    /// Gets the stream writer for writing the file log.
    /// </summary>
    protected StreamWriter? StreamWriter
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
    /// Initializes a new instance of the file tracer class.
    /// </summary>
    public FileTracer()
    {
      this.StreamWriter = null;
      this.FilePath = String.Empty;
      this.FileName = String.Empty;
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
      lock (_StreamLock)
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
    /// <param name="message"></param>
    public void Warn(string message)
    {
      Write(message, TraceType.Warning);
    }

    /// <summary>
    /// Writes an information message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="category">Category of message</param>
    public void Info(string message, TraceCategories category = TraceCategories.Debug)
    {
      Write(message, TraceType.Information);
    }

    /// <summary>
    /// Writes a message
    /// </summary>
    /// <param name="message">Message to write</param>
    /// <param name="type">Trace type</param>
   internal void Write(string message, TraceType type)
    {
      try
      {
        if (CheckStream())
        {
          this.StreamWriter?.WriteLine($"{DateTime.Now.ToIsoDateTimeString()} {type.GetDisplayName()} {message}");

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

    private readonly object _StreamLock = new object();
  }
}
