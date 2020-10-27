using System;
using System.IO;
using System.Reflection;

namespace NTrace
{
  internal class AssemblyInfo
  {
    public Assembly SystemAssembly
    {
      get;
    }

    public string FileName
    {
      get;
    }

    public string FilePath
    {
      get;
    }

    public DateTime LinkerTimeStamp
    {
      get;
    }

    public string Name
    {
      get
      {
        return Path.GetFileNameWithoutExtension(this.FileName);
      }
    }

    public Version Version
    {
      get
      {
        return this.SystemAssembly.GetName().Version;
      }
    }

    public AssemblyInfo(Assembly execAssembly)
    {
      this.SystemAssembly = execAssembly ?? throw new ArgumentNullException(nameof(execAssembly));
      this.FileName = Path.GetFileName(execAssembly.Location);
      this.FilePath = Path.GetFullPath(execAssembly.Location);
      this.LinkerTimeStamp = this.GetLinkerTimestamp(execAssembly.Location);
    }

    protected DateTime GetLinkerTimestamp(string path)
    {
      const int iHeaderOffset = 60;
      const int iLinkerTimestampOffset = 8;

      DateTime Result = new DateTime(1970, 1, 1, 0, 0, 0);
      byte[] abBuffer = new byte[2048];
      Stream fsFile = null;

      try
      {
        fsFile = new FileStream(path, FileMode.Open, FileAccess.Read);
        fsFile.Read(abBuffer, 0, 2048);
      }
      finally
      {
        if (fsFile != null)
        {
          fsFile.Close();
        }
      }

      int iData = BitConverter.ToInt32(abBuffer, iHeaderOffset);
      int iSecondsSince1970 = BitConverter.ToInt32(abBuffer, iData + iLinkerTimestampOffset);

      Result = Result.AddSeconds(iSecondsSince1970);
      Result = Result.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(Result).Hours);

      return Result;
    }
  }
}
