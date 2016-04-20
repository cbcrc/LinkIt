Getting Started
---------------
To execute this example, see [LinkIt.Samples](LinkIt.Samples/GettingStarted.cs). 

### Linked Source
A linked source is a class that has a property called `Model`, and other properties called link targets. For example, let's say you have these two classes that you fetched separately by IDs.

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
Load link by ID
```csharp
var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ById(1);
```

Load link by IDs
```csharp
var actual = _loadLinkProtocol.LoadLink<MediaLinkedSource>().ByIds(
    new List<int>{1, 2, 3}
);
```

However, prior to being able to use `_loadLinkProtocol.LoadLink`, you need to define a load link protocol and a reference loader.

### LoadLinkProtocol
A load link protocol has to be defined in order to load and link a linked source. Here is the protocol configuration for `MediaLinkedSource`.
```csharp
public class MediaLinkedSourceConfig : ILoadLinkProtocolConfig {
    public void ConfigureLoadLinkProtocol(LoadLinkProtocolBuilder loadLinkProtocolBuilder) {
        loadLinkProtocolBuilder.For<MediaLinkedSource>()
            .LoadLinkReferenceById(
                linkedSource => linkedSource.Model.TagIds,
                linkedSource => linkedSource.Tags
            );
    }
}
```

However, we favor convention over configuration. 

By default, the load link protocol is defined by convention
- when a model property name with the suffix `Id` or `Ids` matches a link target name
- when a model property name is the same as a link target name

Since the model property `Model.TagIds` match the link target `Tags`, the `MediaLinkedSourceConfig` defined above is unnecessary and can be omitted.

When invoking `loadLinkProtocolBuilder.Build`, all the assemblies passed by argument will be scanned in order to apply the conventions and the configuration classes implementing `ILoadLinkProtocolConfig`. If a convention conflicts with a configuration explicitly defined in a `ILoadLinkProtocolConfig`, the configuration defined in a `ILoadLinkProtocolConfig` wins.

```csharp
var loadLinkProtocolBuilder = new LoadLinkProtocolBuilder();
_loadLinkProtocol = loadLinkProtocolBuilder.Build(
    ()=>new FakeReferenceLoader(),
    new[] { Assembly.GetExecutingAssembly() },
    LoadLinkExpressionConvention.Default
);
```
To create your own conventions, please see how the [existing conventions are built](LinkIt.Conventions/DefaultConventions). Then, simply pass your convention by argument when calling `loadLinkProtocolBuilder.Build`. 

### ReferenceLoader
LinkIt can load objects spread across many data sources (web services, SQL databases, NoSQL databases, in-memory caches, file systems, Git repositories, etc.) and link them together as long as those objects can be fetched by IDs. LinkIt is not responsible for defining how the objects are loaded, you must define this process for each possible reference type in a reference loader.

LinkIt will always provide all lookup IDs for a reference type in one batch; avoiding the [select N + 1 problem](http://stackoverflow.com/questions/97197/what-is-the-n1-selects-issue). 

For example, if we load link the `MediaLinkedSource` for IDs `"one"` and `"two"`, and the state of those media is  
```json
[
    {
        "id":"one",
        "title":"title-one",
        "tagIds":[1,2]
    },
    {
        "id":"two",
        "title":"title-two",
        "tagIds":[2,3]
    }
]
```

the lookup IDs for `Tag` would then be `1,2,3`. 

Note that lookup IDs are always provided without duplicates and without null IDs. Moreover, all the lookup IDs for the same reference type will always be loaded in one batch regardless of the complexity of the linked sources.

Here is a reference loader that loads fake data for our example. 
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
        //In case you need to dispose database connections or other resources.
 
        //Will always be invoked as soon as the load phase is completed or
        //if an exception is thrown
    }
}
```

### Read more
- [Why Should I Use LinkIt?](why-without-how.md)
- [Slightly More Complex Example](slightly-more-complex-example.md)
- [Known Limitations](known-limitations.md)
- [License](LICENSE.txt)
