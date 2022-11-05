using System.ComponentModel.DataAnnotations;

namespace Project_API.Models.Entities
{
    public class Student
    {
        [Key]
        public String StudentId { get; set; }
        public string StudentName { get; set; }

        public string PhoneNumber { get; set; }

    }
}
