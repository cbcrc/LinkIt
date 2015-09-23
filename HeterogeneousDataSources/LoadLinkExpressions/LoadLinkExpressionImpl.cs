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
            //stle: dry
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);

            LinkSubLinkedSources((TLinkedSource) linkedSource, loadedReferenceContext);
        }

        public void LinkSubLinkedSources(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            var tempIgnoreMixedMode = false;
            var subLinkedSources = new List<TIChildLinkedSource>();

            var links = GetLinks(linkedSource);

            //stle: this is not a link and not a reference but a sub linked source model
            //      find a good abstraction for the concept
            foreach (var link in links)
            {
                if (link == null)
                {
                    subLinkedSources.Add(default(TIChildLinkedSource));
                }
                else
                {
                    var include = _includeSet.GetIncludeWithCreateSubLinkedSource(link);

                    //stle: will not work if sub linked source is mixed with other expressions 
                    //This case is not supported for now
                    if (include == null)
                    {
                        tempIgnoreMixedMode = true;
                    }
                    else
                    {
                        var subLinkedSource = include.CreateSubLinkedSource(link, loadedReferenceContext);
                        //stle: use linq
                        subLinkedSources.Add(subLinkedSource);
                    }
                }
            }

            if (!tempIgnoreMixedMode)
            {
                _setReferences(linkedSource, subLinkedSources);
            }
        }

        public void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            //stle: dry
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            LinkReference((TLinkedSource) linkedSource, loadedReferenceContext);
        }

        //stle: dry
        public void LinkReference(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext)
        {
            var tempIgnoreMixedMode = false;
            var references = new List<TIChildLinkedSource>();

            var links = GetLinks(linkedSource).ToList();

            //stle: this is not a link and not a reference but a sub linked source model
            //      find a good abstraction for the concept
            foreach (var link in links)
            {
                if (link == null){
                    references.Add(default(TIChildLinkedSource));
                }
                else
                {

                    var include = _includeSet.GetIncludeWithGetReference(link);

                    //stle: will not work if sub linked source is mixed with other expressions 
                    //This case is not supported for now
                    if (include == null)
                    {
                        tempIgnoreMixedMode = true;
                    }
                    else
                    {
                        var reference = include.GetReference(link, loadedReferenceContext);
                        //stle: use linq
                        references.Add(reference);
                    }
                }
            }

            if (!tempIgnoreMixedMode)
            {
                _setReferences(
                    linkedSource,
                    references
                        .ToList()
                    );
            }
        }

        public void LinkNestedLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            //stle: dry
            LoadLinkExpressionUtil.EnsureLinkedSourceIsOfTLinkedSource<TLinkedSource>(linkedSource);
            LinkNestedLinkedSource((TLinkedSource) linkedSource, loadedReferenceContext, referenceTypeToBeLinked);
        }



        private void LinkNestedLinkedSource(TLinkedSource linkedSource, LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            //stle: dry with link count and null
            if (GetLinks(linkedSource).Count == 0)
            {
                LinkNestedLinkedSourceListWithZeroReference(linkedSource);
            }
            if (GetLinks(linkedSource).Count == 1)
            {
                LinkNestedLinkedSourceListWithOneReference(linkedSource, loadedReferenceContext, referenceTypeToBeLinked);
            }
            else
            {
                LinkNestedLinkedSourceListWithManyReferences(linkedSource, loadedReferenceContext,
                    referenceTypeToBeLinked);
            }
        }

        private void LinkNestedLinkedSourceListWithZeroReference(TLinkedSource linkedSource)
        {
            _setReferences(linkedSource, new List<TIChildLinkedSource>());
        }

        private void LinkNestedLinkedSourceListWithOneReference(TLinkedSource linkedSource,
            LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked)
        {
            var linksToEntityOfReferenceType = GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLinked);

            //If not of the reference type to be linked
            if (!linksToEntityOfReferenceType.Any())
            {
                return;
            }

            var link = linksToEntityOfReferenceType.Single().Link;

            var include = _includeSet.GetIncludeWithCreateNestedLinkedSource(link);

            //stle: temp to remove expression type
            if (include == null)
            {
                return;
            }

            var childLinkedSource = include
                .CreateNestedLinkedSource(link, loadedReferenceContext, linkedSource, 0);

            _setReferences(linkedSource, new List<TIChildLinkedSource> {childLinkedSource});
        }

        private void LinkNestedLinkedSourceListWithManyReferences(TLinkedSource linkedSource,
            LoadedReferenceContext loadedReferenceContext,
            Type referenceTypeToBeLinked)
        {
            InitListOfReferencesIfNull(linkedSource, loadedReferenceContext);

            var linksToEntityOfReferenceType = GetLinksWithIndexForReferenceType(linkedSource, referenceTypeToBeLinked);

            foreach (var linkToEntityOfReferenceType in linksToEntityOfReferenceType)
            {
                var include = _includeSet.GetIncludeWithCreateNestedLinkedSource(linkToEntityOfReferenceType.Link);

                //stle: temp to remove expression type
                if (include != null)
                {
                    var childLinkedSource = include.CreateNestedLinkedSource(
                        linkToEntityOfReferenceType.Link,
                        loadedReferenceContext,
                        linkedSource,
                        linkToEntityOfReferenceType.Index
                        );

                    _getReferences(linkedSource)[linkToEntityOfReferenceType.Index] = childLinkedSource;
                }
            }
        }

        private void InitListOfReferencesIfNull(TLinkedSource linkedSource,
            LoadedReferenceContext loadedReferenceContext)
        {
            if (_getReferences(linkedSource) == null){
                var polymorphicListToBeBuilt = new TIChildLinkedSource[GetLinkCount(linkedSource)].ToList();
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

        private int GetLinkCount(TLinkedSource linkedSource)
        {
            return GetLinks(linkedSource).Count;
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