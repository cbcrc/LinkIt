LinkIt

LinkIt is an object-oriented data integration library that make it easy to load different kind of objects and link them together. LinkIt is intended to be used in a variety of context such as data APIs, ETL processes, CQRS event handlers, CQRS command handlers, web crawlers, etc.

Features
- built-in support for complext type
- built-in support for polymorphic links
- linked source can be composed and reuse easily
- favor convention over configuration
- data source independant
- generate DTO from linked source easily with LinkItExtensionsForAutoMapper

Getting Started
---------------
All the examples below are executable via LinkIt.Samples project. 

###Linked Source
A *Linked Source* is a class that has a property called `Model`, and other properties called *Link targets*. For example, let's say you have theses two classes that you fetched separately by id:

```csharp
public class Media
{
    public int Id { get; set; }
    public string Title { get; set; }
    public IEnumerable<int> TagIds { get; set; }
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

The *Linked Source* representing a media with its tags would be a class like this:
```csharp
public class MediaLinkedSource
{
    public Media Model { get; set; }
    public List<Tag> Tags { get; set; }
}
```

###LoadLinkProtocol
A *LoadLinkProtocol* has to be defined in order to load and link a linked source. Here is the protocol configuration for MediaLinkedSource.
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

By default, the protocol is defined by convention
- when a model property name with the suffix Id or Ids matches a link target name
- when a model property name is the same as a link target name

The protocol for a *LinkTarget* cannot be defined more than once. If more than one convention apply or if a convention conflict with a protocol configuration the last one executed wins.

To create your own conventions, you can see how the existing conventions are built here. Then, simply pass your convention as an argument when calling ApplyConventions. 


#ReferenceLoader
LinkIt can load object spread across many data source (web services, Sql databases, NoSql databases, in memory cache, file system, Git repository, etc.) and link them together as long as thoses objects can be fetched by id. 

In order to define how objects are loaded, you have to create a *ReferenceLoader*. LinkIt will always provide all lookup ids for a specific type in one batch; avoiding the Select N+1 problem. Moreover, the batch will never contains duplicates or null ids.

For our example, here is a ReferenceLoader that load fake data. 
```csharp
public class FakeReferenceLoader:IReferenceLoader
{
    public void LoadReferences(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext){
        foreach (var referenceType in lookupIdContext.GetReferenceTypes()){
            LoadReference(referenceType, lookupIdContext, loadedReferenceContext);
        }
    }

    private void LoadReference(Type referenceType, LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext)
    {
        if (referenceType == typeof(Tag)){
            LoadTags(lookupIdContext, loadedReferenceContext);
        }
        if (referenceType == typeof(Media)) {
            LoadMedia(lookupIdContext, loadedReferenceContext); 
        }
    }

    private void LoadMedia(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext) {
        var lookupIds = lookupIdContext.GetReferenceIds<Media, string>();
        var references = lookupIds
            .Select(id =>
                new Media{
                    Id = id,
                    Title = "title-" + id,
                    TagIds = new List<string> { 
                        string.Format("tag-{0}-a", id),
                        string.Format("tag-{0}-b", id)
                    }
                }
            )
            .ToList();

        loadedReferenceContext.AddReferences(
            references,
            reference => reference.Id
        );
    }

    private void LoadTags(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext){
        var lookupIds = lookupIdContext.GetReferenceIds<Tag, string>();
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
        //in case you need to dispose database connections or other ressources.
    }
}
```

#Ready to use
We are ready to load link some media linked sources.

Load link by id
```csharp
var actual = loadLinkProtocol.LoadLink<MediaLinkedSource>().ById("one");
```

Load link by ids
```csharp
var actual = loadLinkProtocol.LoadLink<MediaLinkedSource>().ByIds("one", "two", "three");
```

Load link query result
```csharp
var result = GetMediaByKeyword("fish");
var actual = loadLinkProtocol.LoadLink<MediaLinkedSource>().FromModels(result);
```

Slightly more complex example
---------------
Let's say you have a blog post class that reference media, images and tags by ids.
```csharp
public class BlogPost {
    public int Id { get; set; }
    public string Title { get; set; }
    //stle: set this as a IEnumerable
    public List<string> TagIds { get; set; }
    public Author Author { get; set; }
    public MultimediaContentReference MultimediaContentRef { get; set; }
}

public class Author {
    public string Name { get; set; }
    public string Email { get; set; }
    public string ImageId { get; set; }
}

public class MultimediaContentReference {
    public string Type { get; set; }
    public string Id { get; set; }
}

public class Image {
    public string Id { get; set; }
    public string Alt { get; set; }
    public string Url { get; set; }
}
```

The *Linked Source* representing a blog post with its media, images and tags would be defined like this:
```csharp
public class BlogPostLinkedSource : ILinkedSource<BlogPost> {
    public BlogPost Model { get; set; }
    public List<Tag> Tags { get; set; }
    public AuthorLinkedSource Author { get; set; }
    public object MultimediaContent { get; set; }
}

public class AuthorLinkedSource : ILinkedSource<Author> {
    public Author Model { get; set; }
    public Image Image { get; set; }
}
```
Most of the load link protocol can be defined using the default conventions. 
- BlogPostLinkedSource/Tags will match BlogPost/TagIds
- BlogPostLinkedSource/Autor will match BlogPost/Author
- AuthorLinkedSource/Image will match Author/ImageId

The BlogPost object own a nested object of type Author. In order to load link the author with his/her image we need to add the *LinkTarget* BlogPostLinkedSource/Autor which is a nested linked source of type AuthorLinkedSource.

In order to load link the *LinkTarget* BlogPostLinkedSource/MultimediaContent, we need to configure the load link protocol since none of the default conventions applies. Moreover, it is a special configuration since it involved some polymorphism.
```csharp
loadLinkProtocolBuilder.For<BlogPostLinkedSource>()
    .PolymorphicLoadLink(
        linkedSource => linkedSource.Model.MultimediaContentRef,
        linkedSource => linkedSource.MultimediaContent,
        link => link.Type,
        includes => includes
            .Include<Image>().AsReferenceById(
                "image",
                link => link.Id
            )
            .Include<MediaLinkedSource>().AsNestedLinkedSourceById(
                "media",
                link => link.Id
            )
    );
```

If MultimediaContentRef.Type is "image" an image will be loaded and linked; however, if MultimediaContentRef.Type is "media" a media with its tags will be loaded and linked. Here we are reusing the MediaLinkedSource defined in the Getting Started example.


Then you can load blog post linked sources like this
```csharp
var actual = loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ById("LinkIt Rocks!");
```

Known limitations of the current implementation
- does not support recursive loading
- does not support polymorphism at root level
- queries are only supported at root level, all objects has to be fetched by id
- the current protocol does not allow to perform database-side join to resolve the models of nested linked source. This performance optimization could be provided by developping database specific LinkIt extensions but it is out of the scope as of now. For exemple, we could leverage but not over use the database-side join of Mongo DB 3.2 (https://www.mongodb.com/blog/post/joins-and-other-aggregation-enhancements-coming-in-mongodb-3-2-part-1-of-3-introduction).