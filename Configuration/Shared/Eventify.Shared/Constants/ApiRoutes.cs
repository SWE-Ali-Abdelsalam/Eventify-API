namespace Eventify.Shared.Constants
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = $"{Root}/{Version}";

        public static class Auth
        {
            public const string Login = $"{Base}/auth/login";
            public const string Register = $"{Base}/auth/register";
            public const string RefreshToken = $"{Base}/auth/refresh";
            public const string RevokeToken = $"{Base}/auth/revoke";
            public const string ForgotPassword = $"{Base}/auth/forgot-password";
            public const string ResetPassword = $"{Base}/auth/reset-password";
        }

        public static class Users
        {
            public const string GetAll = $"{Base}/users";
            public const string GetById = $"{Base}/users/{{id}}";
            public const string Create = $"{Base}/users";
            public const string Update = $"{Base}/users/{{id}}";
            public const string Delete = $"{Base}/users/{{id}}";
            public const string GetProfile = $"{Base}/users/profile";
            public const string UpdateProfile = $"{Base}/users/profile";
        }

        public static class Events
        {
            public const string GetAll = $"{Base}/events";
            public const string GetById = $"{Base}/events/{{id}}";
            public const string Create = $"{Base}/events";
            public const string Update = $"{Base}/events/{{id}}";
            public const string Delete = $"{Base}/events/{{id}}";
            public const string Publish = $"{Base}/events/{{id}}/publish";
            public const string Cancel = $"{Base}/events/{{id}}/cancel";
        }
    }
}
