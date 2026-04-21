// Services/MemberService.cs
using ChildernOfTheBible.Data;
using ChildernOfTheBible.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildernOfTheBible.Services
{
    public class MemberService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly EncryptionService _enc;

        public MemberService(IDbContextFactory<ApplicationDbContext> factory, EncryptionService enc)
        {
            _factory = factory;
            _enc = enc;
        }

        public async Task<List<Member>> GetAllAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Members.ToListAsync();
        }

        public async Task AddAsync(Member m)
        {
            m.BarcodeId = Guid.NewGuid().ToString("N")[..10].ToUpper();
            m.Email = _enc.Encrypt(m.Email);
            m.PhoneNumber = _enc.Encrypt(m.PhoneNumber);

            await using var context = await _factory.CreateDbContextAsync();
            context.Members.Add(m);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Member m)
        {
            m.Email = _enc.Encrypt(m.Email);
            m.PhoneNumber = _enc.Encrypt(m.PhoneNumber);

            await using var context = await _factory.CreateDbContextAsync();
            context.Members.Update(m);
            await context.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var member = await context.Members.FindAsync(id);
            if (member != null)
            {
                member.IsActive = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Member?> GetByBarcodeAsync(string barcodeId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Members
                .FirstOrDefaultAsync(m => m.BarcodeId == barcodeId && m.IsActive);
        }

        public async Task ReactivateAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var member = await context.Members.FindAsync(id);
            if (member != null)
            {
                member.IsActive = true;
                await context.SaveChangesAsync();
            }
        }
    }
}