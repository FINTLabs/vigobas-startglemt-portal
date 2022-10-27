using System;
using System.ComponentModel.DataAnnotations;

namespace VigoBAS_Start.Models
{
    public class ForgottenUsernameModel
    {
        [Required(ErrorMessage = "Mobilnummer er påkrevd")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(100, ErrorMessage = "Telefonnummeret må være åtte (8) tall.", MinimumLength = 8)]
        [Display(Name = "Mobilnummer")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Skriv inn ditt fødselsnummer")]
        [RegularExpression(pattern:@"^\d{11}$", ErrorMessage = "Fødselsnummer må bestå av nøyaktig 11 siffer")]
        [Display(Name = "Fødselsnummer")]
        public string PersonalNumber { get; set; }

        public bool ShowPersonalNumberField
        {
            get { return ShouldFieldBeDisplayed("ForgotUsernameShowPersNumField"); }
        }

        private bool ShouldFieldBeDisplayed(string key)
        {
            var value = System.Web.Configuration.WebConfigurationManager.AppSettings[key];
            return String.IsNullOrWhiteSpace(value) ? true : value.Equals("on");
        }

    }
}