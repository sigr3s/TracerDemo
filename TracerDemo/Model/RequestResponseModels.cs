using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TracerDemo.Model
{

    public enum UserValidationResponse
    {
        Validated = 1,
        Invalid = 2,
        LockedOut = 3,
        Invalidated = 4
    }

    public class ChangePasswordDataModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class AccountUpdateDataModel
    {

        public string Email { get; set; }

        public string Name { get; set; }
    }

    public class AuthenticateDataModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

    }

    public class RegisterDataModel
    {
        //[Required]
        //public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordDataModel
    {
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }


    public class ForgotPasswordDataModel
    {
        public string Email { get; set; }
    }

    public class TodoItemViewModel
    {
        public string Task { get; set; }
    }

    public class FacebookApiResult
    {
        public string id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string locale { get; set; }
        public FacebookApiResultLocation location { get; set; }
    }

    public class FacebookApiResultLocation
    {
        public string name { get; set; }
    }








    public class IdResponse
    {
        public string Id { get; set; }
    }

    public class MessageResponse
    {
        public string Message { get; set; }
    }

    public class UserSlim
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public List<Rol> Roles { get; set; }
        public DateTime LastSignin { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
