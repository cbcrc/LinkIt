using System;
using HeterogeneousDataSources.LinkedSources;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.ConfigBuilders
{
    public class IncludeAsBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant, TTargetConcreteType>
        where TTargetConcreteType : TIChildLinkedSource
    {
        private readonly IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> _includeBuilder;

        public IncludeAsBuilder(IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> includeBuilder)
        {
            _includeBuilder = includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> AsNestedLinkedSourceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TTargetConcreteType> initChildLinkedSourceAction = null
            )
        {
            var include = LinkedSourceConfigs.GetConfigFor<TTargetConcreteType>()
                .CreateIncludeNestedLinkedSourceById<TLinkedSource, TIChildLinkedSource, TLink, TId>(
                    getLookupIdFunc,
                    initChildLinkedSourceAction
                );

            _includeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> AsNestedLinkedSourceFromModel<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel) 
        {
            //stle: term TTargetConcreteType is incoherent with the rest
            var include = LinkedSourceConfigs.GetConfigFor<TTargetConcreteType>()
                .CreateIncludeNestedLinkedSourceFromModel<TIChildLinkedSource, TLink, TChildLinkedSourceModel>(
                    getNestedLinkedSourceModel,
                    _includeBuilder.LinkTarget
                );

            _includeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeBuilder;
        }

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> AsReferenceById<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc
        )
        {
            _includeBuilder.AddInclude(
                discriminantValue,
                new IncludeReferenceById<TIChildLinkedSource, TLink, TTargetConcreteType, TId>(
                    getLookupIdFunc
                )
            );

            return _includeBuilder;
        }
    }
}