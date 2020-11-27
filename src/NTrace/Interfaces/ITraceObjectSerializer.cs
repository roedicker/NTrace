using System;
using System.Text;

namespace NTrace
{
  internal interface ITraceObjectSerializer
  {
    string Serialize(string name, object data, int? maxDepth = null);
  }
}
