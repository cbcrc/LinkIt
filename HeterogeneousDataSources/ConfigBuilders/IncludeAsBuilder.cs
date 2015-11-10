using System;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.ConfigBuilders
{
    public class IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget>
        where TLinkTarget : TAbstractLinkTarget
    {
        private readonly IncludeBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> _includeBuilder;

        public IncludeAsBuilder(IncludeBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> includeBuilder)
        {
            _includeBuilder = includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceById<TId>(
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

            _includeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsNestedLinkedSourceFromModel<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel) 
        {
            var include = LinkedSourceConfigs.GetConfigFor<TLinkTarget>()
                .CreateIncludeNestedLinkedSourceFromModel<TAbstractLinkTarget, TLink, TChildLinkedSourceModel>(
                    getNestedLinkedSourceModel,
                    _includeBuilder.LinkTarget
                );

            _includeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> AsReferenceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc
        )
        {
            _includeBuilder.AddInclude(
                discriminantValue,
                new IncludeReferenceById<TAbstractLinkTarget, TLink, TLinkTarget, TId>(
                    getLookupIdFunc
                )
            );

            return _includeBuilder;
        }
    }
}