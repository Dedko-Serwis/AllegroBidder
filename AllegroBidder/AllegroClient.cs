using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace AllegroBidder;

public class AllegroClient
{
    private readonly HttpClient _http;
    private readonly AppSettings _settings;

    public AllegroClient(AppSettings settings)
    {
        _settings = settings;
        _http = new HttpClient
        {
            BaseAddress = new Uri(_settings.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds)
        };
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Secrets.AccessToken);
        // Kluczowe: nagłówek Accept dla interfejsu beta licytacji
        _http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.allegro.beta.v1+json"));
    }

    /// <summary>Pobiera podstawowe informacje o ofercie (nazwa, aktualna cena, czas zakończenia).</summary>
    public async Task<OfferInfo?> GetOfferInfoAsync(string offerId)
    {
        var resp = await _http.GetAsync($"/offers/{offerId}");
        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Nie udało się pobrać oferty ({resp.StatusCode}): {err}");
        }

        var json = await resp.Content.ReadAsStringAsync();
        dynamic? data = JsonConvert.DeserializeObject(json);
        if (data?.name == null) return null;

        return new OfferInfo
        {
            Id = offerId,
            Name = (string)data.name,
            // Zwróć uwagę: pole ceny dla ofert licytacyjnych to sellingMode.price
            CurrentPrice = (decimal?)data.sellingMode?.price?.amount ?? 0,
            Currency = (string?)data.sellingMode?.price?.currency ?? "PLN",
            EndingAt = (DateTime?)data.publication?.endingAt
        };
    }

    /// <summary>Składa ofertę licytacji (maksymalna cena, którą jesteś gotów zapłacić).</summary>
    public async Task<BidResult> PlaceBidAsync(string offerId, decimal maxAmount, string currency = "PLN")
    {
        var payload = new
        {
            amount = maxAmount.ToString("F2"),
            currency = currency
        };
        var content = new StringContent(JsonConvert.SerializeObject(payload),
            Encoding.UTF8, "application/vnd.allegro.beta.v1+json");

        var resp = await _http.PutAsync($"/bidding/offers/{offerId}/bid", content);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Nie udało się złożyć oferty ({resp.StatusCode}): {body}");

        dynamic? data = JsonConvert.DeserializeObject(body);
        return new BidResult
        {
            Success = true,
            HighBidder = (bool?)data?.highBidder ?? false,
            CurrentPrice = (decimal?)data?.currentPrice?.amount ?? 0
        };
    }
}

public class OfferInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal CurrentPrice { get; set; }
    public string Currency { get; set; } = "PLN";
    public DateTime? EndingAt { get; set; }
}

public class BidResult
{
    public bool Success { get; set; }
    public bool HighBidder { get; set; }
    public decimal CurrentPrice { get; set; }
}