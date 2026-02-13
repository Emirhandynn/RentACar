using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace RentACar
{
    public partial class frmkayıt : Form
    {
        public frmkayıt()
        {
            InitializeComponent();
        }

        frmSqlBaglantı bgl = new frmSqlBaglantı();

        private void btnKayit_Click(object sender, EventArgs e)
        {
            if (txtKulAdi.Text != "" && txtSifre.Text != "")
            {
                // DÜZELTME: Kayıt yapmak için sp_KayitOl prosedürünü kullanıyoruz
                SqlCommand komut = new SqlCommand("sp_KayitOl", bgl.baglan());
                komut.CommandType = CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@kullaniciAdi", txtKulAdi.Text);
                komut.Parameters.AddWithValue("@sifre", txtSifre.Text);

                // DÜZELTME: Veri eklemek için ExecuteNonQuery kullanılır
                komut.ExecuteNonQuery();
                bgl.baglan().Close();

                MessageBox.Show("Kaydınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close(); // Kayıt bittikten sonra bu formu kapat
            }
            else
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre belirleyiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}