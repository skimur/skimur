using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Membership.Services
{
    /// <summary>
    /// The password manager
    /// </summary>
    public class PasswordManager : IPasswordManager
    {
        #region IPasswordManager

        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">password</exception>
        public string HashPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");
            byte[] salt;
            byte[] bytes;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, 16, 1000))
            {
                salt = rfc2898DeriveBytes.Salt;
                bytes = rfc2898DeriveBytes.GetBytes(32);
            }
            var inArray = new byte[49];
            Buffer.BlockCopy(salt, 0, inArray, 1, 16);
            Buffer.BlockCopy(bytes, 0, inArray, 17, 32);
            return Convert.ToBase64String(inArray);
        }

        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="providedPassword">The provided password.</param>
        /// <returns></returns>
        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null)
                return false;
            if (providedPassword == null)
                throw new ArgumentNullException("providedPassword");
            var numArray = Convert.FromBase64String(hashedPassword);
            if (numArray.Length != 49 || (int)numArray[0] != 0)
                return false;
            var salt = new byte[16];
            Buffer.BlockCopy(numArray, 1, salt, 0, 16);
            var a = new byte[32];
            Buffer.BlockCopy(numArray, 17, a, 0, 32);
            byte[] bytes;
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, 1000))
                bytes = rfc2898DeriveBytes.GetBytes(32);
            return ByteArraysEqual(a, bytes);
        }

        /// <summary>
        /// Determins the password strength
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public PasswordScore PasswordStrength(string password)
        {
            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"d+", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScore)score;
        }

        #endregion

        #region Methods

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a == null || b == null || a.Length != b.Length)
                return false;
            var flag = true;
            for (var index = 0; index < a.Length; ++index)
                flag &= a[index] == b[index];
            return flag;
        }

        #endregion
    }
}
