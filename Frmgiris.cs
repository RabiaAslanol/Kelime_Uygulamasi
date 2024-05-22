using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace Kelime_Uygulamasi
{
    public partial class Frmgiris : Form
    {
        private SqlConnection baglanti; // SqlConnection nesnesini tanımladık

        public Frmgiris()
        {
            InitializeComponent();
            baglanti = new SqlConnection("Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True; TrustServerCertificate=True");
            // SqlConnection nesnesini form oluşturulduğunda hemen açtık
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtUserName.Text;
            string sifre = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Kullanıcı adı ve şifre alanları boş bırakılamaz.");
                return;
            }

            using (baglanti)
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("SELECT COUNT(*) FROM Tbl_users WHERE userName=@k1 AND password=@k2", baglanti);
                komut.Parameters.AddWithValue("@k1", kullaniciAdi);
                komut.Parameters.AddWithValue("@k2", sifre);
                int kullaniciSayisi = (int)komut.ExecuteScalar(); // Sorgunun sonucunu sayısını alıyoruz.

                if (kullaniciSayisi > 0)
                {
                    // Doğru kullanıcı adı ve şifre girildiyse, ana sayfayı göster.
                    FrmAnasayfa anasayfa = new FrmAnasayfa();
                    this.Hide();
                    anasayfa.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Hatalı kullanıcı adı ya da şifre");
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Şifreyi yenileme formunu göster.
            FrmSifreYenileme sifre = new FrmSifreYenileme();
            this.Hide();
            sifre.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Kayıt formunu göster.
            FrmKayit kayit = new FrmKayit();
            this.Hide();
            kayit.ShowDialog();
        }

        private void Frmgiris_Load(object sender, EventArgs e)
        {
            // Form yüklenirken form stilini belirle.
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
        }
    }
}