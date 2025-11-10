namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class DayFreeSummaryDto
    {
        
    public DateTime DateLocal { get; set; }        
    public int TotalFreeMinutes {  get; set; } 
    public DateTime? SuggestedStart {  get; set; }
    public DateTime? SuggestedEnd { get; set; }
    }
}
