using Microsoft.AspNetCore.Identity;
using Tamaris.Entities.Msg;


namespace Tamaris.Entities.Admin
{
    public class User : IdentityUser
    {
        // Custom properties

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Avatar
        /// </summary>
        public byte[] Avatar { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public string Company { get; set; }



        // Custom collections
        public virtual ICollection<Message> ReceivedMessages { get; set; } // FK__messages__receiver_users
        public virtual ICollection<Message> SentMessages { get; set; } // FK__messages__sender_users
    }
}