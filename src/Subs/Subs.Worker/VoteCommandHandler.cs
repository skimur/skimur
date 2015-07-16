using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Membership;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Subs.Commands;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker
{
    public class VoteCommandHandler : ICommandHandler<CaseVote>
    {
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;
        private readonly IEventBus _eventBus;

        public VoteCommandHandler(IMembershipService membershipService,
            IPostService postService,
            IVoteService voteService,
            IEventBus eventBus)
        {
            _membershipService = membershipService;
            _postService = postService;
            _voteService = voteService;
            _eventBus = eventBus;
        }

        public void Handle(CaseVote command)
        {
            var user = _membershipService.GetUserByUserName(command.UserName);

            if (user == null)
                return;

            var post = _postService.GetPostBySlug(command.PostSlug);

            if (post == null)
                return;

            if (command.VoteType.HasValue)
                _voteService.VoteForPost(post.Slug, user.UserName, command.IpAddress, command.VoteType.Value, command.DateCasted);
            else
                _voteService.UnVotePost(post.Slug, user.UserName);

            _eventBus.Publish(new VoteCasted { PostSlug = post.Slug, UserName = user.UserName, VoteType = command.VoteType });
        }
    }
}
