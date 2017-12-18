using LinkIt.ConfigBuilders;
using LinkIt.Samples.Models;

namespace LinkIt.Samples.LinkedSources
{
    public class BlogPostLinkedSourceConfig : ILoadLinkProtocolConfig
    {
        public void ConfigureLoadLinkProtocol(LoadLinkProtocolBuilder loadLinkProtocolBuilder)
        {
            loadLinkProtocolBuilder.For<BlogPostLinkedSource>()
                .PolymorphicLoadLink(
                    linkedSource => linkedSource.Model.MultimediaContentRef,
                    linkedSource => linkedSource.MultimediaContent,
                    link => link.Type,
                    includes => includes
                        .Include<MediaLinkedSource>().AsNestedLinkedSourceById(
                            "media",
                            link => (int) link.Id
                        )
                        .Include<Image>().AsReferenceById(
                            "image",
                            link => (string) link.Id
                        )
                );
        }
    }
}