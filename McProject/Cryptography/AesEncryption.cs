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
        public byte[] Decrypt(byte[] encryptedBuff)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(byte[] buff)
        {
            throw new NotImplementedException();
        }

        public CryptoStream GetStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public bool isValid() => _isValid;

    }
}
