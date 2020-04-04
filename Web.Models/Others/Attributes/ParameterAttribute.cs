using System;
using WeebReader.Data.Entities;

namespace WeebReader.Web.Models.Others.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ParameterAttribute : Attribute
    {
        public Parameter.Types ParameterType { get; }

        public ParameterAttribute(Parameter.Types parameterType) => ParameterType = parameterType;
    }
}