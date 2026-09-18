namespace AllegroBidder;

public static class Secrets
{
    // Uzyskaj z developer.allegro.pl
    public const string ClientId = "TWOJE_CLIENT_ID";
    public const string ClientSecret = "TWOJE_CLIENT_SECRET";

    // Token dostępu OAuth2 (uzyskany z procesu Device Flow)
    // Token jest ważny przez 12 godzin, po wygaśnięciu wymaga odświeżenia
    public static string AccessToken = "TWOJ_ACCESS_TOKEN";
}