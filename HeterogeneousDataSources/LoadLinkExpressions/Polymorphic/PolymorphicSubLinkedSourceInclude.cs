using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicSubLinkedSourceInclude<TIChildLinkedSource, TIChildLinkedSourceModel, TChildLinkedSource, TChildLinkedSourceModel>
        : IPolymorphicSubLinkedSourceInclude<TIChildLinkedSource, TIChildLinkedSourceModel>
        where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
        where TChildLinkedSourceModel: TIChildLinkedSourceModel
    {
        public PolymorphicSubLinkedSourceInclude(){
            ChildLinkedSourceType = typeof(TChildLinkedSource);
        }

        public Type ReferenceType{
            get { return null; }
        }

        public Type ChildLinkedSourceType { get; private set; }

        public TIChildLinkedSource CreateSubLinkedSource(TIChildLinkedSourceModel iChildLinkedSourceModel, LoadedReferenceContext loadedReferenceContext) {
            if (!(iChildLinkedSourceModel is TChildLinkedSourceModel)){
                //stle: all error message should have a way to identity context: at least linked source and target property
                throw new InvalidOperationException(
                    string.Format(
                        "Sub linked source of type {0} cannot have model of type {1}.",
                        typeof(TChildLinkedSource),
                        typeof(TChildLinkedSourceModel)
                    )
                );

            }

            var childLinkedSourceModel = (TChildLinkedSourceModel)iChildLinkedSourceModel;

            var subLinkedSources = LoadLinkExpressionUtil.CreateLinkedSources<TChildLinkedSource, TChildLinkedSourceModel>(
                new List<TChildLinkedSourceModel>{ childLinkedSourceModel }, 
                loadedReferenceContext
            );
            //stle: please make it explicit that include works at single value level not at list level
            return subLinkedSources.SingleOrDefault();
        }
    }
}