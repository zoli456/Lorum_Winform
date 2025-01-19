using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static Lórum_Winform.Main;

namespace Lórum_Winform
{
    public partial class Form1 : Form
    {
        private const string Version = "1.0";
        public Main main = new Main();
        private byte step;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     A betöltésnél elvégezendő műveletek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            main.init();
            Text = "Lórum - " + Version + " " +
                   File.GetLastWriteTime(Assembly.GetEntryAssembly().Location)
                       .ToString()
                       .ToCharArray()
                       .Sum(x => x) + " build";
            main.Tracer("Lórum - " + Version + " " +
                        File.GetLastWriteTime(Assembly.GetEntryAssembly().Location)
                            .ToString()
                            .ToCharArray()
                            .Sum(x => x) + " build");
            pontszám1.Text = string.Format("Pontszám: {0}", Program.Mainform.main.Player1.Pontszam);
            pontszám2.Text = string.Format("Pontszám: {0}", Program.Mainform.main.Player2.Pontszam);
            pontszám3.Text = string.Format("Pontszám: {0}", Program.Mainform.main.Player3.Pontszam);
            pontszám4.Text = string.Format("Pontszám: {0}", Program.Mainform.main.Player4.Pontszam);
            main.adatbazisba(string.Format("INSERT INTO Inditasok (Idopont) VALUES('{0}')", DateTime.Now));
        }

        /// <summary>
        ///     Új játék indítás
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            main.Jatek_Inditas();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[0])],
                1, kartya1);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[1])],
                1, kartya2);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[2])],
                1, kartya3);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[3])],
                1, kartya4);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[4])],
                1, kartya5);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[5])],
                1, kartya6);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[6])],
                1, kartya7);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            main.Kiteszkartyat(main.Player1.PlayerCardIds[main.Player1.PlayerCardIds.IndexOf(main.Jatekoskartyak[7])],
                1, kartya8);
        }

        /// <summary>
        ///     Timer ami késleltetést ad a játékban mikor a gépek lépnek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void idozito_Tick(object sender, EventArgs e)
        {
            step++;
            switch (step)
            {
                case 1:
                    main.Passz_Engedve = false;
                    main.Új_játék_engedve = false;
                    main.LehetKitenni = false;
                    break;
                case 2:
                    main.tabla_rendezes(2);
                    break;
                case 3:
                    main.Player2.GépJáték();
                    break;
                case 4:
                    main.tabla_rendezes(3);
                    break;
                case 5:
                    main.Player3.GépJáték();
                    break;
                case 6:
                    main.tabla_rendezes(4);
                    break;
                case 7:
                    main.Player4.GépJáték();
                    break;
                case 8:
                    main.tabla_rendezes(1);
                    step = 0;
                    main.LehetKitenni = true;
                    main.Passz_Engedve = true;
                    main.Új_játék_engedve = true;
                    idozito.Enabled = false;
                    if (main.Player1.PlayerCardIds.TrueForAll(x => !main.KöverkezőLap(x)))
                        main.Tracer("Nem tudsz lépni semmit.");
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            main.Tracer("Passz használva.");
            if (!main.Passz_Engedve)
                return;
            if (main.Player1.PlayerCardIds.TrueForAll(x => !main.KöverkezőLap(x)) ||
                (main.Kezdőjátékos != 1 && !main.Elkezdve))
                idozito.Enabled = true;
            else
                main.Tracer("Tudsz valamit tenni.");
        }

        private void pirosmezo_Click(object sender, EventArgs e)
        {
            main.Tracer(string.Format("A piros mezőben {0}({1}) van", KártyaNév(main.Piros), main.Piros));
        }

        private void zoldmezo_Click(object sender, EventArgs e)
        {
            main.Tracer(string.Format("A zöld mezőben {0}({1})  van", KártyaNév(main.Zöld), main.Zöld));
        }

        private void makkmezo_Click(object sender, EventArgs e)
        {
            main.Tracer(string.Format("A makk mezőben {0}({1})  van", KártyaNév(main.Makk), main.Makk));
        }

        private void tokkmezo_Click(object sender, EventArgs e)
        {
            main.Tracer(string.Format("A tök mezőben {0}({1})  van", KártyaNév(main.Tök), main.Tök));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ablak = new Statisztika();
            ablak.Show();
        }
    }
}