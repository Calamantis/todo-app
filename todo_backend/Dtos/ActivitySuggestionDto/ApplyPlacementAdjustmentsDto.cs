namespace todo_backend.Dtos.ActivitySuggestionDto
{
    public class ApplyPlacementAdjustmentsDto
    {
        /// <summary>Dzień, którego dotyczy sugestia (z pola "date").</summary>
        public DateTime Date { get; set; }

        /// <summary>Sugerowany start nowej aktywności (z DayOverlapActivitiesDto.SuggestedStart).</summary>
        public DateTime SuggestedStart { get; set; }

        /// <summary>Sugerowany koniec nowej aktywności (z DayOverlapActivitiesDto.SuggestedEnd).</summary>
        public DateTime SuggestedEnd { get; set; }

        /// <summary>Id aktywności, którą chcemy wstawić.</summary>
        public int ActivityId { get; set; }

        /// <summary>Ile minut skrócić poprzednią aktywność.</summary>
        public int ShortenPrevious { get; set; }

        /// <summary>Ile minut skrócić planowaną (tę nową) aktywność.</summary>
        public int ShortenCurrent { get; set; }

        /// <summary>Ile minut skrócić następną aktywność.</summary>
        public int ShortenNext { get; set; }

    }
}
