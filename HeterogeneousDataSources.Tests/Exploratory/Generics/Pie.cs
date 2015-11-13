namespace LinkIt.Tests.Exploratory.Generics
{
    public class Pie<T>
    {
        public Pie(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public string PieContent
        {
            get { return typeof (T).Name; }
        }
    }
}