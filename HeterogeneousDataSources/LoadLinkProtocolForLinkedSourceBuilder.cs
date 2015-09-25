using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    //stle: enhance that: TId could dispear after query are supported
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpressionAction;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpressionAction)
        {
            _addLoadLinkExpressionAction = addLoadLinkExpressionAction;
        }

        #region Root
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> IsRoot<TId>() {
            //stle: simplify the whole root in linked source concept

            var loadLinkExpression = new LoadLinkExpressionImpl<TId, TLinkedSource, TId, bool>(
                typeof(TLinkedSource).ToString(),
                linkedSourceIsIdForRoot => new List<TId> { linkedSourceIsIdForRoot },
                GetReferencesFuncForSingleValue<TId,TLinkedSource>(),
                (linkedSource, linkTargetValue) => { },
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    CreatePolymorphicNestedLinkedSourceIncludeForNestedLinkedSource<TId,TLinkedSource,TId>(
                        null
                    )
                ),
                true
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        #endregion

        #region Reference
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TReference, TId, bool>(
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                GetReferencesFuncForSingleValue<TReference>(),
                SetReferencesActionForSingleValue(linkTarget),
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
                linkTarget.Id,
                getLookupIdsFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
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
           Func<TLinkedSource, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                GetReferencesFuncForSingleValue<TChildLinkedSource>(),
                SetReferencesActionForSingleValue(linkTarget),
                link => true,
                CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(
                    InitChildLinkedSourceActionForSingleValue(initChildLinkedSource)
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
            Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget.Id,
                getLookupIdsFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
                link => true,
                CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(
                    initChildLinkedSource
                )
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        private Dictionary<bool, IInclude> CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource) {
            return CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                CreatePolymorphicNestedLinkedSourceIncludeForNestedLinkedSource<TLinkedSource,TChildLinkedSource, TId>(
                    initChildLinkedSource
                )
            );
        }

        private IInclude CreatePolymorphicNestedLinkedSourceIncludeForNestedLinkedSource<TLinkTargetOwner, TChildLinkedSource, TId>(
            Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource) 
        {
            Type ctorGenericType = typeof(NestedLinkedSourceInclude<,,,,,>);

            var childLinkedSourceType = typeof(TChildLinkedSource);
            var idType = typeof(TId);
            Type[] typeArgs ={
                typeof(TLinkTargetOwner),
                childLinkedSourceType, 
                idType,
                childLinkedSourceType, 
                GetLinkedSourceModelType(childLinkedSourceType),
                idType
            };

            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            //stle: change to single once obsolete constructor is deleted
            var ctor = ctorSpecificType.GetConstructors().First();

            return (IInclude)ctor.Invoke(
                new object[]{
                    CreateIdentityFunc<TId>(),
                    initChildLinkedSource
                }
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
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getSubLinkedSourceModelsFunc),
                GetReferencesFuncForSingleValue<TChildLinkedSource>(),
                SetReferencesActionForSingleValue(linkTarget),
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    new SubLinkedSourceInclude<TChildLinkedSource, TChildLinkedSource, TChildLinkedSourceModel>()
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
                linkTarget.Id,
                getSubLinkedSourceModelsFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    new SubLinkedSourceInclude<TChildLinkedSource,TChildLinkedSource,TChildLinkedSourceModel>()
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
           Action<IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            //stle: dry
            var includeBuilder = new IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getLinkFunc),
                GetReferencesFuncForSingleValue<TIChildLinkedSource>(),
                SetReferencesActionForSingleValue(linkTarget),
                getDiscriminantFunc,
                includeBuilder.Build()
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> PolymorphicLoadLinkForList<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, List<TLink>> getLinksFunc,
           Expression<Func<TLinkedSource, List<TIChildLinkedSource>>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            var includeBuilder = new IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new LoadLinkExpressionImpl<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                linkTarget.Id,
                getLinksFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
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

        private static Action<TLinkedSource, List<TTargetProperty>> ToLinkActionForSingleValue<TTargetProperty>(
            LinkTarget<TLinkedSource, TTargetProperty> linkTarget) {
            return (linkedSource, propertyValues) =>
                linkTarget.SetTargetProperty(linkedSource, propertyValues.SingleOrDefault());
        }

        private LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> AddLoadLinkExpression(
           ILoadLinkExpression loadLinkExpression) {
            _addLoadLinkExpressionAction(loadLinkExpression);
            return this;
        }

        //stle: dry
        public static Type GetLinkedSourceModelType(Type linkedSourceType) {
            var iLinkedSourceTypes = linkedSourceType.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ILinkedSource<>))
                .ToList();

            EnsureILinkedSourceIsImplementedOnceAndOnlyOnce(linkedSourceType, iLinkedSourceTypes);

            var iLinkedSourceType = iLinkedSourceTypes.Single();
            return iLinkedSourceType.GenericTypeArguments.Single();
        }

        private static void EnsureILinkedSourceIsImplementedOnceAndOnlyOnce(Type linkedSourceType, List<Type> iLinkedSourceTypes) {
            if (!iLinkedSourceTypes.Any()) {
                throw new ArgumentException(
                    string.Format(
                        "{0} must implement ILinkedSource<>.",
                        linkedSourceType
                    ), 
                    "TLinkedSource"
                );
            }

            if (iLinkedSourceTypes.Count > 1) {
                throw new ArgumentException(
                    string.Format(
                        "{0} must implement ILinkedSource<> only once.",
                        linkedSourceType
                    ),
                    "TLinkedSource"
                );
            }
        }

        private static Action<TLinkedSource, List<TIChildLinkedSource>> SetReferencesActionForSingleValue<TIChildLinkedSource>(
            LinkTarget<TLinkedSource, TIChildLinkedSource> linkTarget) 
        {
            return (linkedSource, childLinkedSources) =>
                linkTarget.SetTargetProperty(linkedSource, childLinkedSources.SingleOrDefault());
        }

        private static Func<TLinkedSource, List<TIChildLinkedSource>> GetReferencesFuncForSingleValue<TIChildLinkedSource>()
        {
            return GetReferencesFuncForSingleValue<TLinkedSource, TIChildLinkedSource>();
        }

        private static Func<TReferenceOwner, List<TIChildLinkedSource>> GetReferencesFuncForSingleValue<TReferenceOwner, TIChildLinkedSource>() {
            return linkedSource => {
                throw new InvalidOperationException("Cannot get reference list for single reference.");
            };
        }


        private Func<T, T> CreateIdentityFunc<T>() {
            return x => x;
        }

        private Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> InitChildLinkedSourceActionForSingleValue<TChildLinkedSource>(
            Func<TLinkedSource, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource) 
        {
            return (linkedSource, referenceIndex, childLinkedSource) =>
                    initChildLinkedSource(linkedSource, childLinkedSource);
        }

        private TChildLinkedSource NullInitChildLinkedSourceActionForSingleValue<TChildLinkedSource>(
            TLinkedSource linkedsource,
            TChildLinkedSource childLinkedSource) 
        {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
            return childLinkedSource;
        }

        private TChildLinkedSource NullInitChildLinkedSourceAction<TChildLinkedSource>(
            TLinkedSource linkedsource,
            int referenceIndex,
            TChildLinkedSource childLinkedSource) 
        {
            //using null causes problem with generic type inference, 
            //using a special value work around this limitation of generics
            return childLinkedSource;
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