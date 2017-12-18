using System.Collections.Generic;

namespace LinkIt.Samples.Models
{
    public class Media
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<int> TagIds { get; set; } //Tag references
    }
}