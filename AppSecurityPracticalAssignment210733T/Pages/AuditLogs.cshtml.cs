using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsModel : PageModel
    {
        private readonly AuthDbContext _db;
        private readonly AuditLogService _auditLogService;

        public IEnumerable<AuditLog> AuditLogs { get; set; }
        public AuditLogsModel(AuthDbContext db, AuditLogService auditLogService)
        {
            _db = db;
            _auditLogService = auditLogService;
        }

        public void OnGet()
        {
            AuditLogs = _db.AuditLog.OrderBy(x => x.Timestamp).ToList();
        }
    }
}
