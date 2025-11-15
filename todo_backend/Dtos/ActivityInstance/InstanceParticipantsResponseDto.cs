namespace todo_backend.Dtos.ActivityInstance
{
    public class InstanceParticipantsResponseDto
    {
        public int ActivityId { get; set; }
        public int InstanceId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public List<InstanceParticipantDto> Participants { get; set; } = new();
    }
}
