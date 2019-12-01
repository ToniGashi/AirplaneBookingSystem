using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using AirplaneBookingSystem.Data;
using System;
using System.Diagnostics;

namespace AirplaneBookingSystem.Controllers
{
    public class EmailController : Controller
    {

        private readonly Db_Context ctx;

        public EmailController(Db_Context ctx) {
            this.ctx = ctx;
        }


        public async Task<IActionResult> SendEmail(List<string> emails)
        {       
            MimeMessage msg = new MimeMessage();

            msg.Subject = "Flight Postponing";
            msg.From.Add( new MailboxAddress("Admin", "my_airplane@outlook.com"));

            BodyBuilder bodyBuilder = new BodyBuilder();        

            foreach (var email in emails) {
                using (var emailClient = new SmtpClient())
                {
                    await emailClient.ConnectAsync("smtp-mail.outlook.com", 587, false);
                    await emailClient.AuthenticateAsync("my_airplane@outlook.com", "Iamadmin~");

                    var currentUser = ctx.GetUserFromEmail(email);
                    string FirstName = currentUser.FirstName;
                    string LastName = currentUser.LastName;

                    MailboxAddress to = new MailboxAddress(FirstName, email);

                    bodyBuilder.TextBody = "Dear " + FirstName + " " + LastName + 
                        ",\nDue to overbooked seats, we have decided to postpone your current flight.\nSincerely,\n\nAdministrator";

                    msg.To.Add(to);
                    msg.Body = bodyBuilder.ToMessageBody();

                   
                    
                   emailClient.Send(msg);
                    
                    

                    emailClient.Disconnect(true);
                    emailClient.Dispose();
                } 
            }
            
            

            return View(); ;
        }

        private void PrepareEmail() { }
    }
}