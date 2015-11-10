using System;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.ConfigBuilders
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
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TLinkTarget> initChildLinkedSourceAction = null
            )
        {
            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TAbstractLinkTarget, TLink, TId>(
                    getLookupIdFunc,
                    initChildLinkedSourceAction
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
            Func<TLink, TId> getLookupIdFunc
        )
        {
            _includeSetBuilder.AddToIncludeSet(
                discriminantValue,
                new IncludeReferenceById<TAbstractLinkTarget, TLink, TLinkTarget, TId>(
                    getLookupIdFunc
                )
            );

            return _includeSetBuilder;
        }
    }
}