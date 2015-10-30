using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public class SingleValueLinkTarget<TLinkedSource, TTargetProperty>:LinkTargetBase<TLinkedSource, TTargetProperty>
    {
        private readonly Action<TLinkedSource, TTargetProperty> _setterAction;

        public SingleValueLinkTarget(
            string propertyName,
            Action<TLinkedSource, TTargetProperty> setterAction
        )
            :base(propertyName)
        {
            _setterAction = setterAction;
        }

        //stle: simplify by putting loop in the load link expression
        public override void SetLinkTargetValues(
            TLinkedSource linkedSource,
            List<LinkTargetValueWithIndex<TTargetProperty>> listOfLinkTargetValueWithIndex,
            int numOfLinkedTargetValue)
        {
            if (!listOfLinkTargetValueWithIndex.Any()) {
                //In order to avoid overriding value set by another include
                return;
            }

            _setterAction(linkedSource, listOfLinkTargetValueWithIndex.Single().TargetValue);
        }
    }
}