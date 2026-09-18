namespace AllegroBidder;

public class AppSettings
{
    /// <summary>Adres API - produkcja używa api.allegro.pl, sandbox używa api.allegro.pl.allegrosandbox.pl</summary>
    public string ApiBaseUrl { get; set; } = "https://api.allegro.pl";

    /// <summary>Limit czasu żądania (sekundy)</summary>
    public int TimeoutSeconds { get; set; } = 30;
}