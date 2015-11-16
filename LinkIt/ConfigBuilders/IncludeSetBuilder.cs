using System;
using System.Collections.Generic;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.LoadLinkExpressions.Includes;
using LinkIt.LoadLinkExpressions.Includes.Interfaces;

namespace LinkIt.ConfigBuilders
{
    public class IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includeByDiscriminantValue = 
            new Dictionary<TDiscriminant, IInclude>();

        public IncludeSetBuilder(ILinkTarget linkTarget)
        {
            LinkTarget = linkTarget;
        }

        public ILinkTarget LinkTarget { get; private set; }

        public IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget> Include<TLinkTarget>()
            where TLinkTarget : TAbstractLinkTarget
        {
            return new IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget>(this);
        }

        internal void AddToIncludeSet(TDiscriminant discriminant, IInclude include){
            if (_includeByDiscriminantValue.ContainsKey(discriminant)){
                throw new ArgumentException(
                    string.Format(
                        "{0}: cannot have many includes for the same discriminant ({1}).",
                        LinkTarget.Id,
                        discriminant
                    )
                );
            }

            _includeByDiscriminantValue.Add(discriminant,include);
        }

        internal IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> Build(Func<TLink, TDiscriminant> getDiscriminantFunc) {
            return new IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
                _includeByDiscriminantValue,
                getDiscriminantFunc
            );
        }
    }
}