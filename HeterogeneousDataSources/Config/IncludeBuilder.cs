using System;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    public class IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant, TTargetConcreteType>
        where TTargetConcreteType : TIChildLinkedSource
    {
        private readonly IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> _includeTargetConcreteTypeBuilder;

        public IncludeBuilder(IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> includeTargetConcreteTypeBuilder)
        {
            _includeTargetConcreteTypeBuilder = includeTargetConcreteTypeBuilder;
        }

        public IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> AsNestedLinkedSource<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TTargetConcreteType> initChildLinkedSourceAction = null
            )
        {
            var include = LinkedSourceConfigs.GetConfigFor<TTargetConcreteType>()
                .CreateNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink, TId>(
                    getLookupIdFunc,
                    initChildLinkedSourceAction
                );

            _includeTargetConcreteTypeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeTargetConcreteTypeBuilder;
        }

        public IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> AsSubLinkedSource<TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel) 
        {
            //stle: term TTargetConcreteType is incoherent with the rest
            var include = LinkedSourceConfigs.GetConfigFor<TTargetConcreteType>()
                .CreateSubLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSourceModel>(
                    getSubLinkedSourceModel,
                    _includeTargetConcreteTypeBuilder.LinkTarget
                );

            _includeTargetConcreteTypeBuilder.AddInclude(
                discriminantValue,
                include
            );

            return _includeTargetConcreteTypeBuilder;
        }

        public IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> AsReference<TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc
        )
        {
            _includeTargetConcreteTypeBuilder.AddInclude(
                discriminantValue,
                new ReferenceInclude<TIChildLinkedSource, TLink, TTargetConcreteType, TId>(
                    getLookupIdFunc
                )
            );

            return _includeTargetConcreteTypeBuilder;
        }
    }
}