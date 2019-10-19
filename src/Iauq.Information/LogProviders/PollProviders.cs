using System.Text;
using Iauq.Core.Domain;
using Iauq.Data.Logging;

namespace Iauq.Information.LogProviders
{
    public class CreatePollProvider : LogProviderBase<Poll>
    {
        public CreatePollProvider(Poll instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            var choices = new StringBuilder();

            if (Instance.Choices != null)
                foreach (Choice choice in Instance.Choices)
                {
                    var items = new StringBuilder();

                    if (choice.Items != null)
                        foreach (ChoiceItem choiceItem in choice.Items)
                        {
                            choices.AppendFormat("text: \"{0}\", ", choiceItem.Text);
                        }

                    choices.AppendFormat("title: \"{0}\", description: \"{1}\", items: ({2})), ",
                                         choice.Title, choice.Description, items.ToString().Trim(' ', ','));
                }

            log.Message =
                string.Format(
                    "user \"{0}\" created a new poll: (title: \"{1}\", description: \"{2}\", choices: ({3}))",
                    Identity, Instance.Title, Instance.Description, choices.ToString().Trim(' ', ','));
            log.Level = LogLevel.Create;
        }
    }

    public class UpdatePollProvider : LogProviderBase<Poll>
    {
        public UpdatePollProvider(Poll instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            var choices = new StringBuilder();

            if (Instance.Choices != null)
                foreach (Choice choice in Instance.Choices)
                {
                    var items = new StringBuilder();

                    if (choice.Items != null)
                        foreach (ChoiceItem choiceItem in choice.Items)
                        {
                            choices.AppendFormat("id: \"{0}\", text: \"{1}\", ", choiceItem.Id, choiceItem.Text);
                        }

                    choices.AppendFormat("id: \"{0}\", title: \"{1}\", description: \"{2}\", items: ({3})), ",
                                         choice.Id, choice.Title, choice.Description, items.ToString().Trim(' ', ','));
                }

            log.Message =
                string.Format(
                    "user \"{0}\" updated a poll: (id: \"{1}\", title: \"{2}\", description: \"{3}\", choices: ({4}))",
                    Identity,Instance.Id, Instance.Title, Instance.Description, choices.ToString().Trim(' ', ','));
            log.Level = LogLevel.Update;
        }
    }

    public class DeletePollProvider : LogProviderBase<int>
    {
        public DeletePollProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a poll: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }

    public class CreateChoiceProvider : LogProviderBase<Choice>
    {
        public CreateChoiceProvider(Choice instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new choice: (owner-id: \"{1}\", title: \"{2}\", description: \"{3}\")",
                    Identity, Instance.OwnerId, Instance.Title, Instance.Description);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateChoiceProvider : LogProviderBase<Choice>
    {
        public UpdateChoiceProvider(Choice instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a choice: (id: \"{1}\", title: \"{2}\", description: \"{3}\")",
                    Identity, Instance.Id, Instance.Title, Instance.Description);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteChoiceProvider : LogProviderBase<int>
    {
        public DeleteChoiceProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a choice: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }

    public class CreateChoiceItemProvider : LogProviderBase<ChoiceItem>
    {
        public CreateChoiceItemProvider(ChoiceItem instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" created a new choice-item: (owner-id: \"{1}\", text: \"{2}\")",
                    Identity, Instance.OwnerId, Instance.Text);
            log.Level = LogLevel.Create;
        }
    }

    public class UpdateChoiceItemProvider : LogProviderBase<ChoiceItem>
    {
        public UpdateChoiceItemProvider(ChoiceItem instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message =
                string.Format(
                    "user \"{0}\" updated a choice-item: (id: \"{1}\", text: \"{2}\")",
                    Identity, Instance.Id, Instance.Text);
            log.Level = LogLevel.Update;
        }
    }

    public class DeleteChoiceItemProvider : LogProviderBase<int>
    {
        public DeleteChoiceItemProvider(int instance)
            : base(instance)
        {
        }

        protected override void Inject(Log log)
        {
            log.Message = string.Format("user \"{0}\" deleted a choice-item: (id: \"{1}\")", Identity, Instance);
            log.Level = LogLevel.Delete;
        }
    }
}