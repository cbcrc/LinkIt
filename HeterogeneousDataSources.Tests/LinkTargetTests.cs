namespace LinkIt.Tests {
//stle: put back

    //[TestFixture]
    //public class LinkTargetTests
    //{
    //    private PersonLinkedSource _actual;
    //    private LinkTarget<PersonLinkedSource, Image> _sut;
        
    //    [SetUp]
    //    public void SetUp(){
    //        _actual = new PersonLinkedSource {
    //            SummaryImage = new Image { Alt = "prefix" }
    //        };

    //        _sut = LinkTargetFactory.Create<PersonLinkedSource, Image>(
    //            linkedSource => linkedSource.SummaryImage
    //        );
    //    }

    //    [Test]
    //    public void SetTargetProperty()
    //    {
    //        _sut.SetTargetProperty(_actual, new Image{Alt = "the-alt"} );

    //        Assert.That(_actual.SummaryImage.Alt, Is.EqualTo("the-alt"));
    //    }

    //    [Test]
    //    public void GetTargetProperty_ShouldBeAbleToUpdateTargetProperty()
    //    {
    //        _sut.GetTargetProperty(_actual).Alt+="-suffix";

    //        Assert.That(_actual.SummaryImage.Alt, Is.EqualTo("prefix-suffix"));
    //    }

    //    [Test]
    //    public void Create_ShouldSetLinkedSourceType() {
    //        Assert.That(_sut.LinkedSourceType, Is.EqualTo(typeof(PersonLinkedSource)));
    //    }

    //    [Test]
    //    public void Create_ShouldSetPropertyName() {
    //        Assert.That(_sut.PropertyName, Is.EqualTo("SummaryImage"));
    //    }
    //}
}
