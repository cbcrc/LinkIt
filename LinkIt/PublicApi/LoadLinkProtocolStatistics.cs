using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.PublicApi {
    public class LoadLinkProtocolStatistics
    {
        private readonly Dictionary<Type, List<List<Type>>> _loadingLevelsByRootLinkedSourceType;

        public LoadLinkProtocolStatistics(Dictionary<Type, List<List<Type>>> loadingLevelsByRootLinkedSourceType)
        {
            _loadingLevelsByRootLinkedSourceType = loadingLevelsByRootLinkedSourceType;
        }

        public int NumberOfLinkedSources => _loadingLevelsByRootLinkedSourceType.Count;

        public int MaxLoadingLevelDepth => LoadingLevelDepthForEachLinkedSource.First().Value;

        public int MaxNumberOfReferenceTypeInOneLinkedSource => NumberOfReferenceTypeForEachLinkedSource.First().Value;

        public List<KeyValuePair<Type, int>> LoadingLevelDepthForEachLinkedSource{
            get
            {
                return _loadingLevelsByRootLinkedSourceType
                  .Select(item =>
                      new KeyValuePair<Type, int>(
                          item.Key,
                          item.Value.Count
                      )
                  )
                  .OrderByDescending(item=>item.Value)
                  .ThenBy(item=>item.Key.FullName)
                  .ToList();
            }
        }

        public List<KeyValuePair<Type, int>> NumberOfReferenceTypeForEachLinkedSource
        {
            get
            {
                return _loadingLevelsByRootLinkedSourceType
                  .Select(item =>
                      new KeyValuePair<Type, int>(
                          item.Key,
                          item.Value
                              .SelectMany(referencesForOneLoadingLevel=> referencesForOneLoadingLevel)
                              .Count()
                      )
                  )
                  .OrderByDescending(item => item.Value)
                  .ThenBy(item => item.Key.FullName)
                  .ToList();
            }
        }

        public List<KeyValuePair<Type, List<List<Type>>>> LoadingLevelsForEachLinkedSource
        {
            get
            {
                return _loadingLevelsByRootLinkedSourceType
                    //Ensure statistics cannot have side effect on load link protocol
                    .Select(CloneLoadingLevels)
                    .OrderBy(item => item.Key.FullName)
                    .ToList();
            }
        }

        private static KeyValuePair<Type, List<List<Type>>> CloneLoadingLevels(KeyValuePair<Type, List<List<Type>>> item)
        {
            return new KeyValuePair<Type, List<List<Type>>>(
                item.Key,
                item.Value
                    .Select(CloneLoadingLevel)
                    .ToList()
            );
        }

        private static List<Type> CloneLoadingLevel(List<Type> referenceTypes)
        {
            return referenceTypes.ToList();
        }
    }
}
