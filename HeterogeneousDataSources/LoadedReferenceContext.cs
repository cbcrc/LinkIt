using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class LoadedReferenceContext {
        private readonly List<object> _linkedSourcesToBeBuilt = new List<object>();
        private readonly List<object> _linkedSourcesWhereSubLinkedSourceAreLinked = new List<object>();
        private readonly Dictionary<Type, object> _referenceDictionaryByReferenceType= new Dictionary<Type, object>();
        private List<Action> OnLinkCompletedActions = new List<Action>();

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

        //stle: temporary work around for image, fix the root cause by solving the query vs id problem
        public void AddReference<TReference, TId>(TReference reference, TId id) {
            var tReference = typeof(TReference);
            if (!_referenceDictionaryByReferenceType.ContainsKey(tReference)) {
                _referenceDictionaryByReferenceType.Add(tReference, new Dictionary<TId, TReference>());
            }

            var referenceDictionnary = (Dictionary<TId, TReference>)
                _referenceDictionaryByReferenceType[tReference];

            referenceDictionnary.Add(id,reference);
        }

        public void AddLinkedSourceToBeBuilt<TLinkedSource>(TLinkedSource linkedSource)
        {
            if (linkedSource == null){
                throw new InvalidOperationException("Cannot add null linked source to be built.");
            }

            _linkedSourcesToBeBuilt.Add(linkedSource);
        }

        public List<object> LinkedSourcesToBeBuilt{
            get { return _linkedSourcesToBeBuilt.ToList(); }
        }

        public void AddLinkedSourceWhereSubLinkedSourceAreLinked(object linkedSource){
            //assume linkedSource cannot be null
            _linkedSourcesWhereSubLinkedSourceAreLinked.Add(linkedSource);
        }

        public List<object> GetLinkedSourceWithSubLinkedSourceToLink()
        {
            return _linkedSourcesToBeBuilt
                .Except(_linkedSourcesWhereSubLinkedSourceAreLinked)
                .ToList();
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

        public TReference GetOptionalReference<TReference, TId>(TId lookupId) {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();

            //stle: do we want to filter out null? 
                //I'm not sure, maybe it's not the linker responsability
                //Maybe we do not have to clear the list of null
                //Think about the impact on transform, how can we filter out easily?
            //stle: do wa want to allow for struct as TReference? 
                //Does not make sense to me, but why not, especially if null are not filtered out anymore      
            if (!referenceDictionnary.ContainsKey(lookupId)) { return default(TReference); }

            return referenceDictionnary[lookupId];
        }


        //stle: obsolete
        public List<TReference> GetOptionalReferences<TReference, TId>(List<TId> ids)
        {
            var referenceDictionnary = GetReferenceDictionary<TReference, TId>();
            return ids
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

        
        //stle: use event instead
        public void OnLinkCompleted(Action action){
            OnLinkCompletedActions.Add(action);
        }

        public void LinkCompleted(){
            //These actions has to be done after each polymorphic list 
            //are linked because polymorphic list are built in place.
            //Thus, until link is completed it is not possible to determine
            //if a null value in a list is a not resolved yet or if it will never be resolved.

            foreach (var action in OnLinkCompletedActions){
                action();
            }
        }
    }
}