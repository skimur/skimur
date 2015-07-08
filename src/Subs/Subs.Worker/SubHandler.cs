using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure.Membership;
using Infrastructure.Messaging.Handling;
using Infrastructure.Utils;
using Skimur;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker
{
    public class SubHandler : 
        ICommandHandlerResponse<CreateSub, CreateSubResponse>,
        ICommandHandlerResponse<EditSub, EditSubResponse>,
        ICommandHandlerResponse<CreatePost, CreatePostResponse>
    {
        private readonly ISubService _subService;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;

        public SubHandler(ISubService subService, 
            IMembershipService membershipService,
            IPostService postService)
        {
            _subService = subService;
            _membershipService = membershipService;
            _postService = postService;
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
                    Name = command.Name,
                    Description = command.Description,
                    SidebarText = command.SidebarText
                };

                _subService.InsertSub(sub);

                response.SubId = sub.Id;
                response.SubName = sub.Name;

                _subService.SubscribeToSub(user.UserName, sub.Name);
                _subService.AddModToSub(user.UserName, sub.Name);
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

                if (!_subService.CanUserModerateSub(user.UserName, sub.Name))
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

                if (command.PostType == PostType.Link)
                {
                    if (string.IsNullOrEmpty(command.Url))
                    {
                        response.Error = "You must provide a url.";
                        return response;
                    }

                    // todo: improve url validation
                    command.Url = command.Url.ToLower();
                    if (!command.Url.Contains("://"))
                    {
                        command.Url = "http://" + command.Url;
                    }
                    Uri uri;
                    try
                    {
                        uri = new Uri(command.Url);
                    }
                    catch (Exception ex)
                    {
                        response.Error = "The url appears to be invalid.";
                        return response;
                    }

                    switch (uri.Scheme)
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

                }else if (command.PostType == PostType.Text)
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

                var post = new Post();
                post.Id = GuidUtil.NewSequentialId();
                post.DateCreated = Common.CurrentTime();
                post.LastEditDate = null;
                post.SubName = sub.Name;
                post.UserName = user.UserName;
                post.UserIp = command.IpAddress;
                post.PostType = command.PostType;
                post.Title = command.Title;
                if (post.PostType == PostType.Link)
                {
                    post.Url = command.Url;
                }
                else
                {
                    post.Content = command.Content;
                }
                post.SendReplies = command.NotifyReplies;

                post.Slug = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                while (_postService.GetPostBySlug(post.Slug) != null)
                    post.Slug = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());

                _postService.InsertPost(post);

                response.Title = command.Title;
                response.Slug = post.Slug;
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
        }
    }
}
