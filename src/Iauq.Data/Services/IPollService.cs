using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface IPollService
    {
        bool AlreadyVoted(string userNameOrIpAddress, int pollId);
        IQueryable<Poll> GetAllPolls();
        Poll GetPollById(int id);
        Poll GetCurrentPoll();
        void SavePoll(Poll poll);
        void DeletePoll(Poll poll);
        Choice GetChoiceById(int choiceId);
        void SaveChoice(Choice choice);
        void DeleteChoice(Choice choice);
        ChoiceItem GetChoiceItemById(int choiceItemId);
        void SaveChoiceItem(ChoiceItem choiceItem);
        void DeleteChoiceItem(ChoiceItem choiceItem);
    }
}