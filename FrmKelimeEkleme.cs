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
using System.IO;
using System.Net;

namespace Kelime_Uygulamasi
{
    public partial class FrmKelimeEkleme : Form
    {
        // SqlConnection nesnesini tanımladık
        private readonly SqlConnection baglanti;

        public FrmKelimeEkleme()
        {
            InitializeComponent();
            baglanti = new SqlConnection("Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True; TrustServerCertificate=True");
        }

        private void btnKaydet_Click_1(object sender, EventArgs e)
        {
            // Yeni kelimeyi veritabanına kaydetme işlemi
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("INSERT INTO Tbl_words(EnglishWord,TurkishEquivalent,ExampleSentences,ImagePath) VALUES (@p1,@p2,@p3,@p4)", baglanti);
                komut.Parameters.AddWithValue("@p1", txtIngKelime.Text);
                komut.Parameters.AddWithValue("@p2", txtTrKelime.Text);
                komut.Parameters.AddWithValue("@p3", richTextBox1.Text);
                komut.Parameters.AddWithValue("@p4", txtResim.Text);
                komut.ExecuteNonQuery();
                MessageBox.Show("Veriler Kaydedildi");
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Resim seçme işlemi
            openFileDialog1.ShowDialog(); // Bilgisayar dosyalarını açma
            pictureBox1.ImageLocation = openFileDialog1.FileName;
            txtResim.Text = openFileDialog1.FileName;
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            // Ana sayfaya geri dönme işlemi
            FrmAnasayfa anasayfa = new FrmAnasayfa();
            this.Hide();
            anasayfa.ShowDialog();
        }

        private void FrmKelimeEkleme_Load(object sender, EventArgs e)
        {
            // Form yüklenirken form stilini belirleme
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
    }
}
