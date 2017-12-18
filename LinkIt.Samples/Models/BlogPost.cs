using System.Collections.Generic;

namespace LinkIt.Samples.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> TagIds { get; set; }
        public Author Author { get; set; }
        public MultimediaContentReference MultimediaContentRef { get; set; }
    }
}