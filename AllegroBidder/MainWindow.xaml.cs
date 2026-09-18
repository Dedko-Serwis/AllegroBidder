using System.Windows;

namespace AllegroBidder;

public partial class MainWindow : Window
{
    private readonly AllegroClient _client;
    private OfferInfo? _currentOffer;

    public MainWindow()
    {
        InitializeComponent();
        _client = new AllegroClient(new AppSettings());
    }

    private async void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        var offerId = TxtOfferId.Text.Trim();
        if (string.IsNullOrEmpty(offerId)) return;

        BtnLoad.IsEnabled = false;
        TxtStatus.Text = "Pobieranie...";

        try
        {
            _currentOffer = await _client.GetOfferInfoAsync(offerId);
            if (_currentOffer == null)
            {
                TxtStatus.Text = "Nie znaleziono oferty.";
                return;
            }

            TxtOfferName.Text = _currentOffer.Name;
            TxtPrice.Text = $"Aktualna cena: {_currentOffer.CurrentPrice:F2} {_currentOffer.Currency}";
            TxtEnding.Text = _currentOffer.EndingAt.HasValue
                ? $"Data zakończenia: {_currentOffer.EndingAt:yyyy-MM-dd HH:mm:ss}"
                : "Data zakończenia: (brak danych)";

            BtnBid.IsEnabled = true;
            TxtStatus.Text = "Informacje o ofercie pobrane pomyślnie.";
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Błąd: {ex.Message}";
        }
        finally
        {
            BtnLoad.IsEnabled = true;
        }
    }

    private async void BtnBid_Click(object sender, RoutedEventArgs e)
    {
        if (_currentOffer == null) return;

        if (!decimal.TryParse(TxtBidAmount.Text, out var amount) || amount <= 0)
        {
            TxtStatus.Text = "Wprowadź prawidłową kwotę.";
            return;
        }

        BtnBid.IsEnabled = false;
        TxtStatus.Text = "Składanie oferty...";

        try
        {
            var result = await _client.PlaceBidAsync(_currentOffer.Id, amount, _currentOffer.Currency);
            TxtStatus.Text = result.HighBidder
                ? $"Oferta złożona pomyślnie! Obecnie prowadzisz: {result.CurrentPrice:F2} {_currentOffer.Currency}"
                : $"Oferta złożona, ale nie prowadzisz (cena: {result.CurrentPrice:F2}).";
        }
        catch (Exception ex)
        {
            TxtStatus.Text = $"Błąd: {ex.Message}";
        }
        finally
        {
            BtnBid.IsEnabled = true;
        }
    }
}