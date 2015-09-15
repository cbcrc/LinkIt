using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HeterogeneousDataSources.LoadLinkExpressions;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;

namespace HeterogeneousDataSources
{
    //stle: enhance that: TChildLinkedSourceModel could be infered by reflection
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

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> LoadLinkNestedLinkedSource<TChildLinkedSource,TM, TId>(
           Func<TLinkedSource, TId> getLookupIdFunc,
           Expression<Func<TLinkedSource, TChildLinkedSource>> linkTargetFunc)
            where TChildLinkedSource : class, ILinkedSource<TM>, new()
        {
            var linkTarget = LinkTargetFactory.Create(linkTargetFunc);

            var x = new PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TChildLinkedSource, TId, bool>(
                linkTarget.Id,
                ToGetLookupIdsFuncForSingleValue(getLookupIdFunc),
                GetReferencesFuncForSingleValue<TChildLinkedSource>(),
                SetReferencesActionForSingleValue(linkTarget),
                link => true,
                new Dictionary<
                    bool,
                    IPolymorphicNestedLinkedSourceInclude<TChildLinkedSource, TId>
                    >
                {
                    {
                        true, //always one include when not polymorphic
                        new PolymorphicNestedLinkedSourceInclude<
                            TChildLinkedSource,
                            TId,
                            TChildLinkedSource,
                            TM,
                            TId
                        >(link => link)
                    }
                }
            );

            return AddLoadLinkExpression(x);
        }

        private static Action<TLinkedSource, List<TIChildLinkedSource>> SetReferencesActionForSingleValue<TIChildLinkedSource>(
            LinkTarget<TLinkedSource, TIChildLinkedSource> linkTarget)
        {
            return (linkedSource, childLinkedSources) =>
                linkTarget.SetTargetProperty(linkedSource, childLinkedSources.SingleOrDefault());
        }

        private static Func<TLinkedSource, List<TIChildLinkedSource>> GetReferencesFuncForSingleValue<TIChildLinkedSource>(){
            return linkedSource => {
                throw new InvalidOperationException("Cannot get reference list for single reference.");
            };
        }

        private ILoadLinkExpression CreateNestedLinkedSourceLoadLinkExpression<TChildLinkedSource, TId>(
            string linkTargetId,
            Func<TLinkedSource, List<TId>> getLookupIdsFunc, 
            Action<TLinkedSource, List<TChildLinkedSource>> linkAction) 
        {
            Type nestedLinkedSourceLoadLinkExpressionGenericType = typeof(NestedLinkedSourcesLoadLinkExpression<,,,>);
            
            Type[] typeArgs ={
                _linkedSourceType, 
                typeof(TChildLinkedSource),
                GetLinkedSourceModelType(typeof(TChildLinkedSource)),
                typeof(TId)
            };

            Type nestedLinkedSourceLoadLinkExpressionSpecificType = nestedLinkedSourceLoadLinkExpressionGenericType.MakeGenericType(typeArgs);

            //stle: change to single once obsolete constructor is deleted
            var ctor = nestedLinkedSourceLoadLinkExpressionSpecificType.GetConstructors().First();

            return (ILoadLinkExpression)ctor.Invoke(
                new object[]{
                    linkTargetId,
                    getLookupIdsFunc,
                    linkAction
                }
            );
        }

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

        private Type GetLinkedSourceModelType(Type linkedSourceType) {
            var iLinkedSourceTypes = linkedSourceType.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ILinkedSource<>))
                .ToList();

            EnsureILinkedSourceIsImplementedOnceAndOnlyOnce(iLinkedSourceTypes);

            var iLinkedSourceType = iLinkedSourceTypes.Single();
            return iLinkedSourceType.GenericTypeArguments.Single();
        }

        private static void EnsureILinkedSourceIsImplementedOnceAndOnlyOnce(List<Type> iLinkedSourceTypes) {
            if (!iLinkedSourceTypes.Any()) {
                throw new ArgumentException(string.Format("{0} must implement ILinkedSource<> at least once."), "TLinkedSource");
            }

            if (iLinkedSourceTypes.Count > 1) {
                throw new ArgumentException(string.Format("{0} must implement ILinkedSource<> only once."), "TLinkedSource");
            }
        } 
        #endregion
    }
}