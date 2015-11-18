﻿using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.PublicApi;

namespace LinkIt.Tests.Shared {
    
    public class ReferenceLoaderStub:IReferenceLoader
    {
        private readonly Dictionary<Type, IReferenceTypeConfig> _referenceTypeConfigByReferenceType;
        private bool _isConnectionOpen = false;

        //stle: Config must be inside fake reference loader in order to access connection?
        private List<IReferenceTypeConfig> GetDefaultReferenceTypeConfigs()
        {
            return new List<IReferenceTypeConfig>{
                new ReferenceTypeConfig<Image, string>(
                    ids => new ImageRepository(_isConnectionOpen).GetByIds(ids),
                    reference => reference.Id,
                    "ouglo"
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

        public readonly List<ILookupIdContext> RecordedLookupIdContexts = new List<ILookupIdContext>();

        public void LoadReferences(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            RecordedLookupIdContexts.Add(lookupIdContext);
            OpenConnectionIfRequired(lookupIdContext);

            foreach (var referenceType in lookupIdContext.GetReferenceTypes())
            {
                LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
            }
        }

        private void LoadReference(Type referenceType, ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
        {
            if (!_referenceTypeConfigByReferenceType.ContainsKey(referenceType)){
                throw new NotImplementedException(
                    string.Format("There is no loader for reference of type {0}.", referenceType.Name)
                );
            }
            var referenceTypeConfig = _referenceTypeConfigByReferenceType[referenceType];
            referenceTypeConfig.Load(lookupIdContext, loadedReferenceContext);
        }

        private void OpenConnectionIfRequired(ILookupIdContext lookupIdContext) {
            if (GetReferenceTypeThatRequiresOugloConnection()
                .Any(requiresOuglo =>
                    lookupIdContext.GetReferenceTypes().Contains(requiresOuglo)
                )) {
                _isConnectionOpen = true;
            }
        }

        private List<Type> GetReferenceTypeThatRequiresOugloConnection() {
            return _referenceTypeConfigByReferenceType.Values
                .Where(referenceTypeConfig => referenceTypeConfig.RequiredConnection == "ouglo")
                .Select(referenceTypeConfig => referenceTypeConfig.ReferenceType)
                .ToList();
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            _isConnectionOpen = false;
            IsDisposed = true;
        }
    }
}