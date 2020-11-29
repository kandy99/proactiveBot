// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AdaptiveCards.Templating;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PorusSourceCode.Services;
using PorusTeamOrientedBot.Bots;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PorusSourceCode.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private IConfiguration _configuration;
        protected readonly BotStateService _botStateService;
        private long C_uten;
        private string Emailmessage;
        private string projectlink;
        private List<string> ObjID;
        private List<string> ObjectTitle;
        public Attachment cardData = new Attachment();

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences, BotStateService botStateService)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"];
            _configuration = configuration;
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(_appId))
            {
                _appId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }
        }

        public async Task<IActionResult> Get()

        {
            // the problem lies here the conversation reference is saving in the local storage but when we are going to use the state accessor in the blob storatge we need
            // to have a turn context for accepting the same value from blob

            // ConverenceRefrenceData converenceRefrenceData = await _botStateService.ConversationRefrecnceAccessor.GetAsync(turnContext, () => new ConverenceRefrenceData());

            // here lies the problem of accessing the convesation refrence to get an instance and send a message back

            foreach (var conversationReference in _conversationReferences.Values)
            {
                await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, BotCallback, default(CancellationToken));
            }

            // Let the caller know proactive messages have been sent
            return new ContentResult()
            {
                Content = "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            // If you encounter permission-related errors when sending this message, see
            ConverenceRefrenceData converenceRefrenceData = await _botStateService.ConversationRefrecnceAccessor.GetAsync(turnContext, () => new ConverenceRefrenceData());

            // https://aka.ms/BotTrustServiceUrl
            await turnContext.SendActivityAsync("proactive hello");
        }
    }
}