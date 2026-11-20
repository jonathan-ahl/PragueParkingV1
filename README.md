# Prague Parking V1

Konsolprogram för valet parking vid slottet i Prag. Personalen kan parkera, flytta, hämta ut och söka efter bilar och motorcyklar på en parkering med 100 platser (1–100).

## Köra programmet

1. Öppna `PragueParkingV1.slnx` i **Visual Studio 2026 Community**.
2. Tryck **F5** (eller Ctrl + F5).

Inga andra steg behövs. Projektet använder **.NET 10**, som följer med Visual Studio 2026 (arbetsbelastningen ".NET desktop development").

Utan Visual Studio går det att köra från kommandoraden (kräver .NET 10 SDK):

```
cd PragueParking
dotnet run
```

## Användning

Välj ett menyval med en siffra och tryck Enter:

| Val | Funktion |
|---|---|
| 1 | Parkera fordon – programmet anger vilken plats fordonet ska köras till |
| 2 | Flytta fordon till en annan plats |
| 3 | Hämta ut fordon (tar bort det ur systemet) |
| 4 | Sök fordon på registreringsnummer |
| 5 | Visa alla upptagna platser och antal lediga |
| 0 | Avsluta |

## Regler

- En plats rymmer 1 bil, 1 MC eller 2 MC.
- En MC parkeras i första hand bredvid en annan ensam MC.
- Registreringsnummer: 1–10 bokstäver eller siffror (även Å, Ä, Ö), inga mellanslag.
- P-huset är en array med 100 strängar, t.ex. `CAR#ABC123` eller `MC#KLM789|MC#FTP666`.
- Inget sparas när programmet stängs.
