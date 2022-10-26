using System.Linq;
using VigoBAS_Start.IDMServices.Profiles;

namespace VigoBAS_Start.DataAccess
{
    public class ProfilesRepository
    {
        public Profile GetProfile(string username)
        {
            using(var client = new ProfilesClient())
            {
                return client.Get(username);
            }
        }

        public void SetProfilePictureFlags(string username, bool canUseExternal, bool canUseInternal)
        {
            using (var client = new ProfilesClient())
            {
                var profilePicture = new ProfilePicture
                {
                   Username = username,
                   Attachment = new Attachment
                   {
                       CanUseExternal = canUseExternal,
                       CanUseInternal = canUseInternal
                   }
                };
                client.ModifyProfilePictureFlags(profilePicture);
            }
        }
    }
}