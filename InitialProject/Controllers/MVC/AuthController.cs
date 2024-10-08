﻿using Kawkaba.BusinessLayer.Interfaces;
using Kawkaba.Core.DTO.AuthViewModel;
using Kawkaba.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kawkaba.Controllers.MVC
{
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthController(IAccountService accountService,UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            return View();
        }

        [HttpGet]
        public IActionResult Login(string ReturnUrl = null)
        {
            ViewData["Title"] = "Login";
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (isSuccess, token, errorMessage) = await _accountService.Login(model);

            if (isSuccess)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(1)
                };
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, errorMessage);
            return View(model);
        }

        //[HttpGet]
        //public async Task<IActionResult> Profile()
        //{
        //    var token = HttpContext.Request.Cookies["AuthToken"];
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var userId = _accountService.ValidateJwtToken(token);
        //    var user = await _accountService.GetUserById(userId);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var model = new UserProfileModel
        //    {
        //        FullName = user.FullName,
        //        Email = user.Email,
        //        ProfileImage = await _accountService.GetUserProfileImage(user.ProfileId),
        //        RegistrationDate = user.RegistrationDate,
        //    };

        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var isLoggedOut = await _accountService.Logout(user);
                if (isLoggedOut)
                {
                    return RedirectToAction("Login");
                }
            }

            return View();
        }
    }
}
