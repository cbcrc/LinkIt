using System;
using System.Collections.Generic;
using System.Reflection;
using HeterogeneousDataSource.Conventions;

namespace HeterogeneousDataSources
{
    public static class PropertyInfoExtensions
    {
        public static bool IsPublicReadWrite(this PropertyInfo property) {
            return property.CanRead &&
                property.GetGetMethod(false)!=null &&
                property.CanWrite &&
                property.GetSetMethod(false)!=null;
        }

        //stle: move in convention?
        public static LinkTargetKind GetLinkTargetKind(this PropertyInfo property) {
            if (Nullable.GetUnderlyingType(property.PropertyType) != null) {
                return LinkTargetKind.NullableValueType;
            }

            if (property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) 
            {
                return LinkTargetKind.MultiValue;
            }

            return LinkTargetKind.SingleValue;
        }

        //stle: move in convention?
        public static bool MatchLinkedSourceModelPropertyName(this PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty, string suffix)
        {
            return MatchLinkedSourceModelPropertyName(
                linkTargetProperty.Name, 
                linkedSourceModelProperty.Name, 
                suffix
            );
        }

        public static bool MatchLinkedSourceModelPropertyName(this PropertyInfo linkTargetProperty, PropertyInfo linkedSourceModelProperty, string suffix, string lastCharacterToIgnore) {
            if (!linkTargetProperty.Name.EndsWith(lastCharacterToIgnore) ||
                !linkedSourceModelProperty.Name.EndsWith(lastCharacterToIgnore)) {
                return false;
            }

            return MatchLinkedSourceModelPropertyName(
                RemoveLastCharacter(linkTargetProperty, lastCharacterToIgnore),
                RemoveLastCharacter(linkedSourceModelProperty, lastCharacterToIgnore),
                suffix
            );
        }

        private static bool MatchLinkedSourceModelPropertyName(string linkTargetPropertyName, string linkedSourceModelPropertyName, string suffix) {
            return linkTargetPropertyName + suffix == linkedSourceModelPropertyName;
        }

        private static string RemoveLastCharacter(PropertyInfo property, string lastCharacterToIgnore){
            return property.Name.Remove(property.Name.Length - lastCharacterToIgnore.Length);
        }
    }
}