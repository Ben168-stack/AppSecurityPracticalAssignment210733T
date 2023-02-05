using AppSecurityPracticalAssignment210733T.Models;

namespace AppSecurityPracticalAssignment210733T.Services
{
    public class AuditLogService
    {

        private readonly AuthDbContext _context;
        public AuditLogService(AuthDbContext context)
        {
            _context = context;
        }
        public List<AuditLog> GetAll()
        {
            return _context.AuditLog.OrderBy(x => x.Id).ToList();
        }

        public void AddAuditLog(AuditLog auditlog)
        {
            _context.AuditLog.Add(auditlog);
            _context.SaveChanges();
        }
    }
}
