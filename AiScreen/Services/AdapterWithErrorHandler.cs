using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace MaxSystemWebSite.Services
{
    public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public AdapterWithErrorHandler(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger)
            : base(configuration, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                logger.LogError($"Bot Error: {exception.Message}");
                await turnContext.SendActivityAsync("Sorry, something went wrong.");
            };
        }
    }

}
