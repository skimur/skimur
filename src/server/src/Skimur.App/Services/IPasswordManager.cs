using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App.Services
{
    public interface IPasswordManager
    {
        string HashPassword(string password);
        
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
        
        PasswordScore PasswordStrength(string password);
    }
    
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
