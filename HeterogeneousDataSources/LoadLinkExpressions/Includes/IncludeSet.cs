using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public class IncludeSet<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> 
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includes;
        private readonly Func<TLink, TDiscriminant> _getDiscriminantFunc;

        public IncludeSet(Dictionary<TDiscriminant, IInclude> includes, Func<TLink, TDiscriminant> getDiscriminantFunc)
        {
            _includes = includes;
            _getDiscriminantFunc = getDiscriminantFunc;
        }

        public IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink> GetIncludeWithCreateNestedLinkedSource(TLink link) {
            return GetInclude<IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>>(link);
        }

        public IIncludeWithCreateSubLinkedSource<TIChildLinkedSource> GetIncludeWithCreateSubLinkedSource(TLink link) {
            return GetInclude<IIncludeWithCreateSubLinkedSource<TIChildLinkedSource>>(link);
        }

        public IIncludeWithAddLookupId<TLink> GetIncludeWithAddLookupId(TLink linkForReferenceType) {
            return GetInclude<IIncludeWithAddLookupId<TLink>>(linkForReferenceType);
        }

        public IIncludeWithGetReference<TIChildLinkedSource, TLink> GetIncludeWithGetReference(TLink link) {
            return GetInclude<IIncludeWithGetReference<TIChildLinkedSource, TLink>>(link);
        }

        public List<IIncludeWithAddLookupId<TLink>> GetIncludesWithAddLookupId(){
            return GetIncludes<IIncludeWithAddLookupId<TLink>>();
        }

        public List<IIncludeWithChildLinkedSource> GetIncludesWithChildLinkedSource(){
            return GetIncludes<IIncludeWithChildLinkedSource>();
        }

        private TInclude GetInclude<TInclude>(TLink link) 
            where TInclude:class
        {
            var discriminant = _getDiscriminantFunc(link);
            if (!_includes.ContainsKey(discriminant)) {
                throw new InvalidOperationException(
                    string.Format(
                        "There is no include for discriminant={0} in {1}.",
                        discriminant,
                        typeof(TLinkedSource)
                    )
                );
            }

            var include = _includes[discriminant];

            return include as TInclude;
        }

        private List<TInclude> GetIncludes<TInclude>() 
            where TInclude:class
        {
            return _includes.Values
                .Where(include => include is TInclude)
                .Cast<TInclude>()
                .ToList();
        }
    }
}