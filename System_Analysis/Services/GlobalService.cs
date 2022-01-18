using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System_Analysis.DTO;

namespace System_Analysis.Services
{
    public class GlobalService : IGlobalService
    {

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public GlobalService(SignInManager<User> signInManager, UserManager<User> userManager, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<OtpResponseDTO> GetOtp(string mobileNumber)
        {

            if (mobileNumber.Length != 11)
            {
                return new OtpResponseDTO
                {
                    Success = false,
                    Message = "شماره تلفن باید 11 رقم و با 0 شروع شود.",
                };
            }

            var random = new Random();
            var verificationCode = "1111"; // random.Next(1111, 9999).ToString();

            //var smsApi = new KavenegarApi(_siteSettings.SmsApiKey);
            //var result = smsApi.Send("", mobileNumber, $"کد تایید تل بال: {verificationCode}");

            //SendResult result;
            //if (!_env.IsDevelopment())
            //{
            //    //todo //surround with try catch
            //    try
            //    {
            //        result = smsApi.VerifyLookup(mobileNumber, verificationCode, "tellbalOtp");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, ex.Message);

            //        return new OtpResponseDTO
            //        {
            //            Success = false,
            //            Message = "خطا در ارسال پیامک"
            //        };
            //    }
            //}
            //else
            //{
            //    verificationCode = "1111";
            //    result = new SendResult
            //    {
            //        Cost = 250,
            //        Date = 22255151,
            //        GregorianDate = DateTime.Now,
            //        Message = "کد : 1111",
            //        Messageid = 1,
            //        Receptor = mobileNumber,
            //        Sender = "technical team",
            //        Status = 1,
            //        StatusText = "ارسال به مخابرات",
            //    };
            //}

            //if (result == null) // i think this condition has bug(result will not be null if message not send)
            //{
            //    return new OtpResponseDTO
            //    {
            //        Success = false,
            //        Message = "خطا در ارسال پیامک"
            //    };
            //}

            var userFromDb = await _userManager
                .FindByNameAsync(mobileNumber);

            _memoryCache.Set(
                mobileNumber,
                verificationCode,
                DateTimeOffset.Now.AddMinutes(2));

            if (userFromDb != null)
            {
                await _userManager.RemovePasswordAsync(userFromDb);

                await _userManager.AddPasswordAsync(userFromDb, verificationCode);
            }
            else
            {
                var registerResult = await _userManager.CreateAsync(new User
                {
                    UserName = mobileNumber,
                    PhoneNumber = mobileNumber,
                    FirstName = "",
                    LastName = "",
                    SnapShot = "",/// default pic path
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                }, verificationCode);
            }

            return new OtpResponseDTO
            {
                Success = true,
                Message = "پیامک ارسال شد"
            };
        }

        public async Task<LoginResult> Login(LoginInfo dto)
        {

            var result = new LoginResult
            {
                Message = "خطا در ورود"
            };

            _memoryCache.TryGetValue(dto.MobileNumber, out string password);

            if (password != dto.VerificationCode)
            {
                return result;
            }

            var auth = await _signInManager.PasswordSignInAsync(
                dto.MobileNumber,
                dto.VerificationCode,
                false,
                false).ConfigureAwait(false);


            if (auth.Succeeded)
            {
                var userFromDB = await _userManager
                    .FindByNameAsync(dto.MobileNumber).ConfigureAwait(false);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,
                    userFromDB.Id.ToString()),
                    new Claim(ClaimTypes.Name,
                    userFromDB.UserName),
                };

                var roles = await _userManager.GetRolesAsync(userFromDB).ConfigureAwait(false);

                claims.AddRange(
                    roles.ToList()
                    .Select(role =>
                    new Claim(
                        ClaimsIdentity.DefaultRoleClaimType,
                        role)));

                var access_token = JwtTokenCreator(claims, DateTime.Now.AddYears(2));
                var refresh_tokne = JwtTokenCreator(claims, DateTime.Now.AddYears(3));

                result = new LoginResult
                {
                    IsAuthenticated = true,
                    Roles = roles.ToList(),
                    Message = "ورود موفق",
                    Access_Token = access_token,
                    Refresh_Token = refresh_tokne,
                    FirstName = userFromDB.FirstName,
                    LastName = userFromDB.LastName,
                    UserId = userFromDB.Id,
                    SnapShot = userFromDB.SnapShot,
                };
                // kill password
                await _userManager.RemovePasswordAsync(userFromDB);
            }
            return result;
        }
        public string JwtTokenCreator(List<Claim> claims, DateTime? time)
        {
            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: time,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
