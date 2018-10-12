using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UTSMedicalSystem.FrontEnd.Models
{
    public class User
    {
        public int ID { get; set; }
        public string AspNetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string UTSID { get; set; }
        public string History { get; set; }
        public bool IsPatient { get; set; }
        public bool IsDoctor { get; set; }
        public bool IsReceptionist { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
