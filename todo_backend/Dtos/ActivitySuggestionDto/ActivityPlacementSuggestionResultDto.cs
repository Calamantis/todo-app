namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ActivityPlacementSuggestionResultDto
    {
        public DayOfWeek Day { get; set; }
        public DateTime StartProposal { get; set; }
        public DateTime EndProposal { get; set; }
        public int GapMinutes { get; set; }
        public double Score { get; set; }
        public List<ShiftProposalDto>? ShiftSuggestions { get; set; }
    }
}
