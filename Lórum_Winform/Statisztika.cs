using System;
using System.Windows.Forms;

namespace Lórum_Winform
{
    public partial class Statisztika : Form
    {
        public Statisztika()
        {
            InitializeComponent();
        }

        private void Statisztika_Load(object sender, EventArgs e)
        {
            var statisztika = Program.Mainform.main.adatbazisbol("SELECT * FROM Inditasok");
            for (var i = 0; i < statisztika.Count; i++) textBox1.Text += statisztika[i] + Environment.NewLine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var statisztika = Program.Mainform.main.adatbazisbol("SELECT COUNT(id) FROM Inditasok");
            for (var i = 0; i < statisztika.Count; i++) textBox1.Text += "Összes indítás száma: " + statisztika[i];
        }

        private void Statisztika_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var statisztika = Program.Mainform.main.adatbazisbol(textBox2.Text);
            for (var i = 0; i < statisztika.Count; i++) textBox1.Text += statisztika[i];
            textBox2.ScrollToCaret();
        }
    }
}