namespace BuberDinner.Infrastructure;
public static class Constants
{
    public static class Authentication
    {
        public static class Policies
        {
            public const string Admin = nameof(Admin);
            public const string Guest = nameof(Guest);
            public const string Host = nameof(Host);
            public const string User = nameof(User);
        }

        public static class ClaimTypes
        {
            public const string Authorization = nameof(Authorization);
            public const string Name = nameof(Name);
            public const string FirstName = nameof(FirstName);
            public const string LastName = nameof(LastName);
            public const string UserId = nameof(UserId);
            public const string GuestId = nameof(GuestId);
            public const string HostId = nameof(HostId);
            public const string AdminId = nameof(AdminId);
        }
    }
}