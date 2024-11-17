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

        [HttpPost]
        public async Task<IActionResult> SendEmailAsync([FromBody]string email)
        {
            await _mailService.Generate_OTP_Email(email);
            return Ok();
        }
        [HttpGet("ValidateOTP/{email}/{otp}")]
        public ActionResult ValidateOTP(string email, string otp)
        {
            _mailService.Check_OTP(email, otp);
            return Ok();
        }
    }
}
