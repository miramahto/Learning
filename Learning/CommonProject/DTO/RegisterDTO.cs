using AuthApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApi.DTO
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterApps
    {
        
        public string AppID { get; set; }
        public string AppName { get; set; }
        public Guid GUID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class UserApplicationsMapping
    {
         public int AppID { get; set; }
        public string UserID { get; set; }
       

    }
}
