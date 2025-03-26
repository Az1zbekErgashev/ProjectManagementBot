using ManagementBot.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ChatBot.Controllers;

[ApiController]
[Route("/")]
public class BotController : ControllerBase
{
    private readonly IBotService _botService;

    public BotController(IBotService botService)
        => _botService = botService;

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] Update update)
    {
        await _botService.HandleUpdateAsync(update);
        return Ok();
    }

    [HttpGet]
    public Task<string> Get() => Task.FromResult("Telegram bot was started");
}
