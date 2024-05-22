using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Kelime_Uygulamasi
{
    public partial class FrmsinavModul : Form
    {
        // Veritabanı bağlantı dizesi
        private string connectionString = "Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        // Kullanıcı ID'si
        private int userId = 0;

        // Gösterilecek soru sayısı
        private int top = 10;

        // Şu anki sorunun indeksi
        private int currentQuestionIndex = 0;

        // Doğrulama için şu anki sorunun indeksi
        private int currentQuestionIndex2 = 0;

        // Buton tıklama sayısı
        private int clickCount = 1;

        // Soruların listesi
        private List<string[]> questionList = new List<string[]>();

        public FrmsinavModul()
        {
            InitializeComponent();
        }

        // Geri butonuna tıklama olayı
        private void btnGeri_Click(object sender, EventArgs e)
        {
            FrmAnasayfa anasayfa = new FrmAnasayfa();
            this.Hide();
            anasayfa.ShowDialog();
        }

        // Form yüklendiğinde gerçekleşecek işlemler
        private void FrmsinavModul_Load(object sender, EventArgs e)
        {
            // Formun stilini belirleme
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // Kullanıcı ayarlarını yükleme
            LoadUserSettings();

            // Soruları yükleme
            LoadQuestions();
        }

        // Kullanıcı ayarlarını veritabanından yükleme
        private void LoadUserSettings()
        {
            string query = "SELECT WordCount FROM Tbl_UserSettings WHERE UserID = @userId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    top = reader.GetInt32(reader.GetOrdinal("WordCount"));
                }
                reader.Close();
            }
        }

        // Soruları veritabanından yükleme
        private void LoadQuestions()
        {
            string query = @"
                SELECT TOP(@top) 
                    EnglishWord, ExampleSentences, WordID, TurkishEquivalent, ImagePath, Sayac, QuestionDate, SorulmaSayisi 
                FROM Tbl_words 
                WHERE QuestionDate < DATEADD(day, 1, GETDATE()) 
                  AND QuestionDate > DATEADD(day, -1, GETDATE()) 
                ORDER BY NEWID()";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@top", top);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string[] question = new string[]
                    {
                        reader["EnglishWord"].ToString(),
                        reader["ExampleSentences"].ToString(),
                        reader["ImagePath"].ToString(),
                        reader["WordID"].ToString(),
                        reader["QuestionDate"].ToString(),
                        reader["Sayac"].ToString(),
                        reader["TurkishEquivalent"].ToString(),
                        reader["SorulmaSayisi"].ToString()
                    };
                    questionList.Add(question);
                }
                reader.Close();
            }
        }

        // Soruyu görüntüleme
        private void DisplayQuestion(string question, string sentences, string imagePath)
        {
            label1.Text = question;
            richTextBox1.Text = sentences;
            // Gerekirse pictureBox1.Image ayarlayın
        }

        // "Getir" butonuna tıklama olayı
        private void btnGetir_Click(object sender, EventArgs e)
        {
            lblsayac.Text = clickCount.ToString();
            if (currentQuestionIndex < questionList.Count)
            {
                string[] currentQuestion = questionList[currentQuestionIndex];
                DisplayQuestion(currentQuestion[0], currentQuestion[1], currentQuestion[2]);
                currentQuestionIndex++;
                clickCount++;
            }
            else
            {
                MessageBox.Show("Gösterilecek başka soru kalmadı.");
            }
        }

        // "Onayla" butonuna tıklama olayı
        private void btnOnayla_Click(object sender, EventArgs e)
        {
            if (currentQuestionIndex2 < questionList.Count)
            {
                string[] currentQuestion = questionList[currentQuestionIndex2];
                currentQuestionIndex2++;
                int wordID = Convert.ToInt32(currentQuestion[3]);
                int counter = Convert.ToInt32(currentQuestion[5]);
                int askedCount = Convert.ToInt32(currentQuestion[7]);
                string correctAnswer = currentQuestion[6];
                DateTime questionDate = Convert.ToDateTime(currentQuestion[4]);

                if (CheckAnswer(wordID))
                {
                    MessageBox.Show("Doğru");
                    counter = UpdateCounter(wordID, counter, askedCount);
                    DateTime newDate = CalculateNewDate(questionDate, counter);
                    UpdateQuestionDate(wordID, newDate);
                }
                else
                {
                    MessageBox.Show("Yanlış");
                    ResetCounter(wordID, askedCount);
                }
            }
        }

        // Cevabı kontrol etme
        private bool CheckAnswer(int wordID)
        {
            string correctAnswer = GetCorrectAnswer(wordID);
            return textBox1.Text == correctAnswer;
        }

        // Doğru cevabı veritabanından alma
        private string GetCorrectAnswer(int wordID)
        {
            string query = "SELECT TurkishEquivalent FROM Tbl_words WHERE WordID = @id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", wordID);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return reader["TurkishEquivalent"].ToString();
                }
                reader.Close();
            }
            return string.Empty;
        }

        // Sayaç güncelleme
        private int UpdateCounter(int wordID, int counter, int askedCount)
        {
            counter++;
            askedCount++;
            string query = "UPDATE Tbl_words SET Sayac = @counter, SorulmaSayisi = @askedCount WHERE WordID = @wordID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@wordID", wordID);
                command.Parameters.AddWithValue("@counter", counter);
                command.Parameters.AddWithValue("@askedCount", askedCount);
                connection.Open();
                command.ExecuteNonQuery();
            }
            if (counter == 7)
            {
                RemoveQuestion(wordID);
            }
            return counter;
        }

        // Sayaç sıfırlama
        private void ResetCounter(int wordID, int askedCount)
        {
            string query = "UPDATE Tbl_words SET Sayac = 0, SorulmaSayisi = @askedCount WHERE WordID = @wordID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@wordID", wordID);
                command.Parameters.AddWithValue("@askedCount", askedCount + 1);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Yeni tarih hesaplama
        private DateTime CalculateNewDate(DateTime questionDate, int counter)
        {
            switch (counter)
            {
                case 1: return questionDate.AddDays(1);
                case 2: return questionDate.AddDays(6);
                case 3: return questionDate.AddMonths(1);
                case 4: return questionDate.AddMonths(3);
                case 5: return questionDate.AddMonths(6);
                case 6: return questionDate.AddYears(1);
                default: return questionDate;
            }
        }

        // Sorunun tarihini güncelleme
        private void UpdateQuestionDate(int wordID, DateTime newDate)
        {
            string query = "UPDATE Tbl_words SET QuestionDate = @newDate WHERE WordID = @wordID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@wordID", wordID);
                command.Parameters.AddWithValue("@newDate", newDate);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Soruyu kaldırma
        private void RemoveQuestion(int wordID)
        {
            MoveToKnownQuestions(wordID);
            DeleteQuestion(wordID);
        }

        // Bilinen sorulara taşıma
        private void MoveToKnownQuestions(int wordID)
        {
            string query = "INSERT INTO Tbl_KnownQuestions (EnglishWord) SELECT EnglishWord FROM Tbl_words WHERE WordID = @wordID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@wordID", wordID);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Soruyu silme
        private void DeleteQuestion(int wordID)
        {
            string query = "DELETE FROM Tbl_words WHERE WordID = @wordID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@wordID", wordID);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
