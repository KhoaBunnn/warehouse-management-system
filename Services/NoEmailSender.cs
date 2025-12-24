using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace QLKhoHang.Services
{
    public class NoEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Không gửi email -> bỏ qua
            return Task.CompletedTask;
        }
    }
}
