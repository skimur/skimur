using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure.Membership;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Infrastructure.Utils;
using Skimur;
using Subs.Commands;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker
{
    public class SubHandler :
        ICommandHandlerResponse<CreateSub, CreateSubResponse>,
        ICommandHandlerResponse<EditSub, EditSubResponse>,
        ICommandHandlerResponse<CreatePost, CreatePostResponse>,
        ICommandHandlerResponse<SubcribeToSub, SubcribeToSubResponse>,
        ICommandHandlerResponse<UnSubcribeToSub, UnSubcribeToSubResponse>
    {
        private readonly ISubService _subService;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly IEventBus _eventBus;
        private readonly ICommandBus _commandBus;

        public SubHandler(ISubService subService,
            IMembershipService membershipService,
            IPostService postService,
            IEventBus eventBus,
            ICommandBus commandBus)
        {
            _subService = subService;
            _membershipService = membershipService;
            _postService = postService;
            _eventBus = eventBus;
            _commandBus = commandBus;
        }

        public CreateSubResponse Handle(CreateSub command)
        {
            var response = new CreateSubResponse();

            try
            {
                if (string.IsNullOrEmpty(command.Name))
                {
                    response.Error = "Sub name is required.";
                    return response;
                }

                if (!Regex.IsMatch(command.Name, "^[a-zA-Z0-9]*$"))
                {
                    response.Error = "No spaces or special characters.";
                    return response;
                }

                if (command.Name.Length > 20)
                {
                    response.Error = "The name length is limited to 20 characters.";
                    return response;
                }

                if (string.IsNullOrEmpty(command.Description))
                {
                    response.Error = "Please describe your sub.";
                    return response;
                }

                var user = _membershipService.GetUserById(command.CreatedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var sub = new Sub
                {
                    Id = GuidUtil.NewSequentialId(),
                    CreatedDate = Common.CurrentTime(),
                    Name = command.Name,
                    Description = command.Description,
                    SidebarText = command.SidebarText
                };

                _subService.InsertSub(sub);

                response.SubId = sub.Id;
                response.SubName = sub.Name;

                _subService.SubscribeToSub(user.Id, sub.Id);
                _subService.AddModToSub(user.Id, sub.Id);
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
        }

        public EditSubResponse Handle(EditSub command)
        {
            var response = new EditSubResponse();

            try
            {
                var sub = _subService.GetSubByName(command.Name);

                if (sub == null)
                {
                    response.Error = "No sub found with the given name.";
                    return response;
                }

                var user = _membershipService.GetUserById(command.EditedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (!_subService.CanUserModerateSub(user.Id, sub.Id))
                {
                    response.Error = "You are not allowed to modify this sub.";
                    return response;
                }

                if (string.IsNullOrEmpty(command.Description))
                {
                    response.Error = "Please describe your sub.";
                    return response;
                }

                sub.Description = command.Description;
                sub.SidebarText = command.SidebarText;

                _subService.UpdateSub(sub);
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
        }

        public CreatePostResponse Handle(CreatePost command)
        {
            var response = new CreatePostResponse();

            try
            {
                var user = _membershipService.GetUserById(command.CreatedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (string.IsNullOrEmpty(command.SubName))
                {
                    response.Error = "The sub name is required.";
                    return response;
                }

                var sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "That sub doesn't exist.";
                    return response;
                }

                // todo: make sure that the user can post into the sub
                // is admin? is user banned? does user look like spam? 

                // todo: make sure the post type is allowed

                if (string.IsNullOrEmpty(command.Title))
                {
                    response.Error = "The title is required.";
                    return response;
                }

                // remove extrenous white space, and trim whitespace from the edges
                command.Title = Regex.Replace(command.Title, @"\s+", " ").Trim();

                if (command.Title.Length > 100)
                {
                    response.Error = "The title is too long.";
                    return response;
                }

                string domain = null;

                if (command.PostType == PostType.Link)
                {
                    if (string.IsNullOrEmpty(command.Url))
                    {
                        response.Error = "You must provide a url.";
                        return response;
                    }

                    // todo: improve url validation
                    string scheme;
                    if (!UrlParser.TryParseUrl(command.Url, out domain, out scheme))
                    {
                        response.Error = "The url appears to be invalid.";
                        return response;
                    }

                    switch (scheme)
                    {
                        case "http":
                        case "https:":
                            break;
                        default:
                            response.Error = "The scheme is invalid for the url.";
                            return response;
                    }

                    // todo: make sure the domain isn't banned

                    // todo: make sure the url wasn't already submitted


                }
                else if (command.PostType == PostType.Text)
                {
                    // todo: only validate this length is user is not an admin
                    if (!string.IsNullOrEmpty(command.Content) && command.Content.Length > 40000)
                    {
                        response.Error = "The post content is too long (maximum 40000 characters).";
                        return response;
                    }
                }
                else
                {
                    throw new Exception("unknown post type " + command.PostType);
                }

                var post = new Post
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = Common.CurrentTime(),
                    LastEditDate = null,
                    SubId = sub.Id,
                    UserId = user.Id,
                    UserIp = command.IpAddress,
                    PostType = command.PostType,
                    Title = command.Title,
                    SendReplies = command.NotifyReplies
                };
                if (post.PostType == PostType.Link)
                {
                    post.Url = command.Url;
                    post.Domain = domain;
                }
                else
                {
                    post.Content = command.Content;
                }
                
                _postService.InsertPost(post);
                _commandBus.Send(new CastVoteForPost { DateCasted = post.DateCreated, IpAddress = command.IpAddress, PostId = post.Id, UserId = user.Id, VoteType = VoteType.Up });

                response.Title = command.Title;
                response.PostId = post.Id;
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
        }

        public SubcribeToSubResponse Handle(SubcribeToSub command)
        {
            var response = new SubcribeToSubResponse();

            try
            {
                var user = _membershipService.GetUserByUserName(command.UserName);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (string.IsNullOrEmpty(command.SubName))
                {
                    response.Error = "You must provide a sub name.";
                    return response;
                }

                var sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "Invalid sub name.";
                    return response;
                }

                // todo: check for private subs
                // todo: check if the user is subcribed to too many subs

                _subService.SubscribeToSub(user.Id, sub.Id);

                _eventBus.Publish(new SubScriptionChanged
                {
                    Subcribed = true,
                    UserId = user.Id,
                    SubId = sub.Id
                });

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                return response;
            }

            return response;
        }

        public UnSubcribeToSubResponse Handle(UnSubcribeToSub command)
        {
            var response = new UnSubcribeToSubResponse();

            try
            {
                var sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "No sub found with the given name";
                    return response;
                }

                _subService.UnSubscribeToSub(command.UserId, sub.Id);

                _eventBus.Publish(new SubScriptionChanged
                {
                    Unsubscribed = true,
                    UserId = command.UserId,
                    SubId = sub.Id
                });

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                return response;
            }

            return response;
        }
    }
}
