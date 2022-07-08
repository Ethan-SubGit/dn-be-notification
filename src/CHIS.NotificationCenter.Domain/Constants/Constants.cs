namespace CHIS.NotificationCenter.Domain.Constants
{
    public static class WorkStatus
    {
        public const string COMPLETE = "0";
        public const string CHANGED = "1";
        public const string NEW = "2";
    }

    public static class UseStatus
    {
        public const string CURRENT = "0";
        public const string TEMPORARY = "1";
        public const string DISCONTINUE = "2";
    }

    public static class DepartmentType
    {
        public const string PATIENT_DEPARTMENT = "P";
        public const string EMPLOYTEE_DEPARTMENT = "E";
    }

    public static class EncounterClassType
    {
        public const string OUTPATIENT = "O";
        public const string INPATIENT = "I";
        public const string EMERGENCY = "E";
        public const string DISCHARGE = "D";
    }

    public static class DateType
    {
        public const string ADMISSION_DATE = "A";
        public const string DISCHARGE_DATE = "D";
        public const string OPERATION_DATE = "O";
    }

    public static class InspectionType
    {
        public const string DEFICIENCY = "01"; // 미비
        public const string DELINQUENCY = "02"; // 미완성
        public const string ELECTRONIC_SIGNATURE = "03"; // 서명누락
    }


    public static class CriteriaDate
    {
        public const string ADMISSION_DATE = "01"; // 입원일자
        public const string OPERATION_DATE = "02"; // 수술일자
        public const string DISCHARGE_DATE = "03"; // 퇴원일자
        public const string NEXT_DAY_AFTER_ADMISSION = "04"; // 입원 다음날
    }

    public static class CriteriaPeriod
    {
        public const string THE_DAY = "01"; // 당일
        public const string THE_NEXT_DAY = "02"; // 익일
        public const string EVERYDAY = "03"; // 매일
        public const string EVERY_OTHER_DAY = "04"; // 격일
    }

    public static class SignStatus
    {
        public const string NOT_SIGNED = "N";
        public const string SIGNED = "S";
        public const string UNNEEEDED = "U";
    }

    public static class ProgressType
    {
        public const string INITIAL_DEFICIENCY = "01"; // 초기미비
        public const string CHECK_DEFICIENCY = "02"; // 미비확인
        public const string CHECK_DOCTOR = "03"; // 의사확인
        public const string REDEFICIENCY = "04"; // 재미비
        public const string COMPLETE = "09"; // 완료
    }

    public static class EncounterStatus
    {
        public const string PLANNED = "01";
        public const string ARRIVED = "02";
        public const string TRIAGED = "03";
        public const string IN_PROGRESS = "04";
        public const string ON_LEAVE = "05";
        public const string FINISHED = "06";
        public const string CANCELLED = "07";
    }

    public static class RecipientType
    {
        public const string ATTENDING_PHYSICIAN = "01"; // 주치의
        public const string PROCEDURE_PHYSICIAN = "02"; // 시술의
        public const string OPERATING_PHYSICIAN = "03"; // 수술집도의
        public const string OPERATION_SUPPORT_PHYSICIAN = "04"; // 수술보조의
    }

    public static class ParticipantType
    {
        public const string OUTPATIENT_CARE_PROVIDER = "01";
        public const string EMERGENCY_CARE_PROVIDER = "02";
        public const string EMERGENCY_PRIMARY_CARE_PROVIDER = "03";
        public const string ADMISSION_CARE_PROVIDER = "04";
        public const string ADMISSION_PRIMARY_CARE_PROVIDER = "05";
        public const string ADMISSION_ASSIGNED_NURSE = "06";
    }
}
