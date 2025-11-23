using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Models;

public class NhaCungCapRepository : INhaCungCapRepository
{
    private readonly QLKhoHangContext _context;

    public NhaCungCapRepository(QLKhoHangContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NhaCungCap>> GetAllAsync()
    {
        return await _context.NhaCungCap.ToListAsync();
    }

    public async Task<NhaCungCap> GetByIdAsync(string id)
    {
        return await _context.NhaCungCap.FindAsync(id);
    }

    public async Task AddAsync(NhaCungCap ncc)
    {
        await _context.NhaCungCap.AddAsync(ncc);
    }

    public void Update(NhaCungCap ncc)
    {
        _context.NhaCungCap.Update(ncc);
    }

    public void Delete(NhaCungCap ncc)
    {
        _context.NhaCungCap.Remove(ncc);
    }

    public async Task<string> GenerateNewIdAsync()
    {
        var last = await _context.NhaCungCap
            .OrderByDescending(x => x.MaNCC)
            .Select(x => x.MaNCC)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(last))
            return "NCC001";

        int num = int.Parse(last.Substring(3)) + 1;
        return "NCC" + num.ToString("D3");
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
