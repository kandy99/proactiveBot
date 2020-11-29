// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.6.2
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PorusTeamOrientedBot
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger, ShowTypingMiddleware showTypingMiddleware, ConversationState conversationState = null)
           : base(configuration, logger)
        {
            Use(showTypingMiddleware);

            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

                // Send a message to the user
                await SendWithoutMiddleware(turnContext, "Sorry, Could you please try another statement.");

                if (conversationState != null)
                {
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
                }

                // Send a trace activity, which will be displayed in the Bot Framework Emulator
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }

        private static async Task SendWithoutMiddleware(ITurnContext turnContext, string message)
        {
            // Sending the Activity directly through the Adapter rather than through the TurnContext skips the middleware processing
            // this might be important in this particular case because it might have been the TranslationMiddleware that is actually failing!
            var activity = MessageFactory.Text(message);

            // If we are skipping the TurnContext we must address the Activity manually here before sending it.
            activity.ApplyConversationReference(turnContext.Activity.GetConversationReference());

            // Send the actual Activity through the Adapter.
            await turnContext.Adapter.SendActivitiesAsync(turnContext, new[] { activity }, CancellationToken.None);
        }
    }
}