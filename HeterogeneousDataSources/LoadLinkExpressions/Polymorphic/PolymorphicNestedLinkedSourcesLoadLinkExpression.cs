using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> : ILoadLinkExpression
    {
        private readonly Func<TLinkedSource, List<TLink>> _getLinksFunc;
        private readonly Func<TLinkedSource, List<TIChildLinkedSource>> _getReferences;
        private readonly Action<TLinkedSource, List<TIChildLinkedSource>> _setReferences;
        private readonly Func<TLink, TDiscriminant> _getDiscriminantFunc;
        private readonly Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>> _includes;

        public PolymorphicNestedLinkedSourcesLoadLinkExpression(
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            //stle: http://stackoverflow.com/questions/7723744
            Func<TLinkedSource, List<TIChildLinkedSource>> getReferences,
            Action<TLinkedSource, List<TIChildLinkedSource>> setReferences,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>> includes)
        {
            _getLinksFunc = getLinksFunc;
            _getReferences = getReferences;
            _setReferences = setReferences;
            _getDiscriminantFunc = getDiscriminantFunc;
            _includes = includes;

            LinkedSourceType = typeof (TLinkedSource);
            ReferenceTypes = _includes
                .Select(include => include.Value.ReferenceType)
                .ToList();
            LoadLinkExpressionType = LoadLinkExpressionType.NestedLinkedSource;
        }

        public Type LinkedSourceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }
        public LoadLinkExpressionType LoadLinkExpressionType { get; private set; }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded)
        {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            LoadLinkExpressionUtil.EnsureIsOfReferenceType(this,referenceTypeToBeLoaded);

            var linksForReferenceType = GetLinksForReferenceType((TLinkedSource)linkedSource, referenceTypeToBeLoaded);

            foreach (var linkForReferenceType in linksForReferenceType) {
                var include = GetSelectedInclude(linkForReferenceType);
                include.AddLookupIds(linkForReferenceType, lookupIdContext);
            }
        }

        public void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked) {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            LoadLinkExpressionUtil.EnsureIsOfReferenceType(this, referenceTypeToBeLinked);

            Link((TLinkedSource)linkedSource, loadedReferenceContext, referenceTypeToBeLinked);
        }

        private void Link(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked) {
            InitListOfReferencesIfNull(linkedSource);

            var linksToEntityOfReferenceType = GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLinked);

            foreach (var linkToEntityOfReferenceType in linksToEntityOfReferenceType) {
                var include = GetSelectedInclude(linkToEntityOfReferenceType.Link);

                var childLinkedSources = include.CreateChildLinkedSources(linkToEntityOfReferenceType.Link, loadedReferenceContext);

                //stle: need single
                _getReferences(linkedSource)[linkToEntityOfReferenceType.Index] = childLinkedSources.Single();
            }
        }

        private void InitListOfReferencesIfNull(TLinkedSource linkedSource)
        {
            if (_getReferences(linkedSource) == null){
                _setReferences(linkedSource, new TIChildLinkedSource[GetLinkCount(linkedSource)].ToList());
            }
        }

        private List<TLink> GetLinksForReferenceType(TLinkedSource linkedSource, Type referenceTypeToBeLoaded) {
            return GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLoaded)
                .Select(linkWithIndex => linkWithIndex.Link)
                .ToList();
        }


        private List<LinkWithIndex<TLink>> GetLinksWithIndexForReferenceType(TLinkedSource linkedSource, Type referenceTypeToBeLoaded)
        {
            var allLinks = _getLinksFunc(linkedSource);

            return allLinks
                .Select((link, index) => new LinkWithIndex<TLink>(link, index))
                .Where(linkWithIndex => GetSelectedInclude(linkWithIndex.Link).ReferenceType == referenceTypeToBeLoaded)
                .ToList();
        }

        private int GetLinkCount(TLinkedSource linkedSource){
            return _getLinksFunc(linkedSource).Count;
        }

        private IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink> GetSelectedInclude(TLink link) {
            var discriminant = _getDiscriminantFunc(link);
            if (!_includes.ContainsKey(discriminant)) {
                throw new InvalidOperationException(
                    string.Format(
                        "There is no include statement for discriminant={0} in {1}.",
                        discriminant,
                        LinkedSourceType
                    )
                );
            }

            return _includes[discriminant];
        }
    }
}