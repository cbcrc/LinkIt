LinkIt
===============
LinkIt is an object-oriented data integration library that make it easy to load different kind of objects and link them together. 

LinkIt is not an object-relational mapping framework. It can be used for orchestrating the loading of objects and for linking the loaded objects togheter, not for defining how the objects are loaded. LinkIt is intended to be used in a variety of context such as data APIs, ETL processes, CQRS event handlers, web crawlers, etc.

###Features
- Minimize coding effort by leveraging reuse and composition
- Data source independant
- Avoid the Select N + 1 problem
- Built-in support for complex types
- Support polymorphism out of the box
- Favor convention over configuration
- Perform complex projections easily with [LinkItExtensionsForAutoMapper](todo)

Getting Started
---------------
To execute the examples below, see [LinkIt.Samples](todo). 

###Linked Source
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

###Usage
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

###LoadLinkProtocol
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

To create your own conventions, please see how the [existing conventions are built](todo). Then, simply pass your convention by argument when calling `ApplyConventions`. 

###ReferenceLoader
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

###Read more
- [Slightly more complex example](todo)
- [Known limitations](todo)


#*** Will be a separate page ***

Slightly more complex example
---------------
Let's say you have a blog post class that reference media, images and tags by ids. The model and linked source for media and tags are defined in the [getting started example](todo).
```csharp
public class BlogPost {
    public int Id { get; set; }
    public string Title { get; set; }
    public List<int> TagIds { get; set; }
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
    public object Id { get; set; }
}

public class Image {
    public string Id { get; set; }
    public string Alt { get; set; }
    public string Url { get; set; }
}
```

The linked source representing a blog post with its media, images and tags would be defined like this:
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
- `BlogPostLinkedSource/Tags` will match `BlogPost/TagIds`
- `BlogPostLinkedSource/Autor` will match `BlogPost/Author`
- `AuthorLinkedSource/Image` will match `Author/ImageId`

The `BlogPost` object own a nested object of type `Author`. In order to load link the author with the linked image, we need to add the link target `BlogPostLinkedSource/Autor` as a nested linked source of type `AuthorLinkedSource`.

###Polymorphism
For the linked target `BlogPostLinkedSource/MultimediaContent`, we need to configure the load link protocol since none of the default conventions applies. Moreover, it is a special configuration since it involved some polymorphism.
```csharp
loadLinkProtocolBuilder.For<BlogPostLinkedSource>()
    .PolymorphicLoadLink(
        linkedSource => linkedSource.Model.MultimediaContentRef,
        linkedSource => linkedSource.MultimediaContent,
        link => link.Type,
        includes => includes
            .Include<MediaLinkedSource>().AsNestedLinkedSourceById(
                "media",
                link => (int)link.Id
            )
            .Include<Image>().AsReferenceById(
                "image",
                link => (string)link.Id
            )
    );
```

If `MultimediaContentRef.Type` is `"image"` an image will be loaded and linked; however, if `MultimediaContentRef.Type` is `"media"` a media with its tags will be loaded and linked. Here we are reusing `MediaLinkedSource` defined in the [Getting Started](todo) example.

###Usage
Load link by id
```csharp
var actual = loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ById(1);
```

Load link by ids
```csharp
var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().ByIds(
    new List<int>{3,2,1}
);
```

Load link query
```csharp
var models = GetBlogPostByKeyword("fish");
var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModels(models);
```

Load link transiant model (not in a data source)
```csharp
var model = new BlogPost {
    Id = 101,
    Author = new Author{
        Name = "author-name-101",
        Email = "author-email-101",
        ImageId = "distinc-id-loaded-once", //same entity referenced twice
    },
    MultimediaContentRef = new MultimediaContentReference{
        Type = "image",
        Id = "distinc-id-loaded-once" //same entity referenced twice
    },
    TagIds = new List<int>{
        1001,
        1002
    },
    Title = "Title-101"
};

var actual = _loadLinkProtocol.LoadLink<BlogPostLinkedSource>().FromModel(model);
```

#*** Will be a separate page ***


Known limitations
---------------
- Does not support recursive loading
- Does not support polymorphism at root level
- Queries are only supported at root level, all other objects have to be fetched by id
- Nested relation between linked source cannot be resolved by database-side join

The current protocol does not allow to perform database-side join to resolve the models of nested linked source. This performance optimization could be provided by developping database specific LinkIt extensions, but it is out of the scope as of now. For exemple, we could leverage the [database-side join of Mongo DB 3.2](https://www.mongodb.com/blog/post/joins-and-other-aggregation-enhancements-coming-in-mongodb-3-2-part-1-of-3-introduction).

#*** Will be a separate repository ***


LinkIt extensions for AutoMapper
===============

LinkIt extensions for [AutoMapper](http://automapper.org/) can be used to map linked sources to DTOs by conventions.

Here is how how the [LinkIt samples](todo) can be mapped to DTOs seamlessly.

Getting started
---------------

For example, let's say you wish to map to those DTOs:

```csharp
public class MediaDto: IMultimediaContentDto{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<Tag> Tags { get; set; }
}

public class TagDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

Remember our models

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

and our linked source
```csharp
public class MediaLinkedSource
{
    public Media Model { get; set; }
    public List<Tag> Tags { get; set; }
}
```

Map DTOs using our `MapLinkedSource()` extension method:
```csharp
Mapper.CreateMap<MediaLinkedSource, MediaDto>().MapLinkedSource();
```

What this does is, for all the properties of the DTO, map them to matching properties from the linked source, or if none exists, map them to matching properties from the model. It is the equivalent of this.
```csharp
Mapper.CreateMap<MediaLinkedSource, MediaDto>()
    .ForMember(dto => dto.Id, opt => opt.MapFrom(source => source.Model.Id))
    .ForMember(dto => dto.Title, opt => opt.MapFrom(source => source.Model.Title))

```

Of course, in this example, you still need to map the other DTOs, such as `TagDto`. In this case, that object is quite simple, so AutoMapper's default convention works great.
```csharp
Mapper.CreateMap<Tag, TagDto>();
```
We are done, we can leverage [AutoMapper](http://automapper.org/) to perform complex projections on our linked sources!

###Read more
- [Slightly more complex example](todo)


#*** Will be a separate page ***

Slightly more complex example
---------------
In addition to the DTOs from the [getting started example](todo), you also have these DTOs:
```csharp
public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TagDto[] Tags { get; set; }
    public AuthorDto Author { get; set; }
    public IMultimediaContentDto MultimediaContent { get; set; }
}

public class AuthorDto {
    public string Name { get; set; }
    public string Email { get; set; }
    public ImageDto Image { get; set; }
}

public interface IMultimediaContentDto{  
}

public class ImageDto{
    public string Id { get; set; }
    public string Alt { get; set; }
    public string Url { get; set; }
}

```

Remember our models
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

and our linked source
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

And then configure the mappings as such:
```csharp
Mapper.CreateMap<BlogPostLinkedSource, BlogPostDto>().MapLinkedSource();
Mapper.CreateMap<AuthorLinkedSource, AuthorDto>().MapLinkedSource();
```
This would be the equivalent of doing:
```csharp
Mapper.CreateMap<BlogPostLinkedSource, BlogPostDto>()
    .ForMember(dto => dto.Id, opt => opt.MapFrom(source => source.Model.Id))
    .ForMember(dto => dto.Title, opt => opt.MapFrom(source => source.Model.Title))
Mapper.CreateMap<AuthorLinkedSource, AuthorDto>()
    .ForMember(dto => dto.Name, opt => opt.MapFrom(source => source.Model.Name))
    .ForMember(dto => dto.Email, opt => opt.MapFrom(source => source.Model.Email))
```

You still need to map the other DTOs
```csharp
Mapper.CreateMap<Image, ImageDto>();
```

And finaly you need to handle the polymorphic property `BlogPostLinkedSource/MultimediaContent`, but this is a built-in feature of AutoMapper.
```csharp
Mapper.CreateMap<object, Dtos.v1.Interfaces.IMultimediaContentDto>()
    .Include<Image, ImageDto>()
    .Include<MediaLinkedSource, MediaDto>()
```

