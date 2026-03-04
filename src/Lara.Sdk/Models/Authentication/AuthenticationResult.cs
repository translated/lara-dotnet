namespace Lara.Sdk.Models.Authentication;

public class AuthenticationResult
{
    public string Token { get;  }

    public AuthenticationResult(string token)
    {
        Token = token;
    }
}