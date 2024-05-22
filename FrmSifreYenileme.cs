using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kelime_Uygulamasi
{
    public partial class FrmSifreYenileme : Form
    {
        // Bağlantı dizesini sınıf düzeyinde tanımladık
        private readonly SqlConnection baglanti;

        public FrmSifreYenileme()
        {
            InitializeComponent();
            baglanti = new SqlConnection("Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True; TrustServerCertificate=True");
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            // Şifre yenileme işlemi
            try
            {
                baglanti.Open();
                SqlCommand komutGuncelle = new SqlCommand("UPDATE Tbl_users SET password=@a1 WHERE userName=@a2 AND email=@a3", baglanti);
                komutGuncelle.Parameters.AddWithValue("@a1", txtPasswordKayit.Text);
                komutGuncelle.Parameters.AddWithValue("@a2", txtUserNameKayit.Text);
                komutGuncelle.Parameters.AddWithValue("@a3", txtMail.Text);
                komutGuncelle.ExecuteNonQuery();
                MessageBox.Show("Şifre Güncellendi");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Şifre güncellenirken bir hata oluştu: " + ex.Message);
            }
            finally
            {
                // Bağlantıyı kapatma
                baglanti.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Giriş formuna geçiş
            Frmgiris frmGiris = new Frmgiris();
            this.Hide();
            frmGiris.ShowDialog();
        }

        private void FrmSifreYenileme_Load(object sender, EventArgs e)
        {
            // Form yüklenirken form stilini belirleme
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
    }
}
