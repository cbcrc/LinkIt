using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests {
    public class FakeReferenceLoader:IReferenceLoader
    {
        private readonly Dictionary<Type, Action<LookupIdContext, LoadedReferenceContext>> _loadReferencesActions;

        public FakeReferenceLoader()
        {
            _loadReferencesActions = new Dictionary<Type, Action<LookupIdContext, LoadedReferenceContext>>{
                {typeof(Image), LoadImageReferences },
                {typeof(Person), LoadPersonReferences }
            };
        }

        public void LoadReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            foreach (var referenceType in lookupIdContext.GetReferenceTypes())
            {
                LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
            }
        }

        private void LoadReference(Type referenceType, LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            if (!_loadReferencesActions.ContainsKey(referenceType)){
                throw new NotImplementedException(
                    string.Format("There is no loader for reference of type {0}.", referenceType.Name)
                );
            }
            var loadReferenceAction = _loadReferencesActions[referenceType];
            loadReferenceAction(lookupIdContext, loadedReferenceContext);
        }

        private void LoadImageReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            var ids = lookupIdContext.GetReferenceIds<Image, string>();
            var references = new ImageRepository().GetByIds(ids);
            loadedReferenceContext.AddReferences(references, reference=>reference.Id);
        }

        private void LoadPersonReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
        {
            var ids = lookupIdContext.GetReferenceIds<Person, int>();
            var references = new PersonRepository().GetByIds(ids);
            loadedReferenceContext.AddReferences(references, reference => reference.Id);
        }

    }
}
