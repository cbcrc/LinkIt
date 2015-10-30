using System;

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

        public override void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex){
            _setterAction(linkedSource, linkTargetValue);
        }

        public override void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues){
            //Do nothing for single value
        }
    }
}