using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using GuildfordBoroughCouncil.ActiveDirectory.Interfaces;

namespace GuildfordBoroughCouncil.Linq
{
    public static class IIdentityExtensions
    {
        public static string NameX(this IIdentity identity)
        {
            return identity.Name.RemoveDownLevelDomain().ToLower();
        }

        public static IUser ActiveDirectory(this IIdentity identity)
        {
            return new Data.Models.User(identity.NameX()).ActiveDirectory;
        }

        public static string DisplayName(this IIdentity identity)
        {
            return identity.ActiveDirectory().Name;
        }

        public static Data.Models.User GetUser(this IIdentity identity)
        {
            return new Data.Models.User(identity.NameX());
        }

        public static Data.Models.AdObject GetAdObject(this IIdentity identity)
        {
            return new Data.Models.AdObject(identity.NameX());
        }

        public static IEnumerable<string> GetGroupSamAccountNames(this IIdentity identity)
        {
            return GuildfordBoroughCouncil.ActiveDirectory.Lookup.GroupSamAccountNamesForUser(identity.NameX()).Result;
        }
    }
}