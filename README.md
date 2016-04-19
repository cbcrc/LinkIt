![LinkIt](logo.png) 
===============
LinkIt is an object-oriented data integration library that make it easy to load different kinds of objects and link them together. 

LinkIt is not an object-relational mapping framework. It can be used for orchestrating the loading of objects and for linking the loaded objects togheter, not for defining how the objects are loaded. LinkIt is intended to be used in a variety of context such as data APIs, ETL processes, CQRS event handlers, web crawlers, etc.

### Features
- Minimize coding effort by leveraging reuse and composition
- Data source independant
- Avoid the Select N + 1 problem
- Built-in support for references between complex types
- Support polymorphism out of the box
- Favor convention over configuration
- Perform complex projections easily with [LinkIt AutoMapper Extensions](https://github.com/cbcrc/LinkIt.AutoMapperExtensions)

### Read more
- [Why Should I Use LinkIt?](why-without-how.md)
- [Getting Started](getting-started.md)
- [Slightly More Complex Example](slightly-more-complex-example.md)
- [Known Limitations](known-limitations.md)