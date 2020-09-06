using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WeebReader.Data.Services;
using WeebReader.Web.Models.Others.Attributes;

namespace WeebReader.Web.Models.Others.Extensions
{
    public static class ParametersManagerExtensions
    {
        public static async Task<T> GetModel<T>(this ParametersManager parametersManager) where T : new()
        {
            var t = new T();

            foreach (var property in t.GetType().GetProperties().Where(property => Attribute.IsDefined(property, typeof(ParameterAttribute))))
            {
                var attribute = (ParameterAttribute) property.GetCustomAttribute(typeof(ParameterAttribute))!;
                var parameter = await parametersManager.GetValue<object?>(attribute.ParameterType);
                
                if (parameter == null)
                    continue;

                property.SetValue(t, Convert.ChangeType(parameter, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
            }
            
            return t;
        }
        
        public static async Task<T> GetValue<T>(this ParametersManager parametersManager, ParameterTypes parameterTypes) => await parametersManager.GetValue<T>((ushort) parameterTypes);

        public static async Task<bool> Save(this ParametersManager parametersManager, ParameterTypes parameterTypes, string? value) => await parametersManager.Save((ushort) parameterTypes, value);
    }
}