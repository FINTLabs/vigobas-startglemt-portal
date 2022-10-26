namespace VigoBAS_Start.Models
{
    public class ActivationCodesResponse
    {
        public bool Success { get; set; }
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsCellPhoneMissing { get; internal set; }
    }
}