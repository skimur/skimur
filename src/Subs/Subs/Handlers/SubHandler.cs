using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Infrastructure.Utils;
using Subs.Commands;

namespace Subs.Handlers
{
    public class SubCommandHandler : ICommandHandlerResponse<CreateSub, CreateSubResponse>
    {
        private readonly ISubService _subService;

        public SubCommandHandler(ISubService subService)
        {
            _subService = subService;
        }

        public CreateSubResponse Handle(CreateSub command)
        {
            var sub = new Sub
            {
                Id = GuidUtil.NewSequentialId(),
                Name = command.Name,
                SidebarText = command.SidebarText
            };

            _subService.InsertSub(sub);

            return new CreateSubResponse
            {
                SubId = sub.Id
            };
        }
    }
}
