using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kelime_Uygulamasi
{
    public partial class FrmAyar : Form
    {
        private readonly string connectionString;
        private readonly int userId;

        // Constructor, gerekli parametreleri alır
        public FrmAyar()
        {
            InitializeComponent();
            this.connectionString = "Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True; TrustServerCertificate=True";
        }


        private void FrmAyar_Load(object sender, EventArgs e)
        {
            // Formun çerçeve stilini düzeltme
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            LoadUserSettings();
        }

        // Kullanıcı ayarlarını veritabanından yükleme
        private void LoadUserSettings()
        {
            string query = "SELECT WordCount FROM Tbl_UserSettings WHERE UserID = @userId";

            // Bağlantıyı açma ve veritabanından veri okuma
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        txtWordCount.Text = reader.GetInt32(reader.GetOrdinal("WordCount")).ToString();
                    }
                    else
                    {
                        txtWordCount.Text = "10"; // Varsayılan değer
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kullanıcı ayarları yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // Geçerli bir kelime sayısı girilip girilmediğini kontrol etmek için
            if (!int.TryParse(txtWordCount.Text, out int wordCount) || wordCount <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir kelime sayısı girin.");
                return;
            }
            string query = "IF EXISTS (SELECT 1 FROM Tbl_UserSettings WHERE UserID = @userId) " +
                           "UPDATE Tbl_UserSettings SET WordCount = @wordCount WHERE UserID = @userId " +
                           "ELSE INSERT INTO Tbl_UserSettings (UserID, WordCount) VALUES (@userId, @wordCount)";

            try
            {
                // Veritabanına bağlanma ve ayarları kaydetmek için
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@wordCount", wordCount);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Ayarlar kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ayarlar kaydedilirken bir hata oluştu: " + ex.Message);
            }

            // Ana sayfaya yönlendirme
            GoToHomePage();
        }
        private void btnGeri_Click(object sender, EventArgs e)
        {
            // Ana sayfaya yönlendirme
            GoToHomePage();
        }

        // Ana sayfaya yönlendirme işlemi
        private void GoToHomePage()
        {
            FrmAnasayfa anasayfa = new FrmAnasayfa();
            this.Hide();
            anasayfa.ShowDialog();
        }
    }
}
