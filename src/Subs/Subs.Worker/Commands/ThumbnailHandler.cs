using Infrastructure.Messaging.Handling;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class ThumbnailHandler : ICommandHandler<GenerateThumbnailForPost>
    {
        private readonly IPostService _postService;

        public ThumbnailHandler(IPostService postService)
        {
            _postService = postService;
        }

        public void Handle(GenerateThumbnailForPost command)
        {
            var post = _postService.GetPostById(command.PostId);
            if (post == null) return;
            
            if (!command.Force && !string.IsNullOrEmpty(post.Thumb))
                // already created and we aren't trying to for it to be recreated
                return;

           // TODO
        }
    }
}
