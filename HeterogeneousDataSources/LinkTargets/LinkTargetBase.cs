using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources
{
    public abstract class LinkTargetBase<TLinkedSource, TTargetProperty> : ILinkTarget
    {
        public LinkTargetBase(string propertyName)
        {
            LinkedSourceType = typeof (TLinkedSource);
            PropertyName = propertyName;
            Id = string.Format("{0}/{1}", LinkedSourceType.FullName, propertyName);
        }

        public Type LinkedSourceType { get; private set; }
        public string PropertyName { get; private set; }
        public string Id { get; private set; }

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

        //stle: simplify by putting loop in the load link expression
        public abstract void SetLinkTargetValues(
            TLinkedSource linkedSource,
            List<LinkTargetValueWithIndex<TTargetProperty>> listOfLinkTargetValueWithIndex,
            int numOfLinkedTargetValue
        );
    }
}