using System.Data.Entity;
using System.Linq;
using Iauq.Core;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class PollService : IPollService
    {
        private readonly IDbSet<Answer> _answers;
        private readonly IDbSet<ChoiceItem> _choiceItems;
        private readonly IDbSet<Choice> _choices;
        private readonly IDbSet<Poll> _polls;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public PollService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _polls = _unitOfWork.Set<Poll>();
            _answers = _unitOfWork.Set<Answer>();
            _choices = _unitOfWork.Set<Choice>();
            _choiceItems = _unitOfWork.Set<ChoiceItem>();
        }

        #region IPollService Members

        public bool AlreadyVoted(string userNameOrIpAddress, int pollId)
        {
            User user = _userService.GetUserByUserName(userNameOrIpAddress);

            bool isIp = user == null;

            Poll poll = GetPollById(pollId);

            if (poll == null)
                throw new EntityNotFoundException<Poll>(p => p.Id);

            IQueryable<Answer> query = (from polls in _polls
                                        join choices in _choices on polls.Id equals choices.OwnerId
                                        join choiceItems in _choiceItems on choices.Id equals choiceItems.OwnerId
                                        join answers in _answers on choiceItems.Id equals answers.SelectedItemId
                                        select answers);

            if (isIp)
                return query.Any(a => a.IpAddress == userNameOrIpAddress);

            return query.Any(a => a.Answerer.Id == user.Id);
        }

        public IQueryable<Poll> GetAllPolls()
        {
            return _polls.OrderByDescending(p=>p.CreateDateTicks);
        }

        public Poll GetPollById(int id)
        {
            return _polls.Find(id);
        }

        public Poll GetCurrentPoll()
        {
            IQueryable<Poll> query = _polls.Where(p => p.IsActive && p.ShowOnHomePage).OrderByDescending(p=>p.CreateDateTicks);

            return query.FirstOrDefault();
        }

        public void SavePoll(Poll poll)
        {
            _polls.Add(poll);
        }

        public void DeletePoll(Poll poll)
        {
            _polls.Remove(poll);
        }

        public Choice GetChoiceById(int choiceId)
        {
            return _choices.Find(choiceId);
        }

        public void SaveChoice(Choice choice)
        {
            _choices.Add(choice);
        }

        public void DeleteChoice(Choice choice)
        {
            _choices.Remove(choice);
        }

        public ChoiceItem GetChoiceItemById(int choiceItemId)
        {
            return _choiceItems.Find(choiceItemId);
        }

        public void SaveChoiceItem(ChoiceItem choiceItem)
        {
            _choiceItems.Add(choiceItem);
        }

        public void DeleteChoiceItem(ChoiceItem choiceItem)
        {
            _choiceItems.Remove(choiceItem);
        }

        #endregion
    }
}