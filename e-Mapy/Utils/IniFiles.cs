using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace eMapy.Utils
{
    public class IniFile
    {
        #region Global Ini KEYS

        #region Ustawienia

        //Ogólne
        //public static IniKey bAutoMin = new IniKey("Ustawienia Ogólne", "bAutoMin", "false");

        //Import
        //public static IniKey sSerwer = new IniKey("Ustawienia Import", "Serwer", "");
        //public static IniKey sBaza = new IniKey("Ustawienia Import", "Baza", "");
        //public static IniKey bZintegrowane = new IniKey("Ustawienia Import", "Zintegrowane", "false");
        //public static IniKey sHaslo = new IniKey("Ustawienia Import", "Haslo", "");
        //public static IniKey sUzytkownik = new IniKey("Ustawienia Import", "Uzytkownik", "");
        //public static IniKey bLaczAutomatycznie = new IniKey("Ustawienia Import", "LaczAutomatycznie", "true");
        //public static IniKey bZapiszUst = new IniKey("Ustawienia Import", "ZapiszUst", "false");
        //public static IniKey bInterCzyGodz = new IniKey("Ustawienia Import", "InterCzyGodz", "true");
        //public static IniKey sInterwal = new IniKey("Ustawienia Import", "Interwal", "01:00:00");
        //public static IniKey sGodzina = new IniKey("Ustawienia Import", "Godzina", "00:00:00");
        //public static IniKey sDataOstGodz = new IniKey("Ustawienia Import", "DataOstGodz", "2008-01-01");
        //public static IniKey sTimeOstGodz = new IniKey("Ustawienia Import", "TimeOstGodz", "00:00:00");
        //public static IniKey bImpAuto = new IniKey("Ustawienia Import", "ImpAuto", "false");

        ////Licencja
        //public static IniKey sRTxtKodProgLic = new IniKey("Ustawienia Licencja", "Kod programu", "");
        //public static IniKey sRTxtKodAktLic = new IniKey("Ustawienia Licencja", "Kod aktywacji", "");
        //public static IniKey stxtNazwaLic = new IniKey("Ustawienia Licencja", "Nazwa", "");
        //public static IniKey stxtNIPLic = new IniKey("Ustawienia Licencja", "NIP", "");

        #endregion Ustawienia

        #endregion Global Ini KEYS

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
                                            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                                            string key, string def, StringBuilder retVal,
                                            int size, string filePath);

        #region Crypt

        public static string EncryptString(string InputText, string Password)
        {
            // We are now going to create an instance of the
            // Rihndael class.
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            // First we need to turn the input strings into a byte array.
            byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);

            // We are using salt to make it harder to guess our key
            // using a dictionary attack.
            byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

            // The (Secret Key) will be generated from the specified
            // password and salt.
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

            // Create a encryptor from the existing SecretKey bytes.
            // We use 32 bytes for the secret key
            // (the default Rijndael key length is 256 bit = 32 bytes) and
            // then 16 bytes for the IV (initialization vector),
            // (the default Rijndael IV length is 128 bit = 16 bytes)
            ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

            // Create a MemoryStream that is going to hold the encrypted bytes
            MemoryStream memoryStream = new MemoryStream();

            // Create a CryptoStream through which we are going to be processing our data.
            // CryptoStreamMode.Write means that we are going to be writing data
            // to the stream and the output will be written in the MemoryStream
            // we have provided. (always use write mode for encryption)
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

            // Start the encryption process.
            cryptoStream.Write(PlainText, 0, PlainText.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memoryStream into a byte array.
            byte[] CipherBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            // A common mistake would be to use an Encoding class for that.
            // It does not work, because not all byte values can be
            // represented by characters. We are going to be using Base64 encoding
            // That is designed exactly for what we are trying to do.
            string EncryptedData = Convert.ToBase64String(CipherBytes);

            // Return encrypted string.
            return EncryptedData;
        }

        public static string DecryptString(string InputText, string Password)
        {
            try
            {
                RijndaelManaged RijndaelCipher = new RijndaelManaged();

                byte[] EncryptedData = Convert.FromBase64String(InputText);
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);

                // Create a decryptor from the existing SecretKey bytes.
                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

                MemoryStream memoryStream = new MemoryStream(EncryptedData);

                // Create a CryptoStream. (always use Read mode for decryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold EncryptedData;
                // DecryptedData is never longer than EncryptedData.
                byte[] PlainText = new byte[EncryptedData.Length];

                // Start decrypting.
                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

                memoryStream.Close();
                cryptoStream.Close();

                // Convert decrypted data into a string.
                string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

                // Return decrypted string.
                return DecryptedData;
            }
            catch (Exception )
            {
                MessageBox.Show("Błędny ciąg zaszyfrowanych danych!", "Błąd odczytu danych", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                // Convert decrypted data into a string.
                string DecryptedData = "";

                // Return decrypted string.
                return DecryptedData;
            }
        }

        #endregion Crypt

        /*
            MessageBox.Show(Application.CommonAppDataPath.Replace("\\"+Application.ProductVersion,""));
         * Zamiana
         * Application.StartupPath na Application.CommonAppDataPath.Replace("\\"+Application.ProductVersion,"")
            //CommonAppDataPath
         */

        /// <summary>
        /// Funkcja zapisuje dane do pliku INI
        /// </summary>
        /// <param param name="iniKey">Obiekt wpisu do pliku ini</param>
        /// <param name="sValue">Wartość do zapisu</param>
        public static void IniWriteValue(IniKey iniKey, string sValue)
        {
            WritePrivateProfileString(iniKey.Section,
                                      iniKey.Key,
                                      sValue,
                                      Application.CommonAppDataPath.Replace("\\" + Application.ProductVersion, "") + "\\" + Application.ProductName + ".ini");
            //Environment.SystemDirectory;
        }

        /// <summary>
        /// Funkcja zapisuje dane do pliku INI - z Crypt
        /// </summary>
        /// <param param name="iniKey">Obiekt wpisu do pliku ini - z Crypt</param>
        /// <param name="sValue">Wartość do zapisu</param>
        /// <param name="sPass">Hasło kodowania</param>
        public static void IniWriteValueCrypt(IniKey iniKey, string sValue, string sPass)
        {
            WritePrivateProfileString(iniKey.Section,
                                      iniKey.Key,
                                      EncryptString(sValue, sPass),
                                      Application.CommonAppDataPath.Replace("\\" + Application.ProductVersion, "") + "\\" + Application.ProductName + ".ini");
            //Environment.SystemDirectory;
        }

        /// <summary>
        /// Funkcja odczytuje dane z pliku INI
        /// </summary>
        /// <param param name="iniKey">Obiekt wpisu do pliku ini</param>
        /// <returns>Wartość odczytana</returns>
        public static string IniReadValue(IniKey iniKey)
        {
            StringBuilder temp = new StringBuilder(1024);

            int i = GetPrivateProfileString(iniKey.Section,
                                            iniKey.Key,
                                            "",
                                            temp,
                                            1024,
                                            Application.CommonAppDataPath.Replace("\\"+Application.ProductVersion,"") + "\\" + Application.ProductName + ".ini");

            string sTemp = temp.ToString();

            // jeśli odczytana wartość jest pusta to Default
            if (sTemp == string.Empty)
            {
                sTemp = iniKey.DefaultValue;
                IniFile.IniWriteValue(iniKey, sTemp);
            }

            return sTemp;
        }

        /// <summary>
        /// Funkcja odczytuje dane z pliku INI z Crypt
        /// </summary>
        /// <param param name="iniKey">Obiekt wpisu do pliku ini z Crypt</param>
        /// <returns>Wartość odczytana</returns>
        public static string IniReadValueCrypt(IniKey iniKey, string sPass)
        {
            StringBuilder temp = new StringBuilder(1024);

            int i = GetPrivateProfileString(iniKey.Section,
                                            iniKey.Key,
                                            "",
                                            temp,
                                            1024,
                                            Application.CommonAppDataPath.Replace("\\"+Application.ProductVersion,"") + "\\" + Application.ProductName + ".ini");
            string sTemp = "";

            if (temp.ToString() != "")
            {
                sTemp = DecryptString(temp.ToString(), sPass);
            }

            // jeśli odczytana wartość jest pusta to Default
            if (sTemp == string.Empty)
            {
                sTemp = iniKey.DefaultValue;
                IniFile.IniWriteValue(iniKey, sTemp);
            }

            return sTemp;
        }
    }

    /// <summary>
    /// Klasa reprezentująca wpis do pliku ini.
    /// </summary>
    public class IniKey
    {
        private string section = string.Empty;
        private string key = string.Empty;
        private string defaultvalue = string.Empty;

        /// <summary>
        /// Konstruktor domyślny.
        /// </summary>
        /// <param name="sSection">Nazwa sekcji</param>
        /// <param name="sKey">Nazwa klucza</param>
        /// <param name="sDefault">Wartość domyślna</param>
        public IniKey(string sSection, string sKey, string sDefault)
        {
            section = sSection;
            key = sKey;
            defaultvalue = sDefault;
        }

        /// <summary>
        /// Nazwa sekcji.
        /// Tylko do odczytu.
        /// </summary>
        public string Section
        {
            get { return section; }
        }

        /// <summary>
        /// Nazwa klucza.
        /// Tylko do odczytu.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Wartość domyślna.
        /// Tylko do odczytu.
        /// </summary>
        public string DefaultValue
        {
            get { return defaultvalue; }
        }
    }
}