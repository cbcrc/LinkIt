// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using LinkIt.Core.Includes;
using LinkIt.Core.Includes.Interfaces;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.PublicApi;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;

namespace LinkIt.ConfigBuilders
{
    /// <summary>
    /// Builder to configure a polymorphic link.
    /// </summary>
    public class IncludeSetBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>
        where TLinkedSource: ILinkedSource
    {
        private readonly Dictionary<TDiscriminant, IInclude> _includeByDiscriminantValue =
            new Dictionary<TDiscriminant, IInclude>();

        internal IncludeSetBuilder(ILinkTarget linkTarget)
        {
            LinkTarget = linkTarget;
        }

        internal ILinkTarget LinkTarget { get; }

        /// <summary>
        /// Include a polymorphic link.
        /// </summary>
        public IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget> Include<TLinkTarget>()
            where TLinkTarget : TAbstractLinkTarget
        {
            return new IncludeAsBuilder<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant, TLinkTarget>(this);
        }

        internal void AddToIncludeSet(TDiscriminant discriminant, IInclude include)
        {
            if (_includeByDiscriminantValue.ContainsKey(discriminant))
            {
                throw new LinkItException(
                   $"Invalid configuration for linked source {typeof(TLinkedSource).GetFriendlyName()}, polymorphic list target {{{LinkTarget.Expression}}}: cannot have more than one include with the same discriminant ({discriminant})."
               );
            }

            _includeByDiscriminantValue.Add(discriminant, include);
        }

        internal IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant> Build(Func<TLink, TDiscriminant> getDiscriminant)
        {
            return new IncludeSet<TLinkedSource, TAbstractLinkTarget, TLink, TDiscriminant>(
                _includeByDiscriminantValue,
                getDiscriminant
            );
        }
    }
}