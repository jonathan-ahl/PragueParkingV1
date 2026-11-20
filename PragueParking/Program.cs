using System;

namespace PragueParking
{
    // Prague Parking V1 – nivå G
    //
    // P-huset är en array med 100 strängar. Varje element är en P-ruta.
    // Index = platsnummer - 1 (personalen ser 1–100, arrayen använder 0–99).
    //
    // Så lagras fordonen i en ruta:
    //   tom ruta  -> null
    //   en bil    -> "CAR#ABC123"
    //   en MC     -> "MC#KLM789"
    //   två MC    -> "MC#KLM789|MC#FTP666"
    //
    // '#' skiljer fordonstyp från registreringsnummer.
    // '|' skiljer två fordon i samma ruta.
    internal class Program
    {
        const int AntalPlatser = 100;
        const char TypSkiljetecken = '#';
        const char FordonSkiljetecken = '|';
        const string Bil = "CAR";
        const string Mc = "MC";

        static string[] parkingGarage = new string[AntalPlatser];

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // så att Å, Ä och Ö visas rätt
            Console.InputEncoding = System.Text.Encoding.UTF8;  // ... och läses in rätt

            bool programmetKors = true;
            while (programmetKors)
            {
                Console.Clear();
                VisaMeny();
                string val = (Console.ReadLine() ?? "").Trim(); // " 1 " räknas som "1"
                Console.WriteLine();

                switch (val)
                {
                    case "1": ParkeraFordonDialog(); break;
                    case "2": FlyttaFordonDialog(); break;
                    case "3": HamtaUtFordonDialog(); break;
                    case "4": SokFordonDialog(); break;
                    case "5": SkrivUtParkeringen(); break;
                    case "0": programmetKors = false; break;
                    default: Console.WriteLine("Ogiltigt val, välj en siffra från menyn."); break;
                }

                if (programmetKors)
                {
                    Console.WriteLine();
                    Console.WriteLine("Tryck på valfri tangent för att återgå till menyn...");
                    Console.ReadKey(true);
                }
            }
        }

        // ---------------------------------------------------------------
        // MENY
        // ---------------------------------------------------------------

        static void VisaMeny()
        {
            Console.WriteLine("===== PRAGUE PARKING =====");
            Console.WriteLine("1. Parkera fordon");
            Console.WriteLine("2. Flytta fordon");
            Console.WriteLine("3. Hämta ut fordon");
            Console.WriteLine("4. Sök fordon");
            Console.WriteLine("5. Visa hela parkeringen");
            Console.WriteLine("0. Avsluta");
            Console.Write("Välj: ");
        }

        // ---------------------------------------------------------------
        // DIALOGER (all kommunikation med användaren sker här)
        // ---------------------------------------------------------------

        static void ParkeraFordonDialog()
        {
            string typ = LasFordonstyp();
            string regnr = LasRegnr();

            if (HittaFordon(regnr, out int befintligPlats, out string befintligtFordon))
            {
                Console.WriteLine($"{regnr} står redan på plats {befintligPlats + 1}.");
                return;
            }

            int index = HittaLedigPlats(typ);
            if (index == -1)
            {
                Console.WriteLine("Tyvärr, det finns ingen ledig plats för det fordonet.");
                return;
            }

            LaggTillFordon(index, SkapaFordon(typ, regnr));
            Console.WriteLine($"Kör {TypTillText(typ)} {regnr} till plats {index + 1}.");
        }

        static void FlyttaFordonDialog()
        {
            string regnr = LasRegnr();

            if (!HittaFordon(regnr, out int franIndex, out string fordon))
            {
                Console.WriteLine($"Hittade inget fordon med registreringsnummer {regnr}.");
                return;
            }

            Console.WriteLine($"{regnr} står på plats {franIndex + 1}.");
            Console.Write("Till vilken plats ska fordonet flyttas? ");
            int tillIndex = LasPlatsnummer();

            if (tillIndex == franIndex)
            {
                Console.WriteLine("Fordonet står redan på den platsen.");
                return;
            }

            string typ = HamtaTyp(fordon);
            if (!RymsPaPlats(tillIndex, typ))
            {
                Console.WriteLine($"Plats {tillIndex + 1} har inte plats för en {TypTillText(typ)}.");
                return;
            }

            TaBortFordon(franIndex, regnr);
            LaggTillFordon(tillIndex, fordon);
            Console.WriteLine($"Flytta {TypTillText(typ)} {regnr} från plats {franIndex + 1} till plats {tillIndex + 1}.");
        }

        static void HamtaUtFordonDialog()
        {
            string regnr = LasRegnr();

            if (!HittaFordon(regnr, out int index, out string fordon))
            {
                Console.WriteLine($"Hittade inget fordon med registreringsnummer {regnr}.");
                return;
            }

            TaBortFordon(index, regnr);
            Console.WriteLine($"Hämta {TypTillText(HamtaTyp(fordon))} {regnr} på plats {index + 1}. Fordonet är nu borttaget ur systemet.");
        }

        static void SokFordonDialog()
        {
            string regnr = LasRegnr();

            if (HittaFordon(regnr, out int index, out string fordon))
            {
                Console.WriteLine($"{TypTillText(HamtaTyp(fordon))} {regnr} står på plats {index + 1}.");
            }
            else
            {
                Console.WriteLine($"Hittade inget fordon med registreringsnummer {regnr}.");
            }
        }

        static void SkrivUtParkeringen()
        {
            int lediga = 0;
            Console.WriteLine("Upptagna platser:");

            for (int i = 0; i < parkingGarage.Length; i++)
            {
                string[] fordonPaPlats = HamtaFordonPaPlats(i);
                if (fordonPaPlats.Length == 0)
                {
                    lediga++;
                    continue;
                }

                Console.Write($"Plats {i + 1}: ");
                foreach (string fordon in fordonPaPlats)
                {
                    Console.Write($"{HamtaTyp(fordon)} {HamtaRegnr(fordon)}   ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Lediga platser: {lediga} av {AntalPlatser}");
        }

        // ---------------------------------------------------------------
        // INLÄSNING – frågar tills användaren skriver något giltigt
        // ---------------------------------------------------------------

        static string LasFordonstyp()
        {
            while (true)
            {
                Console.Write("Fordonstyp (1 = Bil, 2 = MC): ");
                string svar = (Console.ReadLine() ?? "").Trim();
                if (svar == "1") return Bil;
                if (svar == "2") return Mc;
                Console.WriteLine("Skriv 1 för bil eller 2 för MC.");
            }
        }

        static string LasRegnr()
        {
            while (true)
            {
                Console.Write("Registreringsnummer: ");
                string regnr = Console.ReadLine() ?? ""; // ReadLine kan ge null (t.ex. Ctrl+Z)
                regnr = regnr.Trim();     // tar bort mellanslag före och efter
                regnr = regnr.ToUpper();  // abc123 och ABC123 räknas som samma

                if (ArGiltigtRegnr(regnr)) return regnr;

                Console.WriteLine("Ogiltigt registreringsnummer: 1–10 bokstäver eller siffror, inga mellanslag.");
            }
        }

        static int LasPlatsnummer()
        {
            while (true)
            {
                string svar = Console.ReadLine() ?? "";
                if (int.TryParse(svar, out int platsnummer) && platsnummer >= 1 && platsnummer <= AntalPlatser)
                {
                    return platsnummer - 1; // gör om platsnummer (1–100) till index (0–99)
                }
                Console.Write($"Skriv ett platsnummer mellan 1 och {AntalPlatser}: ");
            }
        }

        // Bara bokstäver (även Å, Ä, Ö, Ü ...) och siffror, max 10 tecken.
        // Då kan registreringsnumret aldrig innehålla # eller |, som vi använder som skiljetecken.
        static bool ArGiltigtRegnr(string regnr)
        {
            if (regnr.Length < 1 || regnr.Length > 10)
            {
                return false;
            }

            foreach (char tecken in regnr)
            {
                if (!char.IsLetterOrDigit(tecken))
                {
                    return false;
                }
            }
            return true;
        }

        // ---------------------------------------------------------------
        // LOGIK (inga Console-anrop här – bara data in och svar ut)
        // ---------------------------------------------------------------

        // Söker igenom alla platser efter ett registreringsnummer.
        // Returnerar true om fordonet finns. Platsens index och hela fordonssträngen
        // ("MC#ABC123") skickas tillbaka via out-parametrar, på samma sätt som TryParse.
        static bool HittaFordon(string regnr, out int index, out string fordon)
        {
            for (int i = 0; i < parkingGarage.Length; i++)
            {
                foreach (string f in HamtaFordonPaPlats(i))
                {
                    if (HamtaRegnr(f) == regnr)
                    {
                        index = i;
                        fordon = f;
                        return true;
                    }
                }
            }

            index = -1;
            fordon = "";
            return false;
        }

        // Returnerar index för en ledig plats, eller -1 om ingen finns.
        // En MC fylls i första hand på en plats där det redan står en ensam MC,
        // så att MC dubbelparkeras och fler hela platser lämnas åt bilar.
        static int HittaLedigPlats(string typ)
        {
            if (typ == Mc)
            {
                for (int i = 0; i < parkingGarage.Length; i++)
                {
                    if (ArEnsamMc(i)) return i;
                }
            }

            for (int i = 0; i < parkingGarage.Length; i++)
            {
                if (string.IsNullOrEmpty(parkingGarage[i])) return i;
            }

            return -1;
        }

        // Får ett fordon av den här typen plats i rutan?
        static bool RymsPaPlats(int index, string typ)
        {
            if (string.IsNullOrEmpty(parkingGarage[index])) return true; // tom ruta tar allt
            if (typ == Mc && ArEnsamMc(index)) return true;              // MC får dela med en annan MC
            return false;
        }

        static bool ArEnsamMc(int index)
        {
            string[] fordonPaPlats = HamtaFordonPaPlats(index);
            return fordonPaPlats.Length == 1 && HamtaTyp(fordonPaPlats[0]) == Mc;
        }

        static void LaggTillFordon(int index, string fordon)
        {
            if (string.IsNullOrEmpty(parkingGarage[index]))
            {
                parkingGarage[index] = fordon;
            }
            else
            {
                // Det står redan en MC här: "MC#AAA111" blir "MC#AAA111|MC#BBB222"
                parkingGarage[index] = string.Join(FordonSkiljetecken, parkingGarage[index], fordon);
            }
        }

        static void TaBortFordon(int index, string regnr)
        {
            string[] fordonPaPlats = HamtaFordonPaPlats(index);
            string kvar = "";

            foreach (string f in fordonPaPlats)
            {
                if (HamtaRegnr(f) != regnr)
                {
                    kvar = f; // högst en MC kan bli kvar
                }
            }

            if (kvar == "")
            {
                parkingGarage[index] = null; // rutan är tom igen
            }
            else
            {
                parkingGarage[index] = kvar;
            }
        }

        // ---------------------------------------------------------------
        // HJÄLPMETODER FÖR STRÄNGFORMATET
        // ---------------------------------------------------------------

        // Delar upp en ruta i separata fordon: "MC#A|MC#B" -> { "MC#A", "MC#B" }
        static string[] HamtaFordonPaPlats(int index)
        {
            if (string.IsNullOrEmpty(parkingGarage[index])) return new string[0];
            return parkingGarage[index].Split(FordonSkiljetecken);
        }

        static string SkapaFordon(string typ, string regnr)
        {
            return typ + TypSkiljetecken + regnr; // "CAR" + "#" + "ABC123"
        }

        static string HamtaTyp(string fordon)
        {
            return fordon.Split(TypSkiljetecken)[0];
        }

        static string HamtaRegnr(string fordon)
        {
            return fordon.Split(TypSkiljetecken)[1];
        }

        static string TypTillText(string typ)
        {
            if (typ == Bil)
            {
                return "bil";
            }
            return "MC";
        }
    }
}
