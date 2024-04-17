using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adatea.classe;

namespace Adatea.classe
{
    public class Appointment
    {
        public int ID_Rdv { get; set; }
        public int? ID_Client { get; set; }
        public int ID_Commercial { get; set; }
        public DateTime Date_Rdv { get; set; }
        public TimeSpan Time_Rdv { get; set; }
        public string Location { get; set; }
        public bool IsVisible { get; set; } = false;
        public string DateDisplay => Date_Rdv.ToString("d MMM yyyy");
        public string TimeDisplay => Time_Rdv.ToString(@"hh\:mm");
    }
}
