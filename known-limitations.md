Known limitations
---------------
- Does not support recursive loading
- Does not support polymorphism at root level
- Queries are only supported at root level, all other objects have to be fetched by id
- Nested relation between linked source cannot be resolved by database-side join

The current protocol does not allow to perform database-side join to resolve the models of nested linked source. This performance optimization could be provided by developping database specific LinkIt extensions, but it is out of the scope as of now. For exemple, we could leverage the [database-side join of Mongo DB 3.2](https://www.mongodb.com/blog/post/joins-and-other-aggregation-enhancements-coming-in-mongodb-3-2-part-1-of-3-introduction).
