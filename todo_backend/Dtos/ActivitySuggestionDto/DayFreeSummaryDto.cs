namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class DayFreeSummaryDto
    {
        
    public DateTime DateLocal { get; set; }        
    public int TotalFreeMinutes {  get; set; } 
    public TimeSpan SuggestedStart {  get; set; }
    public TimeSpan SuggestedEnd { get; set; }
    }
}
