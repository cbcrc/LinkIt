// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;

namespace LinkIt.TopologicalSorting
{
    internal class DependencyType : IEquatable<DependencyType>
    {
        public Type ModelType { get; }
        public Type LinkedSourceType { get; }

        public DependencyType(Type modelType, Type linkedSourceType = null)
        {
            ModelType = modelType ?? throw new ArgumentNullException(nameof(modelType));
            LinkedSourceType = linkedSourceType;
        }

        public override string ToString() => $"type {{ {LinkedSourceType?.Name ?? ModelType.Name} }}";

        public bool Equals(DependencyType other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ModelType == other.ModelType && LinkedSourceType == other.LinkedSourceType;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((DependencyType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ModelType.GetHashCode() * 397) ^ (LinkedSourceType?.GetHashCode() ?? 0);
            }
        }
    }
}