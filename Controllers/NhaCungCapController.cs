using Microsoft.AspNetCore.Mvc;
using QLKhoHang.Models;

public class NhaCungCapController : Controller
{
    private readonly INhaCungCapRepository _repo;

    public NhaCungCapController(INhaCungCapRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _repo.GetAllAsync();
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.NewId = await _repo.GenerateNewIdAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhaCungCap ncc)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.NewId = await _repo.GenerateNewIdAsync();
            return View(ncc);
        }

        await _repo.AddAsync(ncc);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var obj = await _repo.GetByIdAsync(id);
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(NhaCungCap ncc)
    {
        if (!ModelState.IsValid)
            return View(ncc);

        _repo.Update(ncc);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string id)
    {
        var obj = await _repo.GetByIdAsync(id);
        if (obj == null) return NotFound();
        return View(obj);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var obj = await _repo.GetByIdAsync(id);
        if (obj == null) return NotFound();

        _repo.Delete(obj);
        await _repo.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
