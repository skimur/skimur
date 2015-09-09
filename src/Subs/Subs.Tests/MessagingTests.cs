using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Membership;
using Membership.Services;
using NUnit.Framework;
using Skimur.Tests;
using Subs.Commands;
using Subs.Services;
using Subs.Worker;

namespace Subs.Tests
{
    [TestFixture]
    public class MessagingTests : DataTestBase
    {
        private IMembershipService _membershipService;
        private ISubService _subService;
        private ICommandHandlerResponse<CreateSub, CreateSubResponse> _createSubHandler;
        private ICommandHandlerResponse<SendMessage, SendMessageResponse> _sendMessageHandler;
        private ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse> _replyMessageHandler;
        private IMessageService _messageService;

        [Test]
        public void Can_send_message_to_user_and_then_reply()
        {
            // arrange
            var user1 = _membershipService.GetUserByUserName("skimur");
            var user2 = new User
            {
                UserName = "user"
            };
            _membershipService.InsertUser(user2);

            // act
            var sendResponse = _sendMessageHandler.Handle(new SendMessage
            {
                Author = user1.Id,
                Subject = "subject",
                Body = "body",
                To = user2.UserName
            });

            // assert
            Assert.That(string.IsNullOrEmpty(sendResponse.Error));
            var user1Messages = _messageService.GetPrivateMessagesForUser(user1.Id);
            var user2Messages = _messageService.GetPrivateMessagesForUser(user2.Id);
            Assert.That(user1Messages, Has.Count.EqualTo(0));
            Assert.That(user2Messages, Has.Count.EqualTo(1));

            // act
            var replyResponse = _replyMessageHandler.Handle(new ReplyMessage
            {
                ReplyToMessageId = user2Messages[0],
                Author = user2.Id,
                Body = "reply"
            });

            // assert
            Assert.That(string.IsNullOrEmpty(replyResponse.Error));
            user1Messages = _messageService.GetPrivateMessagesForUser(user1.Id);
            user2Messages = _messageService.GetPrivateMessagesForUser(user2.Id);
            Assert.That(user1Messages, Has.Count.EqualTo(1));
            Assert.That(user2Messages, Has.Count.EqualTo(1));
        }

        [Test]
        public void A_message_to_a_user_is_marked_as_unread()
        {
            // arrange
            var user1 = _membershipService.GetUserByUserName("skimur");
            var user2 = new User
            {
                UserName = "user"
            };
            _membershipService.InsertUser(user2);

            // act
            var sendResponse = _sendMessageHandler.Handle(new SendMessage
            {
                Author = user1.Id,
                Subject = "subject",
                Body = "body",
                To = user2.UserName
            });

            // assert
            Assert.That(string.IsNullOrEmpty(sendResponse.Error));
            var user1Messages = _messageService.GetUnreadMessagesForUser(user1.Id);
            var user2Messages = _messageService.GetUnreadMessagesForUser(user2.Id);
            Assert.That(user1Messages, Has.Count.EqualTo(0));
            Assert.That(user2Messages, Has.Count.EqualTo(1));
        }

        [Test]
        public void User_can_send_and_received_messages_to_a_sub()
        {
            // arrange
            var user1 = _membershipService.GetUserByUserName("skimur");
            var user2 = new User
            {
                UserName = "user"
            };
            _membershipService.InsertUser(user2);
            var createSubResponse = _createSubHandler.Handle(new CreateSub { Name = "testsub", CreatedByUserId = user1.Id, Description = "testsub", Type = SubType.Public });
            Assert.That(string.IsNullOrEmpty(createSubResponse.Error));
            var subId = createSubResponse.SubId;
            var moderatorId = user1.Id;
            var userId = user2.Id;

            #region sending message from user to sub

            // act
            var sendResponse = _sendMessageHandler.Handle(new SendMessage
            {
                Author = userId,
                Subject = "subject",
                Body = "body",
                To = "/s/testsub"
            });

            // assert
            Assert.That(string.IsNullOrEmpty(sendResponse.Error));
            Assert.That(_messageService.GetSentMessagesForUser(userId), Has.Count.EqualTo(1).And.Contains(sendResponse.MessageId));
            Assert.That(_messageService.GetPrivateMessagesForUser(userId), Has.Count.EqualTo(0));
            Assert.That(_messageService.GetModeratorMailForSubs(new List<Guid> { subId }), Has.Count.EqualTo(1).And.Contains(sendResponse.MessageId));
            var message = _messageService.GetMessageById(sendResponse.MessageId);
            Assert.That(message.AuthorId, Is.EqualTo(userId));
            Assert.That(message.FromSub, Is.Null);
            Assert.That(message.ToSub, Is.Not.Null.And.EqualTo(subId));
            Assert.That(message.ToUser, Is.Null);

            #endregion

            #region reply to user from sub

            // act
            var replyResponse = _replyMessageHandler.Handle(new ReplyMessage
            {
                Author = moderatorId,
                ReplyToMessageId = sendResponse.MessageId,
                Body = "response"
            });

            // assert
            Assert.That(string.IsNullOrEmpty(replyResponse.Error));
            Assert.That(_messageService.GetSentMessagesForUser(userId), Has.Count.EqualTo(1).And.Contains(sendResponse.MessageId));
            Assert.That(_messageService.GetPrivateMessagesForUser(userId), Has.Count.EqualTo(1).And.Contains(replyResponse.MessageId));
            message = _messageService.GetMessageById(replyResponse.MessageId);
            Assert.That(message.AuthorId, Is.EqualTo(moderatorId));
            Assert.That(message.FromSub, Is.Not.Null.And.EqualTo(subId));
            Assert.That(message.ToSub, Is.Null);
            Assert.That(message.ToUser, Is.Not.Null.And.EqualTo(userId));

            #endregion

            #region moderator replies dont show in personal inboxes

            // assert
            Assert.That(_messageService.GetSentMessagesForUser(moderatorId), Has.Count.EqualTo(0));
            Assert.That(_messageService.GetPrivateMessagesForUser(moderatorId), Has.Count.EqualTo(0));

            #endregion

            #region moderator mail shows on in the correct moderator inboxes

            // assert
            Assert.That(_messageService.GetModeratorMailForSubs(new List<Guid> { subId }), Has.Count.EqualTo(1).And.Contains(sendResponse.MessageId));
            Assert.That(_messageService.GetSentModeratorMailForSubs(new List<Guid> { subId }), Has.Count.EqualTo(1).And.Contains(replyResponse.MessageId));

            #endregion
        }

        protected override void Setup()
        {
            base.Setup();
            _membershipService = _container.GetInstance<IMembershipService>();
            _createSubHandler = _container.GetInstance<ICommandHandlerResponse<CreateSub, CreateSubResponse>>();
            _sendMessageHandler = _container.GetInstance<ICommandHandlerResponse<SendMessage, SendMessageResponse>>();
            _replyMessageHandler = _container.GetInstance<ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse>>();
            _messageService = _container.GetInstance<IMessageService>();
        }
    }
}
