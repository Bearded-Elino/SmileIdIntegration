namespace SmileIdVerify.Models.PhoneVerification
{
    public class PhoneVerificationRequest
    {
        public string CallbackUrl { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string IdNumber { get; set; }
    }
}
