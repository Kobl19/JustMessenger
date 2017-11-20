using AutoMapper;
using BusinessLogic.DTO;
using BusinessLogic.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            ViewBag.Message = "Ваш id: " + id.ToString();
            IEnumerable<UserDTO> userDto=UserService.usersDTO();
            string Name = userDto.FirstOrDefault(x=>x.Id==id).FirstName;
            ViewBag.Name = Name;
            
            return View();
        }
    }
}