namespace Domain
{
    public class Mail
    {
        public string Address { get; set; }
        public string OTP { get; set; } 
    }
    //public class Message
    //{
    //    public string Subject { get; set; }
    //    public string To { get; set; }
    //    public string Content { get; set; }
    //    public Message(string To, string subject, string Content)
    //    {
    //        this.To = To;       
    //        this.Subject = subject; 
    //        this.Content = Content;
    //    }
    //}

    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}