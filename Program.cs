using System;
using System.Collections.Generic;
using System.Threading;

namespace KjkTuzhegy
{
    class Karakter
    {
        public string Nev { get; set; }
        public int Ugyesseg { get; set; }
        public int Eletero { get; set; }
        public int KezdetiEletero { get; set; }
        public int Szerencse { get; set; }

        public Karakter(string nev, int ugyesseg, int eletero, int szerencse)
        {
            Nev = nev;
            Ugyesseg = ugyesseg;
            Eletero = eletero;
            KezdetiEletero = eletero;
            Szerencse = szerencse;
        }
    }

    class Szorny
    {
        public string Nev { get; set; }
        public int Ugyesseg { get; set; }
        public int Eletero { get; set; }

        public Szorny(string nev, int ugyesseg, int eletero)
        {
            Nev = nev;
            Ugyesseg = ugyesseg;
            Eletero = eletero;
        }
    }

    class Valasztas
    {
        public string Szoveg { get; set; }
        public int CelFejezetId { get; set; }

        public Valasztas(string szoveg, int celFejezetId)
        {
            Szoveg = szoveg;
            CelFejezetId = celFejezetId;
        }
    }

    class Fejezet
    {
        public int Id { get; set; }
        public string Szoveg { get; set; }
        public List<Valasztas> Valasztasok { get; set; }
        public Szorny Ellenseg { get; set; }

        public Fejezet(int id, string szoveg, Szorny ellenseg = null)
        {
            Id = id;
            Szoveg = szoveg;
            Ellenseg = ellenseg;
            Valasztasok = new List<Valasztas>();
        }
    }

    class Program
    {
        static Random rnd = new Random();
        static Karakter jatekos;
        static Dictionary<int, Fejezet> konyv = new Dictionary<int, Fejezet>();

        static void Main(string[] args)
        {
            Console.Title = "A Tűzhegy Varázslója - KJK Motor";
            KarakterGeneralas();
            KonyvBetoltese();
            JatekCiklus();
        }

        static void KarakterGeneralas()
        {
            Console.WriteLine("=== KARAKTER GENERÁLÁS ===");
            Console.Write("Add meg a hősöd nevét: ");
            string nev = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nev)) nev = "Ismeretlen Hős";

            int ug = rnd.Next(1, 7) + 6;
            int el = rnd.Next(1, 7) + rnd.Next(1, 7) + 12;
            int sz = rnd.Next(1, 7) + 6;

            jatekos = new Karakter(nev, ug, el, sz);

            Console.Clear();
            KalandlapMegjelenites();
            Console.WriteLine("\nNyomj Entert a kaland megkezdéséhez...");
            Console.ReadLine();
        }

        static void KalandlapMegjelenites()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--- KALANDLAP: " + jatekos.Nev + " ---");
            Console.WriteLine("ÜGYESSÉG: " + jatekos.Ugyesseg + "  |  ÉLETERŐ: " + jatekos.Eletero + " / " + jatekos.KezdetiEletero + "  |  SZERENCSE: " + jatekos.Szerencse);
            Console.ResetColor();
        }

        static int Kockadobas(int darab = 1)
        {
            int osszeg = 1;
            for (int i = 0; i < darab; i++)
            {
                osszeg += rnd.Next(1, 7);
            }
            return osszeg;
        }

        static bool SzerencseProba()
        {
            if (jatekos.Szerencse <= 0)
            {
                Console.WriteLine("Nincs elég Szerencse pontod a próbához!");
                return false;
            }

            int dobas = Kockadobas(2);
            Console.WriteLine("Szerencse-próba dobás: " + dobas + " (Jelenlegi szerencséd: " + jatekos.Szerencse + ")");

            bool sikeres = dobas <= jatekos.Szerencse;
            jatekos.Szerencse--;

            if (sikeres)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Szerencséd volt!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Balszerencséd volt!");
            }
            Console.ResetColor();
            return sikeres;
        }

        static void Harc(Szorny szorny)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("=== CSATA: " + szorny.Nev + " ===");
            Console.ResetColor();
            Console.WriteLine("Szörny tulajdonságai -> ÜGYESSÉG: " + szorny.Ugyesseg + ", ÉLETERŐ: " + szorny.Eletero + "\n");

            int kor = 1;
            while (jatekos.Eletero > 0 && szorny.Eletero > 0)
            {
                Console.WriteLine("--- " + kor + ". Forduló ---");

                int jatekosDobas = Kockadobas(2);
                int jatekosTamadoero = jatekosDobas + jatekos.Ugyesseg;
                Console.WriteLine(jatekos.Nev + " dobása: " + jatekosDobas + " + " + jatekos.Ugyesseg + " = Támadóerő: " + jatekosTamadoero);

                int szornyDobas = Kockadobas(2);
                int szornyTamadoero = szornyDobas + szorny.Ugyesseg;
                Console.WriteLine(szorny.Nev + " dobása: " + szornyDobas + " + " + szorny.Ugyesseg + " = Támadóerő: " + szornyTamadoero);

                if (jatekosTamadoero > szornyTamadoero)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Megsebezted a következőt: " + szorny.Nev + "!");
                    Console.ResetColor();

                    Console.Write("Tedd próbára a SZERENCSÉDET a nagyobb sebzésért? (i/n): ");
                    string valasz = Console.ReadLine().ToLower();

                    int sebzes = 2;
                    if (valasz == "i" || valasz == "y")
                    {
                        if (SzerencseProba()) sebzes = 4;
                        else sebzes = 1;
                    }

                    szorny.Eletero -= sebzes;
                    Console.WriteLine("A szörny " + sebzes + " Életerőt veszített. (Maradt: " + Math.Max(0, szorny.Eletero) + ")");
                }
                else if (szornyTamadoero > jatekosTamadoero)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(szorny.Nev + " megsebzett téged!");
                    Console.ResetColor();

                    Console.Write("Tedd próbára a SZERENCSÉDET a sebzés csökkentéséhez? (i/n): ");
                    string valasz = Console.ReadLine().ToLower();

                    int sebzes = 2;
                    if (valasz == "i" || valasz == "y")
                    {
                        if (SzerencseProba()) sebzes = 1;
                        else sebzes = 3;
                    }

                    jatekos.Eletero -= sebzes;
                    Console.WriteLine("Vesztettél " + sebzes + " Életerőt. (Maradt: " + Math.Max(0, jatekos.Eletero) + ")");
                }
                else
                {
                    Console.WriteLine("Kivédtétek egymás csapását!");
                }

                kor++;
                Console.WriteLine();
                Thread.Sleep(1000);
            }

            if (jatekos.Eletero <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Meghaltál a harcban... A kalandod sajnálatos módon véget ért.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Győzelem! Megölted a következőt: " + szorny.Nev + ".");
                Console.ResetColor();
                Console.WriteLine("Nyomj Entert a folytatáshoz...");
                Console.ReadLine();
            }
        }

        static void KonyvBetoltese()
        {
            Fejezet f1 = new Fejezet(1, "Véget ért a kétnapi gyaloglás... Meggyújtod a lámpásodat, és óvatosan bemerészkedsz a sötétségbe. Néhány méter után egy elágazáshoz érsz.");
            f1.Valasztasok.Add(new Valasztas("Ha nyugat felé indulsz", 71));
            f1.Valasztasok.Add(new Valasztas("Ha kelet felé indulsz", 278));
            konyv.Add(1, f1);

            Fejezet f71 = new Fejezet(71, "Nyugat felé mentél. A folyosó teljesen üres és csendes, a falakról nyirkos víz szivárog. Nincs itt semmi érdekes, jobb lesz visszafordulni.");
            f71.Valasztasok.Add(new Valasztas("Visszatérés az elágazáshoz", 1));
            konyv.Add(71, f71);

            Szorny barbar = new Szorny("Őrült Barbár", 7, 6);
            Fejezet f278 = new Fejezet(278, "Ahogy belépsz a szobába, egy vadembert pillantasz meg, amint nagyokat ugorva feléd rohan, miközben feje felett een hatalmas csatabárdot forgat!", barbar);
            f278.Valasztasok.Add(new Valasztas("Keresztülmész a szobán az északi ajtón át (Vissza az 1. pontra)", 1));
            konyv.Add(278, f278);
        }

        static void JatekCiklus()
        {
            int aktualisId = 1;

            while (jatekos.Eletero > 0)
            {
                Console.Clear();
                KalandlapMegjelenites();
                Console.WriteLine("\n--------------------------------------------------");

                if (!konyv.ContainsKey(aktualisId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Hiba: A(z) " + aktualisId + ". fejezet még nincs kidolgozva a történetben!");
                    Console.ResetColor();
                    break;
                }

                Fejezet aktFejezet = konyv[aktualisId];
                Console.WriteLine("\n[" + aktFejezet.Id + ". PONT]");
                Console.WriteLine(aktFejezet.Szoveg);

                if (aktFejezet.Ellenseg != null && aktFejezet.Ellenseg.Eletero > 0)
                {
                    Console.WriteLine("\nNyomj Entert a harc megkezdéséhez...");
                    Console.ReadLine();
                    Harc(aktFejezet.Ellenseg);

                    if (jatekos.Eletero <= 0) break;
                }

                Console.WriteLine("\nMit teszel?");
                for (int i = 0; i < aktFejezet.Valasztasok.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". -> " + aktFejezet.Valasztasok[i].Szoveg);
                }

                int valasztasIndex = -1;
                while (valasztasIndex < 0 || valasztasIndex >= aktFejezet.Valasztasok.Count)
                {
                    Console.Write("\nVálasztásod száma: ");
                    string bemenet = Console.ReadLine();
                    if (int.TryParse(bemenet, out int szam))
                    {
                        valasztasIndex = szam - 1;
                    }
                }

                aktualisId = aktFejezet.Valasztasok[valasztasIndex].CelFejezetId;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=== JÁTÉK VÉGE ===");
            Console.ResetColor();
            Console.WriteLine("Nyomj egy gombot a kilépéshez...");
            Console.ReadKey();
        }

    }
}
