# Change Log
 
<a name="2.0.0"></a>
## [2.0.0](https://github.com/cbcrc/LinkIt/compare/v1.1.0...v2.0.0) (2018-02-??)
 
### Breaking changes
 
* `ILoadLinker` methods are now async ([#3](https://github.com/cbcrc/LinkIt/issues/3)) ([d97d67f](https://github.com/cbcrc/LinkIt/commit/d97d67fefc3c8b5434863baddb19c758b3af830e))
* The signature of `IReferenceLoader.LoadReferencesAsync` has changed.
  * `ILookupIdsContext` has been replaced by `ILoadingContext`. 
    It is now the only parameter for `LoadReferencesAsync` and is used for both
    getting the IDs to lookup and for adding loading references.
* Reference types that are part of a cycle are loaded in multiple batches.
* The overload of `LoadLinkProtocolForLinkedSourceBuilder.LoadLinkReferenceById` for lists has been renamed `LoadLinkReferencesByIds`. 
* The overload of `LoadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSourceById` for lists has been renamed `LoadLinkNestedLinkedSourcesByIds`. 
* The overload of `LoadLinkProtocolForLinkedSourceBuilder.LoadLinkNestedLinkedSourceFromModel` for lists has been renamed `LoadLinkNestedLinkedSourcesFromModels`. 
* `LoadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLink` has been renamed `LoadLinkPolymorphic`. 
* `LoadLinkProtocolForLinkedSourceBuilder.PolymorphicLoadLinkForList` has been renamed `LoadLinkPolymorphicList`. 
 
 
### Features

* Dependency cycles are allowed, except between linked sources of the same type. 
  In other words, there can be a cycle that includes linked sources with the same model type,
  but cycles with the exact same linked source type is not permitted.
* Reference types can be loaded directly using `ILoadLinkProtocol.Load<TModel>().ByIdAsync({id})` or `ILoadLinkProtocol.Load<TModel>().ByIdsAsync({ids})`.


### Migration from 1.x to 2.0
Included in the NuGet is a migration helper. It includes the removed classes and methods (as extension methods) with 
the `ObsoleteAttribute` to explain with what to replace the deprecated class or method.
