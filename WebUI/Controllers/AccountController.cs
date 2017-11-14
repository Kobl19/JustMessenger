using BusinessLogic.DTO;
using BusinessLogic.Infastracture;
using BusinessLogic.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;
using System.Net.Mail;
using AutoMapper;
using Microsoft.AspNet.Identity;


namespace WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
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

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO { Email = model.Email, Password = model.Password };
                ClaimsIdentity claim = await UserService.Authenticate(userDto);
                if (claim == null)
                {
                    TempData["Message"] = "WrongPassword";
                    return View(model);
                }
                else
                {
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "user"
                };
                OperationDetails operationDetails = await UserService.Create(userDto);
                if (operationDetails.Succedeed)
                {
                    
                    
                    TempData["Message"] = "Succedeed";
                    return RedirectToAction("Login");
                }
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }

        private async Task SetInitialDataAsync()
        {
            await UserService.SetInitialData(new UserDTO
            {

                Email = "deniskobl@yahoo.com",
                FirstName = "Denis",
                Password = "12345",
                LastName = "Kobl",
                Role = "admin"
            }, new List<string> { "user", "admin" });
        }
        
        public ActionResult Edit()
        {
            var Id = User.Identity.GetUserId();
            IEnumerable<UserDTO> usersDtos = UserService.usersDTO();
            Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, EditViewModel>());
            var users=Mapper.Map<IEnumerable<UserDTO>, List<EditViewModel>>(usersDtos);
            var user = users.Where(x => x.Id == Id).FirstOrDefault();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditViewModel model, HttpPostedFileBase upload)
        {
            var Id = User.Identity.GetUserId();
            
            if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    // получаем имя файла
                    int idx = upload.FileName.LastIndexOf('.');
                    string ext = upload.FileName.Substring(idx, upload.FileName.Length - idx);
                    // сохраняем файл в папку Files в проекте
                    upload.SaveAs(Server.MapPath("~/Uploads/" + Id+ext));
                }

                UserDTO userDto = new UserDTO
                {
                    Email = model.Email,
                    Status = model.Status,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Sex = model.Sex,
                    InternalUrl= $"~/Uploads/{Id}.jpg"

                };
                OperationDetails operationDetails =await UserService.Edit(userDto, Id);
                if (operationDetails.Succedeed)
                {
                    TempData["Edit"] = "Succedeed";
                    return RedirectToAction("Index", "Home");
                }
                else
                ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }
        public ActionResult Image()
        {
            var Id = User.Identity.GetUserId();
            IEnumerable<UserDTO> usersDtos = UserService.usersDTO();
            var user = usersDtos.Where(x => x.Id == Id).FirstOrDefault();
            if (user == null) throw new HttpException(404, "Not Found");

            return File(user.InternalUrl, Server.MapPath($"~/Uploads/{Id}.jpg"));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(EditViewModel model)
        {
            var Id = User.Identity.GetUserId();
            IEnumerable<UserDTO> usersDtos = UserService.usersDTO();
            

            if (ModelState.IsValid)
            {

                UserDTO userDto = usersDtos.Where(u => u.Id == Id).FirstOrDefault();
                if (userDto != null)
                {
                    
                    UserDTO user1 = new UserDTO()
                    {
                        NewPassword = model.NewPassword,
                        OldPassword = model.OldPassword
                    };
                    

                    OperationDetails operationDetails = await UserService.ChangePassword(user1, Id);
                    if (operationDetails.Succedeed)
                    {

                        TempData["Change"] = "Succedeed";
                        return RedirectToAction("Edit");
                    }
                    else
                    {
                        TempData["Change"] = "False";
                        ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
                    }

                }

            }
            
                Mapper.Initialize(cfg => cfg.CreateMap<UserDTO, EditViewModel>());
                var users = Mapper.Map<IEnumerable<UserDTO>, List<EditViewModel>>(usersDtos);
                var user = users.Where(u => u.Id == Id).FirstOrDefault();
           
            return View("Edit", user);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Upload(HttpPostedFileBase upload)
        //{
        //    if (upload != null)
        //    {
        //        // получаем имя файла
        //        string fileName = System.IO.Path.GetFileName(upload.FileName);
        //        // сохраняем файл в папку Files в проекте
        //        upload.SaveAs(Server.MapPath("~/Avatars/" + fileName));
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}