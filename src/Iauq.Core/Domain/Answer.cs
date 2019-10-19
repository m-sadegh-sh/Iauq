namespace Iauq.Core.Domain
{
    public class Answer : EntityBase
    {
        public virtual int SelectedItemId { get; set; }
        public virtual ChoiceItem SelectedItem { get; set; }

        public virtual int? AnswererId { get; set; }
        public virtual User Answerer { get; set; }

        public virtual string IpAddress { get; set; }
    }
}