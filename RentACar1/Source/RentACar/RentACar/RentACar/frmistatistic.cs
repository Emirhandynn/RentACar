using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentACar
{
    public partial class frmİstatistic : Form
    {
        public frmİstatistic()
        {
            InitializeComponent();
        }

        // Veritabanı bağlantı sınıfın
        frmSqlBaglantı bgl = new frmSqlBaglantı();

        private void frmİstatistic_Load(object sender, EventArgs e)
        {
            toplamKira();
            ortalamaGun();
        }

        private void toplamKira()
        {
            try
            {
                // Toplam kiralama sayısını çeker
                SqlCommand toplam = new SqlCommand("SELECT COUNT(*) FROM tbl_Kiralamalar", bgl.baglan());
                SqlDataReader dr = toplam.ExecuteReader();
                if (dr.Read())
                {
                    // ist1.PNG'deki label isminin doğruluğundan emin ol
                    lblToplamKira.Text = dr[0].ToString();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata (Toplam): " + ex.Message); }
            finally { bgl.baglan().Close(); }
        }

        private void ortalamaGun()
        {
            try
            {
                // Tarihler arasındaki farkın ortalamasını alır. Tablo boşsa 0 döner.
                // Hesaplama: ISNULL(AVG(DATEDIFF), 0)
                string sorgu = "SELECT ISNULL(AVG(DATEDIFF(day, BaslangicTarihi, BitisTarihi)), 0) FROM tbl_Kiralamalar";
                SqlCommand ortalama = new SqlCommand(sorgu, bgl.baglan());
                SqlDataReader dr = ortalama.ExecuteReader();
                if (dr.Read())
                {
                    // ist1.PNG'deki label isminin doğruluğundan emin ol
                    lblOrtalamaGun.Text = dr[0].ToString();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata (Ortalama): " + ex.Message); }
            finally { bgl.baglan().Close(); }
        }
    }
}