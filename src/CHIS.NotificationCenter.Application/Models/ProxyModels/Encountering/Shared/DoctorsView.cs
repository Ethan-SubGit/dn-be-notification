using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class DoctorsView
    {
        public List<DoctorView> Doctors { get; set; } = new List<DoctorView>();

        public DoctorsView()
        {

        }

        public DoctorsView(List<DoctorView> doctors)
        {
            Doctors = doctors;
        }
    }
}
