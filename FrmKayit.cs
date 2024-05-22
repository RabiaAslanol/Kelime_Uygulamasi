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
    public partial class FrmKayit : Form
    {
        // SqlConnection nesnesini tanımladık
        private readonly SqlConnection baglanti;

        public FrmKayit()
        {
            InitializeComponent();
            baglanti = new SqlConnection("Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True; TrustServerCertificate=True");
        }

        private void btnKayit_Click(object sender, EventArgs e)
        {
            // Kullanıcıyı kaydetme işlemi
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO Tbl_users(userName, password, email) VALUES (@p1, @p2, @p3)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtUserNameKayit.Text);
                komut.Parameters.AddWithValue("@p2", txtPasswordKayit.Text);
                komut.Parameters.AddWithValue("@p3", txtMail.Text);
                komut.ExecuteNonQuery();
                MessageBox.Show("Yeni Kayıt Oluşturuldu");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt işlemi sırasında bir hata oluştu: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Giriş formuna geçiş
            Frmgiris giris = new Frmgiris();
            this.Hide();
            giris.ShowDialog();
        }

        private void FrmKayit_Load(object sender, EventArgs e)
        {
            // Form yüklenirken form stilini belirle
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
    }
}
