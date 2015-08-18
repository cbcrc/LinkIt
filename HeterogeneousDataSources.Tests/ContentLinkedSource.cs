using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using NUnit.Framework;
using RC.Testing;

namespace HeterogeneousDataSources.Tests {
    [UseReporter(typeof(DiffReporter))]
    [TestFixture]
    public class LoadLinkTests
    {
        [Test]
        public void DoIt()
        {
            var content = new Content{
                Id = 1,
                SummaryImageId = "a"
            };
            var linkedSource = new ContentLinkedSource(content);

            var loader = new Loader();
            var dataContext = loader.Load(linkedSource.LoadLinkExpressions);

            ApprovalsExt.VerifyPublicProperties(dataContext);
        }
    }

    public class Loader
    {
        private readonly Dictionary<Type, IReferenceLoader> _referenceLoaders = new Dictionary<Type, IReferenceLoader>
        {
            {typeof (Image), new ImageRepository()}
        };

        public DataContext Load(List<ILoadLinkExpression> loadLinkExpressions)
        {
            var dataContext = new DataContext();
            foreach (var loadLinkExpression in loadLinkExpressions)
            {
                if (!_referenceLoaders.ContainsKey(loadLinkExpression.ReferenceType)){
                    throw new InvalidOperationException(string.Format("No reference loader exists for {0}", loadLinkExpression.ReferenceType.Name));
                }
                var referenceLoader = _referenceLoaders[loadLinkExpression.ReferenceType];

                var referenceIds = loadLinkExpression.ReferenceIds;
                var references = referenceLoader.LoadReferences(referenceIds);

                dataContext.Append(references, loadLinkExpression.ReferenceType);
            }

            return dataContext;
        }
    }

    public class ImageRepository: IReferenceLoader
    {
        public List<Image> GetByIds(List<string> ids)
        {
            return ids
                .Select(id => new Image{
                    Id = id, 
                    Alt = "alt-" + id
                })
                .ToList();
        }

        public List<Object> LoadReferences(List<object> ids)
        {
            return GetByIds(ids.Cast<string>().ToList())
            .Cast<object>()
            .ToList();
        }
    }

    public interface IReferenceLoader
    {
        List<Object> LoadReferences(List<object> ids);
    }

    public class ContentLinkedSource {
        //stle: two steps loading sucks!
        public ContentLinkedSource(Content model)
        {
            Model = model;
        }


        public List<ILoadLinkExpression> LoadLinkExpressions
        {
            get
            {
                return new List<ILoadLinkExpression>{
                    new LoadLinkExpression<Image>(
                        () => Model.SummaryImageId
                    )
                };
            }
        }

        public Content Model { get; private set; }
        public Image SummaryImage { get; set; }
    }

    public class Content {
        public int Id { get; set; }
        public string SummaryImageId { get; set; }
    }

    public class Image {
        public string Id { get; set; }
        public string Alt { get; set; }
    }


}
