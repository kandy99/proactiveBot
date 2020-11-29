using Microsoft.Bot.Builder.AI.Luis;

namespace PorusSourceCode
{
    public interface IBotServices
    {
        LuisRecognizer Dispatch { get; }
    }
}