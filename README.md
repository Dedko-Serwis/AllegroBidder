# AllegroBidder

Lokalna aplikacja WPF do składania ofert w licytacjach na Allegro
z wykorzystaniem oficjalnego REST API Allegro.

## Opis

Aplikacja umożliwia:
- pobranie informacji o aukcji po jej ID (nazwa, aktualna cena, data zakończenia),
- złożenie oferty kupna w licytacji.

## Wymagania

- .NET 8.0
- Konto deweloperskie na https://developer.allegro.pl

## Bezpieczeństwo

Klucze API (ClientId, ClientSecret, AccessToken) przechowywane są
w pliku `Secrets.cs`, który **nie jest** publikowany w repozytorium.

## Autor
Leszek Dedko