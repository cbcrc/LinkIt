using System.Collections.Generic;
using HeterogeneousDataSources.LinkTargets;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.ConfigBuilders
{
    public class IncludeBuilder<TLinkedSource, TILinkTarget, TLink, TDiscriminant>
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includeByDiscriminantValue = 
            new Dictionary<TDiscriminant, IInclude>();

        public IncludeBuilder(ILinkTarget linkTarget)
        {
            LinkTarget = linkTarget;
        }

        public ILinkTarget LinkTarget { get; private set; }

        public IncludeAsBuilder<TLinkedSource, TILinkTarget, TLink, TDiscriminant, TLinkTarget> Include<TLinkTarget>()
            where TLinkTarget : TILinkTarget
        {
            return new IncludeAsBuilder<TLinkedSource, TILinkTarget, TLink, TDiscriminant, TLinkTarget>(this);
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