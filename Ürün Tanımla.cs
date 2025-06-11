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
using Microsoft.Data.SqlClient;


namespace Turkcell_Akif_Abi
{
    public partial class Ürün_Tanımla : Form
    {
        private string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";
        public Ürün_Tanımla()
        {
            InitializeComponent();
        }

        private void Ürün_Tanımla_Load(object sender, EventArgs e)
        {
            KategorileriDoldur();
            MarkalariDoldur();
            urunlerilistele();
        }
        private void KategorileriDoldur()
        {
            kategoricombo.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT KategoriAdi FROM Kategoriler", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    kategoricombo.Items.Add(reader["KategoriAdi"].ToString());
                }
            }
        }
        private void MarkalariDoldur()
        {
            markacombo.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT MarkaAdi FROM Markalar", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    markacombo.Items.Add(reader["MarkaAdi"].ToString());
                }
            }
        }

        private void urunlerilistele()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT UrunAdi FROM Urunler WHERE Stok > 0"; // Stokta olan ürünleri getiriyoruz
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    comboBoxUrunler.Items.Add(reader["UrunAdi"].ToString());
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı yeni stok miktarını girmeli
                int yeniStok = Convert.ToInt32(stoktxt.Text);  // Bu, satın alınan miktar olacak

                // Ürün adı boşsa kullanıcıya uyarı gösterelim
                if (string.IsNullOrWhiteSpace(uaditxt.Text))
                {
                    MessageBox.Show("Ürün adı boş olamaz.");
                    return;
                }

                string urunKodu = kodtxt.Text;
                string urunAdi = uaditxt.Text; // Ürün adı buradan alınacak
                decimal urunAlisFiyati = Convert.ToDecimal(ualisfiyattxt.Text);
                decimal urunSatisFiyati = Convert.ToDecimal(usatisfiyattxt.Text);
                int stok = yeniStok;  // Burada yeni girilen stok miktarını kullanıyoruz
                string kategori = kategoricombo.SelectedItem.ToString();
                string marka = markacombo.SelectedItem.ToString();

                byte[] resimBytes = null;
                if (pictureBoxUrunResmi.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pictureBoxUrunResmi.Image.Save(ms, pictureBoxUrunResmi.Image.RawFormat);
                        resimBytes = ms.ToArray();
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Ürün zaten var mı kontrolü
                    string checkQuery = "SELECT Stok FROM Urunler WHERE UrunKodu = @UrunKodu AND UrunAdi = @UrunAdi";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                    checkCmd.Parameters.AddWithValue("@UrunAdi", urunAdi);

                    object result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        int mevcutStok = Convert.ToInt32(result);

                        // Yeni stok miktarı hesaplanacak
                        int yeniStokMiktari = mevcutStok + yeniStok;

                        // Stok güncelleme
                        string updateQuery = "UPDATE Urunler SET Stok = @YeniStok WHERE UrunKodu = @UrunKodu AND UrunAdi = @UrunAdi";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@YeniStok", yeniStokMiktari);
                        updateCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                        updateCmd.Parameters.AddWithValue("@UrunAdi", urunAdi);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Ürün bulunamadıysa yeni ürün ekleme işlemi yapılacak
                        string insertQuery = "INSERT INTO Urunler (UrunKodu, UrunAdi, UrunAlisFiyati, UrunSatisFiyati, Stok, Kategori, Marka, UrunResmi) " +
                                             "VALUES (@UrunKodu, @UrunAdi, @UrunAlisFiyati, @UrunSatisFiyati, @Stok, @Kategori, @Marka, @UrunResmi)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                        insertCmd.Parameters.AddWithValue("@UrunAdi", urunAdi);
                        insertCmd.Parameters.AddWithValue("@UrunAlisFiyati", urunAlisFiyati);
                        insertCmd.Parameters.AddWithValue("@UrunSatisFiyati", urunSatisFiyati);
                        insertCmd.Parameters.AddWithValue("@Stok", yeniStok);
                        insertCmd.Parameters.AddWithValue("@Kategori", kategori);
                        insertCmd.Parameters.AddWithValue("@Marka", marka);

                        if (resimBytes != null)
                        {
                            insertCmd.Parameters.AddWithValue("@UrunResmi", resimBytes);
                        }
                        else
                        {
                            insertCmd.Parameters.AddWithValue("@UrunResmi", DBNull.Value);
                        }

                        insertCmd.ExecuteNonQuery();
                    }

                    // Gider kaydı ekleme
                    decimal toplamGider = urunAlisFiyati * yeniStok;
                    string giderInsertQuery = "INSERT INTO Giderler (UrunKodu, UrunAdi, Miktar, AlisFiyati, ToplamGider, GiderTarihi, GiderTipi) " +
                                              "VALUES (@UrunKodu, @UrunAdi, @Miktar, @AlisFiyati, @ToplamGider, @GiderTarihi, 'Ürün Satın Alma')";
                    SqlCommand giderInsertCmd = new SqlCommand(giderInsertQuery, conn);
                    giderInsertCmd.Parameters.AddWithValue("@UrunKodu", urunKodu);
                    giderInsertCmd.Parameters.AddWithValue("@UrunAdi", urunAdi);
                    giderInsertCmd.Parameters.AddWithValue("@Miktar", yeniStok);
                    giderInsertCmd.Parameters.AddWithValue("@AlisFiyati", urunAlisFiyati);
                    giderInsertCmd.Parameters.AddWithValue("@ToplamGider", toplamGider);
                    giderInsertCmd.Parameters.AddWithValue("@GiderTarihi", DateTime.Now);

                    giderInsertCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Ürün başarıyla eklendi ve gider kaydedildi!");
                Ürün_Tanımla üt = new Ürün_Tanımla();
                üt.ResetText();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Veri format hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beklenmedik bir hata oluştu: " + ex.Message);
            }
        }

        private void katbut_Click(object sender, EventArgs e)
        {
            string kategoriAdi = kategoriekletxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(kategoriAdi))
            {
                MessageBox.Show("⚠️ Kategori adı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Kategoriler (KategoriAdi) VALUES (@KategoriAdi)", connection);
                    cmd.Parameters.AddWithValue("@KategoriAdi", kategoriAdi);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Kategori başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    KategorileriDoldur();
                    kategoriekletxt.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void marbut_Click(object sender, EventArgs e)
        {
            string markaAdi = markaekletxt.Text.Trim();

            if (string.IsNullOrWhiteSpace(markaAdi))
            {
                MessageBox.Show("⚠️ Marka adı boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Markalar (MarkaAdi) VALUES (@MarkaAdi)", connection);
                    cmd.Parameters.AddWithValue("@MarkaAdi", markaAdi);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("✅ Marka başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MarkalariDoldur();
                    markaekletxt.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void kategoriekletxt_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string resimYolu = openFileDialog.FileName;

                pictureBoxUrunResmi.Image = Image.FromFile(resimYolu);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxUrunler_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ComboBox'tan bir ürün seçildiğinde, ürün bilgilerini dolduralım
            if (comboBoxUrunler.SelectedItem != null)
            {
                string selectedProduct = comboBoxUrunler.SelectedItem.ToString();

                // Stokta mevcut ürün için veritabanından bilgileri alalım
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT UrunKodu, UrunAdi, UrunAlisFiyati, UrunSatisFiyati, Stok, Kategori, Marka FROM Urunler WHERE UrunAdi = @UrunAdi";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UrunAdi", selectedProduct);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Verileri alıp TextBox'lara otomatik dolduralım
                        kodtxt.Text = reader["UrunKodu"].ToString();
                        uaditxt.Text = reader["UrunAdi"].ToString();
                        ualisfiyattxt.Text = reader["UrunAlisFiyati"].ToString();
                        usatisfiyattxt.Text = reader["UrunSatisFiyati"].ToString();
                        // Buradaki stoktxt'yi kullanıcının yeni stok miktarını girebilmesi için bırakıyoruz.
                        kategoricombo.SelectedItem = reader["Kategori"].ToString();
                        markacombo.SelectedItem = reader["Marka"].ToString();
                    }
                }
            }
        }
    }
}
