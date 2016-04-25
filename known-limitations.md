Known Limitations
---------------
- Does not support polymorphism at root level
- Queries are only supported at root level, all other objects have to be fetched by ID
- Does not support recursive loading
- Nested relation between linked source cannot be resolved by database-side join

If recursive loading or database-side joins are required, an object that represents the result of those operations must be created and loaded by the reference loader. Then, this object can be loaded and linked by LinkIt like any other object.

### Read more
- [Why Should I Use LinkIt?](why-without-how.md)
- [Getting Started](getting-started.md)
- [Slightly More Complex Example](slightly-more-complex-example.md)
- [License](LICENSE.txt)

### See also
- Perform complex projections easily with [LinkIt AutoMapper Extensions](https://github.com/cbcrc/LinkIt.AutoMapperExtensions)