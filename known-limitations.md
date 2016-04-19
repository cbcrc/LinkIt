Known Limitations
---------------
- Does not support polymorphism at root level
- Queries are only supported at root level, all other objects have to be fetched by id
- Does not support recursive loading
- Nested relation between linked source cannot be resolved by database-side join

If recursive loading or database-side joins are required, an object that represent the result of these operations must be created and loaded by the reference loader. Then, this object can be loaded and linked by LinkIt like any other objects.

### Read more
- [Why Should I Use LinkIt?](why-without-how.md)
- [Getting Started](getting-started.md)
- [Slightly More Complex Example](slightly-more-complex-example.md)
- [Known Limitations](known-limitations.md)
- [License](LICENSE.txt)