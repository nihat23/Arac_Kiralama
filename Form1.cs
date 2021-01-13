using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.OleDb;//acces kutuphanemizi eklıyoruz..
using System.IO;

namespace Araç_Kiralama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        OleDbConnection baglanti = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "\\Arac_Kayit.accdb");
        OleDbDataAdapter liste_okuma;
        OleDbCommand komutver;
        OleDbDataReader sorgu_okuma;
        DataTable tablo;
        OleDbCommandBuilder guncelle;

        void listem()
        {
            baglanti.Open();
            liste_okuma = new OleDbDataAdapter("select tc AS[TC],AdiSoyadi AS[ADI SOYADI],Telefon AS[TELEFON],Adres AS[ADRES],MarkaModel AS[ARAÇ MARKA/MODEL],K_tarih AS[KİRALAMA TARİH],G_Tarih AS[BİTİŞ TARİH],Resim AS[RESİM],S_Fiyat AS[SAAT],G_fiyat AS[GERİ TESLİM SAAT],T_Tutar AS[TOPLAM TUTAR],odeme  AS[ÖDEME DURUMU]   from Arac_Bilgiler", baglanti);
            tablo = new DataTable();
            liste_okuma.Fill(tablo);

            dataGridView1.DataSource = tablo;
            baglanti.Close();

        }
        void temizle()
        {

            textBox11ToplamTutar.Clear();
            textBox1tc.Clear();
            textBox2AdiSoyadi.Clear();
            textBox3Tlf.Clear();
            textBox4Adres.Clear();
            textBox5Model.Clear();
            textBox8Resim.Clear();
            pictureBox1.ImageLocation = null;
            textBox10GecenSaat.Text = "0";
            textBox9SaatFiyat.Text = "0";
            textBox11ToplamTutar.Text = "0";
        }

        void hasilat()
        {
            baglanti.Open();
            komutver = new OleDbCommand("select sum(T_Tutar) from Arac_Bilgiler", baglanti);
            label13hasilat.Text = komutver.ExecuteScalar().ToString() + " TL";
            baglanti.Close();
        }
        void kiralamaSayisi()
        {
            baglanti.Open();
            komutver = new OleDbCommand("select count(tc) from Arac_Bilgiler", baglanti);
            label14Arac_Sayisi.Text = komutver.ExecuteScalar().ToString();
            baglanti.Close();
        }

        void renklendirme()
        {
            for (int i = 0; i < dataGridView1.Rows.Count ; i++)
            {
                DataGridViewCellStyle renkler = new DataGridViewCellStyle();

                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells[11].Value) == true)
                {
                    renkler.BackColor = Color.Red;
                }
                else
                {
                    renkler.BackColor = Color.White;
                }
                dataGridView1.Rows[i].DefaultCellStyle = renkler;
            }
        }
        private void button1Ekle_Click(object sender, EventArgs e)
        {
            bool varmi = false;
            if (textBox1tc.Text.Length >= 11)
            {
                baglanti.Open();
                komutver = new OleDbCommand("select *from Arac_Bilgiler where tc='" + textBox1tc.Text + "'", baglanti);
                sorgu_okuma = komutver.ExecuteReader();
                while (sorgu_okuma.Read())
                {
                    varmi = true;
                    MessageBox.Show("Girdiginiz tc numarada kayıt var..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    break;
                }
                baglanti.Close();

                if (varmi == false)
                {
                    if (textBox1tc.Text != "" && textBox2AdiSoyadi.Text != "" && textBox3Tlf.Text != "" && textBox4Adres.Text != "" && textBox5Model.Text != "" && dateTimePicker1Kiralanan.Text != "" && dateTimePicker2Geri.Text != "" && textBox8Resim.Text != "" && textBox9SaatFiyat.Text != "" && textBox10GecenSaat.Text != "")
                    {
                        baglanti.Open();                                           //
                        komutver = new OleDbCommand("insert into Arac_Bilgiler(tc,AdiSoyadi,Telefon,Adres,MarkaModel,K_tarih,G_Tarih,Resim,S_Fiyat,G_fiyat,T_Tutar) values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11)", baglanti);
                        komutver.Parameters.AddWithValue("@p1", textBox1tc.Text);
                        komutver.Parameters.AddWithValue("@p2", textBox2AdiSoyadi.Text);
                        komutver.Parameters.AddWithValue("@p3", textBox3Tlf.Text);
                        komutver.Parameters.AddWithValue("@p4", textBox4Adres.Text);
                        komutver.Parameters.AddWithValue("@p5", textBox5Model.Text);
                        komutver.Parameters.AddWithValue("@p6", dateTimePicker1Kiralanan.Value.ToShortDateString());
                        komutver.Parameters.AddWithValue("@p7", dateTimePicker2Geri.Value.ToShortDateString());
                        komutver.Parameters.AddWithValue("@p8", textBox8Resim.Text);
                        komutver.Parameters.AddWithValue("@p9", Convert.ToDouble(textBox9SaatFiyat.Text));
                        komutver.Parameters.AddWithValue("@p10", Convert.ToDouble(textBox10GecenSaat.Text));
                        komutver.Parameters.AddWithValue("@p11", Convert.ToDouble(textBox11ToplamTutar.Text.ToString()));
                        komutver.ExecuteNonQuery();
                        MessageBox.Show("Kayıt Eklendi..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (!Directory.Exists(Application.StartupPath + "\\ArabaResimler"))
                        {
                            Directory.CreateDirectory(Application.StartupPath + "\\ArabaResimler");
                            pictureBox1.Image.Save(Application.StartupPath + "\\ArabaResimler\\" + textBox1tc.Text + ".jpg");
                            MessageBox.Show("Klasör oluşturuldu ve Resim eklendi ..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        baglanti.Close();
                        listem();
                        temizle();
                        hasilat();
                        kiralamaSayisi();
                        renklendirme();

                    }
                    else
                    {
                        MessageBox.Show("Alanları Boş Geçmeyiniz..", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }


            }
            else
            {
                MessageBox.Show("Lütfen 11 Hane Tc Numarası Giriniz..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            listem();
            hasilat();
            kiralamaSayisi();
            renklendirme();


        }

        private void textBox1tc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 126 || e.KeyChar < 58)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox3Tlf_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 126 || e.KeyChar < 58)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox9SaatFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 126 || e.KeyChar < 58)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox10GecenSaat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar > 126 || e.KeyChar < 58)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void button6Resim_Click(object sender, EventArgs e)
        {
            OpenFileDialog resimac = new OpenFileDialog();
            resimac.Filter = "Resim Dosyası|*.jpg;*.png;*.bmp;*.jpeg";
            if (resimac.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = resimac.FileName;
                textBox8Resim.Text = resimac.FileName;
            }
        }

        private void button2Duzenle_Click(object sender, EventArgs e)
        {
            if (textBox1tc.Text != "" && textBox2AdiSoyadi.Text != "" && textBox3Tlf.Text != "" && textBox4Adres.Text != "" && textBox5Model.Text != "" && dateTimePicker1Kiralanan.Text != "" && dateTimePicker2Geri.Text != "" && textBox8Resim.Text != "" && textBox9SaatFiyat.Text != "" && textBox10GecenSaat.Text != "")
            {
                baglanti.Open();
                komutver = new OleDbCommand("update Arac_Bilgiler set AdiSoyadi=@p1,Telefon=@p2,Adres=@p3,MarkaModel=@p4,K_tarih=@p5,G_Tarih=@p6,Resim=@p7,S_Fiyat=@p8,G_fiyat=@p9,T_Tutar=@p10 where tc=@p11", baglanti);
                komutver.Parameters.AddWithValue("@p1", textBox2AdiSoyadi.Text);
                komutver.Parameters.AddWithValue("@p2", textBox3Tlf.Text);
                komutver.Parameters.AddWithValue("@p3", textBox4Adres.Text);
                komutver.Parameters.AddWithValue("@p4", textBox5Model.Text);
                komutver.Parameters.AddWithValue("@p5", dateTimePicker1Kiralanan.Value.ToShortDateString());
                komutver.Parameters.AddWithValue("@p6", dateTimePicker2Geri.Value.ToShortDateString());
                komutver.Parameters.AddWithValue("@p7", textBox8Resim.Text);
                komutver.Parameters.AddWithValue("@p8", textBox9SaatFiyat.Text);
                komutver.Parameters.AddWithValue("@p9", textBox10GecenSaat.Text);
                komutver.Parameters.AddWithValue("@p10", textBox11ToplamTutar.Text);
                komutver.Parameters.AddWithValue("@p11", textBox1tc.Text);//where tc
                komutver.ExecuteNonQuery();
                baglanti.Close();
                listem();
                hasilat();
                kiralamaSayisi();
                renklendirme();
                MessageBox.Show("Kayıtlar Güncellendi.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Question);

            }
            else
            {
                MessageBox.Show("Alanları boş geçmeyiniz..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            textBox1tc.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
            textBox2AdiSoyadi.Text = dataGridView1.Rows[secilen].Cells[1].Value.ToString();
            textBox3Tlf.Text = dataGridView1.Rows[secilen].Cells[2].Value.ToString();
            textBox4Adres.Text = dataGridView1.Rows[secilen].Cells[3].Value.ToString();
            textBox5Model.Text = dataGridView1.Rows[secilen].Cells[4].Value.ToString();
            dateTimePicker1Kiralanan.Text = dataGridView1.Rows[secilen].Cells[5].Value.ToString();
            dateTimePicker2Geri.Text = dataGridView1.Rows[secilen].Cells[6].Value.ToString();
            textBox8Resim.Text = dataGridView1.Rows[secilen].Cells[7].Value.ToString();
            textBox9SaatFiyat.Text = dataGridView1.Rows[secilen].Cells[8].Value.ToString();
            textBox10GecenSaat.Text = dataGridView1.Rows[secilen].Cells[9].Value.ToString();
            textBox11ToplamTutar.Text = dataGridView1.Rows[secilen].Cells[10].Value.ToString();
            pictureBox1.ImageLocation = dataGridView1.Rows[secilen].Cells[7].Value.ToString();
        }

        private void textBox9SaatFiyat_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox9SaatFiyat.Text != "" && textBox10GecenSaat.Text != "")
                {
                    double sayi1 = 0, sayi2 = 0, toplam;
                    sayi1 = double.Parse(textBox9SaatFiyat.Text);
                    sayi2 = double.Parse(textBox10GecenSaat.Text);
                    toplam = sayi1 * sayi2;
                    textBox11ToplamTutar.Text = toplam.ToString("N");
                }
                else
                {
                    MessageBox.Show("Lütfen Bir Deger Giriniz", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Lütfen Bir Deger Giriniz", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

        }

        private void textBox10GecenSaat_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox10GecenSaat.Text != "" || textBox9SaatFiyat.Text != "")
                {
                    double sayi1 = 0, sayi2 = 0, toplam;
                    sayi1 = double.Parse(textBox9SaatFiyat.Text);
                    sayi2 = double.Parse(textBox10GecenSaat.Text);
                    toplam = sayi1 * sayi2;
                    textBox11ToplamTutar.Text = toplam.ToString("N");
                }
                else
                {
                    MessageBox.Show("Lütfen Bir Deger Giriniz", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Lütfen Bir Deger Giriniz", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }


        }

        private void button3Sil_Click(object sender, EventArgs e)
        {
            DialogResult cvp = MessageBox.Show("Silmek istediginizden eminmisiniz..", "Bilgilendirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (cvp == DialogResult.Yes)
            {
                if (textBox1tc.Text != "")
                {
                    baglanti.Open();
                    komutver = new OleDbCommand("delete from Arac_Bilgiler where tc='" + textBox1tc.Text + "'", baglanti);
                    komutver.ExecuteNonQuery();
                    baglanti.Close();
                    MessageBox.Show("Kayıt listeden silindi..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    listem();
                    temizle();
                    hasilat();
                    kiralamaSayisi();
                    renklendirme();

                }
                else
                {
                    MessageBox.Show("Silmek istediginiz TC Numarasını Yazınız..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

            }
            else if (cvp == DialogResult.No)
            {
                listem();
                renklendirme();

            }

        }

        private void button4Temizle_Click(object sender, EventArgs e)
        {
            temizle();
        }

        private void button5Arama_Click(object sender, EventArgs e)
        {
            if (textBox1tc.Text != "")
            {
                baglanti.Open();
                liste_okuma = new OleDbDataAdapter("select tc AS[TC],AdiSoyadi AS[ADI SOYADI],Telefon AS[TELEFON],Adres AS[ADRES],MarkaModel AS[ARAÇ MARKA/MODEL],K_tarih AS[KİRALAMA TARİH],G_Tarih AS[BİTİŞ TARİH],Resim AS[RESİM],S_Fiyat AS[SAAT],G_fiyat AS[GERİ TESLİM SAAT],T_Tutar AS[TOPLAM TUTAR],odeme  AS[ÖDEME DURUMU] from Arac_Bilgiler where tc like '" + textBox1tc.Text + "%'", baglanti);
                tablo = new DataTable();
                liste_okuma.Fill(tablo);

                dataGridView1.DataSource = tablo;
                baglanti.Close();
                renklendirme();
            }
            else
            {
                MessageBox.Show("Lütfen Aramak istediginiz TC numarası  giriniz..", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/n.beyi");
        }

        private void button1Yenile_Click(object sender, EventArgs e)
        {
            listem();
            hasilat();
            kiralamaSayisi();
            renklendirme();
        }

        private void button1Odeme_Click(object sender, EventArgs e)
        {
            guncelle = new OleDbCommandBuilder(liste_okuma);
            liste_okuma.Update(tablo);
            renklendirme();

        }
    }
}
