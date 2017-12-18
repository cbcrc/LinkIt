using LinkIt.PublicApi;
using LinkIt.Samples.Models;

namespace LinkIt.Samples.LinkedSources
{
    public class AuthorLinkedSource : ILinkedSource<Author>
    {
        public Image Image { get; set; }
        public Author Model { get; set; }
    }
}