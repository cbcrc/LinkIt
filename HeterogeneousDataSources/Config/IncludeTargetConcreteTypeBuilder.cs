using System.Collections.Generic;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    public class IncludeTargetConcreteTypeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includeByDiscriminantValue = 
            new Dictionary<TDiscriminant, IInclude>();

        public IncludeTargetConcreteTypeBuilder(ILinkTarget linkTarget)
        {
            LinkTarget = linkTarget;
        }

        public ILinkTarget LinkTarget { get; private set; }

        //stle: fix terminology: TTargetConcreteType
        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant, TTargetConcreteType> Include<TTargetConcreteType>()
            where TTargetConcreteType : TIChildLinkedSource
        {
            return new IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant, TTargetConcreteType>(this);
        }

        //stle: fix namiing Include vs AddInclude is confusing
        //stle: use an interface for public readibility
        internal void AddInclude(TDiscriminant discriminant, IInclude include){
            _includeByDiscriminantValue.Add(discriminant,include);
        }

        internal Dictionary<TDiscriminant, IInclude> Build() {
            return _includeByDiscriminantValue;
        }
    }
}