using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class DataContext {
        private readonly Dictionary<Type,Dictionary<object,object>> _referenceDictionaryByReferenceType = new Dictionary<Type,Dictionary<object,object>>();

        public object State {
            get
            {
                return _referenceDictionaryByReferenceType;
            }
        }

        public void Append(List<object> references, ILoadLinkExpression loadLinkExpression) {
            if (_referenceDictionaryByReferenceType.ContainsKey(loadLinkExpression.ReferenceType)) {
                throw new InvalidOperationException(
                    string.Format(
                        "All references of the same type ({0}) must be loaded at the same time.",
                        loadLinkExpression.ReferenceType.Name)
                );
            }

            var referenceDictionnary = references.ToDictionary(
                keySelector: loadLinkExpression.GetReferenceId,
                elementSelector: reference => reference 
            );

            _referenceDictionaryByReferenceType[loadLinkExpression.ReferenceType] = referenceDictionnary;
        }

        private Dictionary<object, object> GetReferenceDictionary(Type referenceType) {
            if (_referenceDictionaryByReferenceType.ContainsKey(referenceType) == false) {
                _referenceDictionaryByReferenceType[referenceType] = new Dictionary<object, object>();
            }

            return _referenceDictionaryByReferenceType[referenceType];
        }


        public TReference GetOptionalReference<TReference>(object key) {
            var referenceDictionnary = GetReferenceDictionary(typeof(TReference));
            if (referenceDictionnary.ContainsKey(key) == false) { return default(TReference); }

            return (TReference)referenceDictionnary[key];
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