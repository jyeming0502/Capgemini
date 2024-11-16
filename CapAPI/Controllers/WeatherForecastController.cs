using Microsoft.AspNetCore.Mvc;
using Domain;

using CapApplication.Services;

namespace CapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMailService _mailService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet("Get/{email}")]
        public async Task<IActionResult> Get(string email)
        {
            var message = new Message("jyemingchan05@gmail.com", "Test email", "This is the content from our email.");
            try
            {
                await _mailService.SendEmailAsync(message);
            }
            catch
            {
                throw new InvalidOperationException("Email sending failed!");
            }
            return Ok();
        }
    }
}
