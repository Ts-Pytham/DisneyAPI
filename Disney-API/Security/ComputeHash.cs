using System.Security.Cryptography;
using System.Text;

namespace Disney_API.Security
{
    public static class ComputeHash
    {
        public static string ToSHA256(this string str)
        {
            using SHA256 sHA256 = SHA256.Create();
            byte[] bytes = sHA256.ComputeHash(Encoding.UTF8.GetBytes(str));

            // Convert byte array to a string   
            StringBuilder builder = new();
            int len = bytes.Length;
            for (int i = 0; i < len; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();

        }

        public static string ToSHA512(this string str)
        {
            using SHA512 SHA512 = SHA512.Create();
            byte[] bytes = SHA512.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder builder = new();
            int len = bytes.Length;
            for (int i = 0; i < len; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
