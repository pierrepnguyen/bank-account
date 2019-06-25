using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using bank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bank.Controllers
{
  public class HomeController : Controller
  {
    private bankContext dbContext;
    public HomeController(bankContext context)
    {
      dbContext = context;
    }
    
    [HttpGet("")]
    public IActionResult Index()
    {
      return View();
    }

    [HttpGet("index")]
    public IActionResult Success()
    {
      int? UserId = HttpContext.Session.GetInt32("UserId");
      if(HttpContext.Session.GetInt32("UserId") != null)
      {
        User retrievedUser = dbContext.Users
        .Include(u => u.Transactions)
        .FirstOrDefault(u => u.UserId == UserId);
        Both both = new Both();
        both.user = retrievedUser;
        return View("account", both);
      }
      else
      {
        return RedirectToAction("Index");
      }
    }

    [HttpPost("register")]
    public IActionResult Register(Both newUser)
    {
      User user = newUser.user;
      if(ModelState.IsValid)
      {
        if(dbContext.Users.Any(u => u.Email == user.Email))
        {
          ModelState.AddModelError("user.Email", "Email is already registered");
          return View("Index");
        }
        else
        {
          PasswordHasher<User> Hasher = new PasswordHasher<User>();
          user.Password = Hasher.HashPassword(newUser.user, user.Password);
          dbContext.Add(newUser.user);
          dbContext.SaveChanges();
          HttpContext.Session.SetInt32("UserId", user.UserId);
          return RedirectToAction("Success");
        }
      }
      else
      {
        return View("Index");
      }
    }

    [HttpPost("login")]
    public IActionResult Login(Both userSubmission)
    {
      LoginUser login = userSubmission.login;
      if(ModelState.IsValid)
      {
        var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == login.Email);

        if(userInDb == null)
        {
          ModelState.AddModelError("login.Email", "Invalid Email/Password");
          return View("Index");
        }

        var hasher = new PasswordHasher<LoginUser>();
        var result = hasher.VerifyHashedPassword(userSubmission.login, userInDb.Password, login.Password);

        if(result == 0)
        {
          ModelState.AddModelError("login.Email", "Invalid Email/Password");
          return View("Index");
        }
        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
        return RedirectToAction("Success");
      }
      else
      {
        return View("Index");
      }
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      return RedirectToAction("Index");
    }

    [HttpPost("account")]
    public IActionResult Account(Transaction form)
    {
      User user = dbContext.Users
        .Include(data => data.Transactions)
        .FirstOrDefault(users => users.UserId == HttpContext.Session.GetInt32("UserId"));

        int AccountBalance = (int) user.Transactions.Sum(trans => trans.Amount);

        if(ModelState.IsValid)
        {
          if(AccountBalance + form.Amount < 0)
          {
            ModelState.AddModelError("Amount", "You may not overdraw from your account");
            User retrievedUser = dbContext.Users
              .Include(u => u.Transactions)
              .FirstOrDefault(u => u.UserId == (int) HttpContext.Session.GetInt32("UserId"));
            Both both = new Both();
            both.user = retrievedUser;
            return View("account", both);
          }
          else
          {
            Transaction Debit = new Transaction()
            {
              Amount = form.Amount,
              UserId = (int) HttpContext.Session.GetInt32("UserId")
            };
            dbContext.transactions.Add(Debit);
            dbContext.SaveChanges();
            return RedirectToAction("Success");
          }
        }
        else
        {
          return View("account");
        }
    }


  }
}
