namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ShiftProposalDto
    {
        public int ActivityId { get; set; }
        public int SuggestedShiftMinutes { get; set; } // ujemne = wcześniej, dodatnie = później
        public string Type { get; set; } = ""; // "startTime" lub "recurrenceRule"
    }
}
