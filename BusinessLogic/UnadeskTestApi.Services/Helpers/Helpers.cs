using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace UnadeskTest.Services.Helpers
{
    public static class AttributeHelper
    {
        public static string GetDisplayName<T>(this T Enum) where T : System.Enum
            => Enum.GetEnumAttribute<T, DisplayAttribute>()?.Name;

        public static string GetDescription<T>(this T Enum) where T : System.Enum
            => Enum.GetEnumAttribute<T, DisplayAttribute>()?.Description;

        public static DisplayAttribute GetDisplayAttribute<T>(this T Enum) where T : System.Enum
            => Enum.GetEnumAttribute<T, DisplayAttribute>();
        public static TAttribute GetEnumAttribute<TEnum, TAttribute>(this TEnum Enum)
            where TEnum : System.Enum
            where TAttribute : System.Attribute
        {
            var MemberInfo = typeof(TEnum).GetMember(Enum.ToString());
            return MemberInfo[0].GetCustomAttribute<TAttribute>();
        }
    }
}
