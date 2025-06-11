using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;


namespace Turkcell_Akif_Abi
{
    public partial class Kasaa : Form
    {
        public Kasaa()
        {
            InitializeComponent();
        }
        private string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";

        private void Kasaa_Load(object sender, EventArgs e)
        {
            KasaDurumunuGuncelle();
            GiderGöster();
            LoadGelirler();
        }
        private void GiderGöster()
        {
            
            string query = "SELECT * FROM Giderler";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

               
                dataGridViewRapor.DataSource = dt;
            }

            
            decimal toplamGiderler = 0;
            foreach (DataGridViewRow row in dataGridViewRapor.Rows)
            {
                if (row.Cells["ToplamGider"].Value != null)
                {
                    toplamGiderler += Convert.ToDecimal(row.Cells["ToplamGider"].Value);
                }
            }

           
            label9.Text = toplamGiderler.ToString("C"); 
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            string giderTipi = txtGiderTipi.Text; 
            string miktarStr = txtMiktar.Text;
            string alisFiyatiStr = txtAlisFiyati.Text;

            
            alisFiyatiStr = alisFiyatiStr.Replace("₺", "").Replace(".", "").Replace(",", ".");

           
            decimal alisFiyati;
            if (!decimal.TryParse(alisFiyatiStr, out alisFiyati))
            {
                MessageBox.Show("Alış fiyatı geçerli bir sayı olmalıdır.");
                return;
            }

           
            int miktar;
            if (!int.TryParse(miktarStr, out miktar))
            {
                MessageBox.Show("Miktar geçerli bir sayı olmalıdır.");
                return;
            }

           
            decimal toplamGider = miktar * alisFiyati;

            DateTime giderTarihi = DateTime.Now;

           
            string query = "INSERT INTO Giderler (UrunKodu, UrunAdi, Miktar, AlisFiyati, ToplamGider, GiderTarihi, GiderTipi) " +
                           "VALUES (@UrunKodu, @UrunAdi, @Miktar, @AlisFiyati, @ToplamGider, @GiderTarihi, @GiderTipi)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UrunKodu", "");
                cmd.Parameters.AddWithValue("@UrunAdi", giderTipi); 
                cmd.Parameters.AddWithValue("@Miktar", miktar); 
                cmd.Parameters.AddWithValue("@AlisFiyati", alisFiyati); 
                cmd.Parameters.AddWithValue("@ToplamGider", toplamGider); 
                cmd.Parameters.AddWithValue("@GiderTarihi", giderTarihi);
                cmd.Parameters.AddWithValue("@GiderTipi", giderTipi); 

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Gider başarıyla eklendi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veritabanı hatası: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }

            
            GiderGöster();
            KasaDurumunuGuncelle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            string gelirTipi = txtGelirTipi.Text; 
            string fiyatStr = Tutar.Text; 
            fiyatStr = fiyatStr.Replace("₺", "").Replace(".", "").Replace(",", ".");
            decimal fiyat;
            if (!decimal.TryParse(fiyatStr, out fiyat))
            {
                MessageBox.Show("Fiyat geçerli bir sayı olmalıdır.");
                return;
            }

           
            int miktar = 1;

            
            decimal toplamGelir = miktar * fiyat;

            DateTime satisTarihi = DateTime.Now; 

           
            string query = "INSERT INTO Gelirler (UrunKodu, UrunAdi, Miktar, Fiyat, ToplamGelir, SatisTarihi, GelirTipi) " +
                           "VALUES (@UrunKodu, @UrunAdi, @Miktar, @Fiyat, @ToplamGelir, @SatisTarihi, @GelirTipi)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UrunKodu", ""); 
                cmd.Parameters.AddWithValue("@UrunAdi", gelirTipi); 
                cmd.Parameters.AddWithValue("@Miktar", miktar); 
                cmd.Parameters.AddWithValue("@Fiyat", fiyat); 
                cmd.Parameters.AddWithValue("@ToplamGelir", toplamGelir);
                cmd.Parameters.AddWithValue("@SatisTarihi", satisTarihi);
                cmd.Parameters.AddWithValue("@GelirTipi", gelirTipi); 

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Gelir başarıyla eklendi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Veritabanı hatası: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }

           
            LoadGelirler();
            KasaDurumunuGuncelle();
        }
        private void LoadGelirler()
        {
            
            string query = "SELECT * FROM Gelirler";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

               
                dataGridView1.DataSource = dt;
            }

            
            decimal toplamGelirler = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["ToplamGelir"].Value != null)
                {
                    toplamGelirler += Convert.ToDecimal(row.Cells["ToplamGelir"].Value);
                }
            }

           
            label10.Text = toplamGelirler.ToString("C"); 
        }
        private void KasaDurumunuGuncelle()
        {
            decimal toplamGelir = 0;
            decimal toplamGider = 0;
            decimal kasaDurumu = 0;

          

            using (SqlConnection conn = new SqlConnection("Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True"))
            {
                try
                {
                    conn.Open();

                   
                    string gelirSorgu = "SELECT ISNULL(SUM(CAST(ToplamGelir AS DECIMAL(18,2))), 0) FROM Gelirler";
                    SqlCommand cmdGelir = new SqlCommand(gelirSorgu, conn);
                    var gelirSonuc = cmdGelir.ExecuteScalar();
                    toplamGelir = gelirSonuc != DBNull.Value && decimal.TryParse(gelirSonuc.ToString(), out decimal gelir) ? gelir : 0;

                    
                    string giderSorgu = "SELECT ISNULL(SUM(CAST(ToplamGider AS DECIMAL(18,2))), 0) FROM Giderler";
                    SqlCommand cmdGider = new SqlCommand(giderSorgu, conn);
                    var giderSonuc = cmdGider.ExecuteScalar();
                    toplamGider = giderSonuc != DBNull.Value && decimal.TryParse(giderSonuc.ToString(), out decimal gider) ? gider : 0;

                    
                    kasaDurumu = toplamGelir - toplamGider;

                    
                    lblKasaDurumu.Text = $" {kasaDurumu:C}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kasa durumu hesaplanırken bir hata oluştu:\n{ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    
        private void button3_Click(object sender, EventArgs e)
        {
            decimal toplamGelir = 0;
            decimal toplamGider = 0;
            decimal devirTutari = 0;

            
            string gelirText = label10.Text.Replace("₺", "").Trim(); 
            string giderText = label9.Text.Replace("₺", "").Trim(); 

            
            gelirText = gelirText.Replace(",", ".");
            giderText = giderText.Replace(",", ".");

            MessageBox.Show("Gelir: " + gelirText);
            MessageBox.Show("Gider: " + giderText);

            if (!decimal.TryParse(gelirText, out toplamGelir))
            {
                MessageBox.Show("Toplam gelir değeri geçersiz! Lütfen doğru formatta olduğundan emin olun.");
                return;
            }

            if (!decimal.TryParse(giderText, out toplamGider))
            {
                MessageBox.Show("Toplam gider değeri geçersiz! Lütfen doğru formatta olduğundan emin olun.");
                return;
            }

            devirTutari = toplamGelir - toplamGider;

            using (SqlConnection conn = new SqlConnection("Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True"))
            {
                try
                {
                    conn.Open();

                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string arsivleGelir = @"
                    INSERT INTO ArsivGelirler (UrunKodu, UrunAdi, Miktar, Fiyat, ToplamGelir, SatisTarihi, GelirTipi)
                    SELECT UrunKodu, UrunAdi, Miktar, Fiyat, ToplamGelir, SatisTarihi, GelirTipi
                    FROM Gelirler";

                        SqlCommand cmdArsivGelir = new SqlCommand(arsivleGelir, conn, transaction);
                        cmdArsivGelir.ExecuteNonQuery();

                        string arsivleGider = @"
                    INSERT INTO ArsivGiderler (UrunKodu, UrunAdi, Miktar, AlisFiyati, ToplamGider, GiderTarihi, GiderTipi)
                    SELECT UrunKodu, UrunAdi, Miktar, AlisFiyati, ToplamGider, GiderTarihi, GiderTipi
                    FROM Giderler";

                        SqlCommand cmdArsivGider = new SqlCommand(arsivleGider, conn, transaction);
                        cmdArsivGider.ExecuteNonQuery();

                        string kasaQuery = @"
                    INSERT INTO Kasa (Tarih, ToplamGelir, ToplamGider, DevirTutari) 
                    VALUES (@Tarih, @ToplamGelir, @ToplamGider, @DevirTutari)";

                        SqlCommand cmdKasa = new SqlCommand(kasaQuery, conn, transaction);
                        cmdKasa.Parameters.AddWithValue("@Tarih", DateTime.Now.Date);
                        cmdKasa.Parameters.AddWithValue("@ToplamGelir", toplamGelir);
                        cmdKasa.Parameters.AddWithValue("@ToplamGider", toplamGider);
                        cmdKasa.Parameters.AddWithValue("@DevirTutari", devirTutari);
                        cmdKasa.ExecuteNonQuery();

                        string temizleGelir = "DELETE FROM Gelirler";
                        string temizleGider = "DELETE FROM Giderler";

                        SqlCommand cmdTemizleGelir = new SqlCommand(temizleGelir, conn, transaction);
                        SqlCommand cmdTemizleGider = new SqlCommand(temizleGider, conn, transaction);

                        cmdTemizleGelir.ExecuteNonQuery();
                        cmdTemizleGider.ExecuteNonQuery();

                        transaction.Commit();

                        label10.Text = "₺0,00";
                        label9.Text = "₺0,00";

                        MessageBox.Show($"📅 Günsonu Devir ve Arşivleme Başarıyla Tamamlandı!\n" +
                                        $"📈 Toplam Gelir: {toplamGelir:C}\n" +
                                        $"📉 Toplam Gider: {toplamGider:C}\n" +
                                        $"💼 Devir Tutarı: {devirTutari:C}");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"İşlem sırasında hata oluştu: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bağlantı hatası: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
                GiderGöster();
                LoadGelirler();
            }

        }
    }
}
