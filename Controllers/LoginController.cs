using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DotNetCoreSqlDb.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;


            //InitializeUsers();
        }

        public async Task InitializeUsers()
        {
            //var projectUser = new IdentityUser
            //{
            //    UserName = "ProjectLeader"
            //};
            //await _userManager.CreateAsync(projectUser, "ProjectPassword");

            //var studentUser = new IdentityUser
            //{
            //    UserName = "student"
            //};
            //await _userManager.CreateAsync(studentUser, "StudentPassw0rd!");
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login(string usertype, string password, string returnUrl)
        {

            //await _roleManager.CreateAsync(new IdentityRole("Admin"));
            //await _roleManager.CreateAsync(new IdentityRole("Business"));
            //await _roleManager.CreateAsync(new IdentityRole("Team"));

            //await _roleManager.CreateAsync(new IdentityRole("Project"));



            Console.WriteLine("usertype");
            Console.WriteLine(usertype);
            Console.WriteLine(password);


            //var user = new IdentityUser
            //{
            //    UserName = usertype,
            //};

            //var result = await _userManager.CreateAsync(user, password);
            //await _userManager.AddToRolesAsync(user, new List<string> { "Project" });
            //await _userManager.AddToRolesAsync(user, new List<string> { "Team", "Business", "Project" });
            ////await _userManager.AddToRolesAsync(user, new List<string> { "Team", "Business", "Admin" });

            //if (result.Succeeded)
            //{
            //    var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);


            //    if (signInResult.Succeeded)
            //    {
            //        Console.WriteLine("Account created");
            //        return RedirectToAction("Index", "Inventory");
            //    }

            //}

            //return RedirectToAction("Index");


            var user = await _userManager.FindByNameAsync(usertype);

            if (user != null)
            {
                if (password != null)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(usertype, password, false, false);

                    if (signInResult.Succeeded)
                    {

                        // EDIT PASSWORD

                        /*var newPassword = "myNewPassword";
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, "AdminPassword");*/
                        TempData["login"] = "success";
                        Console.WriteLine("Sign in succeeded");
                        Console.WriteLine("Return Url");
                        Console.WriteLine(returnUrl);


                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {
                        TempData["login"] = "fail";
                        Console.WriteLine(TempData["login"]);
                        Console.WriteLine("Sign in failed");
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    TempData["login"] = "fail";
                    Console.WriteLine(TempData["login"]);
                    Console.WriteLine("Sign in failed");
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                Console.WriteLine("Unexpected error: user is null");
            }


            return RedirectToAction("Index", "Login");


        }

        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
