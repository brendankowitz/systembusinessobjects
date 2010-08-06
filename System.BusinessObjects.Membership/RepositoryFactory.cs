using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Data;
using System.BusinessObjects.Infrastructure;

namespace System.BusinessObjects.Membership
{
    public interface IMembershipRepositoryFactory
    {
        IDataObjectRepository<T> GetMembershipRepository<T>() where T : DataObject;
    }

    public sealed class MembershipProviderRepository
    {
        public static void Set(IMembershipRepositoryFactory provider)
        {
            RepositoryFactory._factory = provider;
        }
    }

    internal static class RepositoryFactory
    {
        internal static IMembershipRepositoryFactory _factory = null;

        internal static IDataObjectRepository<Application> GetApplicationRepository()
        {
            return _factory.GetMembershipRepository<Application>();
        }

        internal static IDataObjectRepository<Membership> GetMembershipRepository()
        {
            return _factory.GetMembershipRepository<Membership>();
        }

        internal static IDataObjectRepository<Profile> GetProfileRepository()
        {
            return _factory.GetMembershipRepository<Profile>();
        }

        internal static IDataObjectRepository<Role> GetRoleRepository()
        {
            return _factory.GetMembershipRepository<Role>();
        }

        internal static IDataObjectRepository<User> GetUserRepository()
        {
            return _factory.GetMembershipRepository<User>();
        }
    }
}
