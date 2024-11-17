namespace Domain
{
    public class Mail
    {
        public string Address { get; set; }
        public OTPInfo OTPInfo { get; set; } 
    }

    public class OTPInfo
    {
        public string OTP { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int Attempt { get; set; }
    }

    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}