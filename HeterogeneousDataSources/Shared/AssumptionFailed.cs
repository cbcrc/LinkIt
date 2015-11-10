using System;

namespace HeterogeneousDataSources.Shared {
    public class AssumptionFailed:Exception {
        public AssumptionFailed(string message) 
            :base(message)
        { }
    }
}
