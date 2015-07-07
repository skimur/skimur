using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;

namespace Infrastructure.Email
{
    /// <summary>
    /// Provides methods for managing <see cref="QueuedEmail"/>
    /// </summary>
    public interface IQueuedEmailService
    {
        #region QueuedEmail

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void InsertQueuedEmail(QueuedEmail queuedEmail);

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        void UpdateQueuedEmail(QueuedEmail queuedEmail);

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="id">Queued email identifier</param>
        /// <returns>Queued email</returns>
        QueuedEmail GetQueuedEmail(Guid id);

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="id">The identifier.</param>
        void DeleteQueuedEmail(Guid id);
        
        /// <summary>
        /// Search queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The number of items to take.</param>
        /// <returns>
        /// Email item collection
        /// </returns>
        SeekedList<QueuedEmail> SearchEmails(string fromEmail,
            string toEmail,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool loadNotSentItemsOnly,
            int? maxSendTries,
            bool loadNewest,
            int? skip = null,
            int? take = null);

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        void DeleteAllEmails();

        #endregion
    }
}
