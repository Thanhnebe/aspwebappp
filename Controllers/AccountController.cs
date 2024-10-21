using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Data;
using webappmvcasp.Models;
using System.Linq;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http; // Thêm dòng này để sử dụng CookieOptions

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(User model)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra xem email đã tồn tại chưa
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(model);
            }

            model.Password = HashPassword(model.Password);

            _context.Users.Add(model);
            _context.SaveChanges(); 

            return RedirectToAction("Login");
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user != null && VerifyPassword(password, user.Password))
        {
            // Tạo cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddDays(7) // Cookie có hiệu lực 7 ngày
            };

            Response.Cookies.Append("Username", user.Email, cookieOptions); // Chuyển thành Email
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
        return View();
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("Username");
        return RedirectToAction("Index", "Home");
    }

    private string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));

        return $"{Convert.ToBase64String(salt)}.{hashed}";
    }

    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var parts = storedPassword.Split('.');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = parts[1];

        var enteredHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: enteredPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));

        return hash == enteredHash;
    }
}
