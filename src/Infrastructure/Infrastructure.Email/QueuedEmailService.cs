using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Utils;
using ServiceStack.OrmLite;
using Skimur;

namespace Infrastructure.Email
{
    /// <summary>
    /// Provides methods for managing <see cref="QueuedEmail"/>
    /// </summary>
    public class QueuedEmailService : IQueuedEmailService
    {
        #region Fields

        private readonly IDbConnectionProvider _conn;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedEmailService"/> class.
        /// </summary>
        /// <param name="conn">The connection provider.</param>
        public QueuedEmailService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        #endregion

        #region QueuedEmail

        /// <summary>
        /// Creates a new record of a Queued Email in the system.
        /// <para>Will retrieve a default Email Account Id if the QueuedEmail does not contain one.</para>
        /// </summary>
        /// <param name="queuedEmail"></param>
        public void InsertQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null) throw new Exception("Invalid QueuedEmail.");
            if(queuedEmail.Id != Guid.Empty) throw new Exception("Can not create a Queue Email with a pre-existing id.");
            queuedEmail.Id = GuidUtil.NewSequentialId();
            queuedEmail.CreatedOn = Common.CurrentTime();
            _conn.Perform(conn => conn.Insert(queuedEmail));
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <exception cref="System.Exception">
        /// Invalid QueuedEmail.
        /// or
        /// Invalid Queued Email Id.
        /// or
        /// Failed to update QueudEmail.
        /// </exception>
        public void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null) throw new Exception("Invalid QueuedEmail.");
            if(queuedEmail.Id == Guid.Empty) throw new Exception("Invalid Queued Email Id.");
            _conn.Perform(conn => conn.Update(queuedEmail, x => x.Id == queuedEmail.Id));
        }


        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="System.Exception">Invalid QueuedEmail.
        /// or
        /// Invalid Queued Email Id.
        /// or
        /// Failed to delete Queued Email from the system.</exception>
        public void DeleteQueuedEmail(Guid id)
        {
            _conn.Perform(conn => conn.DeleteById<QueuedEmail>(id));
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="id">Queued email identifier</param>
        /// <returns>
        /// Queued email
        /// </returns>
        public QueuedEmail GetQueuedEmail(Guid id)
        {
            return id == Guid.Empty ? null : _conn.Perform(conn => conn.SingleById<QueuedEmail>(id));
        }


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
        public SeekedList<QueuedEmail> SearchEmails(string fromEmail,
            string toEmail,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool loadNotSentItemsOnly,
            int? maxSendTries,
            bool loadNewest,
            int? skip = null,
            int? take = null)
        {
            fromEmail = (fromEmail ?? string.Empty).Trim();
            toEmail = (toEmail ?? string.Empty).Trim();

            return _conn.Perform(conn =>
            {
                var expression = conn.From<QueuedEmail>();

                if (!string.IsNullOrEmpty(fromEmail))
                    expression.Where(x => x.From.Contains(fromEmail));

                if (!string.IsNullOrEmpty(toEmail))
                    expression.Where(x => x.To.Contains(toEmail));

                if (createdFromUtc.HasValue)
                    expression.Where(x => x.CreatedOn >= createdFromUtc);

                if (createdToUtc.HasValue)
                    expression.Where(x => x.CreatedOn <= createdToUtc);

                if (loadNotSentItemsOnly)
                    expression.Where(x => x.SentOn == null);

                if (maxSendTries.HasValue)
                    expression.Where(x => x.SentTries <= maxSendTries);

                var count = conn.Count(expression);

                expression.Skip(skip).Take(take);

                return new SeekedList<QueuedEmail>(conn.Select(expression), skip ?? 0, take, count);
            });
        }

        /// <summary>
        /// Delete all queued emails
        /// </summary>
        public void DeleteAllEmails()
        {
            _conn.Perform(conn => conn.DeleteAll<QueuedEmail>());
        }

        #endregion
    }
}
