using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Kelime_Uygulamasi
{
    public partial class Frmrapor : Form
    {
        private SqlConnection connection; // Veritabanı bağlantısı için SqlConnection nesnesi
        private int knownWordsCount; // Bilinen kelimelerin sayısı
        private int totalQuestionsCount; // Toplam soruların sayısı
        private int counter; // Bir kelimenin sayaç değeri

        public Frmrapor()
        {
            InitializeComponent();
            connection = new SqlConnection("Data Source=Rabia\\SQLEXPRESS;Initial Catalog=Kelime_Uygulamasi;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
        }

        private void btnGeri_Click(object sender, EventArgs e)
        {
            // Anasayfaya geri dönmek için
            FrmAnasayfa anasayfa = new FrmAnasayfa();
            this.Hide();
            anasayfa.ShowDialog();
        }

        private void Frmrapor_Load(object sender, EventArgs e)
        {
            // Form yüklenirken formun stilini belirler
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // Bilinen kelimelerin ve toplam soruların sayısını alır
            knownWordsCount = GetRecordCount("SELECT COUNT(*) FROM Tbl_KnownQuestions");
            totalQuestionsCount = GetRecordCount("SELECT COUNT(*) FROM Tbl_words");

            // Grafiğe bilinen kelimeleri ekler
            Kelimeler.Series["Kelimeler"].Points.AddXY("Bilinenler", knownWordsCount);
            // Grafiğe toplam soru sayısını ekler
            Kelimeler.Series["Kelimeler"].Points.AddXY("Soru sayısı", totalQuestionsCount);
        }

        private int GetRecordCount(string query)
        {
            // Belirtilen sorguya göre kayıt sayısını döndürür
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            int count = (int)command.ExecuteScalar(); // Tek bir değer döner
            connection.Close();
            return count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TextBox'ta girilen kelimenin sayaç değerini alır ve günceller
            counter = GetCounterForWord(textBox1.Text);
            label3.Text = counter.ToString(); // Sayaç değerini Label'a yazar
            progressBar1.Value += counter; // ProgressBar'ı günceller
        }

        private int GetCounterForWord(string word)
        {
            // Belirtilen kelimenin sayaç değerini döndürür
            int counter = 0;
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT sayac FROM Tbl_KnownQuestions WHERE englishWord = @word", connection);
            command.Parameters.AddWithValue("@word", word);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                counter = Convert.ToInt32(reader["sayac"]); // Sayaç değerini alır
            }
            connection.Close();
            return counter;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Yazdırma işlemi için PrintDocument nesnesi oluşturur
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            // Yazdırma iletişim kutusunu gösterir
            PrintDialog printDialog = new PrintDialog { Document = printDocument };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print(); // Yazdırma işlemini başlatır
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Formu bir bitmap olarak çizdirir ve yazdırma alanına ekler
            Bitmap bitmap = new Bitmap(this.Width, this.Height);
            this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Width, this.Height));
            e.Graphics.DrawImage(bitmap, 0, 0);
        }
    }
}
