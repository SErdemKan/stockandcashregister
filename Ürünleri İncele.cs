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
using Microsoft.Data.SqlClient;


namespace Turkcell_Akif_Abi
{
    public partial class Ürünleri_İncele : Form
    {
        public Ürünleri_İncele()
        {
            InitializeComponent();
        }
        string connectionString = "Data Source=ERDEM;Initial Catalog=bayi;Integrated Security=True;Trust Server Certificate=True";
        private void Ürünleri_İncele_Load(object sender, EventArgs e)
        {
            
            dataGridViewUrunler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUrunler.MultiSelect = false;

            LoadUrunler();
            datagridwievgörünüm();
            KategorileriDoldur();
            MarkalariDoldur();
            dataGridViewUrunler.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridViewUrunler_CellFormatting);


        }
        private void KategorileriDoldur()
        {
            cmbKategori.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT KategoriAdi FROM Kategoriler", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbKategori.Items.Add(reader["KategoriAdi"].ToString());
                }
            }
        }
        private void dataGridViewUrunler_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
            if (dataGridViewUrunler.Columns[e.ColumnIndex].Name == "urunresmi" && e.RowIndex >= 0)
            {
               
                if (e.Value != DBNull.Value)
                {
                   
                    byte[] imageData = (byte[])e.Value;

                    if (imageData != null && imageData.Length > 0)
                    {
                      
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            Image originalImage = Image.FromStream(ms);

                           
                            int newWidth = 50; 
                            int newHeight = 50; 
                            Image resizedImage = new Bitmap(originalImage, newWidth, newHeight);

                           
                            e.Value = resizedImage;
                        }
                    }
                    else
                    {
                        
                        e.Value = null; 
                    }
                }
                else
                {
                    
                    e.Value = null;
                }
            }

        }
        private void MarkalariDoldur()
        {
            cmbMarka.Items.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT MarkaAdi FROM Markalar", connection);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmbMarka.Items.Add(reader["MarkaAdi"].ToString());
                }
            }
        }
        private void LoadUrunler()
        {
            string query = "SELECT * FROM Urunler";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridViewUrunler.DataSource = table;
            }
        }
     

        private void btnUrunGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUrunKodu.Text) || string.IsNullOrEmpty(txtUrunAdi.Text))
                {
                    MessageBox.Show("Ürün Kodu ve Ürün Adı boş bırakılamaz!");
                    return;
                }

                string query = "UPDATE Urunler SET urunadi = @urunadi, urunalisfiyati = @urunalisfiyati, urunsatisfiyati = @urunsatisfiyati, stok = @stok, kategori = @kategori, marka = @marka WHERE urunkodu = @urunkodu";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@urunadi", txtUrunAdi.Text);
                    command.Parameters.AddWithValue("@urunalisfiyati", decimal.TryParse(txtAlisFiyati.Text, out decimal alisFiyati) ? alisFiyati : 0);
                    command.Parameters.AddWithValue("@urunsatisfiyati", decimal.TryParse(txtSatisFiyati.Text, out decimal satisFiyati) ? satisFiyati : 0);
                    command.Parameters.AddWithValue("@stok", int.TryParse(txtStok.Text, out int stok) ? stok : 0);
                    command.Parameters.AddWithValue("@kategori", cmbKategori.Text);
                    command.Parameters.AddWithValue("@marka", cmbMarka.Text);
                    command.Parameters.AddWithValue("@urunkodu", txtUrunKodu.Text);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Ürün başarıyla güncellendi!");
                        LoadUrunler(); 
                    }
                    else
                    {
                        MessageBox.Show("Ürün güncellenemedi. Lütfen tekrar deneyin.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
        private void datagridwievgörünüm()
        {
            dataGridViewUrunler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUrunler.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewUrunler.RowHeadersVisible = false;
            dataGridViewUrunler.AllowUserToAddRows = false;
            dataGridViewUrunler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnUrunSil_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM Urunler WHERE urunkodu=@urunkodu";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@urunkodu", txtUrunKodu.Text);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            LoadUrunler();
        }

        private void dataGridViewUrunler_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
               
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridViewUrunler.Rows[e.RowIndex];

                 
                    txtUrunKodu.Text = row.Cells["urunkodu"].Value.ToString();
                    txtUrunAdi.Text = row.Cells["urunadi"].Value.ToString();
                    txtAlisFiyati.Text = row.Cells["urunalisfiyati"].Value.ToString();
                    txtSatisFiyati.Text = row.Cells["urunsatisfiyati"].Value.ToString();
                    txtStok.Text = row.Cells["stok"].Value.ToString();
                    cmbKategori.Text = row.Cells["kategori"].Value.ToString();
                    cmbMarka.Text = row.Cells["marka"].Value.ToString();

                   
                    if (row.Cells["urunresmi"].Value != DBNull.Value)
                    {
                        byte[] imageData = (byte[])row.Cells["urunresmi"].Value;

                        if (imageData != null && imageData.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                Image image = Image.FromStream(ms);
                                pictureBox1.Image = image; 
                            }
                        }
                        else
                        {
                            pictureBox1.Image = null;
                        }
                    }
                    else
                    {
                        pictureBox1.Image = null; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
