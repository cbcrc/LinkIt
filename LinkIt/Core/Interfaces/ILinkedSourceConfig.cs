using System;

namespace LinkIt.Core.Interfaces
{
    //Responsible to known the LinkedSourceModelType without using reflection
    public interface ILinkedSourceConfig {
        Type LinkedSourceType { get; }
        Type LinkedSourceModelType { get; }
    }
}