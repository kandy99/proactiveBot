// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2
//using Microsoft.Bot.Builder.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PorusSourceCode;

using PorusSourceCode.Services;
using PorusTeamOrientedBot.Bots;
using System.Collections.Concurrent;

namespace PorusTeamOrientedBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddRazorPages();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<IBotServices, LuisBotRecognizer>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.

            services.AddTransient<IBot, ProactiveBot>();

            services.AddSingleton<ShowTypingMiddleware>();

            // Create a global hashset for our ConversationReferences
            services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();

            // Create an instanc of the state service

            // Create an instanc of the state service
            services.AddSingleton<BotStateService>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.

            //services.AddTransient<IBot, DialogBot<MainDialog>>();

            ConfigureState(services);
        }

        public void ConfigureState(IServiceCollection services)
        {
            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            //services.AddSingleton<IStorage, MemoryStorage>();

            var storageAccount = "DefaultEndpointsProtocol=https;AccountName=blobstorageresource;AccountKey=DdU/IfXLT1qI9WPCs7P+mdcMX+YzAQkSftNQlH/XmqiiBnqsL31q/MI4kBweuwS1De4h10N9KXK7MYJe3pqTKA==;EndpointSuffix=core.windows.net";
            var storageContainer = "blobstorageresource";

            services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));

            // Create the User state.
            services.AddSingleton<UserState>();

            // Create the Conversation state.
            services.AddSingleton<ConversationState>();

            // Create an instanc of the state service
            services.AddSingleton<BotStateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                 .UseEndpoints(endpoints =>
                 {
                     endpoints.MapControllers();
                     endpoints.MapRazorPages();
                 });

            // app.UseHttpsRedirection();
        }
    }
}