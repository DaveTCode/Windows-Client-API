using System.Collections.Generic;

namespace SendsafelyApi
{
    /// <summary>
    /// A class describing a sendsafely recipient. This class makes up of an email, 
    /// a unique ID and a flag indicating if approval is needed for the recipient.
    /// </summary>
    public class Recipient
    {
        /// <summary>
        /// The recipient ID. Each recipient is given a unique ID once it's added. 
        /// Use this ID to update the recipient in the future. The ID is unique to every package.
        /// </summary>
        public string RecipientId { get; set; }

        /// <summary>
        /// The email belonging to the recipient.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A flag indicating approval is needed or not for the recipient. The value of this flag will 
        /// depend on your enterprise settings as well as if the email belongs to a domain outside 
        /// of the organization or not.
        /// </summary>
        public bool NeedsApproval { get; set; }

        /// <summary>
        /// A list of all possible approvers for the given recipient.
        /// </summary>
        public List<string> Approvers { get; set; }

        /// <summary>
        /// A list of all phonenumbers that was used for this recipient in the past.
        /// </summary>
        public List<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// A list of all confirmations for the recipient. A confirmation will be added as soon as 
        /// a recipient has downloaded one or more files from the item.
        /// </summary>
        public List<Confirmation> Confirmations { get; set; }

        /// <summary>
        /// The role name associate with the recipient.
        /// </summary>
        public string RoleName { get; set; }
    }
}
