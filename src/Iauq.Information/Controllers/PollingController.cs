using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.Helpers;
using MvcContrib.Pagination;
using StructureMap;

namespace Iauq.Information.Controllers
{
    public class PollingController : ControllerBase
    {
        private readonly IPollService _pollService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHelper _webHelper;

        public PollingController(IUnitOfWork unitOfWork,
                                 IWebHelper webHelper, IPollService pollService)
        {
            _unitOfWork = unitOfWork;
            _webHelper = webHelper;
            _pollService = pollService;
        }

        public ActionResult Archive(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("Archive", new {page = 1});

            IQueryable<Poll> polls =
                _pollService.GetAllPolls().Where(p => p.IsActive);

            polls = polls.OrderBy(c => c.CreateDateTicks);

            IPagination<Poll> results;

            if (ControllerContext.IsChildAction)
                results = new LazyPagination<Poll>(polls, page, Constants.RecordPerPartial);
            else
            {
                results = new LazyPagination<Poll>(polls, page, Constants.RecordPerPage);
            }

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("Archive", new {page = 1});

            return ViewOrPartialView(results);
        }

        public ActionResult Details(int id)
        {
            Poll poll = _pollService.GetPollById(id);

            if (poll == null || !poll.IsActive)
                return EntityNotFoundView();

            User user = _webHelper.GetCurrentUser(HttpContext);

            string ipAddress = ObjectFactory.GetInstance<IWebHelper>().GetIpAddress();

            ViewBag.IsAlreadyVoted = _pollService.AlreadyVoted(user != null ? user.UserName : ipAddress, poll.Id);

            return ViewOrPartialView(poll);
        }

        [HttpPost]
        public ActionResult Details(int id, int[] choiceItemIds)
        {
            Poll poll = _pollService.GetPollById(id);

            if (poll == null)
                return EntityNotFoundView();

            User user = _webHelper.GetCurrentUser(HttpContext);

            string ipAddress = ObjectFactory.GetInstance<IWebHelper>().GetIpAddress();

            if (_pollService.AlreadyVoted(user != null ? user.UserName : ipAddress, poll.Id))
            {
                ViewBag.IsAlreadyVoted = true;
                return ViewOrPartialView(poll);
            }

            if (choiceItemIds != null && choiceItemIds.Length > 0)
            {
                foreach (Choice choice in poll.Choices)
                {
                    bool isAnswered = false;

                    foreach (ChoiceItem choiceItem in choice.Items)
                    {
                        if (!choice.IsMultiSelection && isAnswered)
                            break;

                        if (choiceItemIds.Contains(choiceItem.Id))
                        {
                            choiceItem.Answers.Add(new Answer
                                                       {
                                                           Answerer = user,
                                                           IpAddress = user == null ? ipAddress : null,
                                                           SelectedItem = choiceItem
                                                       });

                            isAnswered = true;
                        }
                    }
                }

                _unitOfWork.SaveChanges();

                ViewBag.IsAlreadyVoted = true;
                return ViewOrPartialView(poll);
            }

            ViewBag.IsAlreadyVoted = false;

            return ViewOrPartialView(poll);
        }

        [ChildActionOnly]
        public ActionResult RelatedPolls(int id)
        {
            Poll poll = _pollService.GetPollById(id);

            if (poll == null)
                return null;

            IQueryable<Poll> polls =
                _pollService.GetAllPolls()
                    .Where(p => p.IsActive);

            polls = polls.Where(c => c.Id != id).Take(10);

            return ViewOrPartialView(polls.ToList());
        }
    }
}