using System;
using System.Linq;

namespace HeterogeneousDataSources
{
    public class ReferenceToLoadTreeFactory
    {
        private readonly LoadLinkConfig _config;

        public ReferenceToLoadTreeFactory(LoadLinkConfig config)
        {
            _config = config;
        }

        public Tree<ReferenceToLoad> Create(Type rootLinkedSourceType) {
            return new Tree<ReferenceToLoad>(
                new ReferenceToLoad(
                    LinkedSourceConfigs.GetConfigFor(rootLinkedSourceType).LinkedSourceModelType, 
                    "root"
                ),
                _config.CreateReferenceTreeForEachLinkTarget(rootLinkedSourceType)
            );
        }
    }
}