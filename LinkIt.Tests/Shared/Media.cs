namespace LinkIt.Tests.Shared
{
    public class Media:IPolymorphicModel {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SummaryImageId { get; set; }
    }
}