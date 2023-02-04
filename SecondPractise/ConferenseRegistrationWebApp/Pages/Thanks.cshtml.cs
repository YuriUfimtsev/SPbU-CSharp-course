using ConferenseRegistrationWebApp.Data;

namespace ConferenceRegistrationWebApp.Pages;
public class ThanksModel : PageModel
{
    public Participant Participant { get; set; } = new();
    public void OnGet(Participant participant)
    {
        Participant = participant;
    }
}
