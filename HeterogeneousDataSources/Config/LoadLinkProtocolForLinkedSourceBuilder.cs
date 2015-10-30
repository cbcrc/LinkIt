using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpressionAction;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpressionAction)
        {
            _addLoadLinkExpressionAction = addLoadLinkExpressionAction;
        }

        #region Reference
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TReference, TId, bool>(
                linkTarget,
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    new ReferenceInclude<TReference, TId, TReference, TId>(
                        CreateIdentityFunc<TId>()
                    )
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TReference>>> linkTargetFunc) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);


            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TReference, TId, bool>(
                linkTarget,
                getLookupIdsFunc,
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    new ReferenceInclude<TReference, TId, TReference, TId>(
                        CreateIdentityFunc<TId>()
                    )
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        } 
        #endregion

        #region NestedLinkedSource
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc) 
        {
            return LoadLinkNestedLinkedSource(
                getLookupIdFunc,
                linkTargetFunc,
                NullInitChildLinkedSourceActionForSingleValue
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc,
           Action<TLinkedSource, TChildLinkedSource> initChildLinkedSourceAction) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget,
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                link => true,
                CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(
                    InitChildLinkedSourceActionForSingleValue(initChildLinkedSourceAction)
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc)
        {
            return LoadLinkNestedLinkedSource(
                getLookupIdsFunc, 
                linkTargetFunc, 
                NullInitChildLinkedSourceAction
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget,
                getLookupIdsFunc,
                link => true,
                CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(
                    initChildLinkedSourceAction
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        private Dictionary<bool, IInclude> CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction)
        {
            var include = LinkedSourceConfigs.GetConfigFor<TChildLinkedSource>()
                .CreateNestedLinkedSourceInclude<TLinkedSource, TChildLinkedSource, TId, TId>(
                    CreateIdentityFunc<TId>(),
                    initChildLinkedSourceAction
                );

            return CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                include
            );
        }
        #endregion

        #region SubLinkedSource
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, TChildLinkedSourceModel> getSubLinkedSourceModelsFunc,
            Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel, bool>(
                linkTarget,
                ToGetLookupIdsFuncForSingleValue(getSubLinkedSourceModelsFunc),
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    new SubLinkedSourceInclude<TChildLinkedSource, TChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>(
                        null
                    )
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getSubLinkedSourceModelsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel, bool>(
                linkTarget,
                getSubLinkedSourceModelsFunc,
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    //stle: dry 
                    new SubLinkedSourceInclude<TChildLinkedSource, TChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>(
                        null
                    )
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        #endregion

        #region Polymorphic
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLink<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, TLink> getLinkFunc,
           Expression<Func<TLinkedSource, TIChildLinkedSource>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            //stle: dry
            var includeBuilder = new IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                linkTarget,
                ToGetLookupIdsFuncForSingleValue(getLinkFunc),
                getDiscriminantFunc,
                includeBuilder.Build()
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLinkForList<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, List<TLink>> getLinksFunc,
           Expression<Func<TLinkedSource, List<TIChildLinkedSource>>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            var includeBuilder = new IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                linkTarget,
                getLinksFunc,
                getDiscriminantFunc,
                includeBuilder.Build()
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }
        #endregion

        #region Shared
        private static Func<TLinkedSource, List<TId>> ToGetLookupIdsFuncForSingleValue<TId>(
            Func<TLinkedSource, TId> getLookupIdFunc) {
            return linkedSource => new List<TId> { getLookupIdFunc(linkedSource) };
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> AddLoadLinkExpression(
           ILoadLinkExpression loadLinkExpression) {
            _addLoadLinkExpressionAction(loadLinkExpression);
            return this;
        }

        public static Func<T, T> CreateIdentityFunc<T>() {
            return x => x;
        }

        private Action<TLinkedSource, int, TChildLinkedSource> InitChildLinkedSourceActionForSingleValue<TChildLinkedSource>(Action<TLinkedSource, TChildLinkedSource> initChildLinkedSourceAction) {


            return (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSourceAction(linkedSource, childLinkedSource);
        }

        private void NullInitChildLinkedSourceActionForSingleValue<TChildLinkedSource>(
            TLinkedSource linkedsource,
            TChildLinkedSource childLinkedSource) 
        {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
        }

        private void NullInitChildLinkedSourceAction<TChildLinkedSource>(
            TLinkedSource linkedsource,
            int referenceIndex,
            TChildLinkedSource childLinkedSource) 
        {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
        }

        private Dictionary<bool, IInclude> CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(IInclude include) {
            return new Dictionary<bool, IInclude>
            {
                {
                    true, //always one include when not polymorphic
                    include
                }
            };
        }

        #endregion
    }
}