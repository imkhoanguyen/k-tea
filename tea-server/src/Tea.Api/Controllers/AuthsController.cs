using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tea.Application.DTOs.Users;
using Tea.Domain.Entities;
using Tea.Domain.Exceptions.BadRequests;
using Tea.Domain.Exceptions.NotFounds;
using Tea.Domain.Exceptions.Unauthorizes;
using Tea.Domain.Repositories;
using Tea.Infrastructure.Configurations;
using Tea.Infrastructure.Interfaces;

namespace Tea.Api.Controllers
{
    public class AuthsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly EmailConfig _emailConfig;
        private readonly ILogger<AuthsController> _logger;
        private readonly IUnitOfWork _unit;
        public AuthsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, IEmailService emailService, IOptions<EmailConfig> emailConfig, ILogger<AuthsController> logger
            , IUnitOfWork unit)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _emailConfig = emailConfig.Value;
            _logger = logger;
            _unit = unit;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName) ??
                await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                _logger.LogWarning("user not found");
                throw new UserNotFoundException($"{request.UserName}");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Wrong password");
                throw new WrongPasswordException();
            }

            (string accessToken, DateTime expiredDateAccessToken) = await _tokenService.CreateAccessTokenAsync(user);
            string refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            return Ok(new UserLoginResponse
            {
                UserName = user.UserName,
                FullName = user.FullName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiredDateAccessToken = expiredDateAccessToken,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await CheckEmailExistAsync(request.Email))
            {
                _logger.LogWarning("Email exists");
                throw new EmailExistsException();
            }

            if (await CheckUserNameExistAsync(request.UserName))
            {
                _logger.LogWarning("Username exists");
                throw new UsernameExistsException();
            }

            var userToAdd = new AppUser
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Email = request.Email,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
            };

            await _unit.BeginTransactionAsync();

            try
            {
                var result = await _userManager.CreateAsync(userToAdd, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Create user failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    throw new AddNewUserFailedException("Đăng ký thất bại vui lòng thử lại sau");
                }

                var resultAddRole = await _userManager.AddToRoleAsync(userToAdd, "Customer");
                if (!resultAddRole.Succeeded)
                {
                    _logger.LogError("Add role to user failed");
                    throw new AddRoleToUserFailedException("Đăng ký thất bại vui lòng thử lại sau");
                }

                await _unit.CommitTransactionAsync();

                return Ok();
            }
            catch
            {
                await _unit.RollbackTransactionAsync(); throw;
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserLoginResponse>> RefreshToken(string rfToken)
        {
            if (rfToken.IsNullOrEmpty())
            {
                _logger.LogWarning("Could not get refresh token");
                throw new InvalidRefreshTokenException("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại");
            }

            return Ok(await _tokenService.ValidRefreshToken(rfToken));
        }

        [HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPassword(CancellationToken cancellationToken,[FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"user with name/username: {email} not exists");
                throw new UserNotFoundException(email);
            }

            string host = _emailConfig.AppUrl;

            string tokenConfirm = await _userManager.GeneratePasswordResetTokenAsync(user);

            string decodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenConfirm));

            string resetPasswordUrl = $"{host}/dat-lai-mat-khau?email={email}&token={decodedToken}";

            string html = $@"<html xmlns=""http://www.w3.org/1999/xhtml"">
  <head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <meta name=""x-apple-disable-message-reformatting"" />
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <meta name=""color-scheme"" content=""light dark"" />
    <meta name=""supported-color-schemes"" content=""light dark"" />
    <title></title>
    <style type=""text/css"" rel=""stylesheet"" media=""all"">
    /* Base ------------------------------ */
    
    @import url(""https://fonts.googleapis.com/css?family=Nunito+Sans:400,700&display=swap"");
    body {{
      width: 100% !important;
      height: 100%;
      margin: 0;
      -webkit-text-size-adjust: none;
    }}
    
    a {{
      color: #3869D4;
    }}
    
    a img {{
      border: none;
    }}
    
    td {{
      word-break: break-word;
    }}
    
    .preheader {{
      display: none !important;
      visibility: hidden;
      mso-hide: all;
      font-size: 1px;
      line-height: 1px;
      max-height: 0;
      max-width: 0;
      opacity: 0;
      overflow: hidden;
    }}
    /* Type ------------------------------ */
    
    body,
    td,
    th {{
      font-family: ""Nunito Sans"", Helvetica, Arial, sans-serif;
    }}
    
    h1 {{
      margin-top: 0;
      color: #333333;
      font-size: 22px;
      font-weight: bold;
      text-align: left;
    }}
    
    h2 {{
      margin-top: 0;
      color: #333333;
      font-size: 16px;
      font-weight: bold;
      text-align: left;
    }}
    
    h3 {{
      margin-top: 0;
      color: #333333;
      font-size: 14px;
      font-weight: bold;
      text-align: left;
    }}
    
    td,
    th {{
      font-size: 16px;
    }}
    
    p,
    ul,
    ol,
    blockquote {{
      margin: .4em 0 1.1875em;
      font-size: 16px;
      line-height: 1.625;
    }}
    
    p.sub {{
      font-size: 13px;
    }}
    /* Utilities ------------------------------ */
    
    .align-right {{
      text-align: right;
    }}
    
    .align-left {{
      text-align: left;
    }}
    
    .align-center {{
      text-align: center;
    }}
    
    .u-margin-bottom-none {{
      margin-bottom: 0;
    }}
    /* Buttons ------------------------------ */
    
    .button {{
      background-color: #3869D4;
      border-top: 10px solid #3869D4;
      border-right: 18px solid #3869D4;
      border-bottom: 10px solid #3869D4;
      border-left: 18px solid #3869D4;
      display: inline-block;
      color: #FFF;
      text-decoration: none;
      border-radius: 3px;
      box-shadow: 0 2px 3px rgba(0, 0, 0, 0.16);
      -webkit-text-size-adjust: none;
      box-sizing: border-box;
    }}
    
    .button--green {{
      background-color: #22BC66;
      border-top: 10px solid #22BC66;
      border-right: 18px solid #22BC66;
      border-bottom: 10px solid #22BC66;
      border-left: 18px solid #22BC66;
    }}
    
    .button--red {{
      background-color: #FF6136;
      border-top: 10px solid #FF6136;
      border-right: 18px solid #FF6136;
      border-bottom: 10px solid #FF6136;
      border-left: 18px solid #FF6136;
    }}
    
    @media only screen and (max-width: 500px) {{
      .button {{
        width: 100% !important;
        text-align: center !important;
      }}
    }}
    /* Attribute list ------------------------------ */
    
    .attributes {{
      margin: 0 0 21px;
    }}
    
    .attributes_content {{
      background-color: #F4F4F7;
      padding: 16px;
    }}
    
    .attributes_item {{
      padding: 0;
    }}
    /* Related Items ------------------------------ */
    
    .related {{
      width: 100%;
      margin: 0;
      padding: 25px 0 0 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
    }}
    
    .related_item {{
      padding: 10px 0;
      color: #CBCCCF;
      font-size: 15px;
      line-height: 18px;
    }}
    
    .related_item-title {{
      display: block;
      margin: .5em 0 0;
    }}
    
    .related_item-thumb {{
      display: block;
      padding-bottom: 10px;
    }}
    
    .related_heading {{
      border-top: 1px solid #CBCCCF;
      text-align: center;
      padding: 25px 0 10px;
    }}
    /* Discount Code ------------------------------ */
    
    .discount {{
      width: 100%;
      margin: 0;
      padding: 24px;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
      background-color: #F4F4F7;
      border: 2px dashed #CBCCCF;
    }}
    
    .discount_heading {{
      text-align: center;
    }}
    
    .discount_body {{
      text-align: center;
      font-size: 15px;
    }}
    /* Social Icons ------------------------------ */
    
    .social {{
      width: auto;
    }}
    
    .social td {{
      padding: 0;
      width: auto;
    }}
    
    .social_icon {{
      height: 20px;
      margin: 0 8px 10px 8px;
      padding: 0;
    }}
    /* Data table ------------------------------ */
    
    .purchase {{
      width: 100%;
      margin: 0;
      padding: 35px 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
    }}
    
    .purchase_content {{
      width: 100%;
      margin: 0;
      padding: 25px 0 0 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
    }}
    
    .purchase_item {{
      padding: 10px 0;
      color: #51545E;
      font-size: 15px;
      line-height: 18px;
    }}
    
    .purchase_heading {{
      padding-bottom: 8px;
      border-bottom: 1px solid #EAEAEC;
    }}
    
    .purchase_heading p {{
      margin: 0;
      color: #85878E;
      font-size: 12px;
    }}
    
    .purchase_footer {{
      padding-top: 15px;
      border-top: 1px solid #EAEAEC;
    }}
    
    .purchase_total {{
      margin: 0;
      text-align: right;
      font-weight: bold;
      color: #333333;
    }}
    
    .purchase_total--label {{
      padding: 0 15px 0 0;
    }}
    
    body {{
      background-color: #F2F4F6;
      color: #51545E;
    }}
    
    p {{
      color: #51545E;
    }}
    
    .email-wrapper {{
      width: 100%;
      margin: 0;
      padding: 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
      background-color: #F2F4F6;
    }}
    
    .email-content {{
      width: 100%;
      margin: 0;
      padding: 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
    }}
    /* Masthead ----------------------- */
    
    .email-masthead {{
      padding: 25px 0;
      text-align: center;
    }}
    
    .email-masthead_logo {{
      width: 94px;
    }}
    
    .email-masthead_name {{
      font-size: 16px;
      font-weight: bold;
      color: #A8AAAF;
      text-decoration: none;
      text-shadow: 0 1px 0 white;
    }}
    /* Body ------------------------------ */
    
    .email-body {{
      width: 100%;
      margin: 0;
      padding: 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
    }}
    
    .email-body_inner {{
      width: 570px;
      margin: 0 auto;
      padding: 0;
      -premailer-width: 570px;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
      background-color: #FFFFFF;
    }}
    
    .email-footer {{
      width: 570px;
      margin: 0 auto;
      padding: 0;
      -premailer-width: 570px;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
      text-align: center;
    }}
    
    .email-footer p {{
      color: #A8AAAF;
    }}
    
    .body-action {{
      width: 100%;
      margin: 30px auto;
      padding: 0;
      -premailer-width: 100%;
      -premailer-cellpadding: 0;
      -premailer-cellspacing: 0;
      text-align: center;
    }}
    
    .body-sub {{
      margin-top: 25px;
      padding-top: 25px;
      border-top: 1px solid #EAEAEC;
    }}
    
    .content-cell {{
      padding: 45px;
    }}
    /*Media Queries ------------------------------ */
    
    @media only screen and (max-width: 600px) {{
      .email-body_inner,
      .email-footer {{
        width: 100% !important;
      }}
    }}
    
    @media (prefers-color-scheme: dark) {{
      body,
      .email-body,
      .email-body_inner,
      .email-content,
      .email-wrapper,
      .email-masthead,
      .email-footer {{
        background-color: #333333 !important;
        color: #FFF !important;
      }}
      p,
      ul,
      ol,
      blockquote,
      h1,
      h2,
      h3,
      span,
      .purchase_item {{
        color: #FFF !important;
      }}
      .attributes_content,
      .discount {{
        background-color: #222 !important;
      }}
      .email-masthead_name {{
        text-shadow: none !important;
      }}
    }}
    
    :root {{
      color-scheme: light dark;
      supported-color-schemes: light dark;
    }}
    </style>
    <!--[if mso]>
    <style type=""text/css"">
      .f-fallback  {{
        font-family: Arial, sans-serif;
      }}
    </style>
  <![endif]-->
  </head>
  <body>
    <span class=""preheader"">Use this link to reset your password. The link is only valid for 24 hours.</span>
    <table class=""email-wrapper"" width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
      <tr>
        <td align=""center"">
          <table class=""email-content"" width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
            <tr>
              <td class=""email-masthead"">
                <a href=""https://github.com/imkhoanguyen/k-tea"" class=""f-fallback email-masthead_name"">
                K TEA
              </a>
              </td>
            </tr>
            <!-- Email Body -->
            <tr>
              <td class=""email-body"" width=""570"" cellpadding=""0"" cellspacing=""0"">
                <table class=""email-body_inner"" align=""center"" width=""570"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                  <!-- Body content -->
                  <tr>
                    <td class=""content-cell"">
                      <div class=""f-fallback"">
                        <h1>Hi {user.FullName},</h1>
                        <p>You recently requested to reset your password for your K TEA account. Use the button below to reset it. <strong>This password reset is only valid for the next 24 hours.</strong></p>
                        <!-- Action -->
                        <table class=""body-action"" align=""center"" width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                          <tr>
                            <td align=""center"">
                              <!-- Border based button
           https://litmus.com/blog/a-guide-to-bulletproof-buttons-in-email-design -->
                              <table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"" role=""presentation"">
                                <tr>
                                  <td align=""center"">
                                    <a href=""{resetPasswordUrl}"" class=""f-fallback button button--green"" target=""_blank"">Reset your password</a>
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                        <p>Thanks,
                          <br>The K TEA team</p>
                      </div>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr>
              <td>
                <table class=""email-footer"" align=""center"" width=""570"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                  <tr>
                    <td class=""content-cell"" align=""center"">
                      <p class=""f-fallback sub align-center"">
                        [K TEA, LLC]
                        <br>Sai Gon University.
                        <br>Viet Nam
                      </p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";

            await _emailService.SendMailAsync(cancellationToken, new SendMailRequest
            {
                To = user.Email,
                Subject = "Reset Your Password ",
                Content = html,
            });

            return Ok(new { message = "Vui lòng kiểm tra email" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning($"user with name/username: {request.Email} not exists");
                throw new UserNotFoundException(request.Email);
            }

            if (string.IsNullOrEmpty(request.Token))
            {
                _logger.LogWarning("empty token");
                throw new InvalidTokenException("Thông tin người dùng không hợp lệ. Vui lòng thử lại sau");
            }

            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var identityResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);

            if (identityResult.Succeeded)
            {
                return Ok(new { message = "Reset password thành công" });
            }
            else
            {
                _logger.LogError("Create user failed: {Errors}", string.Join(", ", identityResult.Errors.Select(e => e.Description)));
                throw new ResetPasswordFailedException("Reset Password thất bại. Vui lòng thử lại sau");
            }
        }


        #region
        private async Task<bool> CheckEmailExistAsync(string text)
        {
            return await _userManager.Users.AnyAsync(u => u.Email.ToLower() == text.ToLower());
        }

        private async Task<bool> CheckUserNameExistAsync(string text)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName.ToLower() == text.ToLower());
        }

        #endregion
    }
}

