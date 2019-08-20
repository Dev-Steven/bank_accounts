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

                HttpContext.Session.SetInt32("id",dbContext.Users.Last().UserId);
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
                List<Transaction> transactions = dbContext.Transactions
                        .Include(x => x.Owner)
                        .Where(trans => trans.UserId == id)
                        .ToList();
                ViewBag.transactions = transactions;
                double sum = 0;
                foreach(var trans in transactions)
                {
                    sum += trans.AmmountDecimal; 
                }
                ViewBag.balance = sum;
            
                
                // Can access information about the user logged in
                User user = dbContext.Users.FirstOrDefault( u => u.UserId == id);
                ViewBag.userId = (int)id;
                ViewBag.user = user.FirstName;
                // had balance here
                

                return View("BankAccount");
            }
        }

        [HttpPost("transaction/{id}")]
        public IActionResult Transaction(Transaction newTrans, int id)
        {
            if(ModelState.IsValid)
            {
                Transaction theTrans = newTrans;
                dbContext.Add(newTrans);

                // Manually assigning UserId from session
                newTrans.UserId = id;
                newTrans.CreatedAt = DateTime.Now;
                dbContext.SaveChanges();

                return RedirectToAction("BankAccount", new {id=id});
            }
            return View("BankAccount", new {id = id});
        }
        
        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}
