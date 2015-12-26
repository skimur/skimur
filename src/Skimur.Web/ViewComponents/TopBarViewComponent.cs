using Microsoft.AspNet.Mvc;
using Skimur.Web.Services;
using Skimur.Web.ViewModels;
using Subs.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewComponents
{
    public class TopbarViewComponent : ViewComponent
    {
        IContextService _contextService;
        ISubDao _subDao;

        public TopbarViewComponent(IContextService contextService, ISubDao subDao)
        {
            _contextService = contextService;
            _subDao = subDao;
        }

        public IViewComponentResult Invoke()
        {
            var model = new TopBarViewModel();
            model.SubscibedSubs.AddRange(_subDao.GetSubsByIds(_contextService.GetSubscribedSubIds()).Select(x => x.Name).OrderBy(x => x));
            model.DefaultSubs.AddRange(_subDao.GetSubsByIds(_subDao.GetDefaultSubs()).Select(x => x.Name));
            return View(model);
        }
    }
}
