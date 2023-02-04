using Microsoft.EntityFrameworkCore;

namespace ConferenseRegistrationWebApp.Data;
public class ConferenceRegistrationDbContext : DbContext
{
    public ConferenceRegistrationDbContext(
    DbContextOptions<ConferenceRegistrationDbContext> options)
    : base(options)
    {
    }
    public DbSet<Participant> Participants => Set<Participant>();
}
