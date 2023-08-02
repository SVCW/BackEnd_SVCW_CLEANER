using System;
using System.Security.Cryptography;
using System.Text;

namespace SVCW.DTOs.Common
{
	public class AuthenLib
	{
		public AuthenLib()
		{
		}

        // Hàm giải mã mật khẩu
        //static string DecryptSHA(string decryptStr)
        //{
        //    if (userPasswords.ContainsKey(username))
        //    {
        //        string savedHash = userPasswords[username];
        //        string salt = savedHash.Substring(savedHash.Length - 8); // Lấy muối từ chuỗi đã lưu
        //        return savedHash.Substring(0, savedHash.Length - 8); // Trả về chuỗi đã bỏ muối
        //    }

        //    return null;
        //}

        static string DecryptPasswordSHA(string hashedPassword, string privateKey)
        {
            string encodedPassword = hashedPassword.Substring(0, hashedPassword.Length - privateKey.Length); // Lấy chuỗi mã hóa đã bỏ muối

            byte[] hashedBytes = Convert.FromBase64String(encodedPassword);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("")); // Chuỗi rỗng để lấy đúng kích thước hash
                byte[] saltedPasswordBytes = new byte[passwordBytes.Length + privateKey.Length];

                passwordBytes.CopyTo(saltedPasswordBytes, 0);
                Encoding.UTF8.GetBytes(privateKey).CopyTo(saltedPasswordBytes, passwordBytes.Length);

                byte[] originalPasswordBytes = new byte[hashedBytes.Length - saltedPasswordBytes.Length];

                for (int i = 0; i < originalPasswordBytes.Length; i++)
                {
                    originalPasswordBytes[i] = (byte)(hashedBytes[i] ^ saltedPasswordBytes[i]);
                }

                return Encoding.UTF8.GetString(originalPasswordBytes);
            }
        }


        // Hàm mã hóa mật khẩu
        static string HashPasswordSHA(string password, string privateKey)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + privateKey);
            byte[] hashedBytes;

            using (SHA256 sha256 = SHA256.Create())
            {
                hashedBytes = sha256.ComputeHash(passwordBytes);
            }

            return Convert.ToBase64String(hashedBytes) + privateKey;
        }

        // Generate private Key
        static string GenerateKeySHA()
        {
            byte[] saltBytes = new byte[4];

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }
    }
}

