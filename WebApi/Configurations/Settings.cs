namespace WebApi.Configurations
{
    public class Settings
    {
        public JwtOptions JWT { get; init; } = new();
        public SmtpOptions SMTP { get; init; } = new();
        public ConnOptions ConnStrings { get; init; } = new();

        public class ConnOptions
        {
            public string SqlDbLocal { get; init; } = string.Empty;
            public string SqlDb { get; init; } = string.Empty;
        }

        public class JwtOptions
        {
            public string Secret { get; init; } = string.Empty;
            public int ExpiresIn { get; init; }
        }

        public class SmtpOptions
        {
            public string ServerEndpoint { get; init; } = string.Empty;
            public string DisplayName { get; init; } = string.Empty;
            public string Username { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
            public bool UseSSL { get; init; } = true;
        }
    }

}