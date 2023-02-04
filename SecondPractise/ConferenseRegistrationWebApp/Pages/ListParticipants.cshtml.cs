using ConferenseRegistrationWebApp.Data;

namespace ConferenceRegistrationWebApp.Pages;
public class ListParticipantsModel : PageModel
{
    private readonly ConferenceRegistrationDbContext context;
    public ListParticipantsModel(ConferenceRegistrationDbContext context)
    => this.context = context;
    public IList<Participant> Participants { get; private set; } = new List<Participant>();
    public void OnGet()
    {
        Participants = context.Participants.OrderBy(p => p.ParticipantId).ToList();
    }
}
