using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VigoBAS_Start.Attributes;
using VigoBAS_Start.Configuration;

namespace VigoBAS_Start.Models
{
    public class ActivateUserModel
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

        [Display(Name = "Telefonnummer")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Fødselsnummer")]
        [RegularExpression(pattern:@"^\d{11}$", ErrorMessage = "Fødselsnummer må bestå av nøyaktig 11 siffer")]
        public string PersonalNumber { get; set; }


        [Required(ErrorMessage = "Angi aktiveringskode")]
        [DataType(DataType.Password)]
        [Display(Name = "Aktiveringskode")]
        public string Code { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "IT-reglementet må godtas for å kunne bli aktivert.")]
        [Display(Name = "Jeg har lest og godtatt IKT-reglementet")]
        public bool AcceptedRegulation { get; set; }

       
        [Display(Name = "Godkjenn bruk av bilder internt")]
        public bool CanUseInternal { get; set; }

 
        [Display(Name = "Godkjenn bruk av bilder eksternt")]
        public bool CanUseExternal { get; set; }


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

        public List<string> PasswordRules => PasswordConfigurationHelper.GetPasswordRules();

        public string UserType { get; internal set; }
    }
}