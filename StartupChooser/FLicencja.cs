//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Windows.Forms;
//using System.Security.Cryptography;
//using System.Threading;
//using System.Windows;
//using eMapy.Utils;

//namespace WymianaFTP
//{
//    public partial class FLicencja
//    {
//        private string[] paramsLic = null;

//        #region Crypt

//        private static string EncryptString(string InputText, string Password)
//        {
//            // We are now going to create an instance of the
//            // Rihndael class.
//            RijndaelManaged RijndaelCipher = new RijndaelManaged();

//            // First we need to turn the input strings into a byte array.
//            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);

//            // We are using salt to make it harder to guess our key
//            // using a dictionary attack.
//            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

//            // The (Secret Key) will be generated from the specified
//            // password and salt.
//            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

//            // Create a encryptor from the existing SecretKey bytes.
//            // We use 32 bytes for the secret key
//            // (the default Rijndael key length is 256 bit = 32 bytes) and
//            // then 16 bytes for the IV (initialization vector),
//            // (the default Rijndael IV length is 128 bit = 16 bytes)
//            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

//            // Create a MemoryStream that is going to hold the encrypted bytes
//            MemoryStream memoryStream = new MemoryStream();

//            // Create a CryptoStream through which we are going to be processing our data.
//            // CryptoStreamMode.Write means that we are going to be writing data
//            // to the stream and the output will be written in the MemoryStream
//            // we have provided. (always use write mode for encryption)
//            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

//            // Start the encryption process.
//            cryptoStream.Write(PlainText, 0, PlainText.Length);

//            // Finish encrypting.
//            cryptoStream.FlushFinalBlock();

//            // Convert our encrypted data from a memoryStream into a byte array.
//            byte[] CipherBytes = memoryStream.ToArray();

//            // Close both streams.
//            memoryStream.Close();
//            cryptoStream.Close();

//            // Convert encrypted data into a base64-encoded string.
//            // A common mistake would be to use an Encoding class for that.
//            // It does not work, because not all byte values can be
//            // represented by characters. We are going to be using Base64 encoding
//            // That is designed exactly for what we are trying to do.
//            string EncryptedData = Convert.ToBase64String(CipherBytes);

//            // Return encrypted string.
//            return EncryptedData;
//        }

//        private static string DecryptString(string InputText, string Password)
//        {
//            string DecryptedData = "";

//            try
//            {
//                RijndaelManaged RijndaelCipher = new RijndaelManaged();

//                byte[] EncryptedData = Convert.FromBase64String(InputText);
//                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

//                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

//                // Create a decryptor from the existing SecretKey bytes.
//                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

//                MemoryStream memoryStream = new MemoryStream(EncryptedData);

//                // Create a CryptoStream. (always use Read mode for decryption).
//                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

//                // Since at this point we don't know what the size of decrypted data
//                // will be, allocate the buffer long enough to hold EncryptedData;
//                // DecryptedData is never longer than EncryptedData.
//                byte[] PlainText = new byte[EncryptedData.Length];

//                // Start decrypting.
//                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

//                memoryStream.Close();
//                cryptoStream.Close();

//                // Convert decrypted data into a string.
//                DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
//            }
//            catch (Exception e)
//            {
//                //MessageBox.Show(e.Message, "WERYFIKACJA LICENCJI", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                //throw;
//                DecryptedData = "";
//            }

//            // Return decrypted string.
//            return DecryptedData;
//        }

//        #endregion Crypt

//        public FLicencja()
//        {
//            InitializeComponent();
//            PobierzDaneLicencji();
//        }

//        private void PobierzDaneLicencji()
//        {
//            this.txtNazwaLic.Text = IniFile.IniReadValue(IniFile.stxtNazwaLic);
//            this.txtNIPLic.Text = IniFile.IniReadValue(IniFile.stxtNIPLic);
//            this.RTxtKodProgLic.Text = IniFile.IniReadValue(IniFile.sRTxtKodProgLic);
//            this.RTxtKodAktLic.Text = IniFile.IniReadValue(IniFile.sRTxtKodAktLic);
//        }

//        private void ZapiszDaneLicencji()
//        {
//            IniFile.IniWriteValue(IniFile.stxtNazwaLic, this.txtNazwaLic.Text);
//            IniFile.IniWriteValue(IniFile.stxtNIPLic, this.txtNIPLic.Text);
//            IniFile.IniWriteValue(IniFile.sRTxtKodProgLic, this.RTxtKodProgLic.Text);
//            IniFile.IniWriteValue(IniFile.sRTxtKodAktLic, this.RTxtKodAktLic.Text);
//        }

//        public static bool AnalizujLicencje(ref string[] str, bool czyKomunikaty)
//        {
//            try
//            {
//                Thread.Sleep(10000);
//                string tempKodAktLic = "";
//                string tempKodProgLic = "";
//                string tempNazwaLic = "";
//                string tempNIPLic = "";
//                string tempIDLic = "";
//                string[] tabKodAktLic;
//                string[] tabKodProgLic;
//                bool iError = false;
//                DateTime dt = new DateTime(1900, 01, 01);

//                if (str == null)
//                {
//                    str = new string[2];
//                }

//                tempNazwaLic = IniFile.IniReadValue(IniFile.stxtNazwaLic);
//                tempNIPLic = konwertujNip(IniFile.IniReadValue(IniFile.stxtNIPLic));
//                tempIDLic = WMI.GetCPUId();

//                tempKodProgLic = IniFile.IniReadValue(IniFile.sRTxtKodProgLic);
//                tempKodProgLic = DecryptString(tempKodProgLic, "WymianaFTP");
//                tabKodProgLic = tempKodProgLic.Split('|');
//                tempKodAktLic = IniFile.IniReadValue(IniFile.sRTxtKodAktLic);
//                tempKodAktLic = tempKodAktLic.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace(" ", "");
//                tempKodAktLic = DecryptString(tempKodAktLic, "WymianaFTP");

//                tabKodAktLic = tempKodAktLic.Split('|');

//                if ((tabKodProgLic.Length == 3) && (tabKodAktLic.Length == 5))
//                {
//                    if ((tabKodProgLic[0] == tabKodAktLic[4]) && (tabKodAktLic[4] == tempNazwaLic))
//                    {
//                        if ((tabKodProgLic[1] == tabKodAktLic[3]) && (tabKodAktLic[3] == tempNIPLic))
//                        {
//                            if ((tabKodProgLic[2] == tabKodAktLic[2]) && (tabKodAktLic[2] == tempIDLic))
//                            {
//                                switch (tabKodAktLic.Length)
//                                {
//                                    case 5:
//                                        if ((tabKodAktLic[1] == "DEMO") || (tabKodAktLic[1] == "PE£NA") || (tabKodAktLic[1] == "IMPORT") || (tabKodAktLic[1] == "EKSPORT"))
//                                        {
//                                            if (tabKodAktLic[1] == "DEMO")
//                                            {
//                                                if (DateTime.TryParse(tabKodAktLic[0], out dt))
//                                                {
//                                                    iError = false;
//                                                }
//                                                else
//                                                {
//                                                    iError = true;
//                                                }
//                                            }
//                                            else
//                                            {
//                                                if (tabKodAktLic[0] == "")
//                                                {
//                                                    iError = false;
//                                                }
//                                                else
//                                                {
//                                                    iError = true;
//                                                }
//                                            }
//                                        }
//                                        break;

//                                    default:
//                                        iError = true;
//                                        break;
//                                }
//                            }
//                            else
//                            {
//                                iError = true;
//                            }
//                        }
//                        else
//                        {
//                            iError = true;
//                        }
//                    }
//                    else
//                    {
//                        iError = true;
//                    }
//                }
//                else
//                {
//                    iError = true;
//                }

//                if (iError == true)
//                {
//                    if (czyKomunikaty == true)
//                    {
//                        MessageBox.Show("          B³êdny kod aktywacyjny !              ", "B£¥D LICENCJI", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    }
//                    return (false);
//                }
//                else
//                {
//                    str[0] = tabKodAktLic[1];
//                    str[1] = dt.ToShortDateString();
//                    return (true);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//                return (false);
//            }
//        }

//        public static string konwertujNip(string sNIP)
//        {
//            string sNumbers = "";
//            MatchCollection numbers = Regex.Matches(sNIP, (@"(\d+\.?\d*|\.\d+)"));

//            foreach (Match number in numbers)
//            {
//                sNumbers += number.Value;
//            }
//            return (sNumbers);
//        }

//        private void btnLLicencja_Click(object sender, EventArgs e)
//        {
//            System.Diagnostics.Process.Start("http://www.digit-al.pl/Umowa_Licencyjna_raporty.pdf");
//        }

//        public static void PokazDialog()
//        {
//            FLicencja frm = new FLicencja();

//            frm.ShowDialog();
//        }

//        private void blokujPanel(bool czyAkt)
//        {
//            this.txtNazwaLic.Enabled = czyAkt;
//            this.txtNIPLic.Enabled = czyAkt;
//            this.btnLGenKlucz.Enabled = czyAkt;
//            this.btnLOK.Enabled = czyAkt;
//            this.btnLLicencja.Enabled = czyAkt;
//        }

//        private void btnLGenKlucz_Click(object sender, EventArgs e)
//        {
//            //MessageBox.Show(EncryptString(this.txtNazwaLic.Text + this.txtNIPLic.Text + WMI.GetCPUId(), "WymianaFTP"));
//            blokujPanel(false);

//            this.RTxtKodAktLic.Text = "";
//            this.RTxtKodProgLic.Text = EncryptString(this.txtNazwaLic.Text + "|" + konwertujNip(this.txtNIPLic.Text) + "|" + WMI.GetCPUId(), "WymianaFTP");

//            blokujPanel(true);

//            //SymCryptography crypt = new SymCryptography(SymCryptography.ServiceProviderEnum.TripleDES);
//            //MessageBox.Show(crypt.Encrypt(this.txtNazwaLic.Text + this.txtNIPLic.Text + WMI.GetCPUId()));
//            //MessageBox.Show(crypt.Encrypt(this.txtNIPLic.Text + WMI.GetCPUId()));
//            //this.txtKluczLic.Text = crypt.Encrypt(this.txtNIPLic.Text + WMI.GetCPUId());
//        }

//        private void btnLOK_Click(object sender, EventArgs e)
//        {
//            ZapiszDaneLicencji();
//            if (AnalizujLicencje(ref paramsLic, true) == true)
//            {
//                Close();
//            }
//        }

//        private void btnLAnuluj_Click(object sender, EventArgs e)
//        {
//            Close();
//        }
//    }
//}