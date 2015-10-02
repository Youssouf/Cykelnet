using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Cykelnet
{
    public class VersionedUser : MembershipUser
    {
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string CyclistType { get; set; }
        public DateTime Birthday { get; set; }

        public VersionedUser(string providername,
                                  string username,
                                  object providerUserKey,
                                  string email,
                                  string passwordQuestion,
                                  string comment,
                                  bool isApproved,
                                  bool isLockedOut,
                                  DateTime creationDate,
                                  DateTime lastLoginDate,
                                  DateTime lastActivityDate,
                                  DateTime lastPasswordChangedDate,
                                  DateTime lastLockedOutDate,
                                  string FullName,
                                  string Address1,
                                  string Address2,
                                  string Country,
                                  string CyclistType,
                                  DateTime Birthday) :
            base(providername,
                                       username,
                                       providerUserKey,
                                       email,
                                       passwordQuestion,
                                       comment,
                                       isApproved,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastActivityDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate)
        {
            this.FullName = FullName;
            this.Address1 = Address1;
            this.Address2 = Address2;
            this.Country = Country;
            this.CyclistType = CyclistType;
            this.Birthday = Birthday;
        }

        public Guid getGuid()
        {
            return (Guid)this.ProviderUserKey;
        }
    }

}