Loggbok Prague Parking V1

**Namn:** Jonathan Ahl
**Projekt:** Prague Parking V1 (Inlämningsuppgift 1)
**Period:** 2026-09-23 – 2026-11-20
**Repo:** https://github.com/jonathan-ahl/PragueParkingV1

---

## 2026-11-09
**Tid:** ca 1 h

**Gjort**
- Läste igenom uppgiftsbeskrivningen två gånger och strök under alla "skall"-krav.
- Gjorde en checklista över G-kraven: parkera, flytta, ta bort, söka, textmeny, regnr max 10 tecken, 100 platser som `string[]`, platser 1–100 för personalen, README och loggbok.

**Tankar**
- Det svåraste verkar bli att två MC ska få plats i samma sträng. Tipset om `CAR#ABC123` och `MC#A|MC#B` med `Split`/`Join` verkar vara vägen att gå.

**Nästa steg:** Sätta upp repo och projekt.

---

## 2026-11-09 – Meny och programflöde
**Tid:** ca 2 h

**Gjort**
- Byggde menyn med en `while`-loop och en `switch` (1 Parkera, 2 Flytta, 3 Hämta ut, 4 Sök, 5 Visa, 0 Avsluta).
- Lade in `Console.Clear()` och "Tryck på valfri tangent" så att skärmen inte blir rörig.

**Problem och lösning**
- *Problem:* Skrev man " 1" (med mellanslag) fick man "Ogiltigt val".
- *Lösning:* Kör `.Trim()` på inmatningen innan `switch`.

**Nästa steg:** Själva parkeringslogiken. Lektionen om metoder i kursinfon gav idén att dela upp i många små metoder.

---

## 2026-11-10 – Parkera fordon
**Tid:** ca 3 h

**Gjort**
- Skapade `string[] parkingGarage = new string[100];`
- Skrev metoderna `SkapaFordon(typ, regnr)` → `"CAR#ABC123"` och `HittaLedigPlats(typ)` som returnerar index eller `-1`.
- Programmet säger nu "Kör bil ABC123 till plats 1".

**Problem och lösning**
- *Problem:* Första bilen hamnade på "plats 0".
- *Orsak:* Jag skrev ut array-index direkt.
- *Lösning:* Bestämde regeln: **all in- och utmatning är platsnummer (1–100), all kod internt använder index (0–99).** Skriver `index + 1` vid utskrift och `platsnummer - 1` vid inläsning, bara på de två ställena.

---

## 2026-11-10 – Två MC på samma plats
**Tid:** ca 2,5 h

**Gjort**
- `LaggTillFordon` använder `string.Join('|', ...)` när det redan står en MC i rutan.
- `HittaLedigPlats` letar i första hand efter en ruta med *en ensam MC* när fordonet är en MC, så att hela rutor sparas åt bilar.
- Hjälpmetoder: `HamtaFordonPaPlats(index)` (Split på `|`), `HamtaTyp` och `HamtaRegnr` (Split på `#`).

**Problem och lösning**
- *Problem:* `IndexOutOfRangeException` i `HamtaRegnr` när rutan var tom.
- *Orsak:* `null.Split(...)` eller tom sträng gav en array med fel längd.
- *Lösning:* `HamtaFordonPaPlats` returnerar en tom array om rutan är `null`/tom, och alla andra metoder går via den.

---

## 2026-11-10 – Söka och hämta ut
**Tid:** ca 2 h

**Gjort**
- `HittaFordon(regnr, out int index, out string fordon)` – returnerar `bool` och skickar tillbaka plats och fordon via `out`, precis som `TryParse`.
- Hämta ut: `TaBortFordon(index, regnr)`.

**Problem och lösning**
- *Problem:* När jag hämtade ut en av två MC försvann båda.
- *Orsak:* Jag satte hela rutan till `null`.
- *Lösning:* `TaBortFordon` går igenom fordonen i rutan och behåller de som har ett annat regnr. Blir inget kvar sätts rutan till `null`.
- *Problem 2:* Sökning på "abc123" hittade inte "ABC123".
- *Lösning:* Gör om alla regnr till versaler med `ToUpper()` direkt vid inläsning.

---

## 2026-11-13 – Flytta fordon
**Tid:** ca 2 h

**Gjort**
- Flytta: sök upp fordonet, fråga efter ny plats, kontrollera med `RymsPaPlats(index, typ)` och flytta.

**Problem och lösning**
- *Problem:* Man kunde flytta en bil till en plats där det stod en MC – rutan blev `MC#X|CAR#Y`.
- *Lösning:* `RymsPaPlats`: tom ruta tar allt, en ensam MC tar bara en MC till, allt annat nej.
- *Problem 2:* Flyttade man ett fordon till samma plats som det redan stod på försvann det.
- *Lösning:* Kontroll `if (tillIndex == franIndex)` innan något tas bort.

---

## 2026-11-14 – Inmatningskontroller
**Tid:** ca 2 h

**Gjort**
- `LasPlatsnummer` frågar igen tills man skrivit ett heltal 1–100 (`int.TryParse`).
- `ArGiltigtRegnr`: 1–10 tecken, bara bokstäver och siffror (`char.IsLetterOrDigit`).
- Samma regnr kan inte parkeras två gånger.

**Problem och lösning**
- *Problem:* Först spärrade jag bara `#` och `|`, men då gick t.ex. "AB-12!" igenom.
- *Lösning:* Bytte till "bara bokstäver och siffror" – då kan skiljetecknen aldrig förekomma och Å/Ä/Ö/Ü räknas ändå som bokstäver.
- *Problem 2:* Å, Ä och Ö visades som frågetecken.
- *Lösning:* `Console.OutputEncoding = Encoding.UTF8;` och `Console.InputEncoding = Encoding.UTF8;` i början av `Main`.

---

## 2026-11-20 – Test och städning
**Tid:** ca 2 h

**Gjort**
- Tog bort bortkommenterad gammal kod och döpte om metoder så att namnet säger vad de gör.


---

## 2026-11-20 – Körning på annan dator
**Tid:** ca 1 h

**Problem och lösning**
- *Problem:* En kompis fick filerna och fick felet att `.sln`-filen inte kunde öppnas.
- *Orsak:* Jag hade flyttat projektet till en ny mapp men den gamla `.sln`-filen pekade på fel sökväg.
- *Lösning:* Skapade en ny lösningsfil (`PragueParkingV1.slnx`), klonade repot till en tom mapp på min egen dator och testade F5 därifrån. Fungerade.

**Gjort**
- Skrev `README.md`: hur man startar (öppna `.slnx`, tryck F5), vilken .NET-version som behövs och hur menyn fungerar.

---

## 2026-11-20 – Repo
**Tid:** ca 1,5 h

**Gjort**
- Skapade ett publikt repo på GitHub.

**Problem och lösning**
- *Problem:* Första pushen innehöll mapparna `bin/`, `obj/` och `.vs/` – flera hundra filer.
- *Orsak:* Jag skapade repot utan `.gitignore`.
- *Lösning:* Lade till en `.gitignore` för Visual Studio, tog bort mapparna ur Git med `git rm -r --cached .` och committade igen.

**Lärdom:** Lägg till `.gitignore` *innan* första commit.

## 2026-11-20 (fredag) – Inlämning
**Tid:** ca 0,5 h

**Gjort**
- Sista genomläsning av kraven mot checklistan från 23/9 – alla G-krav är bockade.
- Kontrollerade att repot är publikt och lämnade in länken till repot (inte en fil) samt den här loggboken.

---



---

## Sammanfattning
- **Total tid:** ca 20 h
- **Svårast:** Att hålla isär platsnummer och index, och att ta bort en av två MC ur samma sträng.
- **Viktigaste lärdomen:** Små metoder som gör en sak var mycket lättare att testa och felsöka. Att lagra flera värden i en sträng fungerar men blir snabbt krångligt – i V2 skulle klasser (`Vehicle`, `Car`, `MC`) göra det enklare.
