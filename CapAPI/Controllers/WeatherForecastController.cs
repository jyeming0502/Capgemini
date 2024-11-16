using Microsoft.AspNetCore.Mvc;
using Domain;

using CapApplication.Services;

namespace CapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       

        public ILogger<WeatherForecastController> _logger;
        public IMailService _mailService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet("GetEmail/{email}")]
        public async Task<ActionResult> GetEmail(string email)
        {            
            try
            {
                await _mailService.SendEmailAsync(email);
            }
            catch
            {
                throw new InvalidOperationException("Email sending failed!");
            }
            return Ok();
        }
    }
}
