using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    //stle: TIChildLinkedSource is not more of a TTarget
    public class LoadLinkExpressionImpl<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> :
        INestedLoadLinkExpression
    {
        private readonly Func<TLinkedSource, List<TLink>> _getLinksFunc;
        private readonly Func<TLinkedSource, List<TIChildLinkedSource>> _getReferences;
        private readonly Action<TLinkedSource, List<TIChildLinkedSource>> _setReferences;
        private readonly IncludeSet<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> _includeSet;

        public LoadLinkExpressionImpl(
            string linkTargetId,
            Func<TLinkedSource, List<TLink>> getLinksFunc,
            //stle: http://stackoverflow.com/questions/7723744
            Func<TLinkedSource, List<TIChildLinkedSource>> getReferences,
            Action<TLinkedSource, List<TIChildLinkedSource>> setReferences,
            Func<TLink, TDiscriminant> getDiscriminantFunc,
            Dictionary<TDiscriminant, IInclude> includes,
            LoadLinkExpressionType tempLoadLinkExpressionType
            )
        {
            LinkTargetId = linkTargetId;
            _getLinksFunc = getLinksFunc;
            _getReferences = getReferences;
            _setReferences = setReferences;
            _includeSet = new IncludeSet<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>(
                includes,
                getDiscriminantFunc
                );
            LinkedSourceType = typeof (TLinkedSource);

            ReferenceTypes = _includeSet.GetIncludesWithAddLookupId()
                .Select(include => include.ReferenceType)
                .ToList();

            ChildLinkedSourceTypes = _includeSet.GetIncludesWithChildLinkedSource()
                .Select(include => include.ChildLinkedSourceType)
                .ToList();

            //stle: this must go away
            LoadLinkExpressionType = tempLoadLinkExpressionType;
        }

        public string LinkTargetId { get; private set; }

        public Type LinkedSourceType { get; private set; }
        public List<Type> ReferenceTypes { get; private set; }
        public LoadLinkExpressionType LoadLinkExpressionType { get; private set; }

        public void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded)
        {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            LoadLinkExpressionUtil.EnsureIsOfReferenceType(this, referenceTypeToBeLoaded);

            var linksForReferenceType = GetLinksForReferenceType((TLinkedSource) linkedSource, referenceTypeToBeLoaded);

            foreach (var linkForReferenceType in linksForReferenceType)
            {
                var include = _includeSet.GetIncludeWithAddLookupId(linkForReferenceType);
                //stle: assume include!=null

                include.AddLookupId(linkForReferenceType, lookupIdContext);
            }
        }

        public void LinkSubLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            SetLinkTargetValue(
                linkedSource,
                linkWithIndex =>
                    GetSubLinkedSource(
                        linkWithIndex,
                        loadedReferenceContext
                    )
            );
        }

        private TIChildLinkedSource GetSubLinkedSource(LinkWithIndex<TLink> linkWithIndex, LoadedReferenceContext loadedReferenceContext) {
            //stle: dry on no include
            var include = _includeSet.GetIncludeWithCreateSubLinkedSource(linkWithIndex.Link);
            if (include == null) {
                //stle: default mean do not assign
                return default(TIChildLinkedSource);
            }

            return include.CreateSubLinkedSource(linkWithIndex.Link, loadedReferenceContext);
        }

        public void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            SetLinkTargetValue(
                linkedSource,
                linkWithIndex =>
                    GetReference(
                        linkWithIndex,
                        loadedReferenceContext
                    )
            );
        }

        private TIChildLinkedSource GetReference(LinkWithIndex<TLink> linkWithIndex, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: dry on no include
            var include = _includeSet.GetIncludeWithGetReference(linkWithIndex.Link);
            if (include == null){
                //stle: default mean do not assign
                return default(TIChildLinkedSource);
            }

            return include.GetReference(linkWithIndex.Link, loadedReferenceContext);
        }

        public void LinkNestedLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            SetLinkTargetValue(
                linkedSource, 
                linkWithIndex => 
                    GetNestedLinkedSource(
                        linkWithIndex,
                        loadedReferenceContext,
                        referenceTypeToBeLinked,
                        linkedSource
                    )
            );
        }

        //stle: maybe responsabilize the Include
        private TIChildLinkedSource GetNestedLinkedSource(
            LinkWithIndex<TLink> linkWithIndex,
            LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked,
            object linkedSource)
        {
            //stle: dry
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            return GetNestedLinkedSource(linkWithIndex, loadedReferenceContext, referenceTypeToBeLinked, (TLinkedSource)linkedSource);
        }

        //stle: maybe responsabilize the Include
        private TIChildLinkedSource GetNestedLinkedSource(
            LinkWithIndex<TLink> linkWithIndex, 
            LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked,
            TLinkedSource linkedSource)
        {

            //stle: split the two query to simplify things
            var include = _includeSet.GetIncludeWithCreateNestedLinkedSource(linkWithIndex.Link);
            if (include == null || include.ReferenceType != referenceTypeToBeLinked){
                //stle: default mean do not assign
                return default(TIChildLinkedSource);
            }

            return include.CreateNestedLinkedSource(
                linkWithIndex.Link,
                loadedReferenceContext,
                linkedSource,
                linkWithIndex.Index
            );
        }












        private void SetLinkTargetValue(object linkedSource, Func<LinkWithIndex<TLink>, TIChildLinkedSource> getLinkTargetValueForLink) {
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            SetLinkTargetValue((TLinkedSource)linkedSource,getLinkTargetValueForLink);
        }


        private void SetLinkTargetValue(TLinkedSource linkedSource, Func<LinkWithIndex<TLink>, TIChildLinkedSource> getLinkTargetValueForLink)
        {
            var linkCount = GetLinks(linkedSource).Count;

            //stle: dry with link count and null
            if (linkCount == 0){
                SetLinkTargetWithZeroValue(linkedSource);
            }
            if (linkCount == 1) {
                SetLinkTargetWithOneValue(linkedSource, getLinkTargetValueForLink);
            }
            else{
                SetLinkTargetWithManyValues(linkedSource, getLinkTargetValueForLink);
            }
        }

        private void SetLinkTargetWithZeroValue(TLinkedSource linkedSource)
        {
            _setReferences(linkedSource, new List<TIChildLinkedSource>());
        }

        private void SetLinkTargetWithOneValue(TLinkedSource linkedSource, Func<LinkWithIndex<TLink>, TIChildLinkedSource> getLinkTargetValueForLink) {
            var listOfLinkTargetWithIndex = GetListOfLinkTargetValueWithIndex(linkedSource, getLinkTargetValueForLink);
            if (!listOfLinkTargetWithIndex.Any()) { return; }

            var targetValue = listOfLinkTargetWithIndex
                .Single()
                .TargetValue;

            _setReferences(linkedSource, new List<TIChildLinkedSource>{ targetValue });
        }

        private void SetLinkTargetWithManyValues(TLinkedSource linkedSource, Func<LinkWithIndex<TLink>, TIChildLinkedSource> getLinkTargetValueForLink) 
        {
            InitListOfReferencesIfNull(linkedSource);
            var listOfLinkTargetWithIndex = GetListOfLinkTargetValueWithIndex(linkedSource, getLinkTargetValueForLink);

            foreach (var linkTargetWithIndex in listOfLinkTargetWithIndex) {
                _getReferences(linkedSource)[linkTargetWithIndex.Index] = linkTargetWithIndex.TargetValue;
            }
        }

        private List<LinkTargetValueWithIndex<TIChildLinkedSource>> GetListOfLinkTargetValueWithIndex(
            TLinkedSource linkedSource,
            Func<LinkWithIndex<TLink>, TIChildLinkedSource> getLinkTargetValueForLink)
        {
            return GetListOfLinkWithIndex(linkedSource)
                .Where(linkWithIndex => linkWithIndex.Link != null)
                .Select(linkWithIndex => CreateLinkTargetValueWithIndex(linkWithIndex, getLinkTargetValueForLink))
                //stle: if has match worth it?
                //stle: do not overwrite for other include
                .Where(linkTargetValueWithIndex => !Equals(linkTargetValueWithIndex.TargetValue, default(TIChildLinkedSource)))
                .ToList();
        }

        private static LinkTargetValueWithIndex<TIChildLinkedSource> CreateLinkTargetValueWithIndex(LinkWithIndex<TLink> linkWithIndex, Func<LinkWithIndex<TLink>, TIChildLinkedSource> getLinkTargetValueForLink)
        {
            return new LinkTargetValueWithIndex<TIChildLinkedSource>(
                getLinkTargetValueForLink(linkWithIndex),
                linkWithIndex.Index
            );
        }









        private List<LinkWithIndex<TLink>> GetListOfLinkWithIndex(TLinkedSource linkedSource) 
        {
            var links = GetLinks(linkedSource);

            return links
                .Select((link, index) => new LinkWithIndex<TLink>(link, index))
                .ToList();
        }

        private void InitListOfReferencesIfNull(TLinkedSource linkedSource)
        {
            if (_getReferences(linkedSource) == null){
                var polymorphicListToBeBuilt = new TIChildLinkedSource[GetLinks(linkedSource).Count].ToList();
                _setReferences(linkedSource, polymorphicListToBeBuilt);
            }
        }

        private List<TLink> GetLinksForReferenceType(TLinkedSource linkedSource, Type referenceTypeToBeLoaded)
        {
            return GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLoaded)
                .Select(linkWithIndex => linkWithIndex.Link)
                .ToList();
        }


        private List<LinkWithIndex<TLink>> GetLinksWithIndexForReferenceType(TLinkedSource linkedSource,
            Type referenceTypeToBeLoaded)
        {
            var links = GetLinks(linkedSource);

            return links
                .Select((link, index) => new LinkWithIndex<TLink>(link, index))
                .Where(linkWithIndex => linkWithIndex.Link != null)
                //stle: assume that GetWithAddLookupIdInclude wont never return null
                .Where(linkWithIndex =>
                    _includeSet.GetIncludeWithAddLookupId(linkWithIndex.Link).ReferenceType == referenceTypeToBeLoaded
                )
                .ToList();
        }

        private List<TLink> GetLinks(TLinkedSource linkedSource)
        {
            var links = _getLinksFunc(linkedSource);

            if (links == null)
            {
                return new List<TLink>();
            }

            return links;
        }

        public List<Type> ChildLinkedSourceTypes { get; private set; }
    }
}