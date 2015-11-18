using System;
using System.Collections.Generic;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;

namespace LinkIt.Core.Interfaces
{
    //Responsible for creating a load linker for a specific root linked source type
    //Responsible for creating include with nested linked source
    public interface IGenericLinkedSourceConfig<TLinkedSource> : ILinkedSourceConfig
    {
        ILoadLinker<TLinkedSource> CreateLoadLinker(
            IReferenceLoader referenceLoader,
            List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel,
            LoadLinkProtocol loadLinkProtocol
        );

        IInclude CreateIncludeNestedLinkedSourceById<TLinkTargetOwner, TAbstractChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupId,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSource = null
        );

        IInclude CreateIncludeNestedLinkedSourceFromModel<TAbstractChildLinkedSource, TLink, TChildLinkedSourceModel>(Func<TLink, TChildLinkedSourceModel> getNestedLinkedSourceModel, ILinkTarget linkTarget);
    }
}