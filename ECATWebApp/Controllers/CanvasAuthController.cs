using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecat.Business.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecat.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class CanvasAuthController : Controller
    {
        private readonly ILmsAdminTokenRepo tokenRepo;

        public CanvasAuthController(ILmsAdminTokenRepo lmsTokenRepo, IHttpContextAccessor accessor)
        {
            tokenRepo = lmsTokenRepo;
        }

        [HttpGet]
        public async Task<ActionResult> CanvasAuth(string code)
        {
            var token = await tokenRepo.GetRefreshToken(code);
            if (token)
            {
                ViewBag.Success = true;
            }
            else
            {
                ViewBag.Success = false;
            }

            return View();
        }
    }
}
