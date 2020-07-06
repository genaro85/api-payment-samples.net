namespace com.mercantilbanco.api.sample
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    internal class Util
    {
        private readonly byte[] iv =
        {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16
        };

        /// <summary>
        /// Metodo para cifrar datos en AES con bloques de 128
        /// </summary>
        /// <param name="secretkey">Clave utilizada para cifrar los datos</param>
        /// <param name="datatocypher">Datos para ser cifrados</param>
        /// <returns></returns>
        public string Encrypt(string secretkey, string datatocypher)
        {
            //var iv = new byte[16];
            byte[] array;

            using (var advanceEncryptionStandard = Aes.Create())
            {
                Debug.Assert(
                    !(advanceEncryptionStandard is null),
                    $"{nameof(advanceEncryptionStandard)} was null");
                advanceEncryptionStandard.Key = ComputeSha256Hash(secretkey);
                advanceEncryptionStandard.IV = iv;
                advanceEncryptionStandard.Padding = PaddingMode.PKCS7;
                advanceEncryptionStandard.Mode = CipherMode.ECB;
                ICryptoTransform encryptor = advanceEncryptionStandard.CreateEncryptor(
                    advanceEncryptionStandard.Key,
                    advanceEncryptionStandard.IV);
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(
                        memoryStream,
                        encryptor,
                        CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(datatocypher);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Metodo para descifrar datos en AES con bloques de 128
        /// </summary>
        /// <param name="secretkey">Clave utilizada para descifrar los datos</param>
        /// <param name="datatodecrpyt">Datos para ser descifrados</param>
        /// <returns></returns>
        public string Decrypt(string secretkey, string datatodecrpyt)
        {
            if (!datatodecrpyt.IsBase64String() || !(datatodecrpyt.Length >= 4))
            {
                return datatodecrpyt;
            }
            //var iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(datatodecrpyt);

            if (!(buffer.Length >= 4))
            {
                return datatodecrpyt;
            }

            using (var advanceEncryptionStandard = Aes.Create())
            {
                Debug.Assert(
                    !(advanceEncryptionStandard is null),
                    $"{nameof(advanceEncryptionStandard)} was null");
                advanceEncryptionStandard.Key = ComputeSha256Hash(secretkey);
                advanceEncryptionStandard.IV = iv;
                advanceEncryptionStandard.Padding = PaddingMode.PKCS7;
                advanceEncryptionStandard.Mode = CipherMode.ECB;
                ICryptoTransform decryptor = advanceEncryptionStandard.CreateDecryptor(
                    advanceEncryptionStandard.Key,
                    advanceEncryptionStandard.IV);

                using (var memoryStream = new MemoryStream(buffer))
                {
                    using (var cryptoStream =
                        new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        private byte[] ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] key = iv;
                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                Array.Copy(hash, key, iv.Length);
                return key;
            }
        }
    }
}
