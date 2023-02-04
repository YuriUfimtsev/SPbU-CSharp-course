namespace ConferenseRegistrationWebApp.Pages;

using ConferenseRegistrationWebApp.Data;

[BindProperties]
public class RegistrationModel : PageModel
{
    private readonly ConferenceRegistrationDbContext _context;
    public RegistrationModel(ConferenceRegistrationDbContext context)
    => _context = context;

    public Participant Participant { get; set; } = new();
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        _context.Participants.Add(Participant);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Thanks", Participant);
    }
}
