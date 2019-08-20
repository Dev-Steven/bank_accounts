using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;   // Add this for [NotMapped]

namespace bank_accounts.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId {get;set;}

        [Required]
        [Display(Name="Deposit/Withdraw:")]
        public double AmmountDecimal {get;set;}

        public int UserId {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public User Owner {get;set;}

    }
}