using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurantApp.Extensions
{
    public static class ReflectionExtensions
    {
        public static string GetPropertyValue<T>(this T item, string propertyValue)
        {
            return item.GetType().GetProperty(propertyValue).GetValue(item, null).ToString();
        }
    }
}
