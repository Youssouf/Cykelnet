using System.Web.Security;
using System;
using Cykelnet;

public class AuthenticationHelper
{
    readonly MembershipProvider _provider;

    public AuthenticationHelper(MembershipProvider provider)
    {
        _provider = provider;
    }

    public Guid getGuid(string userName)
    {
        MembershipUser user = _provider.GetUser(userName, false);
        return (Guid)user.ProviderUserKey;
    }

    public static Guid getCurrentUserID()
    {
        return (Guid)Membership.GetUser().ProviderUserKey;
    }

    public static VersionedUser getVersionedUser(Guid ID, bool updateOnline)
    {
        return (VersionedUser)Membership.GetUser(ID, updateOnline);
    }
}