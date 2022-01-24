using Chat.Web.ViewModels;
using Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

        [HttpGet("findbyusername/{username}")]
        public async Task<ActionResult<UserViewModel>> FindMemberByUsername(string username)
        {
            UserViewModel user = await _globalService.FindMember(username);

            return Ok(user);
        }
        [HttpGet("findbymobilenumber/{mobileNumber}")]
        public async Task<ActionResult<UserViewModel>> FindMemberByMobile(string mobileNumber)
        {
            UserViewModel user = await _globalService.FindMemberByMobile(mobileNumber);

            return Ok(user);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserViewModel>>> SearchMembers([FromQuery] string username)
        {
            List<UserViewModel> users = await _globalService.SearchMembers(username);

            return Ok(users);
        }

        [HttpPut("edit")]
        public async Task<ActionResult> EditProfile(UserDTO userDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            bool res = await _globalService.EditProfile(userDto, Guid.Parse(userId));

            return Ok(res);
        }


    }
}
