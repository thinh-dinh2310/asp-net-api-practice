using Utils;
using Microsoft.AspNetCore.Mvc;
namespace Utils.Models
{
    public class AuthenticationResponse
    {
        public static ActionResult Failed() =>
            new StatusCodeResult(CommonEnums.STATUS_CODE.AUTH_FAILED);

        public static ActionResult Success(string RefreshToken, string AccessToken) =>
            new OkObjectResult(new { RefreshToken, AccessToken });
    }
}
