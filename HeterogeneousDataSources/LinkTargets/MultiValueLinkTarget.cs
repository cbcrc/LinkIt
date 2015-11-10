using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LinkTargets
{
    public class MultiValueLinkTarget<TLinkedSource, TTargetProperty>:ILinkTarget<TLinkedSource, TTargetProperty>
    {
        private readonly Func<TLinkedSource, List<TTargetProperty>> _getterFunc;
        private readonly Action<TLinkedSource, List<TTargetProperty>> _setterAction;

        public MultiValueLinkTarget(
            string id,
            Func<TLinkedSource, List<TTargetProperty>> getterFunc, 
            Action<TLinkedSource, List<TTargetProperty>> setterAction)
        {
            Id = id;
            _getterFunc = getterFunc;
            _setterAction = setterAction;
        }

        public void SetLinkTargetValue(TLinkedSource linkedSource, TTargetProperty linkTargetValue, int linkTargetValueIndex)
        {
            _getterFunc(linkedSource)[linkTargetValueIndex] = linkTargetValue;
        }

        public void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues)
        {
            if (_getterFunc(linkedSource) == null) {
                var polymorphicListToBeBuilt = new TTargetProperty[numOfLinkedTargetValues].ToList();
                _setterAction(linkedSource, polymorphicListToBeBuilt);
            }
        }

        public string Id { get; private set; }

        public bool Equals(ILinkTarget other) {
            if (other == null) { return false; }

            return Id.Equals(other.Id);
        }
    }
}