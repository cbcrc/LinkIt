using System;
using System.Linq.Expressions;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{

    //stle: enhance that: TChildLinkedSourceModel could be infered by reflection
    //stle: enhance that: TId could dispear after query are supported
    public class LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource, TChildLinkedSourceModel, TChildLinkedSourceModelId>
        where TLinkedSource : class, ILinkedSource<TChildLinkedSourceModel>, new() 
    {
        private readonly Action<ILoadLinkExpression> _addLoadLinkExpressionAction;

        public LoadLinkProtocolForLinkedSourceBuilder(Action<ILoadLinkExpression> addLoadLinkExpressionAction)
        {
            _addLoadLinkExpressionAction = addLoadLinkExpressionAction;
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource, TChildLinkedSourceModel, TChildLinkedSourceModelId> LoadLinkReference<TReference, TId>(
            Func<TLinkedSource, TId> getLookupIdFunc,
            Expression<Func<TLinkedSource, TReference>> linkTargetFunc) 
        {
            var setterAction = LinkTargetFactory.Create(linkTargetFunc);

            var loadLinkExpression = new ReferenceLoadLinkExpression<TLinkedSource, TReference, TId>(
                getLookupIdFunc,
                setterAction.SetTargetProperty
            );

            _addLoadLinkExpressionAction(loadLinkExpression);

            return this;
        }

        public LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource, TChildLinkedSourceModel, TChildLinkedSourceModelId> IsRoot()
        {
            var loadLinkExpression = new RootLoadLinkExpression<TLinkedSource, TChildLinkedSourceModel, TChildLinkedSourceModelId>();

            _addLoadLinkExpressionAction(loadLinkExpression);

            return this;
        }

    }
}