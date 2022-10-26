using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.ServiceModel.Configuration;
using System.Text.RegularExpressions;
using Vigo.Bas.ManagementAgent.Log;
using VigoBAS_Start.Configuration;

namespace VigoBAS_Start.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class PasswordValidationAttribute : ValidationAttribute
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value == null)
            {
                return new ValidationResult("Angi passord");
            }
            try
            {
                var password = value.ToString();
                var validationRules = ConfigurationManager.GetSection("passwordValidation") as PasswordValidationSection;

                if (validationRules == null)
                {
                    return ValidationResult.Success;
                }
                var errorMessage = "Passord matcher ikke passordregel.";
                foreach (PasswordValidationInstanceElement rule in validationRules.Instances)
                {
                    var regex = new Regex(rule.Regex);
                    if (!regex.IsMatch(password))
                    {
                        if (!string.IsNullOrEmpty(rule.ErrorMessage))
                        {
                            errorMessage = rule.ErrorMessage;
                        }
                        return new ValidationResult(errorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Error(e);
                Logger.Log.Error(e.Message);
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }
}

