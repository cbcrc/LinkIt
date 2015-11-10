using System.Reflection;

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

        private static bool MatchLinkedSourceModelPropertyName(string linkTargetPropertyName, string linkedSourceModelPropertyName, string suffix) {
            return linkTargetPropertyName + suffix == linkedSourceModelPropertyName;
        }

        private static string RemoveLastCharacter(PropertyInfo property, string lastCharacterToIgnore){
            return property.Name.Remove(property.Name.Length - lastCharacterToIgnore.Length);
        }

        public static string GetLinkTargetId(this PropertyInfo property) {
            return string.Format(
                "{0}/{1}",
                property.DeclaringType,
                property.Name
            );
        }
    }
}