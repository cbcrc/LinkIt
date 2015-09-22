using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public class SubLinkedSourceInclude<TIChildLinkedSource, TChildLinkedSource, TChildLinkedSourceModel>
        : ISubLinkedSourceInclude<TIChildLinkedSource>
        where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
    {
        public SubLinkedSourceInclude(){
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType{
            get { return null; }
        }

        public Type ChildLinkedSourceType { get; private set; }

        public TIChildLinkedSource CreateSubLinkedSource(object childLinkedSourceModel, LoadedReferenceContext loadedReferenceContext) {
            if (!(childLinkedSourceModel is TChildLinkedSourceModel)) {
                //stle: all error message should have a way to identity context: at least linked source and target property
                throw new InvalidOperationException(
                    string.Format(
                        "Sub linked source of type {0} cannot have model of type {1}.",
                        typeof(TChildLinkedSource),
                        childLinkedSourceModel.GetType()
                    )
                );
            }

            var castedChildLinkedSourceModel = (TChildLinkedSourceModel)childLinkedSourceModel;

            var subLinkedSources = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(
                new List<TChildLinkedSourceModel> { castedChildLinkedSourceModel }, 
                loadedReferenceContext
            );
            //stle: please make it explicit that include works at single value level not at list level
            return subLinkedSources.SingleOrDefault();
        }
    }
}