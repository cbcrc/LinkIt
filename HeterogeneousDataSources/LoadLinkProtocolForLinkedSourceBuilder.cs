using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;

namespace HeterogeneousDataSources
{
    //stle: enhance that: TId could dispear after query are supported
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource>
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpressionAction;
        private Type _linkedSourceType;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpressionAction)
        {
            _addLoadLinkExpressionAction = addLoadLinkExpressionAction;
            _linkedSourceType = typeof(TLinkedSource);
        }

        #region Root
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> IsRoot<TId>() {
            return AddLoadLinkExpression(CreateRootLoadLinkExpression<TId>());
        }

        private ILoadLinkExpression CreateRootLoadLinkExpression<TId>() {
            Type rootLoadLinkExpressionGenericType = typeof(RootLoadLinkExpression<,,>);
            Type[] typeArgs ={
                _linkedSourceType, 
                GetLinkedSourceModelType(_linkedSourceType), 
                typeof(TId)
            };
            Type rootLoadLinkExpressionSpecificType = rootLoadLinkExpressionGenericType.MakeGenericType(typeArgs);

            return (ILoadLinkExpression)Activator.CreateInstance(rootLoadLinkExpressionSpecificType);
        }
        #endregion

        #region Reference
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TReference>> linkTargetFunc) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            return AddLoadLinkExpression(
                new ReferencesLoadLinkExpression<TLinkedSource, TReference, TId>(
                    linkTarget.Id,
                    ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                    ToLinkActionForSingleValue(linkTarget)
                )
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkReference<TReference, TId>(
            Func<TLinkedSource, List<TId>> getLookupIdsFunc,
            Expression<Func<TLinkedSource, List<TReference>>> linkTargetFunc) {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            return AddLoadLinkExpression(
                new ReferencesLoadLinkExpression<TLinkedSource, TReference, TId>(
                    linkTarget.Id,
                    getLookupIdsFunc,
                    linkTarget.SetTargetProperty
                )
            );
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

            var loadLinkExpression = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                GetReferencesFuncForSingleValue<TChildLinkedSource>(),
                SetReferencesActionForSingleValue(linkTarget),
                link => true,
                CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(
                    InitChildLinkedSourceActionForSingleValue(initChildLinkedSourceAction)
                ),
                LoadLinkExpressionType.NestedLinkedSource
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

            var loadLinkExpression = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget.Id,
                getLookupIdsFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
                link => true,
                CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(
                    initChildLinkedSourceAction
                ),
                LoadLinkExpressionType.NestedLinkedSource
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, TLink> getLinkFunc,
           Expression<Func<TLinkedSource, TIChildLinkedSource>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            //stle: dry
            var includeBuilder = new IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getLinkFunc),
                GetReferencesFuncForSingleValue<TIChildLinkedSource>(),
                SetReferencesActionForSingleValue(linkTarget),
                getDiscriminantFunc,
                includeBuilder.Build(),
                LoadLinkExpressionType.NestedLinkedSource
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }


        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TIChildLinkedSource, TLink, TDiscriminant>(
           Func<TLinkedSource, List<TLink>> getLinksFunc,
           Expression<Func<TLinkedSource, List<TIChildLinkedSource>>> linkTargetFunc,
           Func<TLink, TDiscriminant> getDiscriminantFunc,
           Action<IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            var includeBuilder = new IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                linkTarget.Id,
                getLinksFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
                getDiscriminantFunc,
                includeBuilder.Build(),
                LoadLinkExpressionType.NestedLinkedSource
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        private Dictionary<bool, IPolymorphicInclude> CreateNestedLinkedSourceIncludeForNonPolymorphicLoadLinkExpression<TChildLinkedSource, TId>(Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction) {
            return CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                CreatePolymorphicNestedLinkedSourceIncludeForNestedLinkedSource<TChildLinkedSource, TId>(
                    initChildLinkedSourceAction
                )
            );
        }

        private IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TChildLinkedSource, TId> CreatePolymorphicNestedLinkedSourceIncludeForNestedLinkedSource<TChildLinkedSource, TId>(
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction) 
        {
            Type ctorGenericType = typeof(PolymorphicNestedLinkedSourceInclude<,,,,,>);

            var childLinkedSourceType = typeof(TChildLinkedSource);
            var idType = typeof(TId);
            Type[] typeArgs ={
                _linkedSourceType,
                childLinkedSourceType, 
                idType,
                childLinkedSourceType, 
                GetLinkedSourceModelType(childLinkedSourceType),
                idType
            };

            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            //stle: change to single once obsolete constructor is deleted
            var ctor = ctorSpecificType.GetConstructors().First();

            return (IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TChildLinkedSource, TId>)ctor.Invoke(
                new object[]{
                    CreateIdentityFunc<TId>(),
                    initChildLinkedSourceAction
                }
            );
        } 
        #endregion

        #region SubLinkedSource
        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, TChildLinkedSourceModel> getSubLinkedSourceModelsFunc,
            Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            return AddLoadLinkExpression(
                new SubLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel>(
                    ToGetLookupIdsFuncForSingleValue(getSubLinkedSourceModelsFunc),
                    ToLinkActionForSingleValue(linkTarget)
                )
            );
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LinkSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            Func<TLinkedSource, List<TChildLinkedSourceModel>> getSubLinkedSourceModelsFunc,
            Expression<Func<TLinkedSource, List<TChildLinkedSource>>> linkTargetFunc
        )
            where TChildLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TChildLinkedSourceModel, bool>(
                linkTarget.Id,
                getSubLinkedSourceModelsFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
                link => true,
                CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(
                    new PolymorphicSubLinkedSourceInclude<TChildLinkedSource,TChildLinkedSource,TChildLinkedSourceModel>()
                ),
                LoadLinkExpressionType.SubLinkedSource
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LinkSubLinkedSource<TIChildLinkedSource, TIChildLinkedSourceModel, TDiscriminant>(
   Func<TLinkedSource, List<TIChildLinkedSourceModel>> getSubLinkedSourceModelsFunc,
   Expression<Func<TLinkedSource, List<TIChildLinkedSource>>> linkTargetFunc,
   Func<TIChildLinkedSourceModel, TDiscriminant> getDiscriminantFunc,
   Action<IncludeBuilder<TLinkedSource, TIChildLinkedSource, TIChildLinkedSourceModel, TDiscriminant>> includes) {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);
            var includeBuilder = new IncludeBuilder<TLinkedSource, TIChildLinkedSource, TIChildLinkedSourceModel, TDiscriminant>();
            includes(includeBuilder);

            var loadLinkExpression = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TIChildLinkedSource, TIChildLinkedSourceModel, TDiscriminant>(
                linkTarget.Id,
                getSubLinkedSourceModelsFunc,
                linkTarget.GetTargetProperty,
                linkTarget.SetTargetProperty,
                getDiscriminantFunc,
                includeBuilder.Build(),
                LoadLinkExpressionType.SubLinkedSource
            );

            return AddLoadLinkExpression(loadLinkExpression);
        }


        //public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LinkSubLinkedSource<TIChildLinkedSource, TIChildLinkedSourceModel, TDiscriminant>(
        //    Func<TLinkedSource, List<TIChildLinkedSourceModel>> getSubLinkedSourceModelsFunc,
        //    Expression<Func<TLinkedSource, List<TIChildLinkedSource>>> linkTargetFunc,
        //    Func<TIChildLinkedSourceModel, TDiscriminant> getDiscriminantFunc,
        //    Action<IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>> includes) 
        //{

          
        //}


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

        private static Func<TLinkedSource, List<TIChildLinkedSource>> GetReferencesFuncForSingleValue<TIChildLinkedSource>() {
            return linkedSource => {
                throw new InvalidOperationException("Cannot get reference list for single reference.");
            };
        }

        private Func<T, T> CreateIdentityFunc<T>() {
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

        private Dictionary<bool, IPolymorphicInclude> CreatePolymorphicIncludesForNonPolymorphicLoadLinkExpression(IPolymorphicInclude include) {
            return new Dictionary<bool, IPolymorphicInclude>
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