using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;
using Skimur.Embed;
using Skimur.Messaging.Handling;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
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
