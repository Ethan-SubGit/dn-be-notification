
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder
//{
//    public class HospitalType : Enumeration
//    {
//        public static readonly HospitalType Hospital = new HospitalType(1, "Hospital");
//        public static readonly HospitalType Clinic = new HospitalType(2, "Clinic");

//        public HospitalType()
//        { }

//        public HospitalType(int id, string name) : base(id, name)
//        { }

//        public static IEnumerable<HospitalType> List() => new[] { Hospital, Clinic };

//        public static HospitalType FromName(string name)
//        {
//            var type = List().SingleOrDefault(c => string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
//            if (type == null)
//            {
//                throw new ArgumentException($"Possible values for {nameof(HospitalType)}: {string.Join(",", List().Select(c => c.Name))}");
//            }

//            return type;
//        }

//        public static HospitalType FromId(int id)
//        {
//            var type = List().SingleOrDefault(c => c.Id == id);
//            if (type == null)
//            {
//                throw new ArgumentException($"Possible values for {nameof(HospitalType)}: {string.Join(",", List().Select(s => s.Name))}");
//            }

//            return type;
//        }
//    }
//}
