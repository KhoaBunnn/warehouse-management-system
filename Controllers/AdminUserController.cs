using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace QLKhoHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminUserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Danh sách tài khoản
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Form phân quyền
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.AllRoles = _roleManager.Roles.ToList();
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(user);
        }
        public IActionResult CheckRole()
        {
            var roles = User.Claims
                .Where(c => c.Type.Contains("role"))
                .Select(c => c.Value);
            return Content("Roles: " + string.Join(", ", roles));
        }


        // Lưu phân quyền
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(string id, List<string>? roles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Nếu roles null → gán list rỗng
            roles ??= new List<string>();

            // Lấy roles hợp lệ trong DB
            var validRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var rolesToAssign = roles.Where(r => validRoles.Contains(r)).ToList();

            // Xóa quyền cũ
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa quyền cũ");
                return RedirectToAction("Edit", new { id });
            }

            // Thêm quyền mới
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAssign);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Không thể thêm quyền mới");
                return RedirectToAction("Edit", new { id });
            }

            return RedirectToAction("Index");
        }
    }
}
