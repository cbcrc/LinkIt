using System.Collections.Generic;

namespace LinkIt.PublicApi
{
    //Responsible for loading and linking root linked sources
    public interface ILoadLinker<TRootLinkedSource>
    {
        TRootLinkedSource FromModel<TRootLinkedSourceModel>(TRootLinkedSourceModel model);
        List<TRootLinkedSource> FromModels<TRootLinkedSourceModel>(List<TRootLinkedSourceModel> models);
        TRootLinkedSource ById<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId);
        List<TRootLinkedSource> ByIds<TRootLinkedSourceModelId>(List<TRootLinkedSourceModelId> modelIds);
    }
}