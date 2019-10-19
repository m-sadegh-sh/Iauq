using System;
using System.Linq;
using System.Web.Mvc;
using Iauq.Core.Domain;
using Iauq.Data;
using Iauq.Data.Services;
using Iauq.Information.App_GlobalResources;
using Iauq.Information.Helpers;
using Iauq.Information.LogProviders;
using MvcContrib.Pagination;

namespace Iauq.Information.Areas.Administration.Controllers
{
    [CustomAuthorize(Roles = "Administrators, Moderators, Editors")]
    public class PollsController : AdministrationControllerBase
    {
        private readonly IPollService _pollService;
        private readonly IUnitOfWork _unitOfWork;

        public PollsController(IUnitOfWork unitOfWork, IPollService pollService)
        {
            _unitOfWork = unitOfWork;
            _pollService = pollService;
        }

        [HttpGet]
        public ActionResult List(int page = 1)
        {
            if (page < 1)
                return RedirectToActionPermanent("List", new {page = 1});

            var results = new LazyPagination<Poll>(_pollService.GetAllPolls().OrderBy(p => p.Id), page,
                                                   Constants.RecordPerPage);

            if (!results.Any() && page != 1)
                return RedirectToActionPermanent("List", new {page = 1});

            return ViewOrPartialView(results);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var poll = new Poll();

            return ViewOrPartialView(poll);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Poll poll)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(poll);
            }

            poll.CreateDate = DateTime.UtcNow;

            _pollService.SavePoll(poll);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new CreatePollProvider(poll));
            else
            {
                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(poll);
            }

            if (Request["continue"] != null)
                return RedirectToAction("Edit", new {poll.Id});

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Poll poll = _pollService.GetPollById(id);

            if (poll == null)
                return EntityNotFoundView();

            return ViewOrPartialView(poll);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int id)
        {
            Poll dbPoll = _pollService.GetPollById(id);

            if (dbPoll == null)
                return EntityNotFoundView();

            TryUpdateModel(dbPoll,
                           new[] {"IsActive", "ShowOnHomePage", "Title", "Description"});

            if (!TryValidateModel(dbPoll))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbPoll);
            }

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new UpdatePollProvider(dbPoll));
            else
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbPoll);
            }

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return ViewOrPartialView(dbPoll);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Poll dbPoll = _pollService.GetPollById(id);

            if (dbPoll == null)
                return EntityNotFoundView();

            _pollService.DeletePoll(dbPoll);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new DeletePollProvider(dbPoll.Id));
            else
                TempData["Error"] = ValidationResources.DeleteFailure;

            if (IsReferrerValid())
                return Redirect(Request.UrlReferrer.AbsolutePath);

            return RedirectToAction("List", new {page = 1});
        }

        [HttpGet]
        public ActionResult CreateChoice(int pollId)
        {
            Poll poll = _pollService.GetPollById(pollId);

            if (poll == null)
                return EntityNotFoundView();

            var choice = new Choice {Id = _unitOfWork.GetNextTableIdentity("Choices"), OwnerId = poll.Id};

            return ViewOrPartialView(choice);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChoice(int pollId, Choice choice)
        {
            Poll poll = _pollService.GetPollById(pollId);

            if (poll == null)
                return EntityNotFoundView();

            choice.OwnerId = poll.Id;
            choice.Owner = poll;

            if (!ModelState.IsValid)
            {
                TempData.Add("", ValidationResources.InvalidState);

                RedirectToAction("Edit", new {id = choice.OwnerId});
            }

            _pollService.SaveChoice(choice);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new CreateChoiceProvider(choice));
            else
                TempData.Add("", ValidationResources.CreationFailure);

            return RedirectToAction("Edit", new {id = choice.OwnerId});
        }

        [HttpGet]
        public ActionResult EditChoice(int id)
        {
            Choice choice = _pollService.GetChoiceById(id);

            if (choice == null)
                return EntityNotFoundView();

            return ViewOrPartialView(choice);
        }

        [HttpPost]
        [ActionName("EditChoice")]
        [ValidateAntiForgeryToken]
        public ActionResult EditChoicePost(int id, FormCollection values)
        {
            Choice dbChoice = _pollService.GetChoiceById(id);

            if (dbChoice == null)
                return EntityNotFoundView();

            TryUpdateModel(dbChoice,
                           new[] {"Title", "Description", "IsMultiSelection"});

            if (!TryValidateModel(dbChoice))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbChoice);
            }

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new UpdateChoiceProvider(dbChoice));
            else
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbChoice);
            }

            return RedirectToAction("EditChoice", new {dbChoice.Id});
        }

        [HttpGet]
        public ActionResult DeleteChoice(int id)
        {
            Choice dbChoice = _pollService.GetChoiceById(id);

            if (dbChoice == null)
                return EntityNotFoundView();

            _pollService.DeleteChoice(dbChoice);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
            {
                Logger.SaveLog(new DeleteChoiceProvider(dbChoice.Id));
            }

            return null;
        }

        [HttpGet]
        public ActionResult CreateChoiceItem(int choiceId)
        {
            Choice choice = _pollService.GetChoiceById(choiceId);

            if (choice == null)
                return EntityNotFoundView();

            var item = new ChoiceItem {Id = _unitOfWork.GetNextTableIdentity("ChoiceItems"), OwnerId = choice.Id};

            return ViewOrPartialView(item);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChoiceItem(int choiceId, ChoiceItem choiceItem)
        {
            Choice choice = _pollService.GetChoiceById(choiceId);

            if (choice == null)
                return EntityNotFoundView();

            choiceItem.OwnerId = choice.Id;
            choiceItem.Owner = choice;

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(choice);
            }

            _pollService.SaveChoiceItem(choiceItem);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new CreateChoiceItemProvider(choiceItem));
            else
            {
                ModelState.AddModelError("", ValidationResources.CreationFailure);

                return ViewOrPartialView(choiceItem);
            }

            return RedirectToAction("EditChoiceItem", new {choiceItem.Id});
        }

        [HttpGet]
        public ActionResult EditChoiceItem(int id)
        {
            ChoiceItem choiceItem = _pollService.GetChoiceItemById(id);

            if (choiceItem == null)
                return EntityNotFoundView();

            return ViewOrPartialView(choiceItem);
        }

        [HttpPost]
        [ActionName("EditChoiceItem")]
        [ValidateAntiForgeryToken]
        public ActionResult EditChoiceItemPost(int id, FormCollection values)
        {
            ChoiceItem dbChoiceItem = _pollService.GetChoiceItemById(id);

            if (dbChoiceItem == null)
                return EntityNotFoundView();

            TryUpdateModel(dbChoiceItem,
                           new[] {"Text"});

            if (!TryValidateModel(dbChoiceItem))
            {
                ModelState.AddModelError("",
                                         ValidationResources.InvalidState);

                return ViewOrPartialView(dbChoiceItem);
            }

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new UpdateChoiceItemProvider(dbChoiceItem));
            if (!isSaved)
            {
                ModelState.AddModelError("", ValidationResources.UpdateFailure);

                return ViewOrPartialView(dbChoiceItem);
            }

            return RedirectToAction("EditChoiceItem", new {dbChoiceItem.Id});
        }

        [HttpGet]
        public ActionResult DeleteChoiceItem(int id)
        {
            ChoiceItem dbChoiceItem = _pollService.GetChoiceItemById(id);

            if (dbChoiceItem == null)
                return EntityNotFoundView();

            _pollService.DeleteChoiceItem(dbChoiceItem);

            bool isSaved;

            try
            {
                isSaved = _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                isSaved = false;
            }

            if (isSaved)
                Logger.SaveLog(new DeleteChoiceItemProvider(dbChoiceItem.Id));

            return null;
        }
    }
}