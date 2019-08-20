using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bank_accounts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace bank_accounts.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User NewUser)
        {
            if(ModelState.IsValid)
            {
                        // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == NewUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    
                    // You may consider returning to the View at this point
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    NewUser.Password = Hasher.HashPassword(NewUser, NewUser.Password);
                    dbContext.Add(NewUser);
                    dbContext.SaveChanges();
                    dbContext.SaveChanges();
                    
                    HttpContext.Session.SetInt32("id",dbContext.Users.Last().UserId);
                    int? id = HttpContext.Session.GetInt32("id");

                    return RedirectToAction("BankAccount", new {id = id});
                   
                }   
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("logging")]
        public IActionResult Logging(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    return View("Index");
                }
                
                User currentUser = dbContext.Users
                    .FirstOrDefault(user => user.Email == userSubmission.LoginEmail);  
                
                // Now that I have an email, I want to find user with the unique email and grab that user's id
            
                HttpContext.Session.SetInt32("id", currentUser.UserId);
                
                int? id = HttpContext.Session.GetInt32("id");

                return RedirectToAction("BankAccount", new {id = id});
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("bankaccount/{id}")]
        public IActionResult BankAccount(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("id");
            if(UserId == null)
            {
                return View("Index");
            }
            else
            {
                // This is a list of all users
        
                // Can access information about the user logged in
                User user = dbContext.Users
                    .Include(u => u.MyTransactions)
                    .FirstOrDefault( u => u.UserId == id);

                return View("BankAccount", user);
            }
        }

        [HttpPost("transaction")]
        public IActionResult Transaction(Transaction newTrans)
        {
            User user = dbContext.Users
                .Include(u => u.MyTransactions)
                .FirstOrDefault( u => u.UserId == newTrans.UserId);

            if(ModelState.IsValid)
            {
                Decimal trans = (decimal)newTrans.AmmountDecimal;

                if(trans < 0 && Math.Abs(trans) > user.Balance)
                {
                    ModelState.AddModelError("AmmountDecimal", "Not enough money");

                    return View("BankAccount", user);
                }
                else
                {
                    Transaction theTrans = newTrans;
                    dbContext.Add(newTrans);
                    dbContext.SaveChanges();

                    return RedirectToAction("BankAccount", new {id=newTrans.UserId});
                }
            }
            return View("BankAccount", user);
        }
        
        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
