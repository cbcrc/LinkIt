using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TReference> : ILoadLinkExpression {
        public LoadLinkExpression(Func<object> loadExpression) {
            LoadExpression = loadExpression;
        }

        private Func<object> LoadExpression { get; set; }

        public List<object> ReferenceIds
        {
            get
            {
                return new List<object>{LoadExpression()};
            }
        }

        public Type ReferenceType
        {
            get { return typeof(TReference); }
        }
    }
}
