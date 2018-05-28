using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeUserProfile
    {
        public Guid Id;
        public string Username;
        public string Email;
        public string Phone;

    }

    public class FeUserAccountCheckCode
    {
        public bool Success;
        public string ErrorMessage;
    }

}