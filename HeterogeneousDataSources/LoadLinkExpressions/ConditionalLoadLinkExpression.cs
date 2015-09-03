using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class ConditionalLoadLinkExpression<TLinkedSource, TDiscriminant>: ILoadLinkExpression{
        private readonly Func<TLinkedSource, TDiscriminant> _discriminantFunc;
        private readonly Dictionary<TDiscriminant, ILoadLinkExpression> _conditionalLoadLinkExpressions;

        public ConditionalLoadLinkExpression(
            Func<TLinkedSource, TDiscriminant> discriminantFunc, 
            Dictionary<TDiscriminant, ILoadLinkExpression> conditionalLoadLinkExpressions)
        {
            _discriminantFunc = discriminantFunc;
            _conditionalLoadLinkExpressions = conditionalLoadLinkExpressions;

            LinkedSourceType = typeof (TLinkedSource);
            ReferenceTypes = _conditionalLoadLinkExpressions.Values
                .SelectMany(conditionalLoadLinkExpression => conditionalLoadLinkExpression.ReferenceTypes)
                .Distinct()
                .ToList();

            //stle: ensure all of the same .net type and that at least one
            LoadLinkExpressionType = _conditionalLoadLinkExpressions.Values.First().LoadLinkExpressionType;
        }

        public Type LinkedSourceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }
        
        public LoadLinkExpressionType LoadLinkExpressionType { get; private set; }

        public bool DoesMatchReferenceTypeToBeLoaded(object linkedSource, List<Type> referenceTypesToBeLoaded) {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            var selectedLoadLinkExpression = GetSelectedLoadLinkExpression((TLinkedSource)linkedSource);
            return selectedLoadLinkExpression.DoesMatchReferenceTypeToBeLoaded(linkedSource, referenceTypesToBeLoaded);
        }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext){
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            var selectedLoadLinkExpression = GetSelectedLoadLinkExpression((TLinkedSource)linkedSource);
            selectedLoadLinkExpression.AddLookupIds(linkedSource, lookupIdContext);
        }

        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext){
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            var selectedLoadLinkExpression = GetSelectedLoadLinkExpression((TLinkedSource)linkedSource);
            selectedLoadLinkExpression.Link(linkedSource, loadedReferenceContext);
        }

        private ILoadLinkExpression GetSelectedLoadLinkExpression(TLinkedSource linkedSource)
        {
            var discriminant = _discriminantFunc(linkedSource);
            if (!_conditionalLoadLinkExpressions.ContainsKey(discriminant)){
                throw new InvalidOperationException(
                    string.Format(
                        "There is no conditional load link expression for discriminant={0} in {1}.",
                        discriminant,
                        LinkedSourceType
                    )
                );
            }

            return _conditionalLoadLinkExpressions[discriminant];
        }
    }
}
