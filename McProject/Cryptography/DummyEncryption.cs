using System.Security.Cryptography;

namespace Common.Cryptography
{
    public class DummyEncryption : IEncryption
    {
        public void Decrypt(byte[] encryptedBuff) { }

        public void Encrypt(byte[] buff) { }

        public CryptoStream GetStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public bool isValid() => true;
    }
}
