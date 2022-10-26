using VigoBAS_Start.IDMServices.ActivationCodes;
using VigoBAS_Start.Models;

namespace VigoBAS_Start.DataAccess
{
    public class ActivationCodesRepository
    {
        public ActivationCodesResponse SetAndSendActivationCode(string username)
        {
            ActivationCodesResponse response;
            using (var client = new ActivationCodesClient())
            {
                var user = client.GetUserByUsername(username);
                response = ValidateUserInfo(user);
                if (!response.Success)
                {
                    return response;
                }
                client.SetAndSendActivationCode(username, ActivationCodesActivationCodeType.Glemt);
            }
            return response;
        }

        public ActivationCodesResponse ValidateActivationCodeStart(string username, string activationCode)
        {
            using (var client = new ActivationCodesClient())
            {
                var user = client.GetUserByUsername(username);
                var response = ValidateUserInfoStart(user);
                if (!response.Success)
                {
                    return response;
                }
                if (!client.ValidateActivationCode(username, activationCode, ActivationCodesActivationCodeType.Start))
                {
                    response.PropertyName = "Code";
                    response.ErrorMessage = "Aktiveringskoden er feil.";
                    response.Success = false;
                    return response;
                }
                response.Success = true;
                return response;
            }
        }
        private static ActivationCodesResponse ValidateUserInfoStart(Profile user)
        {
            var response = new ActivationCodesResponse { Success = false };
            if (user == null)
            {
                response.PropertyName = "Username";
                response.ErrorMessage = "Fant ikke bruker.";
                return response;
            }

            response.Success = true;
            return response;
        }

        private static ActivationCodesResponse ValidateUserInfo(Profile user)
        {
            var response = new ActivationCodesResponse {Success = false};
            if (user == null)
            {
                response.PropertyName = "Username";
                response.ErrorMessage = "Fant ikke bruker.";
                return response;
            }

            response.IsCellPhoneMissing = string.IsNullOrEmpty(string.Join("",  user.PrivateMobile, user.PrivatePhone, user.WorkPhone, user.WorkMobile));
            response.Success = true;

            return response;
        }

        public bool ValidateActivationCodeForgotten(string username, string activationCode)
        {
            using (var client = new ActivationCodesClient())
            {
                return client.ValidateActivationCode(username, activationCode, ActivationCodesActivationCodeType.Glemt);
            }
        }

        public void SetActiveDirectoryPassword(string username, string password, ActivationCodesActivationCodeType activationType)
        {
            using (var client = new ActivationCodesClient())
            {
                client.SetActiveDirectoryPassword(username, password, activationType);
            }
        }

        public void SendUsernameBySms(string phoneNumber)
        {
            using (var client = new ActivationCodesClient())
            {
                client.GetUsername(phoneNumber);
            }
        }

        public Profile GetProfileByPhoneNumber(string phoneNumber)
        {
            using (var client = new ActivationCodesClient())
            {
                return client.GetUserByPhonenumber(phoneNumber);
            }
        }
    }
}