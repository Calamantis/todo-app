//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace todo_backend.Models
//{
//    public class TimelineRecurrenceException
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int ExceptionId { get; set; }

//        [ForeignKey(nameof(Activity))]
//        public int ActivityId { get; set; }

//        /// <summary>
//        /// Data wyjątku (dzień, którego dotyczy)
//        /// </summary>
//        [Required]
//        public DateTime ExceptionDate { get; set; }

//        /// <summary>
//        /// Nowa godzina rozpoczęcia (opcjonalna, jeśli zmieniono tylko czas trwania)
//        /// </summary>
//        public TimeSpan? NewStartTime { get; set; }

//        /// <summary>
//        /// Nowy czas trwania w minutach (opcjonalny)
//        /// </summary>
//        public int? NewDurationMinutes { get; set; }

//        public RecurrenceExceptionMode Mode { get; set; } = RecurrenceExceptionMode.ReplaceExisting;

//        // 🔗 Powiązanie z główną aktywnością
//        public Activity? Activity { get; set; }
//    }

//    public enum RecurrenceExceptionMode
//    {
//        ReplaceExisting = 0, // zastąp istniejące wystąpienie
//        SkipExisting = 1,    // pomiń całkowicie
//        AddAdditional = 2    // dodaj nowe (dodatkowe)
//    }

//}
