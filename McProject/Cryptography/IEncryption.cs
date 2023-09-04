using System.Security.Cryptography;

namespace Common.Cryptography
{
    public interface IEncryption
    {
        public CryptoStream GetStream(Stream stream);
        public byte[] Decrypt(byte[] encryptedBuff);
        public byte[] Encrypt(byte[] buff);
        public bool isValid();
    }
}
//Royal van der leun