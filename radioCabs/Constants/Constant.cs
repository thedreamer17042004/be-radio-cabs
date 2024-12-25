namespace radioCabs.Constants
{
    public class Constant
    {
        //TRẠNG thái hoạt động 1 là hoạt động, 0 là không hoạt động

        // Payment status
        public static string PENDING = "PENDING";
        public static string DONE = "DONE";
        public static string FAIL = "FAIL";

        //membershipType 
        public static string PREMIUM = "Premium";
        public static string BASIC = "Basic";
        public static string FREE = "Free";

        // feedback type: Khiếu nại, Góp ý, Khen ngợi
        public static string COMPLAINT = "Complaint";
        public static string SUGGESTION = "Suggestion";
        public static string COMPLIMENT = "Compliment";

        //Payment plan type:  đăng ký , tài xế , quảng cáo 
        public static string DRIVER = "Driver";
        public static string ADVERTISE = "Advertise";
        public static string COMPANY = "Company";

        //payment type month, quater
        public static string MONTH = "Month";
        public static string QUARTER = "Quarter";
    }
}
