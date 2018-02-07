# Change Log
 
<a name="2.0.0"></a>
## [2.0.0](https://github.com/cbcrc/LinkIt/compare/v1.1.0...v2.0.0) (2018-02-??)
 
### Breaking changes
 
* `ILoadLinker` methods are now async ([#3](https://github.com/cbcrc/LinkIt/issues/3)) ([d97d67f](https://github.com/cbcrc/LinkIt/commit/d97d67fefc3c8b5434863baddb19c758b3af830e))
* The signature of `IReferenceLoader.LoadReferencesAsync` has changed.
  * `ILookupIdsContext` has been renamed `ILoadingContext`. 
    It is now the only parameter for `LoadReferencesAsync` and is used for both
    getting the IDs to lookup and for adding loading references.
* Reference types that are part of a cycle are loaded in multiple batches.
 
 
### Features

* Dependency cycles are allowed, except between linked sources of the same type. 
  In other words, there can be a cycle that includes linked sources with the same model type,
  but cycles with the exact same linked source type is not permitted.