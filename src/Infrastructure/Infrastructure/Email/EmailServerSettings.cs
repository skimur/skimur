using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Email
{
    /// <summary>
    /// The server settings for the system
    /// </summary>
    public class EmailServerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailServerSettings"/> class.
        /// </summary>
        public EmailServerSettings()
        {
            FromEmail = "noreply@domain.com";
            FromName = "No Reply";
            Host = "localhost";
            Port = 25;
        }

        /// <summary>
        /// The from name
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// The from email
        /// </summary>
        public string FromEmail { get; set; }

        /// <summary>
        /// The host of the server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The port of the server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The user name for the server
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password for the server
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Should we use SSL?
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Use default credentials?
        /// </summary>
        public bool UseDefaultCredentials { get; set; }
    }
}
