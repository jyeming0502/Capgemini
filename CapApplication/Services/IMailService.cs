using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapApplication.Services
{
    public interface IMailService
    {
        Task Generate_OTP_Email(string message);
        string Check_OTP(string email, string otp);
    }
}
