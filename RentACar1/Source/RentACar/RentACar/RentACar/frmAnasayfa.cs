using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentACar
{
    public partial class frmAnasayfa : Form
    {
        public frmAnasayfa()
        {
            InitializeComponent();
        }

        // Veritabanı bağlantı sınıfı
        frmSqlBaglantı bgl = new frmSqlBaglantı();

        private void frmAnasayfa_Load(object sender, EventArgs e)
        {
            Listele();
            AracDoldur();
        }

        // 1. LİSTELEME: Tüm kayıtları isimlerle birlikte getirir
        private void Listele()
        {
            try
            {
                string sorgu = @"SELECT K.KiralamaID, M.AdSoyad, M.TCNo, M.Telefon, 
                                 A.MarkaModel, A.GunlukUcret, K.BaslangicTarihi, K.BitisTarihi, 
                                 K.MusteriID, K.AracID 
                                 FROM tbl_Kiralamalar K 
                                 INNER JOIN tbl_Musteriler M ON K.MusteriID = M.MusteriID 
                                 INNER JOIN tbl_Araclar A ON K.AracID = A.AracID";

                SqlDataAdapter da = new SqlDataAdapter(sorgu, bgl.baglan());
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 
            }
            catch (Exception ex) { MessageBox.Show("Liste Hatası: " + ex.Message); }
            finally { bgl.baglan().Close(); }
        }

        // 2. ARAÇ DOLDURMA
        private void AracDoldur()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT AracID, MarkaModel, GunlukUcret FROM tbl_Araclar WHERE Durum = 2", bgl.baglan());
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbMarka.DataSource = dt;
                cmbMarka.DisplayMember = "MarkaModel";
                cmbMarka.ValueMember = "AracID";
                cmbMarka.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Araç Doldurma Hatası: " + ex.Message); }
            finally { bgl.baglan().Close(); }
        }

        // 3. SEÇİM DEĞİŞTİĞİNDE ÜCRET GÖSTER
        private void cmbMarka_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMarka.SelectedIndex != -1 && cmbMarka.SelectedItem != null)
            {
                DataRowView satir = (DataRowView)cmbMarka.SelectedItem;
                txtGunlukUcret.Text = satir["GunlukUcret"].ToString();
                Hesapla();
            }
        }

        // 4. GÜN HESAPLAMA
        private void Hesapla()
        {
            TimeSpan fark = dtpBitis.Value.Date - dtpBaslangic.Value.Date;
            int gun = fark.Days;
            txtToplamGün.Text = gun > 0 ? gun.ToString() : "1";
        }

        private void dtpBaslangic_ValueChanged(object sender, EventArgs e) => Hesapla();
        private void dtpBitis_ValueChanged(object sender, EventArgs e) => Hesapla();

        // 5. KAYDET BUTONU
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbMarka.SelectedValue == null || string.IsNullOrEmpty(txtAdSoyad.Text))
            {
                MessageBox.Show("Lütfen müşteri bilgilerini ve araç seçimini tamamlayınız!", "Uyarı");
                return;
            }

            try
            {
                SqlCommand komut = new SqlCommand("AracKirala", bgl.baglan());
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@adSoyad", txtAdSoyad.Text);
                komut.Parameters.AddWithValue("@tcNo", txtTcNo.Text);
                komut.Parameters.AddWithValue("@tel", txtTel.Text);
                komut.Parameters.AddWithValue("@aracID", cmbMarka.SelectedValue);
                komut.Parameters.AddWithValue("@baslangic", dtpBaslangic.Value);
                komut.Parameters.AddWithValue("@bitis", dtpBitis.Value);
                komut.Parameters.AddWithValue("@tutar", 0);

                komut.ExecuteNonQuery();
                MessageBox.Show("Kiralama Başarıyla Tamamlandı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Listele();
                AracDoldur();
                temizle();
            }
            catch (Exception ex) { MessageBox.Show("Kayıt Hatası: " + ex.Message); }
            finally { bgl.baglan().Close(); }
        }

        // 6. GÜNCELLEME İŞLEMİ (Yeni SQL Prosedürü ile)
        private void btnGüncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMusteriID.Text))
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz kaydı listeden seçin!", "Uyarı");
                return;
            }

            DialogResult dr = MessageBox.Show($"{txtMusteriID.Text} ID'li kayıt güncellenecek. Onaylıyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    SqlCommand guncelleKomut = new SqlCommand("guncelle", bgl.baglan());
                    guncelleKomut.CommandType = CommandType.StoredProcedure;

                    // Prosedür parametrelerini formdaki kutularla eşleştiriyoruz
                    guncelleKomut.Parameters.AddWithValue("@kiraID", int.Parse(txtMusteriID.Text));
                    guncelleKomut.Parameters.AddWithValue("@ad", txtAdSoyad.Text);
                    guncelleKomut.Parameters.AddWithValue("@tc", txtTcNo.Text);
                    guncelleKomut.Parameters.AddWithValue("@tel", txtTel.Text);
                    guncelleKomut.Parameters.AddWithValue("@aracID", cmbMarka.SelectedValue);
                    guncelleKomut.Parameters.AddWithValue("@baslangic", dtpBaslangic.Value);
                    guncelleKomut.Parameters.AddWithValue("@bitis", dtpBitis.Value);

                    guncelleKomut.ExecuteNonQuery();
                    MessageBox.Show("Güncelleme İşlemi Başarılı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Listele();
                }
                catch (Exception ex) { MessageBox.Show("Güncelleme Hatası: " + ex.Message); }
                finally { bgl.baglan().Close(); }
            }
        }

        // 7. SİLME BUTONU
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMusteriID.Text)) return;

            DialogResult dr = MessageBox.Show("Seçili kaydı silmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    SqlCommand sil = new SqlCommand("DELETE FROM tbl_Kiralamalar WHERE KiralamaID=@id", bgl.baglan());
                    sil.Parameters.AddWithValue("@id", txtMusteriID.Text);
                    sil.ExecuteNonQuery();

                    SqlCommand bosalt = new SqlCommand("UPDATE tbl_Araclar SET Durum=2 WHERE MarkaModel=@marka", bgl.baglan());
                    bosalt.Parameters.AddWithValue("@marka", cmbMarka.Text);
                    bosalt.ExecuteNonQuery();

                    MessageBox.Show("Kayıt başarıyla silindi.");
                    Listele();
                    AracDoldur();
                    temizle();
                }
                catch (Exception ex) { MessageBox.Show("Silme Hatası: " + ex.Message); }
                finally { bgl.baglan().Close(); }
            }
        }

        // 8. HÜCREYE TIKLANDIĞINDA VERİLERİ ÇEK
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int secilen = dataGridView1.SelectedCells[0].RowIndex;

                txtMusteriID.Text = dataGridView1.Rows[secilen].Cells["KiralamaID"].Value.ToString();
                txtAdSoyad.Text = dataGridView1.Rows[secilen].Cells["AdSoyad"].Value.ToString();
                txtTcNo.Text = dataGridView1.Rows[secilen].Cells["TCNo"].Value.ToString();
                txtTel.Text = dataGridView1.Rows[secilen].Cells["Telefon"].Value.ToString();
                cmbMarka.Text = dataGridView1.Rows[secilen].Cells["MarkaModel"].Value.ToString();
                txtGunlukUcret.Text = dataGridView1.Rows[secilen].Cells["GunlukUcret"].Value.ToString();

                dtpBaslangic.Value = Convert.ToDateTime(dataGridView1.Rows[secilen].Cells["BaslangicTarihi"].Value);
                dtpBitis.Value = Convert.ToDateTime(dataGridView1.Rows[secilen].Cells["BitisTarihi"].Value);

                Hesapla();
            }
        }

        private void temizle()
        {
            txtMusteriID.Clear();
            txtAdSoyad.Clear();
            txtTcNo.Clear();
            txtTel.Clear();
            txtGunlukUcret.Clear();
            txtToplamGün.Text = "0";
            cmbMarka.SelectedIndex = -1;
        }

        private void btnFormuTemizle_Click(object sender, EventArgs e) => temizle();
        private void btnListele_Click(object sender, EventArgs e) => Listele();

        private void label6_Click(object sender, EventArgs e) { }
        private void btnİstatistik_Click(object sender, EventArgs e)
        { // Daha önce oluşturduğumuz istatistik formunu bir nesne olarak tanımlıyoruz
            frmİstatistic fr = new frmİstatistic();

            // Formu ekranda gösteriyoruz
            fr.Show();
        }
    }
}