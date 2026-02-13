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

namespace RentACar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        frmSqlBaglantı bgl = new frmSqlBaglantı();

        // HATA ÇÖZÜMÜ: Designer dosyasında beklenen boş metotları ekledik
        private void label2_Click(object sender, EventArgs e)
        {
            // Boş kalabilir, hata gitmesi için gereklidir
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Boş kalabilir, hata gitmesi için gereklidir
        }

        // Kayıt Ol Formunu Açan Buton
        private void btnKayit_Click(object sender, EventArgs e)
        {
            frmkayıt fr = new frmkayıt();
            fr.Show();
        }

        // Giriş Yapan Buton
        private void btnGiris_Click(object sender, EventArgs e)
        {
            if (txtKulAdi.Text != "" && txtSifre.Text != "")
            {
                // Giriş prosedürünü çağırıyoruz
                SqlCommand komut = new SqlCommand("sp_GirisYap", bgl.baglan());
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@kullaniciAdi", txtKulAdi.Text);
                komut.Parameters.AddWithValue("@sifre", txtSifre.Text);

                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    MessageBox.Show("Giriş Başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Anasayfaya yönlendirme
                    frmAnasayfa fr = new frmAnasayfa();
                    this.Hide();
                    fr.Show();
                }
                else
                {
                    MessageBox.Show("Hatalı Kullanıcı Adı veya Şifre", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                bgl.baglan().Close();
            }
            else
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre alanlarını doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}