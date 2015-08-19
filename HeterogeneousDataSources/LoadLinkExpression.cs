using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources {
    public class LoadLinkExpression<TLinkedSource, TReference> : ILoadLinkExpression {
        public LoadLinkExpression(Func<TLinkedSource, object> loadExpression, Action<TLinkedSource, TReference> linkAction, Func<TReference, object> getIdFromReferenceFunc)
        {
            LinkAction = linkAction;
            GetIdFromReferenceFunc = getIdFromReferenceFunc;
            LoadExpression = loadExpression;
        }

        private Func<TLinkedSource, object> LoadExpression { get; set; }

        private Action<TLinkedSource, TReference> LinkAction { get; set; }
        private Func<TReference, object> GetIdFromReferenceFunc { get; set; }

        public List<object> GetLookupIds(TLinkedSource linkedSource)
        {
            return new List<object> { LoadExpression(linkedSource) };
        }

        public List<object> GetLookupIds(object linkedSource)
        {
            return GetLookupIds((TLinkedSource) linkedSource);
        }

        public Type ReferenceType
        {
            get { return typeof (TReference); } 
        }

        public void Link(TLinkedSource linkedSource, DataContext dataContext) {
            var id = LoadExpression(linkedSource);
            var reference = dataContext.GetOptionalReference<TReference>(id);
            LinkAction(linkedSource, reference);
        }

        public void Link(object linkedSource, DataContext dataContext)
        {
            Link((TLinkedSource) linkedSource, dataContext);
        }

        public object GetReferenceId(object reference)
        {
            var asTReference = (TReference)reference;
            return GetIdFromReferenceFunc(asTReference);
        }
    }
}
