using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicNestedLinkedSourcesLoadLinkExpression<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> : ILoadLinkExpression, INestedLoadLinkExpression
    {
        private readonly Func<TLinkedSource, List<TLink>> _getLinksFunc;
        private readonly Func<TLinkedSource, List<TIChildLinkedSource>> _getReferences;
        private readonly Action<TLinkedSource, List<TIChildLinkedSource>> _setReferences;
        private readonly Func<TLink, TDiscriminant> _getDiscriminantFunc;
        private readonly Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TLinkedSource,TIChildLinkedSource, TLink>> _includes;

        public PolymorphicNestedLinkedSourcesLoadLinkExpression(
            string linkTargetId,
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            //stle: http://stackoverflow.com/questions/7723744
            Func<TLinkedSource, List<TIChildLinkedSource>> getReferences,
            Action<TLinkedSource, List<TIChildLinkedSource>> setReferences,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>> includes)
        {
            LinkTargetId = linkTargetId;
            _getLinksFunc = getLinksFunc;
            _getReferences = getReferences;
            _setReferences = setReferences;
            _getDiscriminantFunc = getDiscriminantFunc;
            _includes = includes;

            LinkedSourceType = typeof (TLinkedSource);
            ReferenceTypes = _includes.Values
                .Select(include => include.ReferenceType)
                .ToList();
            ChildLinkedSourceTypes = _includes.Values
                .Select(include => include.ChildLinkedSourceType)
                .ToList();
            LoadLinkExpressionType = LoadLinkExpressionType.NestedLinkedSource;
        }

        public string LinkTargetId { get; private set; }

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

        private void Link(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked)
        {
            if (GetLinks(linkedSource).Count <= 1){
                LinkListWithZeroOrOneReference(linkedSource, loadedReferenceContext, referenceTypeToBeLinked);
            }
            else{
                LinkListWithManyReferences(linkedSource, loadedReferenceContext, referenceTypeToBeLinked);
            }
        }

        private void LinkListWithZeroOrOneReference(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked)
        {
            var link = GetLinks(linkedSource).SingleOrDefault();
            if (link == null){
                _setReferences(linkedSource, new List<TIChildLinkedSource>());
                return;
            }

            var include = GetSelectedInclude(link);
            if (include.ReferenceType != referenceTypeToBeLinked) { return; }

            var childLinkedSources = include
                .CreateChildLinkedSources(link, loadedReferenceContext, linkedSource, 0)
                .Where(childLinkedSource => childLinkedSource != null)
                .ToList();

            _setReferences(linkedSource, childLinkedSources);
        }

        private void LinkListWithManyReferences(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            InitListOfReferencesIfNull(linkedSource, loadedReferenceContext);

            var linksToEntityOfReferenceType = GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLinked);

            foreach (var linkToEntityOfReferenceType in linksToEntityOfReferenceType)
            {
                var include = GetSelectedInclude(linkToEntityOfReferenceType.Link);

                var childLinkedSources = include.CreateChildLinkedSources(
                    linkToEntityOfReferenceType.Link,
                    loadedReferenceContext,
                    linkedSource,
                    linkToEntityOfReferenceType.Index
                );

                //stle: need single
                _getReferences(linkedSource)[linkToEntityOfReferenceType.Index] = childLinkedSources.SingleOrDefault();
            }
        }

        private void InitListOfReferencesIfNull(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            if (_getReferences(linkedSource) == null)
            {
                var polymorphicListToBeBuilt = new TIChildLinkedSource[GetLinkCount(linkedSource)].ToList();
                loadedReferenceContext.OnLinkCompleted(
                    ()=>RemoveNullFromPolymorphicList(polymorphicListToBeBuilt)
                );

                _setReferences(linkedSource, polymorphicListToBeBuilt);
            }
        }

        private void RemoveNullFromPolymorphicList<TIChildLinkedSource>(List<TIChildLinkedSource> polymorphicListToBeBuilt)
        {
            polymorphicListToBeBuilt.RemoveAll(item => item == null);
        }

        private List<TLink> GetLinksForReferenceType(TLinkedSource linkedSource, Type referenceTypeToBeLoaded) {
            return GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLoaded)
                .Select(linkWithIndex => linkWithIndex.Link)
                .ToList();
        }


        private List<LinkWithIndex<TLink>> GetLinksWithIndexForReferenceType(TLinkedSource linkedSource, Type referenceTypeToBeLoaded)
        {
            var links = GetLinks(linkedSource);

            return links
                .Select((link, index) => new LinkWithIndex<TLink>(link, index))
                .Where(linkWithIndex => linkWithIndex.Link != null)
                .Where(linkWithIndex => GetSelectedInclude(linkWithIndex.Link).ReferenceType == referenceTypeToBeLoaded)
                .ToList();
        }

        private int GetLinkCount(TLinkedSource linkedSource){
            return GetLinks(linkedSource).Count;
        }

        private List<TLink> GetLinks(TLinkedSource linkedSource){
            var links = _getLinksFunc(linkedSource);

            if (links == null) { return new List<TLink>(); }

            return links;
        }

        private IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink> GetSelectedInclude(TLink link) {
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

        public List<Type> ChildLinkedSourceTypes { get; private set; }
    }
}