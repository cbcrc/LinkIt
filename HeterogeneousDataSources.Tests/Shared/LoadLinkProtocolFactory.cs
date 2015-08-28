using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions;

namespace HeterogeneousDataSources.Tests.Shared {
    public class LoadLinkProtocolFactory<TReference, TId> where TReference:class
    {
        private readonly List<ILoadLinkExpression> _loadLinkExpressions;
        private readonly Func<TReference, TId> _getReferenceIdFunc;
        private readonly List<Type>[] _fakeReferenceTypeForLoadingLevel;
        private readonly IReferenceTypeConfig[] _customReferenceTypeConfigs;

        public LoadLinkProtocolFactory(List<ILoadLinkExpression> loadLinkExpressions, Func<TReference, TId> getReferenceIdFunc, List<Type>[] fakeReferenceTypeForLoadingLevel, params IReferenceTypeConfig[] customReferenceTypeConfigs)
        {
            _loadLinkExpressions = loadLinkExpressions;
            _getReferenceIdFunc = getReferenceIdFunc;
            _fakeReferenceTypeForLoadingLevel = fakeReferenceTypeForLoadingLevel;
            _customReferenceTypeConfigs = customReferenceTypeConfigs;
        }

        public LoadLinkProtocol Create(TReference fixedValue=null)
        {
            var customConfig = GetCustomConfig(fixedValue, _getReferenceIdFunc);

            var referenceLoader = new FakeReferenceLoader(customConfig);

            return new LoadLinkProtocol(
                referenceLoader,
                new LoadLinkConfig(
                    _loadLinkExpressions,
                    _fakeReferenceTypeForLoadingLevel
                )
            );
        }

        private IReferenceTypeConfig[] GetCustomConfig(TReference fixedValue, Func<TReference, TId> getReferenceIdFunc)
        {
            if (fixedValue == null) { return _customReferenceTypeConfigs; }

            var fixedReferenceTypeConfig = new ReferenceTypeConfig<TReference, TId>(
                ids => ids.Select(id => fixedValue).ToList(),
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
