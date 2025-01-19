using System;
using System.Collections.Generic;
using System.Linq;

namespace Lórum_Winform
{
    internal class Computer
    {
        private readonly List<Hezag> hezagok = new List<Hezag>();
        private readonly Random rnd = new Random();
        private int laptávolság1;
        private int laptávolság2;
        private int laptávolság3;
        private int laptávolság4;
        private int lépés;
        private byte lórumcard;
        private byte lorumpiros, lorumzöld, lorummakk, lorumtök;
        private bool Loselórum;
        private int Találat;

        public Computer(byte jatekos)
        {
            Jatekos = jatekos;
            Pontszam = 75;
        }

        public List<byte> PlayerCardIds { get; } = new List<byte>();

        public int JátékosKártyákSzáma { get; set; } = 8;

        public byte Jatekos { get; }

        public int Pontszam { get; set; }

        public static Computer Create(byte jatekos)
        {
            return new Computer(jatekos);
        }

        /// <summary>
        ///     Játékos előkészítése a játékhoz
        /// </summary>
        public void init()
        {
            JátékosKártyákSzáma = 8;
            lorumpiros = 0;
            lorumzöld = 0;
            lorummakk = 0;
            lorumtök = 0;
        }

        internal void GépJáték()
        {
            if (Jatekos == 1)
                Program.Mainform.main.Tracer(Jatekos + " játékos segítséget kért");
            else
                Program.Mainform.main.Tracer(Jatekos + " játékos lép");
            var jelöltMakk = 0;
            var jelöltPiros = 0;
            var jelöltTök = 0;
            var jelöltZöld = 0;
            byte pirosak = 0;
            byte zöldek = 0;
            byte makkok = 0;
            byte tökök = 0;
            byte result = 0;
            if (PlayerCardIds.TrueForAll(x => !Program.Mainform.main.KöverkezőLap(x))) // lehet-e kitenni lapot
                return;
            for (byte n = 0; n < PlayerCardIds.Count; n++)
            {
                if (PlayerCardIds[n] <= 8) pirosak++;
                if (PlayerCardIds[n] >= 9 && PlayerCardIds[n] <= 16) zöldek++;
                if (PlayerCardIds[n] >= 17 && PlayerCardIds[n] <= 24) makkok++;
                if (PlayerCardIds[n] >= 25) tökök++;
            }

            Program.Mainform.main.Tracer(Jatekos + " játékosnak van " + pirosak + " pirosa van");
            Program.Mainform.main.Tracer(Jatekos + " játékosnak van " + zöldek + " zöld van");
            Program.Mainform.main.Tracer(Jatekos + " játékosnak van " + makkok + " makk van");
            Program.Mainform.main.Tracer(Jatekos + " játékosnak van " + tökök + " tök van");
            if (Program.Mainform.main.Piros == 0 && Program.Mainform.main.Zöld == 0 &&
                Program.Mainform.main.Makk == 0 &&
                Program.Mainform.main.Tök == 0) //ha a játékos kezd
            {
//megvizsgáljam hogy lehet-e lórum
                if (pirosak == 1) LorumEszleles(jelöltPiros);
                if (zöldek == 1) LorumEszleles(jelöltZöld);
                if (makkok == 1) LorumEszleles(jelöltMakk);
                if (tökök == 1) LorumEszleles(jelöltTök);
                result = Kezdés(pirosak, zöldek, makkok, tökök);
            }
            else
            {
                result = Játék(pirosak, zöldek, makkok, tökök);
            }

            Program.Mainform.main.Tracer(string.Format("{0} játékos kifogja tenni a {1} lapot.", Jatekos,
                Main.KártyaNév(result)));
            Program.Mainform.main.Tracer(Jatekos + " játékos kiteszi a " + Main.KártyaNév(result) +
                                         " lapot");
            if (result <= 8)
            {
                Program.Mainform.main.LórumPiros = false;
                Program.Mainform.main.kitettPiros++;
            }

            if (result >= 9 && result <= 16)
            {
                Program.Mainform.main.LórumZöld = false;
                Program.Mainform.main.kitettZöld++;
            }

            if (result >= 17 && result <= 24)
            {
                Program.Mainform.main.LórumMakk = false;
                Program.Mainform.main.kitettMakk++;
            }

            if (result >= 25)
            {
                Program.Mainform.main.LórumTök = false;
                Program.Mainform.main.kitettTök++;
            }

            PlayerCardIds.Remove(result); //a kitett kártya eltávolítása
            Program.Mainform.main.Tracer(Jatekos + " játékos kiteszi a " + Main.KártyaNév(result) +
                                         " lapot");
            Program.Mainform.main.Kiteszkartyat(result, Jatekos);
        }

        /// <summary>
        ///     Észleli, hogy lehet-e Lórum a játékosnak
        /// </summary>
        /// <param name="alany"></param>
        /// Vizsgált lap sorszáma
        public void LorumEszleles(int alany)
        {
            if (alany <= 8) lorumpiros = PlayerCardIds.Find(x => x == x + 8 || x == x + 16 || x == x + 24);
            if (alany >= 9 && alany <= 16) lorumzöld = PlayerCardIds.Find(x => x == x - 8 || x == x + 8 || x == x + 16);
            if (alany >= 17 && alany <= 24)
                lorummakk = PlayerCardIds.Find(x => x == x - 16 || x == x + 8 || x == x + 16);
            if (alany >= 25) lorumtök = PlayerCardIds.Find(x => x == x - 24 || x == x - 16 || x == x - 8);
        }

        /// <summary>
        ///     A számítógép megteszi a kezdő lépést
        /// </summary>
        /// <param name="pirosak"></param>
        /// Rendelkezésre álló pirosak
        /// <param name="zöldek"></param>
        /// Rendelkezésre álló zöldek
        /// <param name="makkok"></param>
        /// Rendelkezésre álló makkok
        /// <param name="tökök"></param>
        /// Rendelkezésre álló tökök
        /// <returns></returns>
        private byte Kezdés(byte pirosak, byte zöldek, byte makkok, byte tökök)
        {
            hezagok.Clear();
            HezagokSzamitasKezdes(pirosak, zöldek, makkok, tökök, lorumpiros);
            hezagok.Add(new Hezag((byte)(laptávolság1 + laptávolság2 + laptávolság3 + laptávolság4), lorumpiros));
            HezagokSzamitasKezdes(pirosak, zöldek, makkok, tökök, lorumzöld);
            hezagok.Add(new Hezag((byte)(laptávolság1 + laptávolság2 + laptávolság3 + laptávolság4), lorumzöld));
            HezagokSzamitasKezdes(pirosak, zöldek, makkok, tökök, lorummakk);
            hezagok.Add(new Hezag((byte)(laptávolság1 + laptávolság2 + laptávolság3 + laptávolság4), lorummakk));
            HezagokSzamitasKezdes(pirosak, zöldek, makkok, tökök, lorumtök);
            hezagok.Add(new Hezag((byte)(laptávolság1 + laptávolság2 + laptávolság3 + laptávolság4), lorumtök));
            if (hezagok.Count != 0)
            {
                var min = hezagok.Min(x => x.Ertek);
                if (((min == hezagok[0].Ertek) & (zöldek == 1)) | (makkok == 1) | (tökök == 1) && lorumpiros != 0)
                    return lorumpiros;
                if (((min == hezagok[1].Ertek) & (pirosak == 1)) | (makkok == 1) | (tökök == 1) && lorumzöld != 0)
                    return lorumzöld;
                if (((min == hezagok[2].Ertek) & (zöldek == 1)) | (pirosak == 1) | (tökök == 1) && lorummakk != 0)
                    return lorummakk;
                if (((min == hezagok[3].Ertek) & (zöldek == 1)) | (makkok == 1) | (pirosak == 1) && lorumtök != 0)
                    return lorumtök;
            }

            lórumcard = 0;
            for (byte i = 0; i < PlayerCardIds.Count; i++)
            {
                HezagokSzamitasKezdes(pirosak, zöldek, makkok, tökök, PlayerCardIds[i]);
                hezagok.Add(new Hezag((byte)(laptávolság1 + laptávolság2 + laptávolság3 + laptávolság4),
                    PlayerCardIds[i]));
            }

            for (byte i = 0; i < hezagok.Count; i++)
                if (hezagok[i].Ertek != hezagok.Min(x => x.Ertek))
                    hezagok.RemoveAt(i);

            return hezagok[rnd.Next(hezagok.Count)].Kartya;
        }

        private byte Játék(byte pirosak, byte zöldek, byte makkok, byte tökök)
        {
            hezagok.Clear();
            Loselórum = false;
            byte jelöltPiros = 0, jelöltMakk = 0, jelöltZöld = 0, jelöltTök = 0;
            for (byte n = 0; n < PlayerCardIds.Count; n++)
            {
                if (PlayerCardIds[n] <= 8)
                    jelöltPiros = Program.Mainform.main.KöverkezőLap(PlayerCardIds[n]) ? PlayerCardIds[n] : jelöltPiros;
                if (PlayerCardIds[n] >= 9 && PlayerCardIds[n] <= 16)
                    jelöltZöld = Program.Mainform.main.KöverkezőLap(PlayerCardIds[n]) ? PlayerCardIds[n] : jelöltZöld;
                if (PlayerCardIds[n] >= 17 && PlayerCardIds[n] <= 24)
                    jelöltMakk = Program.Mainform.main.KöverkezőLap(PlayerCardIds[n]) ? PlayerCardIds[n] : jelöltMakk;
                if (PlayerCardIds[n] >= 25)
                    jelöltTök = Program.Mainform.main.KöverkezőLap(PlayerCardIds[n]) ? PlayerCardIds[n] : jelöltTök;
            }

            if ((jelöltPiros == lórumcard) & (jelöltZöld == 0) & (jelöltMakk == 0) & (jelöltTök == 0)) Loselórum = true;
            if ((jelöltZöld == lórumcard) & (jelöltPiros == 0) & (jelöltMakk == 0) & (jelöltTök == 0)) Loselórum = true;
            if ((jelöltMakk == lórumcard) & (jelöltZöld == 0) & (jelöltPiros == 0) & (jelöltTök == 0)) Loselórum = true;
            if ((jelöltTök == lórumcard) & (jelöltZöld == 0) & (jelöltMakk == 0) & (jelöltPiros == 0)) Loselórum = true;
            if (jelöltPiros != 0)
            {
                Hézag(pirosak, zöldek, makkok, tökök, jelöltPiros);
                hezagok.Add(new Hezag((byte)laptávolság1, jelöltPiros));
            }

            if (jelöltZöld != 0)
            {
                Hézag(pirosak, zöldek, makkok, tökök, jelöltZöld);
                hezagok.Add(new Hezag((byte)laptávolság1, jelöltZöld));
            }

            if (jelöltMakk != 0)
            {
                Hézag(pirosak, zöldek, makkok, tökök, jelöltMakk);
                hezagok.Add(new Hezag((byte)laptávolság1, jelöltMakk));
            }

            if (jelöltTök != 0)
            {
                Hézag(pirosak, zöldek, makkok, tökök, jelöltTök);
                hezagok.Add(new Hezag((byte)laptávolság1, jelöltTök));
            }

            for (byte i = 0; i < hezagok.Count; i++)
                if (hezagok[i].Ertek != hezagok.Select(x => x.Ertek).Max())
                    hezagok.RemoveAt(i);

            return Loselórum ? lórumcard : hezagok[rnd.Next(hezagok.Count)].Kartya;
        }

        /// <summary>
        ///     Hézag számítás a kezdéshez
        /// </summary>
        /// <param name="pirosak"></param>
        /// <param name="zöldek"></param>
        /// <param name="makkok"></param>
        /// <param name="tökök"></param>
        /// <param name="alany"></param>
        public void HezagokSzamitasKezdes(int pirosak, int zöldek, int makkok, int tökök, int alany)
        {
            laptávolság1 = 0;
            laptávolság2 = 0;
            laptávolság3 = 0;
            laptávolság4 = 0;
            if (alany == 0) return;
            //Lap távolságok vizsgálat
            //piros lapok
            if ((alany == 1) | (alany == 2) | (alany == 3) | (alany == 4) | (alany == 5) | (alany == 6) | (alany == 7) |
                (alany == 8))
            {
                Találat = 0;
                lépés = alany;
                if (pirosak > 1)
                    while (Találat != pirosak)
                    {
                        lépés++;
                        if (lépés == 9) lépés = 1;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany + 8;
                if (zöldek > 0)
                    while (Találat != zöldek)
                    {
                        lépés++;
                        if (lépés == 17) lépés = 9;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság2++;
                        }
                        else
                        {
                            laptávolság2 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany + 16;
                if (makkok > 0)
                    while (Találat != makkok)
                    {
                        lépés++;
                        if (lépés == 25) lépés = 17;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság3++;
                        }
                        else
                        {
                            laptávolság3 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany + 24;
                if (tökök > 0)
                    while (Találat != tökök)
                    {
                        lépés++;
                        if (lépés == 33) lépés = 25;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság4++;
                        }
                        else
                        {
                            laptávolság4 += 2;
                        }
                    }

                return;
            }

            //zöld lapok
            if ((alany == 9) | (alany == 10) | (alany == 11) | (alany == 12) | (alany == 13) | (alany == 14) |
                (alany == 15) |
                (alany == 16))
            {
                Találat = 0;
                lépés = alany - 8;
                if (pirosak > 0)
                    while (Találat != pirosak)
                    {
                        lépés++;
                        if (lépés == 9) lépés = 1;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany;
                if (zöldek > 1)
                    while (Találat != zöldek)
                    {
                        lépés++;
                        if (lépés == 17) lépés = 9;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság2++;
                        }
                        else
                        {
                            laptávolság2 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany + 8;
                if (makkok > 0)
                    while (Találat != makkok)
                    {
                        lépés++;
                        if (lépés == 25) lépés = 17;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság3++;
                        }
                        else
                        {
                            laptávolság3 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany + 16;
                if (tökök > 0)
                    while (Találat != tökök)
                    {
                        lépés++;
                        if (lépés == 33) lépés = 25;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság4++;
                        }
                        else
                        {
                            laptávolság4 += 2;
                        }
                    }

                return;
            }

            //mak lapok
            if ((alany == 17) | (alany == 18) | (alany == 19) | (alany == 20) | (alany == 21) | (alany == 22) |
                (alany == 23) |
                (alany == 24))
            {
                Találat = 0;
                lépés = alany - 16;
                if (pirosak > 0)
                    while (Találat != pirosak)
                    {
                        lépés++;
                        if (lépés == 9) lépés = 1;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany - 8;
                if (zöldek > 0)
                    while (Találat != zöldek)
                    {
                        lépés++;
                        if (lépés == 17) lépés = 9;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság2++;
                        }
                        else
                        {
                            laptávolság2 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany;
                if (makkok > 1)
                    while (Találat != makkok)
                    {
                        lépés++;
                        if (lépés == 25) lépés = 17;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság3++;
                        }
                        else
                        {
                            laptávolság3 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany + 8;
                if (tökök > 0)
                    while (Találat != tökök)
                    {
                        lépés++;
                        if (lépés == 33) lépés = 25;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság4++;
                        }
                        else
                        {
                            laptávolság4 += 2;
                        }
                    }

                return;
            }

            //tök lapok
            if ((alany == 25) | (alany == 26) | (alany == 27) | (alany == 28) | (alany == 29) | (alany == 30) |
                (alany == 31) |
                (alany == 32))
            {
                Találat = 0;
                lépés = alany - 24;
                if (pirosak > 0)
                    while (Találat != pirosak)
                    {
                        lépés++;
                        if (lépés == 9) lépés = 1;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany - 16;
                if (zöldek > 0)
                    while (Találat != zöldek)
                    {
                        lépés++;
                        if (lépés == 17) lépés = 9;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság2++;
                        }
                        else
                        {
                            laptávolság2 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany - 8;
                if (makkok > 0)
                    while (Találat != makkok)
                    {
                        lépés++;
                        if (lépés == 25) lépés = 17;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság3++;
                        }
                        else
                        {
                            laptávolság3 += 2;
                        }
                    }

                Találat = 0;
                lépés = alany;
                if (tökök > 1)
                    while (Találat != tökök)
                    {
                        lépés++;
                        if (lépés == 33) lépés = 25;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság4++;
                        }
                        else
                        {
                            laptávolság4 += 2;
                        }
                    }
            }

            Program.Mainform.main.Tracer(string.Format("laptávoság1={0}", laptávolság1));
            Program.Mainform.main.Tracer(string.Format("laptávoság2={0}", laptávolság2));
            Program.Mainform.main.Tracer(string.Format("laptávoság3={0}", laptávolság3));
            Program.Mainform.main.Tracer(string.Format("laptávoság4={0}", laptávolság4));
        }

        /// <summary>
        ///     Hézag számítás játékközben
        /// </summary>
        /// <param name="pirosak"></param>
        /// <param name="zöldek"></param>
        /// <param name="makkok"></param>
        /// <param name="tökök"></param>
        /// <param name="alany"></param>
        private void Hézag(int pirosak, int zöldek, int makkok, int tökök, int alany)
        {
            laptávolság1 = 0;
            if (alany == 0) return;
            if ((alany == 1) | (alany == 2) | (alany == 3) | (alany == 4) | (alany == 5) | (alany == 6) | (alany == 7) |
                (alany == 8))
            {
                if (pirosak == 1)
                {
                    laptávolság1 = 0;
                    return;
                }

                Találat = 0;
                lépés = Program.Mainform.main.Piros;
                if (pirosak > 1)
                    while (Találat != pirosak)
                    {
                        lépés++;
                        if (lépés == 9) lépés = 1;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                return;
            }

            if ((alany == 9) | (alany == 10) | (alany == 11) | (alany == 12) | (alany == 13) | (alany == 14) |
                (alany == 15) |
                (alany == 16))
            {
                if (zöldek == 1)
                {
                    laptávolság1 = 0;
                    return;
                }

                Találat = 0;
                lépés = Program.Mainform.main.Zöld;
                if (zöldek > 1)
                    while (Találat != zöldek)
                    {
                        lépés++;
                        if (lépés == 17) lépés = 9;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                return;
            }

            if ((alany == 17) | (alany == 18) | (alany == 19) | (alany == 20) | (alany == 21) | (alany == 22) |
                (alany == 23) |
                (alany == 24))
            {
                if (makkok == 1)
                {
                    laptávolság1 = 0;
                    return;
                }

                Találat = 0;
                lépés = Program.Mainform.main.Makk;
                if (makkok > 1)
                    while (Találat != makkok)
                    {
                        lépés++;
                        if (lépés == 25) lépés = 17;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }

                return;
            }

            if ((alany == 25) | (alany == 26) | (alany == 27) | (alany == 28) | (alany == 29) | (alany == 30) |
                (alany == 31) |
                (alany == 32))
            {
                if (tökök == 1)
                {
                    laptávolság1 = 0;
                    return;
                }

                Találat = 0;
                lépés = Program.Mainform.main.Tök;
                if (tökök > 1)
                    while (Találat != tökök)
                    {
                        lépés++;
                        if (lépés == 33) lépés = 25;
                        if (PlayerCardIds.Contains((byte)lépés))
                        {
                            Találat++;
                            laptávolság1++;
                        }
                        else
                        {
                            laptávolság1 += 2;
                        }
                    }
            }

            Program.Mainform.main.Tracer(string.Format("laptávoság1={0}", laptávolság1));
        }
    }

    internal class Hezag
    {
        public Hezag(byte ertek, byte kartya)
        {
            Ertek = ertek;
            Kartya = kartya;
        }

        public byte Ertek { get; }
        public byte Kartya { get; }
    }
}