using System;

namespace Example_02.Core
{
  public interface IApplicationService
  {
    void BeginApplication(string[] args);
    void ExecuteApplication();
    void EndApplication();
  }
}
