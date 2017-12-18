#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System.Reflection;

namespace LinkIt.Conventions
{
    public static class PropertyInfoExtensionsForLoadLinkExpressionConventions
    {
        public static bool MatchLinkedSourceModelPropertyName(this PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty, string suffix)
        {
            return MatchLinkedSourceModelPropertyName(
                linkTargetProperty.Name,
                linkedSourceModelProperty.Name,
                suffix
            );
        }

        public static bool MatchLinkedSourceModelPropertyName(this PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty, string suffix, string lastCharacterToIgnore)
        {
            if (!linkTargetProperty.Name.EndsWith(lastCharacterToIgnore) ||
                !linkedSourceModelProperty.Name.EndsWith(lastCharacterToIgnore))
                return false;

            return MatchLinkedSourceModelPropertyName(
                RemoveLastCharacter(linkTargetProperty, lastCharacterToIgnore),
                RemoveLastCharacter(linkedSourceModelProperty, lastCharacterToIgnore),
                suffix
            );
        }

        private static bool MatchLinkedSourceModelPropertyName(string linkTargetPropertyName, string linkedSourceModelPropertyName, string suffix)
        {
            return linkTargetPropertyName + suffix == linkedSourceModelPropertyName;
        }

        private static string RemoveLastCharacter(PropertyInfo property, string lastCharacterToIgnore)
        {
            return property.Name.Remove(property.Name.Length - lastCharacterToIgnore.Length);
        }
    }
}