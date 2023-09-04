using System.Security.Cryptography;

namespace Common.Cryptography
{
    public class DummyEncryption : IEncryption
    {
        public byte[] Decrypt(byte[] encryptedBuff) => encryptedBuff;

        public byte[] Encrypt(byte[] buff) => buff;

        public CryptoStream GetStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        public bool isValid() => true;
    }
}
