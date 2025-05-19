using DOOR.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class AuthController : Controller
{
    private readonly string _userFilePath = Path.Combine(Directory.GetCurrentDirectory(), "users.json");

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var users = JsonSerializer.Deserialize<List<AppUser>>(System.IO.File.ReadAllText(_userFilePath));

        var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            HttpContext.Session.SetString("Username", user.Username);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Неверный логин или пароль.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); 
        return RedirectToAction("Index", "Home"); 
    }
}
