using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UTSMedicalSystem.FrontEnd.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        public string Notes { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public virtual User Patient { get; set; }
    }
}
