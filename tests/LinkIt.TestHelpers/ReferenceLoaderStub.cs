// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkIt.PublicApi;
using LinkIt.ReadableExpressions.Extensions;

namespace LinkIt.TestHelpers
{
    public class ReferenceLoaderStub : IReferenceLoader
    {
        private readonly Dictionary<Type, IReferenceTypeConfig> _referenceTypeConfigByReferenceType;

        public readonly List<ILoadingContext> RecordedLookupIdContexts = new List<ILoadingContext>();

        public ReferenceLoaderStub(params IReferenceTypeConfig[] customReferenceTypeConfigs)
        {
            var config = customReferenceTypeConfigs
                .Concat(GetDefaultReferenceTypeConfigs())
                .ToList();

            _referenceTypeConfigByReferenceType = config.ToDictionary(
                referenceTypeConfig => referenceTypeConfig.ReferenceType,
                referenceTypeConfig => referenceTypeConfig
            );
        }

        public bool IsDisposed { get; private set; }

        public Task LoadReferencesAsync(ILoadingContext loadingContext)
        {
            RecordedLookupIdContexts.Add(loadingContext);

            foreach (var referenceType in loadingContext.ReferenceTypes)
            {
                LoadReference(referenceType, loadingContext);
            }

#if NETSTANDARD
            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        private static List<IReferenceTypeConfig> GetDefaultReferenceTypeConfigs()
        {
            return new List<IReferenceTypeConfig>
            {
                new ReferenceTypeConfig<Image, string>(
                    ids => new ImageRepository().GetByIds(ids),
                    reference => reference.Id
                ),
                new ReferenceTypeConfig<Person, string>(
                    ids => new PersonRepository().GetByIds(ids),
                    reference => reference.Id
                ),
                new ReferenceTypeConfig<Media, int>(
                    ids => new MediaRepository().GetByIds(ids),
                    reference => reference.Id
                )
            };
        }

        private void LoadReference(Type referenceType, ILoadingContext loadingContext)
        {
            if (!_referenceTypeConfigByReferenceType.ContainsKey(referenceType))
            {
                throw new NotSupportedException(
                   $"There is no loader for reference of type {referenceType.GetFriendlyName()}."
               );
            }

            var referenceTypeConfig = _referenceTypeConfigByReferenceType[referenceType];
            referenceTypeConfig.Load(loadingContext);
        }
    }
}
