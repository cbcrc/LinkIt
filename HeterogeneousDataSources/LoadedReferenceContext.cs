using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class LoadedReferenceContext {
        private readonly List<object> _linkedSourcesToBeBuilt = new List<object>();
        private readonly Dictionary<Type, object> _referenceDictionaryByReferenceType= new Dictionary<Type, object>();

        public object State {
            get
            {
                return _referenceDictionaryByReferenceType;
            }
        }

        public void AddReferences<TReference, TId>(List<TReference> references, Func<TReference,TId> getReferenceIdFunc){
            var tReference = typeof (TReference);
            if (_referenceDictionaryByReferenceType.ContainsKey(tReference)) {
                throw new InvalidOperationException(
                    string.Format(
                        "All references of the same type ({0}) must be loaded at the same time.",
                        tReference.Name)
                );
            }

            var referenceDictionnary = references.ToDictionary(
                keySelector: getReferenceIdFunc,
                elementSelector: reference => reference 
            );

            _referenceDictionaryByReferenceType[tReference] = referenceDictionnary;
        }

        public void AddLinkedSourceToBeBuilt(object linkedSource){
            //null linked source does not have to be built
            if (linkedSource == null) { return; }

            _linkedSourcesToBeBuilt.Add(linkedSource);
        }
        
        public List<object> LinkedSourcesToBeBuilt{
            get { return _linkedSourcesToBeBuilt.ToList(); }
        }

        private Dictionary<TId, TReference> GetReferenceDictionary<TReference, TId>() {
            var tReference = typeof (TReference);
            if (!_referenceDictionaryByReferenceType.ContainsKey(tReference)) {
                throw new InvalidOperationException(
                    string.Format(
                        "References of type {0} were not loaded.",
                        tReference.Name)
                );
            }

            return (Dictionary<TId, TReference>)_referenceDictionaryByReferenceType[tReference];
        }

        public List<TReference> GetOptionalReferences<TReference, TId>(List<TId> ids)
        {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            return ids
                .DistinctWithoutNull()
                .Where(referenceDictionnary.ContainsKey)
                .Select(id => referenceDictionnary[id])
                .ToList();
        }


        //public TData GetMandatoryReference<TData>(object key)
        //{
        //    var result = GetOptionalReference<TData>(key);
        //    //Ensure.Assumption(
        //    //    ReferenceEquals(result,default(TData)) == false, 
        //    //    "Mandatory reference cannot be resolved."
        //    //);
        //    return result;
        //}

        //private static KeyValuePair<object, TData> ToKeyValuePair<TData>(TData data, Func<TData, object> keyFunc) {
        //    return new KeyValuePair<object, TData>(
        //        keyFunc(data),
        //        data
        //    );
        //}
    }
}