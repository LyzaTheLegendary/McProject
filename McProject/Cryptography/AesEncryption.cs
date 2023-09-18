using System.Security.Cryptography;

namespace Common.Cryptography
{
    public class AesEncryption : IEncryption
    {
        private Aes _aes = Aes.Create();
        private bool _isValid = false;
        public AesEncryption(byte[] key)
        {
            _aes.Padding = PaddingMode.None;
            _aes.Mode = CipherMode.CFB;
            _aes.KeySize = 128;
            _aes.FeedbackSize = 8;
            _aes.Key = key;
            _aes.IV = (byte[])key.Clone();
        }
        public void Decrypt(byte[] encryptedBuff)
        {
            throw new NotImplementedException();
        }

        public void Encrypt(byte[] buff)
        { 
            using (MemoryStream ms = new(buff))
                using (CryptoStream crypto = new(ms, _aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    crypto.Write(buff);
                    crypto.FlushFinalBlock(); // finish up the block so it can decrypt it! as AES works in blocks and all of those have to be completed for it to be able to work
                }
        }

        public CryptoStream GetStream(Stream stream)
            => new CryptoStream(stream, _aes.CreateEncryptor(), CryptoStreamMode.Read);

        public bool isValid() => _isValid;

    }
}
