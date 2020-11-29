using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PorusTeamOrientedBot.Bots;
using System;
using System.Collections.Generic;

namespace PorusSourceCode.Services
{
    public class BotStateService
    {
        #region Variables

        // State Variables
        public ConversationState ConversationState { get; }

        public UserState UserState { get; }

        public ConverenceRefrenceData ConversationRefrecnce { get; }

        // IDs
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";

        public static string LangProfileId { get; } = $"{nameof(BotStateService)}.LangProfile";

        public static string ConversationDataId { get; } = $"{nameof(BotStateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";

        public static string ConversationRefrecnceId { get; } = $"{nameof(BotStateService)}.ConversationRefrecnce";

        // Accessors

        public IStatePropertyAccessor<Dictionary<string, (string ActivityId, int InputCount)>> InputCardStateAccessor { get; internal set; }
        public IStatePropertyAccessor<ConverenceRefrenceData> ConversationRefrecnceAccessor { get; set; }

        public IStatePropertyAccessor<string> LangProfileAccessor { get; set; }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        #endregion Variables

        public BotStateService(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));

            InitializeAccessors();
        }

        public void InitializeAccessors()
        {
            // Initialize Conversation State Accessors

            ConversationRefrecnceAccessor = ConversationState.CreateProperty<ConverenceRefrenceData>(ConversationRefrecnceId);

            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}