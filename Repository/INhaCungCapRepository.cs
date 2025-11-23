using QLKhoHang.Models;

public interface INhaCungCapRepository
{
    Task<IEnumerable<NhaCungCap>> GetAllAsync();
    Task<NhaCungCap> GetByIdAsync(string id);

    Task AddAsync(NhaCungCap ncc);
    void Update(NhaCungCap ncc);
    void Delete(NhaCungCap ncc);

    Task<string> GenerateNewIdAsync();
    Task SaveChangesAsync();
}
