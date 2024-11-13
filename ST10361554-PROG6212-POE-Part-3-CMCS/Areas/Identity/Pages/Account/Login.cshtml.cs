// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// This is the input model for the login page.
        /// </summary>
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            ViewData["HideSidebar"] = true;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Fetch the user by email
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

                // Check if the user is in the "Lecturer" role
                if (user != null && await _signInManager.UserManager.IsInRoleAsync(user, "Lecturer"))
                {
                    // Check if the lecturer is approved
                    if (!user.IsLecturerApproved)
                    {
                        // Lecturer is not approved, return an error
                        ModelState.AddModelError(string.Empty, "Your account has not been approved by an Academic Manager or HR.");
                        ViewData["HideSidebar"] = true;
                        return Page();
                    }
                }

                // Check if the user is in the "Lecturer" role
                if (user != null && await _signInManager.UserManager.IsInRoleAsync(user, "Academic Manager"))
                {
                    // Check if the lecturer is approved
                    if (!user.IsManagerApproved)
                    {
                        // Lecturer is not approved, return an error
                        ModelState.AddModelError(string.Empty, "Your account has not been approved by HR.");
                        ViewData["HideSidebar"] = true;
                        return Page();
                    }
                }


                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (User.IsInRole("Lecturer"))
                    {
                        _logger.LogInformation("Lecturer User logged in.");
                        return RedirectToAction("GetLecturerDashboard", "Dashboards");
                    }

                    if (User.IsInRole("Academic Manager"))
                    {
                        _logger.LogInformation("Academic Manager User logged in.");
                        return RedirectToAction("GetAcademicManagerDashboard", "Dashboards");
                    }

                    if (User.IsInRole("HR"))
                    {
                        _logger.LogInformation("HR User logged in.");
                        return RedirectToAction("GetHRDashboard", "Dashboards");
                    }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Account for this username and password can't be found.");
                    ViewData["HideSidebar"] = true;
                    return Page();
                }
            }

            ViewData["HideSidebar"] = true;
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
