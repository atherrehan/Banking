using System.ComponentModel;
using System.Text;

namespace Banking.FanFinancing.Shared.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo is not null)
            {
                var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
            }

            return enumValue.ToString();
        }
    }
}
