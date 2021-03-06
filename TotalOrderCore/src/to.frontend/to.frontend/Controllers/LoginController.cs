﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using to.contracts;
using to.contracts.data.domain;
using to.frontend.Constants;
using to.frontend.Factories;
using to.frontend.Models.Login;

namespace to.frontend.Controllers
{
    public class LoginController : Controller
    {
        private const string errorMessageString = "errorMessage";

        [Route("Login")]
        [HttpGet]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (TempData != null && !string.IsNullOrEmpty((string)TempData[errorMessageString]))
            {
                ModelState.AddModelError("", (string)TempData[errorMessageString]);
            }
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult PostLogin(UserLoginViewModel model, string returnUrl)
        {
            try
            {
                var redirectUrl = nameof(Login);
                var loginRequest = new LoginRequest
                {
                    Password = model.Password,
                    Username = model.UserName
                };

                var handlerFactory = new RequestHandlerFactory();
                var requestHandler = handlerFactory.GetHandler();

                requestHandler.HandleLoginQuery(loginRequest,
                    // onSuccess
                    async (user) =>
                    {
                        await CreateCookie(user);
                        redirectUrl = returnUrl ?? "/Home";
                    },
                    //onFailure
                    errorMessage => { TempData[errorMessageString] = errorMessage; });

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                TempData["Exception"] = ex;
                TempData["Model"] = model;
                return View("Error");
            }
        }

        private async Task CreateCookie(UserLoginQueryResult user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim(CustomClaims.Permission, permission.ToString()));
            }

            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(userIdentity),
                new AuthenticationProperties{ IsPersistent = true });
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<ActionResult> GetLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home");
        }

        [HttpGet]
        [Route("Register")]
        public ActionResult GetRegister()
        {
            return View("Login");
        }
    }
}
