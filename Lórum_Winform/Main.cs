using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Lórum_Winform.Properties;

namespace Lórum_Winform
{
    public class Main
    {
        private const int kezdopont = 75;
        internal const bool TraceEnabled = true;
        internal readonly Random rnd = new Random();
        internal bool Adás_Engedve;
        internal int[] Asztal = new int[5];
        internal int FirstPlayer;
        internal int helpcard;
        internal bool Játék_befejezve;
        internal bool JátékFolyamatban;
        internal int Kezdőjátékos;
        internal int KezdőLap;
        internal int Kitettmakk;
        internal int kitettPiros, kitettZöld, kitettMakk, kitettTök;
        internal int Kitettpiros;
        internal int Kitetttök;
        internal int Kitettzöld;
        internal bool Lórum;
        internal bool LórumMakk;
        internal bool LórumPiros;
        internal bool LórumTök;
        internal bool LórumZöld;
        internal int Makk;
        internal int Pakli = 1;
        internal bool Passz_Engedve;
        internal int Piros;
        internal int SegítségÁr;

        internal int Tök;
        internal bool Új_játék_engedve = true;
        internal int Zöld;
        internal bool LehetKitenni { get; set; }
        internal int UtolsóGyőztes { get; set; }
        internal Computer Player1 { get; } = Computer.Create(1);
        internal Computer Player2 { get; } = Computer.Create(2);
        internal Computer Player3 { get; } = Computer.Create(3);
        internal Computer Player4 { get; } = Computer.Create(4);

        internal List<byte> Jatekoskartyak { get; private set; } = new List<byte>();

        public bool Elkezdve { get; set; }

        /// <summary>
        ///     Induló beállítások megadása
        /// </summary>
        internal void init()
        {
            if (TraceEnabled)
            {
                Trace.Listeners.Add(new TextWriterTraceListener("log.txt"));
                Trace.AutoFlush = true;
            }
        }

        /// <summary>
        ///     Kiíratja a logokat azonos formátumban
        /// </summary>
        /// <param name="message"></param>
        /// Üzenet kiíratáshoz
        internal void Tracer(string message)
        {
            if (TraceEnabled)
                Trace.WriteLine(DateTime.Now + " - " + message);
        }

        /// <summary>
        ///     Kártyák kioszátása az összes játékosnak és sorba is rendezi
        /// </summary>
        internal void KartyaKiosztas()
        {
            Tracer("Kártyák kiosztása a játékosok között.");
            Piros = 0;
            Zöld = 0;
            Makk = 0;
            Tök = 0;
            var kartyak = Enumerable.Range(1, 32).ToList().OrderBy(x => rnd.Next()).ToArray();
            for (var n = 0; n <= 31; n++)
            {
                if (n < 8)
                {
                    Player1.PlayerCardIds.Add((byte)kartyak[n]);
                    Tracer(string.Format("1. játékoshoz került a {0}", KártyaNév(kartyak[n])));
                }

                if (n >= 8 && n < 16)
                {
                    Player2.PlayerCardIds.Add((byte)kartyak[n]);
                    Tracer(string.Format("2. játékoshoz került a {0}", KártyaNév(kartyak[n])));
                }

                if (n >= 16 && n < 24)
                {
                    Player3.PlayerCardIds.Add((byte)kartyak[n]);
                    Tracer(string.Format("3. játékoshoz került a {0}", KártyaNév(kartyak[n])));
                }

                if (n >= 24)
                {
                    Player4.PlayerCardIds.Add((byte)kartyak[n]);
                    Tracer(string.Format("4. játékoshoz került a {0}", KártyaNév(kartyak[n])));
                }
            }

            Player1.PlayerCardIds.Sort();
            Player2.PlayerCardIds.Sort();
            Player3.PlayerCardIds.Sort();
            Player4.PlayerCardIds.Sort();
            Jatekoskartyak = Player1.PlayerCardIds.ToList();
            Tracer("A kártyák kiosztása megtörtént.");
        }

        /// <summary>
        ///     Beállítja a kezdő értékeket a táblán, hogy a következő lap ellenőrző függvény eltudja dönteni, hogy az
        ///     következik-e.
        /// </summary>
        /// <param name="kezdőLap"></param>
        /// A kezdőlap
        internal void KezdésMegálapítás(int kezdőLap)
        {
            switch (kezdőLap)
            {
                case 1:
                    Zöld = 16;
                    Makk = 24;
                    Tök = 32;
                    break;
                case 2:
                    Zöld = 9;
                    Makk = 17;
                    Tök = 25;
                    break;
                case 3:
                    Zöld = 10;
                    Makk = 18;
                    Tök = 26;
                    break;
                case 4:
                    Zöld = 11;
                    Makk = 19;
                    Tök = 27;
                    break;
                case 5:
                    Zöld = 12;
                    Makk = 20;
                    Tök = 28;
                    break;
                case 6:
                    Zöld = 13;
                    Makk = 21;
                    Tök = 29;
                    break;
                case 7:
                    Zöld = 14;
                    Makk = 22;
                    Tök = 30;
                    break;
                case 8:
                    Zöld = 15;
                    Makk = 23;
                    Tök = 31;
                    break;
                case 9:
                    Piros = 8;
                    Makk = 16;
                    Tök = 32;
                    break;
                case 10:
                    Piros = 1;
                    Makk = 17;
                    Tök = 25;
                    break;
                case 11:
                    Piros = 2;
                    Makk = 18;
                    Tök = 26;
                    break;
                case 12:
                    Piros = 3;
                    Makk = 19;
                    Tök = 27;
                    break;
                case 13:
                    Piros = 4;
                    Makk = 20;
                    Tök = 28;
                    break;
                case 14:
                    Piros = 5;
                    Makk = 21;
                    Tök = 29;
                    break;
                case 15:
                    Piros = 6;
                    Makk = 22;
                    Tök = 30;
                    break;
                case 16:
                    Piros = 7;
                    Makk = 23;
                    Tök = 31;
                    break;
                case 17:
                    Piros = 8;
                    Zöld = 16;
                    Tök = 32;
                    break;
                case 18:
                    Piros = 1;
                    Zöld = 9;
                    Tök = 25;
                    break;
                case 19:
                    Piros = 2;
                    Zöld = 10;
                    Tök = 26;
                    break;
                case 20:
                    Piros = 3;
                    Zöld = 11;
                    Tök = 27;
                    break;
                case 21:
                    Piros = 4;
                    Zöld = 12;
                    Tök = 28;
                    break;
                case 22:
                    Piros = 5;
                    Zöld = 13;
                    Tök = 29;
                    break;
                case 23:
                    Piros = 6;
                    Zöld = 14;
                    Tök = 30;
                    break;
                case 24:
                    Piros = 7;
                    Zöld = 15;
                    Tök = 31;
                    break;
                case 25:
                    Piros = 8;
                    Zöld = 16;
                    Makk = 24;
                    break;
                case 26:
                    Piros = 1;
                    Zöld = 9;
                    Makk = 17;
                    break;
                case 27:
                    Piros = 2;
                    Zöld = 10;
                    Makk = 18;
                    break;
                case 28:
                    Piros = 3;
                    Zöld = 11;
                    Makk = 19;
                    break;
                case 29:
                    Piros = 4;
                    Zöld = 12;
                    Makk = 20;
                    break;
                case 30:
                    Piros = 5;
                    Zöld = 13;
                    Makk = 21;
                    break;
                case 31:
                    Piros = 6;
                    Zöld = 14;
                    Makk = 22;
                    break;
                case 32:
                    Piros = 7;
                    Zöld = 15;
                    Makk = 23;
                    break;
            }
        }

        /// <summary>
        ///     Következő lap eldöntése
        /// </summary>
        /// <param name="kártyaszám"></param>
        /// A vizsgált lap
        /// <returns></returns>
        /// Bool az jön vagy nem
        internal bool KöverkezőLap(byte kártyaszám)
        {
            switch (kártyaszám)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    if (Piros == 0) return true;
                    if ((Piros + 1 == kártyaszám) & (kártyaszám != 8)) return true;
                    if ((kártyaszám == 1) & (Piros == 8)) return true;
                    if ((Piros == 7) & (kártyaszám == 8)) return true;
                    break;
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    if (Zöld == 0) return true;
                    if ((Zöld + 1 == kártyaszám) & (kártyaszám != 16)) return true;
                    if ((kártyaszám == 9) & (Zöld == 16)) return true;
                    if ((Zöld == 15) & (kártyaszám == 16)) return true;
                    break;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                    if (Makk == 0) return true;
                    if ((Makk + 1 == kártyaszám) & (kártyaszám != 24)) return true;
                    if ((kártyaszám == 17) & (Makk == 24)) return true;
                    if ((Makk == 23) & (kártyaszám == 24)) return true;
                    break;
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                    if (Tök == 0) return true;
                    if ((Tök + 1 == kártyaszám) & (kártyaszám != 32)) return true;
                    if ((kártyaszám == 25) & (Tök == 32)) return true;
                    if ((Tök == 31) & (kártyaszám == 32)) return true;
                    break;
            }

            Tracer(string.Format("{0} nem a következő lap.", KártyaNév(kártyaszám)));
            return false;
        }

        /// <summary>
        ///     Számok alapján megmondja, hogy mi a kártya neve
        /// </summary>
        /// <param name="cardNumber"></param>
        /// a kártya száma
        /// <returns></returns>
        /// vissza adja a kártya nevét
        public static string KártyaNév(int cardNumber)
        {
            switch (cardNumber)
            {
                case 1:
                    return "Piros Hetes";
                case 2:
                    return "Piros Nyolcas";
                case 3:
                    return "Piros Kilences";
                case 4:
                    return "Piros Tízes";
                case 5:
                    return "Piros Alsó";
                case 6:
                    return "Piros Felső";
                case 7:
                    return "Piros Csikó";
                case 8:
                    return "Piros Ász";
                case 9:
                    return "Zöld Hetes";
                case 10:
                    return "Zöld Nyolcas";
                case 11:
                    return "Zöld Kilences";
                case 12:
                    return "Zöld Tízes";
                case 13:
                    return "Zöld Alsó";
                case 14:
                    return "Zöld Felső";
                case 15:
                    return "Zöld Csikó";
                case 16:
                    return "Zöld Ász";
                case 17:
                    return "Makk Hetes";
                case 18:
                    return "Makk Nyolcas";
                case 19:
                    return "Makk Kilences";
                case 20:
                    return "Makk Tízes";
                case 21:
                    return "Makk Alsó";
                case 22:
                    return "Makk Felső";
                case 23:
                    return "Makk Csikó";
                case 24:
                    return "Makk Ász";
                case 25:
                    return "Tök Hetes";
                case 26:
                    return "Tök Nyolcas";
                case 27:
                    return "Tök Kilences";
                case 28:
                    return "Tök Tízes";
                case 29:
                    return "Tök Alsó";
                case 30:
                    return "Tök Felső";
                case 31:
                    return "Tök Csikó";
                case 32:
                    return "Tök Ász";
                case 0:
                    return "------";
            }

            return null;
        }

        /// <summary>
        ///     Új játék indításának folyamata
        /// </summary>
        internal void Jatek_Inditas()
        {
            if (!Új_játék_engedve)
                return;
            Tracer("Új játék kezdése");
            Program.Mainform.pictureBox13.Visible = true;
            Program.Mainform.pictureBox14.Visible = true;
            Program.Mainform.pictureBox15.Visible = true;
            Program.Mainform.pictureBox16.Visible = true;
            Program.Mainform.pictureBox17.Visible = true;
            Program.Mainform.pictureBox18.Visible = true;
            Program.Mainform.pictureBox19.Visible = true;
            Program.Mainform.pictureBox20.Visible = true;
            Program.Mainform.pictureBox29.Visible = true;
            Program.Mainform.pictureBox30.Visible = true;
            Program.Mainform.pictureBox31.Visible = true;
            Program.Mainform.pictureBox32.Visible = true;
            Program.Mainform.pictureBox33.Visible = true;
            Program.Mainform.pictureBox34.Visible = true;
            Program.Mainform.pictureBox35.Visible = true;
            Program.Mainform.pictureBox36.Visible = true;
            Program.Mainform.pictureBox21.Visible = true;
            Program.Mainform.pictureBox22.Visible = true;
            Program.Mainform.pictureBox23.Visible = true;
            Program.Mainform.pictureBox24.Visible = true;
            Program.Mainform.pictureBox25.Visible = true;
            Program.Mainform.pictureBox26.Visible = true;
            Program.Mainform.pictureBox27.Visible = true;
            Program.Mainform.pictureBox28.Visible = true;
            Elkezdve = false;
            LórumPiros = true;
            LórumZöld = true;
            LórumMakk = true;
            LórumTök = true;
            kitettPiros = 0;
            kitettZöld = 0;
            Kitettmakk = 0;
            kitettTök = 0;
            Player2.init();
            Player3.init();
            Player4.init();
            Program.Mainform.kartya1.Visible = true;
            Program.Mainform.kartya2.Visible = true;
            Program.Mainform.kartya3.Visible = true;
            Program.Mainform.kartya4.Visible = true;
            Program.Mainform.kartya5.Visible = true;
            Program.Mainform.kartya6.Visible = true;
            Program.Mainform.kartya7.Visible = true;
            Program.Mainform.kartya8.Visible = true;
            Program.Mainform.pirosmezo.Image = null;
            Program.Mainform.zoldmezo.Image = null;
            Program.Mainform.makkmezo.Image = null;
            Program.Mainform.tokkmezo.Image = null;
            Player1.PlayerCardIds.Clear();
            Player2.PlayerCardIds.Clear();
            Player3.PlayerCardIds.Clear();
            Player4.PlayerCardIds.Clear(); //megmaradt kártyák elvétel
            Player1.init();
            Player2.init();
            Player3.init();
            Player4.init();
            kitettPiros = 0;
            kitettZöld = 0;
            kitettMakk = 0;
            kitettTök = 0; //mindenből 0 van kitéve
            if (Player1.Pontszam <= 0)
                Player1.Pontszam = kezdopont;
            if (Player2.Pontszam <= 0)
                Player2.Pontszam = kezdopont;
            if (Player3.Pontszam <= 0)
                Player3.Pontszam = kezdopont;
            if (Player4.Pontszam <= 0)
                Player4.Pontszam = kezdopont; //Pontok feltöltése ha 0
            Kezdőjátékos++; // akövetkező játékos lesz a kezdő
            if (Kezdőjátékos == 5)
                Kezdőjátékos = 1; //Ha már körbe ért akkor újra a játékos kezd
            LehetKitenni = Kezdőjátékos == 1; // ha te kezdel akkor lehet kitenni kártyát
            tabla_rendezes(Kezdőjátékos); // tábla előkészítése
            switch (Kezdőjátékos)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }

            Passz_Engedve = true;
            KartyaKiosztas();
            Program.Mainform.kartya1.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[0]);
            Program.Mainform.kartya2.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[1]);
            Program.Mainform.kartya3.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[2]);
            Program.Mainform.kartya4.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[3]);
            Program.Mainform.kartya5.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[4]);
            Program.Mainform.kartya6.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[5]);
            Program.Mainform.kartya7.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[6]);
            Program.Mainform.kartya8.Image =
                Program.Mainform.main.GetKartyaKép(Program.Mainform.main.Player1.PlayerCardIds[7]);
        }

        /// <summary>
        ///     Előkészíti  a táblát a kezdéshez pl.: valaki megveszi a kezdést
        /// </summary>
        /// <param name="kezdőjátékos"></param>
        /// A kezdő játékos sorszáma
        internal void tabla_rendezes(int kezdőjátékos)
        {
            switch (kezdőjátékos)
            {
                case 1:
                    Program.Mainform.down.Visible = true;
                    Program.Mainform.right.Visible = false;
                    Program.Mainform.up.Visible = false;
                    Program.Mainform.left.Visible = false;
                    break;
                case 2:
                    Program.Mainform.down.Visible = false;
                    Program.Mainform.right.Visible = true;
                    Program.Mainform.up.Visible = false;
                    Program.Mainform.left.Visible = false;
                    break;
                case 3:
                    Program.Mainform.down.Visible = false;
                    Program.Mainform.right.Visible = false;
                    Program.Mainform.up.Visible = true;
                    Program.Mainform.left.Visible = false;
                    break;
                case 4:
                    Program.Mainform.down.Visible = false;
                    Program.Mainform.right.Visible = false;
                    Program.Mainform.up.Visible = false;
                    Program.Mainform.left.Visible = true;
                    break;
            }

            Tracer(string.Format("táblarendezés a {0} játékos szerint.", kezdőjátékos));
        }

        /// <summary>
        ///     Átírja a táblán a számokat miután valamelyik játékos kitett egy kártyát
        /// </summary>
        /// <param name="kartya"></param>
        /// kártya száma
        /// <param name="jatekos"></param>
        /// a kártáyt kitett játékos
        internal void Kiteszkartyat(byte kartya, byte jatekos, PictureBox doboz = null)
        {
            if (LehetKitenni || jatekos != 1)
                if (KöverkezőLap(kartya))
                {
                    if (!Elkezdve)
                    {
                        Elkezdve = true;
                        KezdésMegálapítás(kartya);
                    }

                    Program.Mainform.idozito.Enabled = true;
                    Tracer(jatekos + " játékos kitette a " + KártyaNév(kartya) + " kártyát");
                    if (doboz != null) doboz.Visible = false;
                    if (kartya <= 8)
                    {
                        Program.Mainform.pirosmezo.Image = GetKartyaKép(kartya);
                        LórumPiros = false;
                        kitettPiros++;
                        Piros = kartya;
                    }

                    if (kartya >= 9 && kartya <= 16)
                    {
                        Program.Mainform.zoldmezo.Image = GetKartyaKép(kartya);
                        LórumZöld = false;
                        kitettZöld++;
                        Zöld = kartya;
                    }

                    if (kartya >= 17 && kartya <= 24)
                    {
                        Program.Mainform.makkmezo.Image = GetKartyaKép(kartya);
                        LórumMakk = false;
                        kitettMakk++;
                        Makk = kartya;
                    }

                    if (kartya >= 25)
                    {
                        Program.Mainform.tokkmezo.Image = GetKartyaKép(kartya);
                        LórumTök = false;
                        kitettTök++;
                        Tök = kartya;
                    }

                    switch (jatekos)
                    {
                        case 1:
                            Player1.PlayerCardIds.Remove(kartya);
                            Player1.JátékosKártyákSzáma--;
                            break;
                        case 2:
                            Player2.PlayerCardIds.Remove(kartya);
                            Player2.JátékosKártyákSzáma--;
                            break;
                        case 3:
                            Player3.PlayerCardIds.Remove(kartya);
                            Player3.JátékosKártyákSzáma--;
                            break;
                        case 4:
                            Player4.PlayerCardIds.Remove(kartya);
                            Player4.JátékosKártyákSzáma--;
                            break; //kártya eltávolítása a játékosok kezéből
                    }

                    switch (Player2.JátékosKártyákSzáma)
                    {
                        case 0:
                            Program.Mainform.pictureBox13.Visible = false;
                            break;
                        case 1:
                            Program.Mainform.pictureBox14.Visible = false;
                            break;
                        case 2:
                            Program.Mainform.pictureBox15.Visible = false;
                            break;
                        case 3:
                            Program.Mainform.pictureBox16.Visible = false;
                            break;
                        case 4:
                            Program.Mainform.pictureBox17.Visible = false;
                            break;
                        case 5:
                            Program.Mainform.pictureBox18.Visible = false;
                            break;
                        case 6:
                            Program.Mainform.pictureBox19.Visible = false;
                            break;
                        case 7:
                            Program.Mainform.pictureBox20.Visible = false;
                            break;
                    }

                    switch (Player3.JátékosKártyákSzáma)
                    {
                        case 0:
                            Program.Mainform.pictureBox29.Visible = false;
                            break;
                        case 1:
                            Program.Mainform.pictureBox30.Visible = false;
                            break;
                        case 2:
                            Program.Mainform.pictureBox31.Visible = false;
                            break;
                        case 3:
                            Program.Mainform.pictureBox32.Visible = false;
                            break;
                        case 4:
                            Program.Mainform.pictureBox33.Visible = false;
                            break;
                        case 5:
                            Program.Mainform.pictureBox34.Visible = false;
                            break;
                        case 6:
                            Program.Mainform.pictureBox35.Visible = false;
                            break;
                        case 7:
                            Program.Mainform.pictureBox36.Visible = false;
                            break;
                    }

                    switch (Player4.JátékosKártyákSzáma)
                    {
                        case 0:
                            Program.Mainform.pictureBox21.Visible = false;
                            break;
                        case 1:
                            Program.Mainform.pictureBox22.Visible = false;
                            break;
                        case 2:
                            Program.Mainform.pictureBox23.Visible = false;
                            break;
                        case 3:
                            Program.Mainform.pictureBox24.Visible = false;
                            break;
                        case 4:
                            Program.Mainform.pictureBox25.Visible = false;
                            break;
                        case 5:
                            Program.Mainform.pictureBox26.Visible = false;
                            break;
                        case 6:
                            Program.Mainform.pictureBox27.Visible = false;
                            break;
                        case 7:
                            Program.Mainform.pictureBox28.Visible = false;
                            break;
                    }
                }

            if (Player1.JátékosKártyákSzáma <= 0) Program.Mainform.main.PlayerWin(1);
            if (Player2.JátékosKártyákSzáma <= 0) Program.Mainform.main.PlayerWin(2);
            if (Player3.JátékosKártyákSzáma <= 0) Program.Mainform.main.PlayerWin(3);
            if (Player4.JátékosKártyákSzáma <= 0) Program.Mainform.main.PlayerWin(4);
        }

        internal void PlayerWin(byte jatekos)
        {
            Tracer(string.Format("{0} játékos megnyerte a játékot.", jatekos));
            Program.Mainform.main.UtolsóGyőztes = jatekos;
            Program.Mainform.idozito.Enabled = false;
            Játék_befejezve = true;
            Új_játék_engedve = true;
            JátékFolyamatban = false;
            Adás_Engedve = false;
            Passz_Engedve = false;
            Kezdőjátékos++;
            if (Kezdőjátékos == 5)
                Kezdőjátékos = 1;
            Player1.Pontszam += jatekos == 1
                ? !LórumMakk && !LórumPiros && !LórumTök && !LórumZöld
                    ? Player2.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma
                    : (Player2.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma) * 2
                : -Player1.JátékosKártyákSzáma;
            Player2.Pontszam += jatekos == 2
                ? !LórumMakk && !LórumPiros && !LórumTök && !LórumZöld
                    ? Player1.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma
                    : (Player1.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma) * 2
                : -Player2.JátékosKártyákSzáma;
            Player3.Pontszam += jatekos == 3
                ? !LórumMakk && !LórumPiros && !LórumTök && !LórumZöld
                    ? Player2.JátékosKártyákSzáma + Player1.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma
                    : (Player2.JátékosKártyákSzáma + Player1.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma) * 2
                : -Player3.JátékosKártyákSzáma;
            Player4.Pontszam += jatekos == 4
                ? !LórumMakk && !LórumPiros && !LórumTök && !LórumZöld
                    ? Player2.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player1.JátékosKártyákSzáma
                    : (Player2.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player1.JátékosKártyákSzáma) * 2
                : -Player4.JátékosKártyákSzáma;
            adatbazisba(string.Format("INSERT INTO Eredmenyek (Eredmeny,Idopont,Nyeremeny) VALUES('{0}','{1}',{2})",
                jatekos == 1 ? "Győzelem" : "Vereség", DateTime.Now, jatekos == 1
                    ? !LórumMakk && !LórumPiros && !LórumTök && !LórumZöld
                        ? Player2.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma
                        : (Player2.JátékosKártyákSzáma + Player3.JátékosKártyákSzáma + Player4.JátékosKártyákSzáma) * 2
                    : -Player1.JátékosKártyákSzáma));
            if (jatekos == 1)
                MessageBox.Show("Gratulálok!\n Megnyerted ezt a játékot.", "Gratulálok!", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
                MessageBox.Show(string.Format("Sajnos vesztettél!\n A játékot a {0} játékos nyerte.", jatekos),
                    "Vereség", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (Player1.Pontszam <= 0)
                Player1.Pontszam = kezdopont;
            if (Player2.Pontszam <= 0)
                Player2.Pontszam = kezdopont;
            if (Player3.Pontszam <= 0)
                Player3.Pontszam = kezdopont;
            if (Player4.Pontszam <= 0)
                Player4.Pontszam = kezdopont;
            Program.Mainform.pontszám1.Text = string.Format("Pontszám: {0}", Player1.Pontszam);
            Program.Mainform.pontszám2.Text = string.Format("Pontszám: {0}", Player2.Pontszam);
            Program.Mainform.pontszám3.Text = string.Format("Pontszám: {0}", Player3.Pontszam);
            Program.Mainform.pontszám4.Text = string.Format("Pontszám: {0}", Player4.Pontszam);
        }

        /// <summary>
        ///     Visszaadja a kártya képet az azonosítóból
        /// </summary>
        /// <param name="kartyaszam"></param>
        /// Kártya száma
        /// <returns></returns>
        public Image GetKartyaKép(byte kartyaszam)
        {
            switch (kartyaszam)
            {
                case 1:
                    return Resources.piros_hetes_2;
                case 2:
                    return Resources.piros_nyolcas_2;
                case 3:
                    return Resources.piros_kilences_2;
                case 4:
                    return Resources.piros_tízes_2;
                case 5:
                    return Resources.piros_alsó_2;
                case 6:
                    return Resources.piros_felső_2;
                case 7:
                    return Resources.piros_csikó_2;
                case 8:
                    return Resources.piros_ász_2;
                case 9:
                    return Resources.zöld_hetes_2;
                case 10:
                    return Resources.zöld_nyolcas_2;
                case 11:
                    return Resources.zöld_kilences_2;
                case 12:
                    return Resources.zöld_tízes_2;
                case 13:
                    return Resources.zöld_alsó_2;
                case 14:
                    return Resources.zöld_felső_2;
                case 15:
                    return Resources.zöld_csikó_2;
                case 16:
                    return Resources.zöld_ász_2;
                case 17:
                    return Resources.makk_hetes_2;
                case 18:
                    return Resources.makk_nyolcas_2;
                case 19:
                    return Resources.makk_kilences_2;
                case 20:
                    return Resources.makk_tízes_2;
                case 21:
                    return Resources.makk_alsó_2;
                case 22:
                    return Resources.makk_felső_2;
                case 23:
                    return Resources.makk_csikó_2;
                case 24:
                    return Resources.makk_ász_2;
                case 25:
                    return Resources.tök_hetes_2;
                case 26:
                    return Resources.tök_nyolcas_2;
                case 27:
                    return Resources.tök_kilences_2;
                case 28:
                    return Resources.tök_tízes_2;
                case 29:
                    return Resources.tök_alsó_2;
                case 30:
                    return Resources.tök_felső_2;
                case 31:
                    return Resources.tök_csikó_2;
                case 32:
                    return Resources.tök_ász_2;
            }

            return null;
        }

        public void adatbazisba(string my_querry)
        {
            var conn = new OleDbConnection();
            conn.ConnectionString =
                @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\adatbazis.accdb;Persist Security Info=True";
            // @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\adatbazis.mdf";
            try
            {
                conn.Open();
                var cmd = new OleDbCommand(my_querry, conn);
                cmd.ExecuteNonQuery();
                Tracer(string.Format("{0} Sikeresen lefutott.", my_querry));
            }
            catch (Exception ex)
            {
                Tracer(string.Format("{0}", ex.Message));
            }
            finally
            {
                conn.Close();
            }
        }

        public List<string> adatbazisbol(string my_querry)
        {
            var eredmeny = new List<string>();
            var conn = new OleDbConnection();
            conn.ConnectionString =
                @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\adatbazis.accdb;Persist Security Info=True";
            try
            {
                conn.Open();
                OleDbDataReader reader = null;
                var cmd = new OleDbCommand(my_querry, conn);
                reader = cmd.ExecuteReader();
                while (reader.Read()) eredmeny.Add(reader.GetString(0));
                Tracer(string.Format("{0} Sikeresen lefutott.", my_querry));
            }
            catch (Exception ex)
            {
                Tracer(string.Format("{0}", ex.Message));
            }
            finally
            {
                conn.Close();
            }

            return eredmeny;
        }
    }
}