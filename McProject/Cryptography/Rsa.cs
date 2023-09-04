using System.Security.Cryptography;

namespace Common.Cryptography
{
    public class Rsa
    {
        private RSA rsa = RSA.Create();
        public byte[] Get509Cert() => rsa.ExportSubjectPublicKeyInfo();
        public byte[] Decrypt(byte[] buff) => rsa.Decrypt(buff, RSAEncryptionPadding.Pkcs1);
    }
}
