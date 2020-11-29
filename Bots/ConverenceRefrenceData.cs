using Microsoft.Bot.Schema;

namespace PorusTeamOrientedBot.Bots
{
    public class ConverenceRefrenceData
    {
        public string activityId;
        public ChannelAccount user;
        public ChannelAccount bot;
        public ConversationAccount conversation;
        public string serviceUrl;
        public string channelId;
    }
}