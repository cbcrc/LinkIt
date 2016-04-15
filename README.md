LinkIt
===============
LinkIt is an object-oriented data integration library that make it easy to load different kind of objects and link them together. 

LinkIt is not an object-relational mapping framework. It can be used for orchestrating the loading of objects and for linking the loaded objects togheter, not for defining how the objects are loaded. LinkIt is intended to be used in a variety of context such as data APIs, ETL processes, CQRS event handlers, web crawlers, etc.

### Features
- Minimize coding effort by leveraging reuse and composition
- Data source independant
- Avoid the Select N + 1 problem
- Built-in support for complex types
- Support polymorphism out of the box
- Favor convention over configuration
- Perform complex projections easily with [LinkItExtensionsForAutoMapper](https://github.com/cbcrc/LinkIt.AutoMapperExtensions)

Getting Started
---------------
To execute this example, see [LinkIt.Samples](LinkIt.Samples/GettingStarted.cs). 

### Linked Source
A linked source is a class that has a property called `Model`, and other properties called link targets. For example, let's say you have theses two classes that you fetched separately by ids.

```csharp
public class Media {
    public int Id { get; set; }
    public string Title { get; set; }
    public List<int> TagIds { get; set; } //Tag references
}

public class Tag {
    public int Id { get; set; }
    public string Name { get; set; }
}
```

The linked source representing a media with its tags would be a class like this.
```csharp
public class MediaLinkedSource : ILinkedSource<Media> {
    public Media Model { get; set; }
    public List<Tag> Tags { get; set; }
}
```

### Usage
Load link by id
```csharp
var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ById(1);
```

Load link by ids
```csharp
var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ByIds(
    new List<int>{1, 2, 3}
);
```

However, prior to be able to use `_loadLinkProtocol.LoadLink` you need to define a load link protocol and a reference loader.

### LoadLinkProtocol
A load link protocol has to be defined in order to load and link a linked source. Here is the protocol configuration for `MediaLinkedSource`.
```csharp
var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
loadLinkProtocolBuilder.For<MediaLinkedSource>()
    .LoadLinkReferenceById(
        linkedSource => linkedSource.Model.TagIds,
        linkedSource => linkedSource.Tags
    );
```

However, we favor convention over configuration. The same protocol could be defined as follow.
```csharp
var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
loadLinkProtocolBuilder.ApplyConventions(
    new[]{Assembly.GetExecutingAssembly()},
    loadLinkProtocolBuilder.DefaultConventions()
);
```

By default, the load link protocol is defined by convention
- when a model property name with the suffix `Id` or `Ids` matches a link target name
- when a model property name is the same as a link target name

The load link protocol for a link target cannot be defined more than once. If more than one convention apply or if a convention conflict with a protocol configuration the last one executed wins.

To create your own conventions, please see how the [existing conventions are built](LinkIt.Conventions/DefaultConventions). Then, simply pass your convention by argument when calling `ApplyConventions`. 

### ReferenceLoader
LinkIt can load object spread across many data source (web services, Sql databases, NoSql databases, in memory cache, file system, Git repository, etc.) and link them together as long as thoses objects can be fetched by ids. In order to define how objects are loaded, you have to create a reference loader. 

LinkIt will always provide all lookup ids for a specific type in one batch; avoiding the Select N+1 problem. For example, if we load link `MediaLinkedSource` by ids (`"one", "two", "three"`), the lookup ids for `Tag` would be the id of the tags related to those three `Media` without duplicates and without null ids. Moreover, those ids will be passed to the `ReferenceLoader` in the same `LookupIdContext`.

Here is a reference loader that load fake data for our example. 
```csharp
public class FakeReferenceLoader:IReferenceLoader
{
    public void LoadReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext){
        foreach (var referenceType in lookupIdContext.GetReferenceTypes()){
            LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
        }
    }

    private void LoadReference(Type referenceType, ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext)
    {
        if (referenceType == typeof(Media)) {
            LoadMedia(lookupIdContext, loadedReferenceContext);
        }
        if (referenceType == typeof(Tag)){
            LoadTags(lookupIdContext, loadedReferenceContext);
        }
    }

    private void LoadMedia(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext) {
        var lookupIds = lookupIdContext.GetReferenceIds<Media, int>();
        var references = lookupIds
            .Select(id =>
                new Media{
                    Id = id,
                    Title = "title-" + id,
                    TagIds = new List<int> { 
                        1000+id,
                        1001+id
                    }
                }
            )
            .ToList();

        loadedReferenceContext.AddReferences(
            references,
            reference => reference.Id
        );
    }

    private void LoadTags(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext){
        var lookupIds = lookupIdContext.GetReferenceIds<Tag, int>();
        var references = lookupIds
            .Select(id=>
                new Tag {
                    Id = id, 
                    Name = id+"-name"
                }
            )
            .ToList();
        
        loadedReferenceContext.AddReferences(
            references,
            reference => reference.Id
        );
    }

    public void Dispose(){
        //In case you need to dispose database connections or other ressources.
 
        //Will always be invoked as soon as the load phase is completed or
        //if an exception is thrown
    }
}
```

Then the load link protocol can be built link this: 
```csharp
_loadLinkProtocol = loadLinkProtocolBuilder.Build(()=>new FakeReferenceLoader());
```

### Read more
- [Slightly more complex example](slightly-more-complex-example.md)
- [Known limitations](known-limitations.md)
