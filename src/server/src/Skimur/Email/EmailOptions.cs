using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Email
{
    public class EmailOptions
    {
        public EmailOptions()
        {
            FromEmail = "noreply@domain.com";
            FromName = "No Reply";
            Host = "localhost";
            Port = 1025;
        }
        
        public string FromName { get; set; }
        
        public string FromEmail { get; set; }
        
        public string Host { get; set; }
        
        public int Port { get; set; }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
        
        public bool EnableSsl { get; set; }
        
        public bool UseDefaultCredentials { get; set; }
    }
}
