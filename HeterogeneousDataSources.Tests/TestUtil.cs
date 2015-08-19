namespace HeterogeneousDataSources.Tests
{
    public static class TestUtil
    {
        public static readonly ReferenceTypeConfig<Image, string> ImageReferenceTypeConfig = new ReferenceTypeConfig<Image, string>(
            image => image.Id,
            ids => new ImageRepository().GetByIds(ids)
            );
    }
}