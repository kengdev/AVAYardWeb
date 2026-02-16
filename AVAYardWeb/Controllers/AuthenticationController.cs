using AVAYardWeb.Models;
using AVAYardWeb.Models.Entities;
using AVAYardWeb.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AVAYardWeb.Controllers;

public class AuthenticationController : Controller
{
    private readonly DbavayardContext db;

    public AuthenticationController(DbavayardContext context)
    {
        db = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignIn() => (IActionResult)this.View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SignIn(LoginModel model)
    {
        var _log = new LogRepository(db);
        var result = db.AccAccounts.Where(w => w.AccUsername == model.Username &&
                                                w.AccPassword == this.SaltHash(model.Password)).FirstOrDefault();
        if (ModelState.IsValid && result != null)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(principal);

            _log.AddSystemLog("-", "Login successfuly.", model.Username);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            _log.AddSystemLog("-", "Login failed.", model.Username);
            return View(model);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> logoff()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login", "Authentication");
    }


    public IActionResult AccessDenied()
    {
        return View();
    }

    private string SaltHash(string input)
    {
        Rfc2898DeriveBytes PBKDF2_hash = new Rfc2898DeriveBytes(input, Encoding.ASCII.GetBytes("rm4fSDh0sofK"), 20000);
        HMACSHA256 HMACSHA256_hash = new HMACSHA256();
        //System.Text.Encoding.UTF8.GetString(bytes);
        string hPassword = Convert.ToBase64String(PBKDF2_hash.GetBytes(32));

        return hPassword;
    }
}
