using SmileIdVerify.Models;

namespace SmileIdVerify.Models.BasicKyc
{
    public class BasicKycResponse
    {
        public string SmileJobID { get; set; }
        public PartnerParams PartnerParams { get; set; }
        public string ResultText { get; set; }
        public string ResultCode { get; set; }
        public Dictionary<string, string> Actions { get; set; }
        public string Source { get; set; }
        public string signature { get; set; }
        public string timestamp { get; set; }
    }
}
