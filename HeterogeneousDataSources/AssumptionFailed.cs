using System;

namespace HeterogeneousDataSources {
    public class AssumptionFailed:Exception {
        public AssumptionFailed(string message) 
            :base(message)
        { }
    }
}
