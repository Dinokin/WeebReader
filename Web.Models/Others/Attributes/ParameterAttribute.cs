using System;

namespace WeebReader.Web.Models.Others.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        public ParameterTypes ParameterType { get; }

        public ParameterAttribute(ParameterTypes parameterType) => ParameterType = parameterType;
    }
}