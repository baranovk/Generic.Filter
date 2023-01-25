using System.Reflection;

namespace Generic.Filter.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type? GetUnderlyingType(this MemberInfo member) => member.MemberType switch
        {
            MemberTypes.Event => ((EventInfo)member).EventHandlerType,
            MemberTypes.Field => ((FieldInfo)member).FieldType,
            MemberTypes.Method => ((MethodInfo)member).ReturnType,
            MemberTypes.Property => ((PropertyInfo)member).PropertyType,
            _ => throw new ArgumentException(Resources.ErrorUnsupportedMemberInfoType)
        };

        public static bool IsProperty(this MemberInfo member) => member.MemberType.Equals(MemberTypes.Property);
    }
}
