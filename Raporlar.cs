using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml;

namespace Turkcell_Akif_Abi
{
    public partial class Raporlar : Form
    {
        public Raporlar()
        {
            InitializeComponent();
        }



        private void Raporlar_Load(object sender, EventArgs e)
        {


        }
        string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "";
                SqlCommand cmd = null;

                // Tarih aralıklarını alıyoruz
                DateTime baslangicTarihi = dateTimePickerBaslangic.Value;
                DateTime bitisTarihi = dateTimePickerBitis.Value;

                // SQL için tarihleri uygun formata getiriyoruz
                DateTime baslangicTarihiForSql = baslangicTarihi.Date;
                DateTime bitisTarihiForSql = bitisTarihi.Date.AddDays(1).AddMilliseconds(-1); // Bitis tarihi bitiş gününün son anı

                // SQL sorgusu, iki tablodan veri almak için UNION kullanıyoruz
                query = @"
            -- Bugünkü veriler (Gelirler tablosu)
            SELECT UrunAdi, SUM(Miktar) AS SatilanAdet, SUM(ToplamGelir) AS ToplamGelir
            FROM Gelirler 
            WHERE CAST(SatisTarihi AS DATE) = CAST(GETDATE() AS DATE)
            GROUP BY UrunAdi
            UNION
            -- Geçmiş tarihli veriler (ArsivGelirler tablosu)
            SELECT UrunAdi, SUM(Miktar) AS SatilanAdet, SUM(ToplamGelir) AS ToplamGelir
            FROM ArsivGelirler 
            WHERE SatisTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi
            GROUP BY UrunAdi";

                // SQL komutunu oluşturuyoruz
                cmd = new SqlCommand(query, new SqlConnection(connectionString));
                cmd.Parameters.AddWithValue("@BaslangicTarihi", baslangicTarihiForSql);
                cmd.Parameters.AddWithValue("@BitisTarihi", bitisTarihiForSql);

                // Veritabanı bağlantısını açıyoruz
                cmd.Connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Veri bulunamazsa kullanıcıya mesaj gösteriyoruz
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Seçilen tarihler aralığında veri bulunamadı.");
                    return;
                }

                // DataGridView'e veriyi yüklüyoruz
                dataGridViewRapor.DataSource = dt;
                cmd.Connection.Close();

                // Başarı mesajı veriyoruz
                MessageBox.Show("Rapor başarıyla alındı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "SELECT UrunKodu, UrunAdi, Stok FROM Urunler ORDER BY Stok ASC";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);


                    dataGridViewRapor.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Tarih aralığını alıyoruz
                DateTime baslangicTarihi = dateTimePickerBaslangic.Value.Date;
                DateTime bitisTarihi = dateTimePickerBitis.Value.Date;

                // Tarih formatlarının doğru olduğundan emin olalım
                DateTime baslangicTarihiForSql = baslangicTarihi.Date;
                DateTime bitisTarihiForSql = bitisTarihi.AddDays(1).AddMilliseconds(-1); // bitiş tarihinin son anını alıyoruz

                // Gelir ve gider sorgularını tarih aralığına göre belirliyoruz
                string gelirQuery = "SELECT UrunAdi, SUM(Miktar) AS SatilanAdet, SUM(ToplamGelir) AS ToplamGelir FROM Gelirler WHERE SatisTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi GROUP BY UrunAdi " +
                                     "UNION ALL " +
                                     "SELECT UrunAdi, SUM(Miktar) AS SatilanAdet, SUM(ToplamGelir) AS ToplamGelir FROM ArsivGelirler WHERE ArsivTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi GROUP BY UrunAdi";
                string giderQuery = "SELECT UrunAdi, SUM(Miktar) AS SatilanAdet, SUM(ToplamGider) AS ToplamGider FROM Giderler WHERE GiderTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi GROUP BY UrunAdi " +
                                    "UNION ALL " +
                                    "SELECT UrunAdi, SUM(Miktar) AS SatilanAdet, SUM(ToplamGider) AS ToplamGider FROM ArsivGiderler WHERE ArsivTarihi BETWEEN @BaslangicTarihi AND @BitisTarihi GROUP BY UrunAdi";

                // Raporu göstermek için veri tablosu oluşturuyoruz
                DataTable raporDataTable = new DataTable();
                raporDataTable.Columns.Add("UrunAdi", typeof(string));
                raporDataTable.Columns.Add("Gelir", typeof(decimal));
                raporDataTable.Columns.Add("Gider", typeof(decimal));

                using (SqlConnection connection = new SqlConnection("Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True"))
                {
                    connection.Open();

                    // Gelirleri sorguluyoruz
                    SqlCommand gelirCmd = new SqlCommand(gelirQuery, connection);
                    gelirCmd.Parameters.AddWithValue("@BaslangicTarihi", baslangicTarihiForSql);
                    gelirCmd.Parameters.AddWithValue("@BitisTarihi", bitisTarihiForSql);

                    SqlDataAdapter gelirAdapter = new SqlDataAdapter(gelirCmd);
                    DataTable gelirDataTable = new DataTable();
                    gelirAdapter.Fill(gelirDataTable);

                    // Giderleri sorguluyoruz
                    SqlCommand giderCmd = new SqlCommand(giderQuery, connection);
                    giderCmd.Parameters.AddWithValue("@BaslangicTarihi", baslangicTarihiForSql);
                    giderCmd.Parameters.AddWithValue("@BitisTarihi", bitisTarihiForSql);

                    SqlDataAdapter giderAdapter = new SqlDataAdapter(giderCmd);
                    DataTable giderDataTable = new DataTable();
                    giderAdapter.Fill(giderDataTable);

                    // Giderleri hızlı eşleştirmek için dictionary kullanıyoruz
                    var giderDict = giderDataTable.AsEnumerable()
                                                  .ToDictionary(row => row.Field<string>("UrunAdi"), row => row.Field<decimal>("ToplamGider"));

                    // Gelirleri raporDataTable'a ekliyoruz
                    foreach (DataRow gelirRow in gelirDataTable.Rows)
                    {
                        var urunAdi = gelirRow["UrunAdi"].ToString();
                        var toplamGelirValue = Convert.ToDecimal(gelirRow["ToplamGelir"]);
                        if (giderDict.ContainsKey(urunAdi))
                        {
                            raporDataTable.Rows.Add(urunAdi, toplamGelirValue, giderDict[urunAdi]);
                        }
                        else
                        {
                            raporDataTable.Rows.Add(urunAdi, toplamGelirValue, 0); // Gider yoksa 0 olarak ekliyoruz
                        }
                    }

                    // Giderleri raporDataTable'a ekliyoruz (Gelir olmayan ürünler için)
                    foreach (DataRow giderRow in giderDataTable.Rows)
                    {
                        var urunAdi = giderRow["UrunAdi"].ToString();
                        if (!raporDataTable.AsEnumerable().Any(r => r.Field<string>("UrunAdi") == urunAdi))
                        {
                            var toplamGiderValue = Convert.ToDecimal(giderRow["ToplamGider"]);
                            raporDataTable.Rows.Add(urunAdi, 0, toplamGiderValue); // Gelir yoksa 0 gelir olarak ekliyoruz
                        }
                    }
                }

                // Veriyi DataGridView'e bağlıyoruz
                dataGridViewRapor.DataSource = raporDataTable;

                // Toplam gelir ve giderleri hesaplıyoruz
                decimal toplamGelirDegeri = raporDataTable.AsEnumerable().Sum(row => row.Field<decimal>("Gelir"));
                decimal toplamGiderDegeri = raporDataTable.AsEnumerable().Sum(row => row.Field<decimal>("Gider"));

                // Mesaj kutusunda sonucu gösteriyoruz
                MessageBox.Show($"Toplam Gelir: {toplamGelirDegeri} \nToplam Gider: {toplamGiderDegeri} \nNet Gelir: {toplamGelirDegeri - toplamGiderDegeri}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnRaporGoster_Click(object sender, EventArgs e)
        {


        }

      
    }

}