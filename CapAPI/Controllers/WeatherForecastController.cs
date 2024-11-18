using Microsoft.AspNetCore.Mvc;
using CapPersistence.Services;
using Domain;

namespace CapAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {       
        private readonly MailService _mailService;

        public WeatherForecastController(MailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<ActionResult> SendEmailAsync([FromBody]string email)
        {
            await _mailService.Generate_OTP_Email(email);
            return Ok();
        }
        [HttpGet("ValidateOTP/{email}/{otp}")]
        public ActionResult<OTPResponse> ValidateOTP(string email, string otp)
        {
            OTPResponse response = _mailService.Check_OTP(email, otp);
            return Ok(response);
        }
    }
}
