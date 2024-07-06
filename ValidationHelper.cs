using Iden.DTOs;
using Iden.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Iden
{
    public static class ValidationHelper
    {
        public static List<ValidationError> GetValidationErrors(ModelStateDictionary modelState, Type modelType)
        {
            return modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError
                {
                    field = GetDisplayName(modelType, key),
                    message = x.ErrorMessage
                })).ToList();
        }

        private static string GetDisplayName(Type modelType, string propertyName)
        {
            var property = modelType.GetProperty(propertyName);
            if (property != null)
            {
                var displayAttribute = property.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                if (displayAttribute != null)
                {
                    return displayAttribute.Name;
                }
            }
            return propertyName;
        }
    }
}
