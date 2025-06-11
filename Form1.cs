using Microsoft.Data.SqlClient;
using System.Data;

namespace Turkcell_Akif_Abi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {


        }

        private void LoadSepetData()
        {

            DataTable sepetDataTable = new DataTable();

            sepetDataTable.Columns.Add("UrunKodu");
            sepetDataTable.Columns.Add("UrunAdi");
            sepetDataTable.Columns.Add("Miktar");
            sepetDataTable.Columns.Add("Fiyat");
            sepetDataTable.Columns.Add("IndirimliFiyat");
            sepetDataTable.Columns.Add("IndirimTutari");
            sepetDataTable.Columns.Add("ToplamTutar");

            using (SqlConnection connection = new SqlConnection("Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True"))
            {
                connection.Open();
                string query = "SELECT UrunKodu, UrunAdi, Miktar, Fiyat, IndirimliFiyat, IndirimTutari, ToplamTutar FROM Sepet";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DataRow row = sepetDataTable.NewRow();
                    row["UrunKodu"] = reader["UrunKodu"];
                    row["UrunAdi"] = reader["UrunAdi"];
                    row["Miktar"] = reader["Miktar"];
                    row["Fiyat"] = reader["Fiyat"];
                    row["IndirimliFiyat"] = reader["IndirimliFiyat"];
                    row["IndirimTutari"] = reader["IndirimTutari"];
                    row["ToplamTutar"] = reader["ToplamTutar"];

                    sepetDataTable.Rows.Add(row);
                }

                dataGridViewSepet.DataSource = sepetDataTable;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
            StokKontrol();

            LoadSepetData();

            comboBoxSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBoxSearch.AutoCompleteSource = AutoCompleteSource.ListItems;



            string query = "SELECT UrunAdi FROM Urunler";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();


                comboBoxSearch.Items.Clear();

                while (reader.Read())
                {
                    comboBoxSearch.Items.Add(reader["UrunAdi"].ToString());
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ürün_Tanýmla üt = new Ürün_Tanýmla();
            üt.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hizmet_Tanýmla ht = new Hizmet_Tanýmla();
            ht.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Ürünleri_Ýncele üi = new Ürünleri_Ýncele();
            üi.Show();
        }



        private void button7_Click(object sender, EventArgs e)
        {
            Kasaa ks = new Kasaa();
            ks.Show();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Raporlar rp = new Raporlar();
            rp.Show();

        }


        private void urunaditxt_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSearch.SelectedItem != null)
            {
                string selectedProduct = comboBoxSearch.SelectedItem.ToString();

                string query = "SELECT UrunKodu, UrunAlisFiyati, UrunSatisFiyati FROM Urunler WHERE UrunAdi = @UrunAdi";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UrunAdi", selectedProduct);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        urunKoduTextBox.Text = reader["UrunKodu"].ToString();
                        alisFiyatiTextBox.Text = reader["UrunAlisFiyati"].ToString();
                        satisFiyatiTextBox.Text = reader["UrunSatisFiyati"].ToString();
                    }
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                string urunKodu = urunKoduTextBox.Text.Trim();
                decimal miktar = Convert.ToDecimal(miktartxt.Text.Trim());
                decimal indirimliFiyat = 0;
                decimal urunFiyat = 0;

                if (string.IsNullOrEmpty(urunKodu) || miktar <= 0)
                {
                    MessageBox.Show("Lütfen geçerli bir ürün kodu ve miktar giriniz.");
                    return;
                }

                string urunAdi = "";
                int mevcutStok = 0;

                using (SqlConnection connection = new SqlConnection("Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True"))
                {
                    connection.Open();

                    // Ürün bilgilerini ve stok kontrolünü yap
                    string query = "SELECT UrunAdi, UrunSatisFiyati, Stok FROM Urunler WHERE UrunKodu = @UrunKodu";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UrunKodu", urunKodu);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        urunAdi = reader["UrunAdi"].ToString();
                        urunFiyat = Convert.ToDecimal(reader["UrunSatisFiyati"]);
                        mevcutStok = Convert.ToInt32(reader["Stok"]);
                    }
                    else
                    {
                        MessageBox.Show("Ürün kodu bulunamadý.");
                        return;
                    }

                    reader.Close();
                }

                // Stok kontrolü
                if (mevcutStok < miktar)
                {
                    MessageBox.Show($"Yetersiz stok! Mevcut stok: {mevcutStok}");
                    return;
                }

                if (!string.IsNullOrEmpty(indirimtext.Text))
                {
                    indirimliFiyat = Convert.ToDecimal(indirimtext.Text.Trim());
                }
                else
                {
                    indirimliFiyat = urunFiyat;
                }

                decimal toplamTutar = indirimliFiyat * miktar;

                using (SqlConnection connection = new SqlConnection("Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True"))
                {
                    connection.Open();
                    string query = "INSERT INTO Sepet (UrunKodu, UrunAdi, Miktar, Fiyat, IndirimliFiyat, IndirimTutari, ToplamTutar) " +
                                   "VALUES (@UrunKodu, @UrunAdi, @Miktar, @Fiyat, @IndirimliFiyat, @IndirimTutari, @ToplamTutar)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UrunKodu", urunKodu);
                    command.Parameters.AddWithValue("@UrunAdi", urunAdi);
                    command.Parameters.AddWithValue("@Miktar", miktar);
                    command.Parameters.AddWithValue("@Fiyat", urunFiyat);
                    command.Parameters.AddWithValue("@IndirimliFiyat", indirimliFiyat);
                    command.Parameters.AddWithValue("@IndirimTutari", urunFiyat - indirimliFiyat);
                    command.Parameters.AddWithValue("@ToplamTutar", toplamTutar);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Ürün sepete eklendi.");
                }

                LoadSepetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eksikleri Kontrol Ediniz: " + ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridViewSepet.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string urunKodu = row.Cells["UrunKodu"].Value.ToString();
                        string urunAdi = row.Cells["UrunAdi"].Value.ToString();
                        int miktar = Convert.ToInt32(row.Cells["Miktar"].Value);
                        decimal indirimliFiyat = Convert.ToDecimal(row.Cells["IndirimliFiyat"].Value);  // Ýndirimli fiyat
                        decimal toplamGelir = Convert.ToDecimal(row.Cells["ToplamTutar"].Value);  // Toplam tutar
                        DateTime satisTarihi = DateTime.Now;

                        // Gelirler tablosuna indirimli fiyatý ekliyoruz
                        string satisQuery = "INSERT INTO Gelirler (UrunKodu, UrunAdi, Miktar, Fiyat, ToplamGelir, SatisTarihi, GelirTipi) " +
                                            "VALUES (@UrunKodu, @UrunAdi, @Miktar, @Fiyat, @ToplamGelir, @SatisTarihi, @GelirTipi)";

                        using (SqlCommand satisCmd = new SqlCommand(satisQuery, connection))
                        {
                            satisCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                            satisCmd.Parameters.AddWithValue("@UrunAdi", urunAdi);
                            satisCmd.Parameters.AddWithValue("@Miktar", miktar);
                            satisCmd.Parameters.AddWithValue("@Fiyat", indirimliFiyat); // Burada indirimli fiyat kullanýlýyor
                            satisCmd.Parameters.AddWithValue("@ToplamGelir", toplamGelir); // Toplam tutar indirimli fiyat üzerinden hesaplanmýþ
                            satisCmd.Parameters.AddWithValue("@SatisTarihi", satisTarihi);
                            satisCmd.Parameters.AddWithValue("@GelirTipi", "Satýþ Geliri");

                            satisCmd.ExecuteNonQuery();
                        }

                        string updateStokQuery = "UPDATE Urunler SET Stok = Stok - @Miktar WHERE UrunKodu = @UrunKodu";
                        using (SqlCommand updateStokCmd = new SqlCommand(updateStokQuery, connection))
                        {
                            updateStokCmd.Parameters.AddWithValue("@Miktar", miktar);
                            updateStokCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                            updateStokCmd.ExecuteNonQuery();
                        }

                        string deleteQuery = "DELETE FROM Sepet WHERE UrunKodu = @UrunKodu";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }

                    LoadSepetData();
                    MessageBox.Show("Satýþ baþarýyla kaydedildi!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluþtu: " + ex.Message);
            }


        }
        private void StokKontrol()
        {
            try
            {
                // SQL sorgusu, stokta kalmayan ürünleri buluyor
                string query = "SELECT UrunAdi FROM Urunler WHERE Stok = 0";

                // SQL komutunu oluþturuyoruz
                SqlCommand cmd = new SqlCommand(query, new SqlConnection(connectionString));

                // Veritabaný baðlantýsýný açýyoruz
                cmd.Connection.Open();

                // Veriyi alýyoruz
                SqlDataReader reader = cmd.ExecuteReader();
                List<string> urunler = new List<string>();

                // Stokta olmayan ürünleri listeye ekliyoruz
                while (reader.Read())
                {
                    urunler.Add(reader["UrunAdi"].ToString());
                }

                // Veritabaný baðlantýsýný kapatýyoruz
                cmd.Connection.Close();

                // Stokta olmayan ürünler varsa, MessageBox ile gösteriyoruz
                if (urunler.Count > 0)
                {
                    string urunlerListesi = string.Join(Environment.NewLine, urunler);
                    MessageBox.Show("Stokta olmayan ürünler:\n" + urunlerListesi, "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string query = "SELECT UrunAdi FROM Urunler";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                comboBoxSearch.Items.Clear();

                while (reader.Read())
                {
                    comboBoxSearch.Items.Add(reader["UrunAdi"].ToString());
                }
            }
        }

        private void button90_Click(object sender, EventArgs e)
        {
            try
            {
                // DataGridView'den seçilen satýrý alýyoruz
                if (dataGridViewSepet.SelectedRows.Count > 0)
                {
                    // Seçilen satýrdaki UrunKodu'nu alýyoruz
                    string urunKodu = dataGridViewSepet.SelectedRows[0].Cells["UrunKodu"].Value.ToString();

                    // SQL baðlantýsý açýlýyor
                    string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Sepet tablosundan ürünü silmek için SQL sorgusu
                        string deleteQuery = "DELETE FROM Sepet WHERE UrunKodu = @UrunKodu";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);

                            // Komut çalýþtýrýlýyor
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            // Eðer ürün baþarýyla silindiyse
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Ürün sepetteki listeden kaldýrýldý.");
                                LoadSepetData();  // Sepet verisini yeniden yükleyelim
                            }
                            else
                            {
                                MessageBox.Show("Silme iþlemi sýrasýnda bir hata oluþtu.");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lütfen silmek istediðiniz ürünü seçin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluþtu: " + ex.Message);
            }
        }
    }
}