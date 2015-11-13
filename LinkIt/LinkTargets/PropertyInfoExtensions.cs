using System.Reflection;

namespace LinkIt.LinkTargets
{
    public static class PropertyInfoExtensions
    {
        public static bool IsPublicReadWrite(this PropertyInfo property) {
            return property.CanRead &&
                property.GetGetMethod(false)!=null &&
                property.CanWrite &&
                property.GetSetMethod(false)!=null;
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