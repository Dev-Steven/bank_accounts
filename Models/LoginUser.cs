using System.ComponentModel.DataAnnotations;

namespace bank_accounts.Models
{
    public class LoginUser
    {
        // No other fields!
        public string LoginEmail {get;set;}

        [DataType(DataType.Password)]
        public string LoginPassword {get;set;}
    }
}