using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression {
        public LoadLinkExpression(Func<object> loadExpression, Type referenceType) {
            LoadExpression = loadExpression;
            ReferenceType = referenceType;
        }

        private Func<object> LoadExpression { get; set; }

        public List<object> ReferenceIds
        {
            get
            {
                return new List<object>{LoadExpression()};
            }
        }


        public Type ReferenceType { get; private set; }
    }
}
