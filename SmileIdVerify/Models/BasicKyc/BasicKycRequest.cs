using SmileIdVerify.Models;


namespace SmileIdVerify.Models.BasicKyc
{
    public class BasicKycRequest
    {
        public string callback_url { get; set; }
        public string country { get; set; }
        public string dob { get; set; }
        public string first_name { get; set; }
        public string gender { get; set; }
        public string id_number { get; set; }
        public string id_type { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string partner_id { get; set; }
        public PartnerParams partner_params { get; set; }
        public string phone_number { get; set; }
        public string signature { get; set; }
        public string source_sdk_version { get; set; }
        public string source_sdk { get; set; }
        public string timestamp { get; set; }
    }


    public class PartnerParams
    {
        public string job_id { get; set; }
        public string user_id { get; set; }
    }
}
