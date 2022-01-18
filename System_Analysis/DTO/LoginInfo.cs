using System.ComponentModel.DataAnnotations;

namespace System_Analysis.DTO
{
    public class LoginInfo
    {
        [StringLength(12)]
        public string MobileNumber { get; set; }
        [StringLength(4)]
        public string VerificationCode { get; set; }
    }
}
