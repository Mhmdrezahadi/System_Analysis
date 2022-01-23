using Chat.Web.ViewModels;
using Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System_Analysis.DTO;
using System_Analysis.Models;
using System_Analysis.Services;

namespace System_Analysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IGlobalService _globalService;

        public MemberController(IGlobalService globalService)
        {
            _globalService = globalService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResult>> Login(LoginInfo login)
        {
            LoginResult res = await _globalService.Login(login);

            return Ok(res);
        }

        [HttpGet("otp")]
        public async Task<ActionResult<OtpResponseDTO>> GetOtp([Required][FromQuery] string mobileNumber)
        {

            OtpResponseDTO dto = await _globalService.GetOtp(mobileNumber);

            return Ok(dto);
        }

        [HttpGet("find/{username}")]
        public async Task<ActionResult> FindMember(string username)
        {
            UserViewModel user = await _globalService.FindMember(username);

            return Ok(user);
        }
    }
}
