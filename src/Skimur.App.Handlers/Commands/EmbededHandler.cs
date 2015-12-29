using Skimur.App.Commands;
using Skimur.App.Services;
using Skimur.Embed;
using Skimur.Messaging.Handling;

namespace Skimur.App.Handlers.Commands
{
    public class EmbededHandler : ICommandHandler<GenerateEmbeddedMediaObject>
    {
        private readonly IPostService _postService;
        private readonly IEmbeddedProvider _embeddedProvider;
        private readonly IMapper _mapper;

        public EmbededHandler(IPostService postService,
            IEmbeddedProvider embeddedProvider,
            IMapper mapper)
        {
            _postService = postService;
            _embeddedProvider = embeddedProvider;
            _mapper = mapper;
        }

        public void Handle(GenerateEmbeddedMediaObject command)
        {
            var post = _postService.GetPostById(command.PostId);
            if (post == null) return;

            if (post.PostType != PostType.Link) return;

            if (!command.Force && !string.IsNullOrEmpty(post.Media))
                // already created and we aren't trying to for it to be recreated
                return;

            if (!_embeddedProvider.IsEnabled) return;

            var result = _embeddedProvider.Embed(post.Url);

            if (result == null) return;

            _postService.UpdateMediaObjectForPost(post.Id, _mapper.Map<IEmbeddedResult, Post.MediaObject>(result));
        }
    }
}
