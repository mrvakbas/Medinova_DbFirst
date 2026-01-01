using System.ComponentModel.DataAnnotations;

namespace Medinova.DTOs
{
    public class HealthQuestion
    {
        [Required]
        public string Message { get; set; }
    }
}