using System;

namespace LinkIt.Shared {
    public class AssumptionFailed:Exception {
        public AssumptionFailed(string message) 
            :base(message)
        { }
    }
}
