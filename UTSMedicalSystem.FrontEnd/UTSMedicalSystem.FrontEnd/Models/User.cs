using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UTSMedicalSystem.FrontEnd.Models
{
    public class User
    {


        public int ID { get; set; }

        public string AspNetUserId { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "This field must be 20 characters max.")]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "This field must be 20 characters max.")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^([0]?[0-9]|[12][0-9]|[3][01])[/]([0]?[1-9]|[1][0-2])[/]([0-9]{4}|[0-9]{2})$",
                           ErrorMessage = "Your DOB must be in this format: dd/mm/yyyy.")]
        public string DOB { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Your UTS ID must be at least 8 digits long.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Your UTS ID must be numeric.")]
        public string UTSID { get; set; }
        public string History { get; set; }
        public string Role { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
