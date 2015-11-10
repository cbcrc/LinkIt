using System;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.ConfigBuilders
{
    public class IncludeAsBuilder<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant, TTargetConcreteType>
        where TTargetConcreteType : TAbstractChildLinkedSource
    {
        private readonly IncludeBuilder<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant> _includeBuilder;

        public IncludeAsBuilder(IncludeBuilder<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant> includeBuilder)
        {
            _includeBuilder = includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant> AsNestedLinkedSourceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TTargetConcreteType> initChildLinkedSourceAction = null
            )
        {
            var include = LinkedSourceConfigs.GetConfigFor<TTargetConcreteType>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink, TId>(
                    getLookupIdFunc,
                    initChildLinkedSourceAction
                );

            _includeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant> AsNestedLinkedSourceFromModel<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel) 
        {
            //stle: term TTargetConcreteType is incoherent with the rest
            var include = LinkedSourceConfigs.GetConfigFor<TTargetConcreteType>()
                .CreateIncludeNestedLinkedSourceFromModel<TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(
                    getNestedLinkedSourceModel,
                    _includeBuilder.LinkTarget
                );

            _includeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TAbstractChildLinkedSource, TLink, TDiscriminant> AsReferenceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc
        )
        {
            _includeBuilder.AddInclude(
                discriminantValue,
                new IncludeReferenceById<TAbstractChildLinkedSource, TLink, TTargetConcreteType, TId>(
                    getLookupIdFunc
                )
            );

            return _includeBuilder;
        }
    }
}