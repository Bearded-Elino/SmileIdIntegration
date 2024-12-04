using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileIdVerify.Models.BasicKyc;
using SmileIdVerify.Models.PhoneVerification;
using SmileIdVerify.Services;

namespace SmileIdVerify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KYCController : ControllerBase
    {
        private readonly SmileIdService _smileIdService;
        public KYCController(SmileIdService smileIdService)
        {
            _smileIdService = smileIdService;
        }

        [HttpPost]
        public async Task<IActionResult> VerifyIdentity([FromBody] BasicKycRequest requestData)
        {
            string endpoint = "https://testapi.smileidentity.com/v2/verify";

            try
            {
                var response = await _smileIdService.VerifyIdentityAsync(endpoint, requestData);
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpPost("verify-phone-number")]
        public async Task<IActionResult> VerifyPhoneNumber([FromBody] PhoneVerificationRequest phoneVerificationRequest)
        {
            try
            {
                var matchFields = new Dictionary<string, string>
        {
            { "first_name", phoneVerificationRequest.FirstName },
            { "last_name", phoneVerificationRequest.LastName },
            { "other_name", phoneVerificationRequest.OtherName },
            { "id_number", phoneVerificationRequest.IdNumber }
        };

                var response = await _smileIdService.VerifyPhoneNumberAsync(
                    phoneVerificationRequest.CallbackUrl,
                    phoneVerificationRequest.Country,
                    phoneVerificationRequest.PhoneNumber,
                    matchFields
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
