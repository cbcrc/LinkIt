using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources.Tests.Shared {
    public class LoadLinkProtocolFactory<TReference, TId> where TReference:class
    {
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;
        private readonly Func<TReference, TId> _getReferenceIdFunc;
        private readonly IReferenceTypeConfig[] _customReferenceTypeConfigs;

        public LoadLinkProtocolFactory(List<ILoadLinkExpression> loadLinkExpressions, Func<TReference, TId> getReferenceIdFunc, params IReferenceTypeConfig[] customReferenceTypeConfigs)
        {
            _loadLinkExpressions = loadLinkExpressions;
            _getReferenceIdFunc = getReferenceIdFunc;
            _customReferenceTypeConfigs = customReferenceTypeConfigs;
        }

        public LoadLinkProtocol Create(TReference fixedValue)
        {
            var customConfig = GetCustomConfig(fixedValue, _getReferenceIdFunc);

            var referenceLoader = new FakeReferenceLoader(customConfig);

            return new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(
                    _loadLinkExpressions,


                    new List<List<Type>>{
                        new List<Type>{typeof(WithPolymorphicReference)},
                        new List<Type>{typeof(Person), typeof(Image)},
                    }

                )
            );
        }

        public LoadLinkProtocol Create() {
            var referenceLoader = new FakeReferenceLoader(_customReferenceTypeConfigs);

            return new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(
                    _loadLinkExpressions
                )
            );
        }

        private IReferenceTypeConfig[] GetCustomConfig(TReference fixedValue, Func<TReference, TId> getReferenceIdFunc)
        {
            var fixedReferenceTypeConfig = new ReferenceTypeConfig<TReference, TId>(
                ids => ids
                    .Select(id => fixedValue)
                    .Where(id => id!=null)
                    .ToList(),
                getReferenceIdFunc
            );

            var overriddenReferenceTypeConfig = _customReferenceTypeConfigs
                .Where(customReferenceTypeConfig =>
                    !customReferenceTypeConfig.ReferenceType.Equals(fixedReferenceTypeConfig.ReferenceType)
                )
                .ToList();

            overriddenReferenceTypeConfig.Add(fixedReferenceTypeConfig);

            return overriddenReferenceTypeConfig.ToArray();
        }
    }
}
