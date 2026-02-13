using System.Data.SqlClient;

namespace RentACar
{
    internal class frmSqlBaglantı
    {
        string adres = @"Data Source=DESKTOP-H6DMK6F;Initial Catalog=RentACarDB;Integrated Security=True;Encrypt=False";

        public SqlConnection baglan()
        {
            SqlConnection baglanti = new SqlConnection(adres);
            baglanti.Open();
            return baglanti;
        }
    }
}
