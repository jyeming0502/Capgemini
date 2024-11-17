using Microsoft.Extensions.Logging;
using CapApplication.Services;
using MailKit.Net.Smtp;
using MimeKit;
using Domain;

namespace CapPersistence.Services
{
    public class MailService: IMailService
    {
        private ILogger<MailService> _logger;
        private readonly EmailConfiguration _emailConfig;
        private readonly Dictionary<string, OTPInfo> storeOTPInfo = new();
        public MailService(ILogger<MailService> logger, EmailConfiguration emailConfig)
        {
            _logger = logger;                        
            _emailConfig = emailConfig; 
        }       

        public string GenerateOTP()
        {
            Random rand = new Random();
            string randNum = rand.Next(999999).ToString();
            return randNum;
        }
        private MimeMessage CreateEmailMessage(string email)
        {
            string otp = GenerateOTP();
            storeOTPInfo[email] = new OTPInfo { OTP = otp, ExpiryTime = DateTime.Now.AddMinutes(1) };
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Jyeming", _emailConfig.From));
            emailMessage.To.Add(new MailboxAddress("Jyeming", email));
            emailMessage.Subject = "Verify your account";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Your OTP code is {otp}. The code is valid for 1 minute."
            };
            return emailMessage;
        }
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    throw new Exception("STATUS_EMAIL_FAIL: email address does not exist or sending to the email has failed.");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
        public async Task Generate_OTP_Email(string email)
        {
            var emailMessage = CreateEmailMessage(email);
            await SendAsync(emailMessage);           
        }       
        public string Check_OTP(string email, string otpInput)
        {
            storeOTPInfo[email].Attempt++;
            // check for email exists
            if (!storeOTPInfo.ContainsKey(email))
            {
                throw new Exception("STATUS_OTP_NOTSENT: OTP is not sent to your email.");
            }
            // check for otp expiry
            if(DateTime.Now > storeOTPInfo[email].ExpiryTime)
            {
                storeOTPInfo.Remove(email);
                throw new Exception("STATUS_OTP_TIMEOUT: timeout after 1 min");
            }
            // check for otp attempt
            if (storeOTPInfo[email].Attempt >= 3)
            {
                storeOTPInfo.Remove(email);
                throw new Exception("STATUS_OTP_FAIL: OTP is wrong after 10 tries");
            }
            // validate the otp input vs the otp sent 
            if (storeOTPInfo[email].OTP == otpInput)
            {
                storeOTPInfo.Remove(email);
                return "STATUS_OTP_OK: OTP is valid and checked";
            }
            // return if otp is invalid
            throw new Exception("STATUS_OTP_INVALID: OTP is invalid. Please try again.");
        }        
    }
}
