using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class PatientRdo
    {
        public string PatientId { get; set; }
        public string DisplayId { get; set; }
        public string FullName { get; set; }
        public BusinessItemRdo Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string MotherPatientId { get; set; }
        public string DemographicId { get; set; }
        public PatientAgeRdo Age { get; set; }

        public PatientRdo() { }
        public PatientRdo(string patientId, string displayId, string fullName, BusinessItemRdo gender, DateTime birthDate, string motherPatientId,
            string demographicId, PatientAgeRdo age)
        {
            PatientId = patientId;
            DisplayId = displayId;
            FullName = fullName;
            Gender = gender;
            BirthDate = birthDate;
            MotherPatientId = motherPatientId;
            DemographicId = demographicId;
            Age = age;
        }

        public static Tuple<int, int, int> ProcGetAge(DateTime birthDay)
        {
            int calAge = 0, calMonths = 0;
            int calDays = 0;
            calAge = GetCalAge(birthDay);
            calMonths = GetCalMonths(birthDay);
            calDays = GetCalDays(birthDay);

            return new Tuple<int, int, int>(calAge, calMonths, calDays);
        }

        private static int GetCalAge(DateTime birthDay)
        {
            string _birth = birthDay.ToString("yyyy-MM-dd");
            int calAge = 0;
            int birthYear = Convert.ToInt32(_birth.Substring(2, 2));
            int nowYear = DateTime.Now.Year;

            if (_birth.Substring(0, 2) == "19")
            {
                calAge = (nowYear - (1900 + birthYear));
            }
            else
            {
                calAge = (nowYear - (2000 + birthYear));
            }

            int birthMonth = Convert.ToInt32(_birth.Substring(5, 2));
            int nowMonth = DateTime.Now.Month;

            if (birthMonth == nowMonth)
            {
                int birthDays = Convert.ToInt32(_birth.Substring(8, 2));
                int nowDay = DateTime.Now.Day;

                if (birthDays > nowDay)
                {
                    calAge = calAge - 1;
                }
            }
            else if (birthMonth > nowMonth)
            {
                calAge = calAge - 1;
            }

            return calAge;
        }

        private static int GetCalMonths(DateTime birthDay)
        {
            int calMonths = 0;

            string _birth = birthDay.ToString("yyyy-MM-dd");
            int birthYear = Convert.ToInt32(_birth.Substring(0, 4));
            int birthMonth = Convert.ToInt32(_birth.Substring(5, 2));
            int nowYear = DateTime.Now.Year;
            int nowMonth = DateTime.Now.Month;
            calMonths = (nowYear - birthYear) * 12 + (nowMonth - birthMonth);

            return calMonths;
        }

        private static int GetCalDays(DateTime birthDay)
        {
            TimeSpan diffDate = DateTime.Now - birthDay;
            int calDays = (int)diffDate.TotalDays + 1;

            return calDays;
        }
    }

    public class PatientAgeRdo
    {
        public int Age { get; set; }
        public int Months { get; set; }
        public int Days { get; set; }

        public PatientAgeRdo() { }
        public PatientAgeRdo(int age, int months, int days)
        {
            Age = age;
            Months = months;
            Days = days;
        }
    }
}
