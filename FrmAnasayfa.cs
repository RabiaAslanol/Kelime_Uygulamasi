using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kelime_Uygulamasi
{
    public partial class FrmAnasayfa : Form
    {
        public FrmAnasayfa()
        {
            InitializeComponent();
        }

        private void btnKelimeEkle_Click_1(object sender, EventArgs e)
        {
            FrmKelimeEkleme kelimeEkle = new FrmKelimeEkleme();
            this.Hide();
            kelimeEkle.ShowDialog();
        }

        private void btnSinav_Click_1(object sender, EventArgs e)
        {
            FrmsinavModul sinav = new FrmsinavModul();
            this.Hide();
            sinav.ShowDialog();
        }

        private void btnAyarlar_Click_1(object sender, EventArgs e)
        {
            FrmAyar ayar = new FrmAyar();
            this.Hide();
            ayar.ShowDialog();
        }

        private void btnRapor_Click_1(object sender, EventArgs e)
        {
            Frmrapor rapor = new Frmrapor();
            this.Hide();
            rapor.ShowDialog();
        }

        private void FrmAnasayfa_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        }

    }
}
