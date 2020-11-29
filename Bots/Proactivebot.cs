using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using PorusSourceCode.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PorusTeamOrientedBot.Bots
{
    public class ProactiveBot : ActivityHandler
    {
        // Message to send to users when the bot receives a Conversation Update event
        private const string WelcomeMessage = "Welcome to the Proactive Bot sample.  Navigate to http://localhost:3978/api/notify to proactively message everyone who has previously messaged this bot.";

        private BotState _userState;

        protected readonly BotStateService _botStateService;

        // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public ProactiveBot(ConcurrentDictionary<string, ConversationReference> conversationReferences, BotStateService botStateService, UserState
            userState)
        {
            _conversationReferences = conversationReferences;

            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            _userState = userState ?? throw new NullReferenceException(nameof(userState));
        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(WelcomeMessage), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //here is the converation refrence is added and I am saving it to my blob below

            AddConversationReference(turnContext.Activity as Activity);
            var converenceRefrenceData = await _botStateService.ConversationRefrecnceAccessor.GetAsync(turnContext, () => new ConverenceRefrenceData());

            var conversationReference = turnContext.Activity.GetConversationReference();

            converenceRefrenceData.activityId = conversationReference.ActivityId;
            converenceRefrenceData.bot = conversationReference.Bot;
            converenceRefrenceData.channelId = conversationReference.ChannelId;
            converenceRefrenceData.conversation = conversationReference.Conversation;
            converenceRefrenceData.serviceUrl = conversationReference.ServiceUrl;
            converenceRefrenceData.user = conversationReference.User;

            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);

            // Echo back what the user said
            await turnContext.SendActivityAsync(MessageFactory.Text($"You sent '{turnContext.Activity.Text}'"), cancellationToken);
        }
    }
}