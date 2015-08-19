using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TReference> : ILoadLinkExpression {
        public LoadLinkExpression(Func<object> loadExpression, Action<TReference> linkAction, Func<TReference, object> getIdFromReferenceFunc)
        {
            LinkAction = linkAction;
            GetIdFromReferenceFunc = getIdFromReferenceFunc;
            LoadExpression = loadExpression;
        }

        private Func<object> LoadExpression { get; set; }

        private Action<TReference> LinkAction { get; set; }
        private Func<TReference, object> GetIdFromReferenceFunc { get; set; }

        public void Link(DataContext dataContext)
        {
            var id = LoadExpression();
            var reference = dataContext.GetOptionalReference<TReference>(id);
            LinkAction(reference);
        }

        public List<object> ReferenceIds
        {
            get
            {
                return new List<object>{LoadExpression()};
            }
        }

        public Type ReferenceType
        {
            get { return typeof (TReference); } 
        }

        public object GetReferenceId(object reference)
        {
            var asTReference = (TReference)reference;
            return GetIdFromReferenceFunc(asTReference);
        }
    }
}
