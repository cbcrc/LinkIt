using System;

namespace HeterogeneousDataSources
{
    public class LinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget
    {
        private readonly Func<TLinkedSource, TTargetProperty> _getterFunc;
        private readonly Action<TLinkedSource, TTargetProperty> _setterAction;

        public LinkTarget(
            string propertyName,
            Func<TLinkedSource, TTargetProperty> getterFunc, 
            Action<TLinkedSource, TTargetProperty> setterAction)
        {
            LinkedSourceType = typeof (TLinkedSource);
            PropertyName = propertyName;
            _getterFunc = getterFunc;
            _setterAction = setterAction;
            Id = string.Format("{0}/{1}", LinkedSourceType.FullName, propertyName);
        }

        public Type LinkedSourceType { get; private set; }
        public string PropertyName { get; private set; }
        public string Id { get; private set; }

        public TTargetProperty GetTargetProperty(TLinkedSource linkedSource){
            return _getterFunc(linkedSource);
        }

        public void SetTargetProperty(TLinkedSource linkedSource, TTargetProperty value){
            _setterAction(linkedSource, value);
        }

        public bool Equals(ILinkTarget other)
        {
            if(other==null){return false;}

            if (LinkedSourceType != other.LinkedSourceType) { return false; }

            return PropertyName == other.PropertyName;
        }

        public override bool Equals(object obj) {
            var asILinkTarget = obj as ILinkTarget;
            return Equals(asILinkTarget);
        }
    }
}