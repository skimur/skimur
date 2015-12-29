namespace Membership.Services
{
    /// <summary>
    /// Manages everything with passwords
    /// </summary>
    public interface IPasswordManager
    {
        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies the hashed password.
        /// </summary>
        /// <param name="hashedPassword">The hashed password.</param>
        /// <param name="providedPassword">The provided password.</param>
        /// <returns></returns>
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);

        /// <summary>
        /// Determins the password strength
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        PasswordScore PasswordStrength(string password);
    }

    /// <summary>
    /// The strength of the password
    /// </summary>
    public enum PasswordScore
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }
}
