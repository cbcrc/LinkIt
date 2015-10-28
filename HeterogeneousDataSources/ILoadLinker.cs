using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadLinker<TRootLinkedSource>
    {
        TRootLinkedSource FromModel<TRootLinkedSourceModel>(TRootLinkedSourceModel model);
        List<TRootLinkedSource> FromModels<TRootLinkedSourceModel>(params TRootLinkedSourceModel[] models);
        TRootLinkedSource ById<TRootLinkedSourceModelId>(TRootLinkedSourceModelId modelId);
    }
}