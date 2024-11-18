using MailKit.Net.Smtp;
using MimeKit;
using Domain;

namespace CapPersistence.Services
{
    public class MailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly Dictionary<string, OTPInfo> storeOTPInfo = new();
        public MailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig; 
        }       

        // generate 6 digits otp, thus the minimum value of the otp starts from 100000
        public string GenerateOTP()
        {
            Random rand = new Random();
            string randNum = rand.Next(100000, 999999).ToString();
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
        // configure the smtp server for gmail
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
        public OTPResponse Check_OTP(string email, string otpInput)
        {
            // check for email exists
            if (!storeOTPInfo.ContainsKey(email))
            {
                return new OTPResponse()
                {
                    StatusCode = "STATUS_OTP_NOTSENT",
                    Message = "OTP is not sent to your email."
                };
            }
            // check for otp expiry
            if(DateTime.Now > storeOTPInfo[email].ExpiryTime)
            {
                storeOTPInfo.Remove(email);
                return new OTPResponse()
                {
                    StatusCode = "STATUS_OTP_TIMEOUT",
                    Message = "Timeout after 1 min"
                };
            }
            // check for otp attempt
            if (storeOTPInfo[email].Attempt >= 10)
            {
                storeOTPInfo.Remove(email);
                return new OTPResponse()
                {
                    StatusCode = "STATUS_OTP_FAIL",
                    Message = "OTP is wrong after 10 tries"
                };
            }
            // we put this count below of the attempt condition checking so that 
            // if it is the 10 attempt and the OTP is correct, we shall prompt STATUS_OTP_OK, instead of STATUS_OTP_FAIL. 
            // if we put the count at the beginning of this method, the attempt is already 10, but then if user input the correct OTP
            // user will still get STATUS_OTP_FAIL, which is wrong already.
            storeOTPInfo[email].Attempt++;
            // validate the otp input vs the otp sent 
            if (storeOTPInfo[email].OTP == otpInput)
            {
                storeOTPInfo.Remove(email);
                return new OTPResponse()
                {
                    StatusCode = "STATUS_OTP_OK",
                    Message = "OTP is valid and checked"
                };
            }
            // return if otp is invalid
            return new OTPResponse()
            {
                StatusCode = "STATUS_OTP_INVALID",
                Message = "OTP is invalid. Please try again. "
            };
        }        
    }
}
