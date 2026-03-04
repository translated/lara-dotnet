namespace Lara.Sdk.Models.Authentication;

public class AuthToken
{
    public string Token { get;  }
    public string RefreshToken { get;  }

    public AuthToken(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }
}