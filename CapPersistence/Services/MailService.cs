using Microsoft.Graph.Me.SendMail;
using Microsoft.Extensions.Logging;
using CapApplication.Services;
using FluentEmail.Core;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Graph.Groups.Item.Team.Channels.Item.DoesUserHaveAccessuserIdUserIdTenantIdTenantIdUserPrincipalNameUserPrincipalName;
using Domain;
using Microsoft.Graph.Models.TermStore;

namespace CapPersistence.Services
{
    public class MailService: IMailService
    {
        private ILogger<MailService> _logger;
        //private GraphServiceClient _graphServiceClient;
        //private readonly IFluentEmail _fluentEmail;
        private readonly EmailConfiguration _emailConfig;
        public MailService(ILogger<MailService> logger, EmailConfiguration emailConfig)
        {
            _logger = logger;            
            
            _emailConfig = emailConfig; 
        }
        //public SendMailPostRequestBody EmailBody(string mailAddress, string otp)
        //{
        //    return new SendMailPostRequestBody
        //    {
        //        Message = new Message
        //        {
        //            Subject = $"OTP for verification",
        //            Body = new ItemBody
        //            {
        //                ContentType = BodyType.Html,
        //                Content = $"Your OTP Code is {otp}. The code is valid for 1 minute."
        //            },
        //            ToRecipients = new List<Recipient>
        //            {
        //                new Recipient
        //                {
        //                    EmailAddress = new EmailAddress
        //                    {
        //                        Address = mailAddress
        //                    }
        //                }
        //            }
        //        },
        //        SaveToSentItems = false
        //    };
        //}

        public string GenerateOTPAndSendMail()
        {
            Random rand = new Random();
            string randNum = rand.Next(999999).ToString();
            return randNum;

            //await SendMailAsync(emailAddress, randNum);
        }
        public async Task SendEmailAsync(Message message)
        {
            string otp = GenerateOTPAndSendMail();
            var emailMessage = CreateEmailMessage(message, otp);
            await SendAsync(emailMessage);
            //await _fluentEmail
            //    .To("jyeming_chan@hotmail.com")
            //    .Subject("Validate your account")
            //    .Body("Your OTP Code is here. The code is valid for 1 minute.")
            //    .SendAsync();
            //try
            //{               
            //    SendMailPostRequestBody emailBody = new SendMailPostRequestBody();                  
            //    emailBody = EmailBody(emailAddress, otp);                      

            //    await _graphServiceClient.Me.SendMail.PostAsync(emailBody);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Fail to send mail.");
            //    throw;
            //}
        }
        private MimeMessage CreateEmailMessage(Message message, string otp)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Jyeming", _emailConfig.From));
            emailMessage.To.Add(new MailboxAddress("Jyeminggg","jyeming_chan@hotmail.com"));
            emailMessage.Subject = "Verify your account";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { 
                Text = $"Your OTP code is { otp }. The code is valid for 1 minute"
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
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
