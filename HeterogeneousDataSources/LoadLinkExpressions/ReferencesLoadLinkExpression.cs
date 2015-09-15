using System;
using System.Collections;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions {
    public class ReferencesLoadLinkExpression<TLinkedSource, TReference, TId>
        : LoadLinkExpression<TLinkedSource, TReference, TId>, ILoadLinkExpression
    {
        private readonly string _linkTargetId;
        private readonly Func<TLinkedSource, List<TId>> _getLookupIdsFunc;
        private readonly Action<TLinkedSource, List<TReference>> _linkAction;

        public ReferencesLoadLinkExpression(
            string linkTargetId,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Action<TLinkedSource, List<TReference>> linkAction) 
        {
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TLinkedSource>(linkTargetId, "TLinkedSource");
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TReference>(linkTargetId, "TReference");
            LoadLinkExpressionUtil.EnsureGenericParameterCannotBeList<TId>(linkTargetId, "TId");

            _linkTargetId = linkTargetId;
            _getLookupIdsFunc = getLookupIdsFunc;
            _linkAction = linkAction;
            ModelType = typeof(TReference);
        }

        //stle: delete
        [Obsolete]
        public ReferencesLoadLinkExpression(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Action<TLinkedSource, List<TReference>> linkAction)
            : this("obsolete",getLookupIdsFunc,linkAction) 
        { }

        protected override List<TId> GetLookupIdsTemplate(TLinkedSource linkedSource)
        {
            return _getLookupIdsFunc(linkedSource);
        }

        protected override void LinkAction(TLinkedSource linkedSource, List<TReference> references, LoadedReferenceContext loadedReferenceContext)
        {
            _linkAction(linkedSource, references);
        }

        public override LoadLinkExpressionType LoadLinkExpressionType {
            get { return LoadLinkExpressionType.Reference; }
        }

        public Type ModelType { get; private set; }

        public override string LinkTargetId
        {
            get { return _linkTargetId; }
        }
    }
}
