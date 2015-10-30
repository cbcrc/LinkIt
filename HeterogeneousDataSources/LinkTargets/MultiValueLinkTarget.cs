using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class MultiValueLinkTarget<TLinkedSource, TTargetProperty>:LinkTargetBase<TLinkedSource, TTargetProperty>
    {
        private readonly Func<TLinkedSource, List<TTargetProperty>> _getterFunc;
        private readonly Action<TLinkedSource, List<TTargetProperty>> _setterAction;

        public MultiValueLinkTarget(
            string propertyName,
            Func<TLinkedSource, List<TTargetProperty>> getterFunc, 
            Action<TLinkedSource, List<TTargetProperty>> setterAction
        )
            :base(propertyName)
        {
            _getterFunc = getterFunc;
            _setterAction = setterAction;
        }

        //stle: simplify by putting loop in the load link expression
        public override void SetLinkTargetValues(
            TLinkedSource linkedSource,
            List<LinkTargetValueWithIndex<TTargetProperty>> listOfLinkTargetValueWithIndex,
            int numOfLinkedTargetValue
            )
        {
            LazyInit(linkedSource, numOfLinkedTargetValue);

            //In order to avoid overriding value set by another include
            foreach (var linkTargetValueWithIndex in listOfLinkTargetValueWithIndex) {
                _getterFunc(linkedSource)[linkTargetValueWithIndex.Index] = linkTargetValueWithIndex.TargetValue;
            }
        }

        private void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValue)
        {
            if (_getterFunc(linkedSource) == null) {
                var polymorphicListToBeBuilt = new TTargetProperty[numOfLinkedTargetValue].ToList();
                _setterAction(linkedSource, polymorphicListToBeBuilt);
            }
        }
    }
}