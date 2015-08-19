using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class DataContext {
        private readonly Dictionary<Type, object> _referenceDictionaryByReferenceType = new Dictionary<Type, object>();

        public object State {
            get
            {
                return _referenceDictionaryByReferenceType;
            }
        }

        public void Append<TReference, TId>(List<TReference> references, Func<TReference,TId> getReferenceIdFunc){
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

        private Dictionary<TId, TReference> GetReferenceDictionary<TReference, TId>() {
            var tReference = typeof (TReference);
            if (_referenceDictionaryByReferenceType.ContainsKey(tReference) == false) {
                throw new InvalidOperationException(
                    string.Format(
                        "The type {0} is not supported by this data context.",
                        tReference.Name)
                );
            }

            return (Dictionary<TId, TReference>)_referenceDictionaryByReferenceType[tReference];
        }


        public TReference GetOptionalReference<TReference, TId>(TId key) {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            if (referenceDictionnary.ContainsKey(key) == false) { return default(TReference); }

            return referenceDictionnary[key];
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