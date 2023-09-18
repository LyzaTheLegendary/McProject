using System.Security.Cryptography;

namespace Common.Cryptography
{
    public interface IEncryption
    {
        public CryptoStream GetStream(Stream stream);
        public void Decrypt(byte[] encryptedBuff);
        public void Encrypt(byte[] buff);
        public bool isValid();
    }
}
//Royal van der leun