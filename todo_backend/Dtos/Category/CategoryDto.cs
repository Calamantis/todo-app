﻿using System.ComponentModel.DataAnnotations;

namespace todo_backend.Dtos.Category
{
    public class CategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(7)]
        public string? ColorHex { get; set; }
    }
}
