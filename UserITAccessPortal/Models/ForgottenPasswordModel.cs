using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VigoBAS_Start.Attributes;
using VigoBAS_Start.Configuration;

namespace VigoBAS_Start.Models
{
    public class ForgottenPasswordModel
    {
        [Required(ErrorMessage = "Skriv inn brukernavn")]
        [Display(Name = "Brukernavn")]
        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
                this._Username = (value == null || value.Trim().Length == 0) ? null : value.Trim();
            }
        }
        private string _Username { get; set; }

        [Required(ErrorMessage = "Skriv inn ditt fødselsnummer")]
        [RegularExpression(pattern:@"^\d{11}$", ErrorMessage = "Fødselsnummer må bestå av nøyaktig 11 siffer")]
        [Display(Name = "Fødselsnummer")]
        public string PersonalNumber { get; set; }

        [Required(ErrorMessage = "Skriv inn telefonnummer")]
        [Phone(ErrorMessage = "Skriv inn et gyldig telefonnummer")]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Angi engangskode")]
        [DataType(DataType.Password)]
        [Display(Name = "Engangskode")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Angi passord")]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt passord")]
        [PasswordValidation]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Du må bekrefte passord")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("NewPassword", ErrorMessage = "Passord samsvarer ikke.")]
        public string ConfirmPassword { get; set; }

        public bool IsCellPhoneMissing { get; set; } = false;

        public List<string> PasswordRules => PasswordConfigurationHelper.GetPasswordRules();

        public bool ShowPersonalNumberField
        {
            get { return ShouldFieldBeDisplayed("ForgotPassShowPersNumField"); }
        }
        public bool ShowPhoneNumberField
        {
            get { return ShouldFieldBeDisplayed("ForgotPassShowPhoneNum"); }
        }

        private bool ShouldFieldBeDisplayed(string key)
        {
            var value = System.Web.Configuration.WebConfigurationManager.AppSettings[key];
            return String.IsNullOrWhiteSpace(value) ? true : value.Equals("on");
        }

    }
}