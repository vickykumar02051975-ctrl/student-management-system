
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Roll Number is required")]
        [StringLength(20)]
        public string RollNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student Name is required")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Enter a valid phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course is required")]
        public string Course { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
