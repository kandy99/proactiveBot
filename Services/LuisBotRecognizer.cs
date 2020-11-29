using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace PorusSourceCode
{
    public class LuisBotRecognizer : IBotServices
    {
        public LuisBotRecognizer(IConfiguration configuration)
        {
            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
               $"{configuration["LuisAPIHostName"]}");

            var recognizerOptions = new LuisRecognizerOptionsV2(luisApplication)
            {
                IncludeAPIResults = true,
                PredictionOptions = new LuisPredictionOptions()
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            Dispatch = new LuisRecognizer(recognizerOptions);
        }

        public LuisRecognizer Dispatch { get; private set; }
    }
}