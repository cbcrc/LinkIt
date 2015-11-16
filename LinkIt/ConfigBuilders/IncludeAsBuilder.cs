using System;
using LinkIt.LinkedSources;
using LinkIt.LoadLinkExpressions.Includes;

namespace LinkIt.ConfigBuilders
{
    public class IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget>
        where TLinkTarget : TAbstractLinkTarget
    {
        private readonly IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> _includeSetBuilder;

        public IncludeAsBuilder(IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> includeSetBuilder)
        {
            _includeSetBuilder = includeSetBuilder;
        }

        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupId,
            Action<TLinkedSource, int, TLinkTarget> initChildLinkedSource = null
            )
        {
            if (getLookupIdFunc == null) { throw new ArgumentNullException("getLookupIdFunc"); }

            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TAbstractLinkTarget, TLink, TId>(
                    getLookupId,
                    initChildLinkedSource
                );

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                include
            );

            return _includeSetBuilder;
        }

        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceFromModel<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel) 
        {
            if (getNestedLinkedSourceModel == null) { throw new ArgumentNullException("getNestedLinkedSourceModel"); }

            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceFromModel<TAbstractLinkTarget, TLink, TChildLinkedSourceModel>(
                    getNestedLinkedSourceModel,
                    _includeSetBuilder.LinkTarget
                );

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                include
            );

            return _includeSetBuilder;
        }

        public IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsReferenceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupId
        )
        {
            if (getLookupIdFunc == null) { throw new ArgumentNullException("getLookupIdFunc"); }

            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                new IncludeReferenceById<TAbstractLinkTarget, TLink, TLinkTarget, TId>(
                    getLookupId
                )
            );

            return _includeSetBuilder;
        }
    }
}