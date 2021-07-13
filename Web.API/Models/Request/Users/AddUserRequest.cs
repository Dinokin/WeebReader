using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WeebReader.Web.API.Utilities;

namespace WeebReader.Web.API.Models.Request.Users
{
    public class AddUserRequest : IValidatableObject
    {
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "UsernameRequired")]
        [MinLength(3, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinimumUsernameLength")]
        public string Username { get; init; } = string.Empty;
        
        [MinLength(8, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinimumPasswordLength")]
        public string? Password { get; init; }
        
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "ValidEmailRequired")]
        public string Email { get; init; } = string.Empty;
        
        public string Role { get; init; } = Identity.Roles.None;
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Identity.Roles.GetRoles().Any(str => str == Role))
                return results;

            if (Role == Identity.Roles.None)
                return results;
            
            results.Add(new(string.Format(Messages.InvalidRole, Role), new[] {nameof(Role)}));
            
            return results;
        }
    }
}