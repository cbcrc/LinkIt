using System;
using LinkIt.LinkTargets.Interfaces;

namespace LinkIt.LinkTargets
{
    public class SingleValueLinkTarget<TLinkedSource, TTargetProperty>:ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly Action<TLinkedSource, TTargetProperty> _setterAction;

        public SingleValueLinkTarget(
            string id,
            Action<TLinkedSource, TTargetProperty> setterAction)
        {
            _setterAction = setterAction;
            Id = id;
        }

        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex){
            _setterAction(linkedSource, linkTargetValue);
        }

        public void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues){
            //Do nothing for single value
        }

        public string Id { get; private set; }

        public bool Equals(ILinkTarget other) {
            if (other == null) { return false; }

            return Id.Equals(other.Id);
        }
    }
}