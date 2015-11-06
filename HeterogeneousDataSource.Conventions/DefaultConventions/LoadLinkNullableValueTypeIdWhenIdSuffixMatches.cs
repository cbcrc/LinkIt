﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using HeterogeneousDataSources;

namespace HeterogeneousDataSource.Conventions.DefaultConventions {
    public class LoadLinkNullableValueTypeIdWhenIdSuffixMatches : INullableValueTypeIdConvention {
        public bool DoesApply(
            PropertyInfo linkTargetProperty,
            PropertyInfo linkedSourceModelProperty) 
        {
            return linkTargetProperty.MatchLinkedSourceModelPropertyName(linkedSourceModelProperty, "Id");
        }

        public void Apply<TLinkedSource, TLinkTargetProperty, TLinkedSourceModelProperty>(
            LoadLinkProtocolForLinkedSourceBuilder<TLinkedSource> loadLinkProtocolForLinkedSourceBuilder, 
            Expression<Func<TLinkedSource, TLinkTargetProperty>> getLinkTargetProperty,
            Func<TLinkedSource, TLinkedSourceModelProperty?> getLinkedSourceModelProperty
        ) 
            where TLinkedSourceModelProperty : struct
        {
            loadLinkProtocolForLinkedSourceBuilder.LoadLinkReference(
                getLinkedSourceModelProperty,
                getLinkTargetProperty
            );
        }
    }
}
