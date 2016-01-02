using System;
using System.Collections.Generic;
using FluentAssertions;
using Skimur.App.Commands;
using Skimur.App.Services;
using Skimur.Messaging.Handling;
using Skimur.Tests;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Skimur.App.Tests
{
    public class MessagingTests : DataTestBase
    {
        private IMembershipService _membershipService;
        private ISubService _subService;
        private ICommandHandlerResponse<CreateSub, CreateSubResponse> _createSubHandler;
        private ICommandHandlerResponse<SendMessage, SendMessageResponse> _sendMessageHandler;
        private ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse> _replyMessageHandler;
        private IMessageService _messageService;

        public MessagingTests()
        {
            _membershipService = _serviceProvider.GetService<IMembershipService>();
            _createSubHandler = _serviceProvider.GetService<ICommandHandlerResponse<CreateSub, CreateSubResponse>>();
            _sendMessageHandler = _serviceProvider.GetService<ICommandHandlerResponse<SendMessage, SendMessageResponse>>();
            _replyMessageHandler = _serviceProvider.GetService<ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse>>();
            _messageService = _serviceProvider.GetService<IMessageService>();
        }

        [Fact]
        public void Can_send_message_to_user_and_then_reply()
        {
            // arrange
            var user1 = new User()
            {
                UserName = "user1"
            };
            _membershipService.InsertUser(user1);
            var user2 = new User
            {
                UserName = "user2"
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
            sendResponse.Error.Should().BeNullOrEmpty();
            var user1Messages = _messageService.GetPrivateMessagesForUser(user1.Id);
            var user2Messages = _messageService.GetPrivateMessagesForUser(user2.Id);
            user1Messages.Count.Should().Be(0);
            user2Messages.Count.Should().Be(1);

            // act
            var replyResponse = _replyMessageHandler.Handle(new ReplyMessage
            {
                ReplyToMessageId = user2Messages[0],
                Author = user2.Id,
                Body = "reply"
            });

            // assert
            replyResponse.Error.Should().BeNullOrEmpty();
            user1Messages = _messageService.GetPrivateMessagesForUser(user1.Id);
            user2Messages = _messageService.GetPrivateMessagesForUser(user2.Id);
            user1Messages.Count.Should().Be(1);
            user2Messages.Count.Should().Be(1);
        }

        [Fact]
        public void A_message_to_a_user_is_marked_as_unread()
        {
            // arrange
            var user1 = new User()
            {
                UserName = "user1"
            };
            _membershipService.InsertUser(user1);
            var user2 = new User
            {
                UserName = "user2"
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
            sendResponse.Error.Should().BeNullOrEmpty();
            var user1Messages = _messageService.GetUnreadMessagesForUser(user1.Id);
            var user2Messages = _messageService.GetUnreadMessagesForUser(user2.Id);
            user1Messages.Count.Should().Be(0);
            user2Messages.Count.Should().Be(1);
        }

        [Fact]
        public void User_can_send_and_received_messages_to_a_sub()
        {
            // arrange
            var user1 = new User()
            {
                UserName = "user1"
            };
            _membershipService.InsertUser(user1);
            var user2 = new User
            {
                UserName = "user2"
            };
            _membershipService.InsertUser(user2);
            var createSubResponse = _createSubHandler.Handle(new CreateSub { Name = "testsub", CreatedByUserId = user1.Id, Description = "testsub", Type = SubType.Public });
            createSubResponse.Error.Should().BeNullOrEmpty();
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
            sendResponse.Error.Should().BeNullOrEmpty();
            _messageService.GetSentMessagesForUser(userId).Should().HaveCount(1).And.Contain(sendResponse.MessageId);
            _messageService.GetPrivateMessagesForUser(userId).Should().HaveCount(0);
            _messageService.GetModeratorMailForSubs(new List<Guid> { subId }).Should().HaveCount(1).And.Contain(sendResponse.MessageId);
            var message = _messageService.GetMessageById(sendResponse.MessageId);
            message.AuthorId.Should().Be(userId);
            message.FromSub.Should().Be(null);
            message.ToSub.Should().HaveValue().And.Be(subId);
            message.ToUser.Should().Be(null);

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
            replyResponse.Error.Should().BeNullOrEmpty();
            _messageService.GetSentMessagesForUser(userId).Should().HaveCount(1).And.Contain(sendResponse.MessageId);
            _messageService.GetPrivateMessagesForUser(userId).Should().HaveCount(1).And.Contain(replyResponse.MessageId);
            message = _messageService.GetMessageById(replyResponse.MessageId);
            message.AuthorId.Should().Be(moderatorId);
            message.FromSub.Should().HaveValue().And.Be(subId);
            message.ToSub.Should().Be(null);
            message.ToUser.Should().HaveValue().And.Be(userId);

            #endregion

            #region moderator replies dont show in personal inboxes

            // assert
            _messageService.GetSentMessagesForUser(moderatorId).Should().HaveCount(0);
            _messageService.GetPrivateMessagesForUser(moderatorId).Should().HaveCount(0);

            #endregion

            #region moderator mail shows on in the correct moderator inboxes

            // assert
            _messageService.GetModeratorMailForSubs(new List<Guid> { subId }).Should().HaveCount(1).And.Contain(sendResponse.MessageId);
            _messageService.GetSentModeratorMailForSubs(new List<Guid> { subId }).Should().HaveCount(1).And.Contain(replyResponse.MessageId);

            #endregion
        }

        [Fact]
        public void User_cant_respond_to_incoming_mod_mail_unless_moderator()
        {
            // arrange
            var user1 = new User()
            {
                UserName = "user1"
            };
            _membershipService.InsertUser(user1);
            var user2 = new User
            {
                UserName = "user2"
            };
            _membershipService.InsertUser(user2);
            var user3 = new User
            {
                UserName = "user3"
            };
            _membershipService.InsertUser(user3);
            var createSubResponse = _createSubHandler.Handle(new CreateSub { Name = "testsub", CreatedByUserId = user1.Id, Description = "testsub", Type = SubType.Public });
            createSubResponse.Error.Should().BeNullOrEmpty();
            var subId = createSubResponse.SubId;
            var moderatorId = user1.Id;
            var userId = user2.Id;
            var invalidUserId = user3.Id;
            // setup the initial message that will get an attempted illegal reply
            var sendResponse = _sendMessageHandler.Handle(new SendMessage
            {
                Author = userId,
                To = "/s/testsub",
                Subject = "subject",
                Body = "body"
            });
            sendResponse.Error.Should().BeNullOrEmpty();

            // act
            var replyResponse = _replyMessageHandler.Handle(new ReplyMessage
            {
                ReplyToMessageId = sendResponse.MessageId,
                Author = invalidUserId, // this user isn't a moderator, nor is he involved in the discussion
                Body = "response"
            });

            // assert
            replyResponse.Error.Should().NotBeNullOrEmpty();
        }
    }
}
