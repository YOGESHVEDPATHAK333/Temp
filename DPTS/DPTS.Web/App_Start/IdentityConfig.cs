﻿using System.Security.Claims;
using System.Threading.Tasks;
using DPTS.Web.ASPSMSX2;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using DPTS.Web.Models;
using Microsoft.Owin.Security;

namespace DPTS.Web
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            //var Twilio = new TwilioRestClient("AC6eec9ab16de06b6555d5823d3d7d009f", "26cbda1255a0952c962af6b27fa216af");
            ////System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"],
            ////System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"]);
            ////System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"]
            //var result = Twilio.SendMessage("+14703759131", message.Destination, message.Body);
            ////Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
            //Trace.TraceInformation(result.Status);

            //ASPSMS Begin
            var soapSms = new ASPSMSX2SoapClient("ASPSMSX2Soap");
            soapSms.SendSimpleTextSMS(
                System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"],
                System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"],
                message.Destination,
                System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"],
                message.Body);
            soapSms.Close();
            //ASPSMS End
            return Task.FromResult(0);
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        /* public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager);
        }*/

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}