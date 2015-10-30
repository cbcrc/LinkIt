using System;
using System.Collections.Generic;
using System.Linq;

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

        public override void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            _getterFunc(linkedSource)[linkTargetValueIndex] = linkTargetValue;
        }

        public override void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues)
        {
            if (_getterFunc(linkedSource) == null) {
                var polymorphicListToBeBuilt = new TTargetProperty[numOfLinkedTargetValues].ToList();
                _setterAction(linkedSource, polymorphicListToBeBuilt);
            }
        }
    }
}