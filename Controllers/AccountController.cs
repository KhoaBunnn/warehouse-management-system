using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        // Trả về Partial View thông báo logout
        TempData["LogoutMessage"] = "Bạn đã đăng xuất thành công!";
        return RedirectToAction("Index", "Home");
    }
}
