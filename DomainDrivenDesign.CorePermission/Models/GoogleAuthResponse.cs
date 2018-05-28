using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.CorePermission.Models
{

    public class GoogleAuthResponse
    {
        public string iss;
        public string sub;
        public string azp;
        public string aud;
        public string iat;
        public string exp;
        public string email;
        public string email_verified;
        public string name;
        public string picture;
        public string given_name;
        public string family_name;
        public string locale;
    }

    public class FacebookAuthResponse
    {
        public string id;
        public string name;
        public string email;
    }
}
