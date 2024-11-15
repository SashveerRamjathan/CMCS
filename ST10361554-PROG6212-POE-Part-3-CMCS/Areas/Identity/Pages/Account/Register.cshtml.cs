// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// Input Model Class for Register Page
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            [Display(Name = "Surname")]
            public string LastName { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [Display(Name = "Street Address")]
            public string Street { get; set; }

            [Required]
            [Display(Name = "Residential Area")]
            public string Area { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Province")]
            public string Province { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }


            // Lecturer
            [Display(Name = "Faculty")]
            public string Faculty { get; set; }

            [Required]
            [Display(Name = "Module")]
            public string Module { get; set; }

            [Display(Name = "Bank Account Number")]
            public string AccountNumber { get; set; }

            [Display(Name = "Bank Name")]
            public string BankName { get; set; }

            [Display(Name = "Branch Code")]
            public string BranchCode { get; set; }

            [Range(0.1, 500, ErrorMessage = "Hourly Rate can not be 0 or exceed 500")]
            [Display(Name = "Hourly Rate")]
            public decimal HourlyRate { get; set; }
        }


        public void OnGetAsync(string returnUrl = null)
        {
            ViewData["HideSidebar"] = true;

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            bool isUserValid = true;

            #region Check if Academic Manager Info is Valid

            if (Input.Role == "Academic Manager")
            {
                if (string.IsNullOrEmpty(Input.FirstName))
                {
                    ModelState.AddModelError(string.Empty, "First name is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.LastName))
                {
                    ModelState.AddModelError(string.Empty, "Last name is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.PhoneNumber))
                {
                    ModelState.AddModelError(string.Empty, "Phone number is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Street))
                {
                    ModelState.AddModelError(string.Empty, "Street address is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Area))
                {
                    ModelState.AddModelError(string.Empty, "Area is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.City))
                {
                    ModelState.AddModelError(string.Empty, "City is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Province))
                {
                    ModelState.AddModelError(string.Empty, "Province is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Role))
                {
                    ModelState.AddModelError(string.Empty, "Role is required.");
                    isUserValid = false;
                }
            }

            #endregion

            #region Check if Lecturer Info is Valid

            if (Input.Role == "Lecturer")
            {
                if (string.IsNullOrEmpty(Input.FirstName))
                {
                    ModelState.AddModelError(string.Empty, "First name is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.LastName))
                {
                    ModelState.AddModelError(string.Empty, "Last name is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.PhoneNumber))
                {
                    ModelState.AddModelError(string.Empty, "Phone number is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Street))
                {
                    ModelState.AddModelError(string.Empty, "Street address is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Area))
                {
                    ModelState.AddModelError(string.Empty, "Area is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.City))
                {
                    ModelState.AddModelError(string.Empty, "City is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Province))
                {
                    ModelState.AddModelError(string.Empty, "Province is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Role))
                {
                    ModelState.AddModelError(string.Empty, "Role is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Faculty))
                {
                    ModelState.AddModelError(string.Empty, "Faculty is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.Module))
                {
                    ModelState.AddModelError(string.Empty, "Module is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.AccountNumber))
                {
                    ModelState.AddModelError(string.Empty, "Account number is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.BankName))
                {
                    ModelState.AddModelError(string.Empty, "Bank name is required.");
                    isUserValid = false;
                }

                if (string.IsNullOrEmpty(Input.BranchCode))
                {
                    ModelState.AddModelError(string.Empty, "Branch code is required.");
                    isUserValid = false;
                }
            }

            #endregion

            if (isUserValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = Input.FirstName,
                    Surname = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    UserName = Input.Email,
                    Email = Input.Email,
                    StreetAddress = Input.Street,
                    AreaAddress = Input.Area,
                    City = Input.City,
                    Province = Input.Province,
                    IsLecturerApproved = false,
                    IsManagerApproved = false
                };

                if (Input.Role.Equals("Lecturer"))
                {
                    user.Faculty = Input.Faculty;
                    user.Module = Input.Module;
                    user.AccountNumber = Input.AccountNumber;
                    user.BankName = Input.BankName;
                    user.BranchCode = Input.BranchCode;
                    user.HourlyRate = Input.HourlyRate;
                }

                return await CreateUserAsync(user, Input.Role, returnUrl);

            }

            ViewData["HideSidebar"] = true;

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<IActionResult> CreateUserAsync(ApplicationUser user, string role, string returnUrl)
        {
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                var roleResult = await _userManager.AddToRoleAsync(user, role);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation($"User assigned to role '{role}'.");

                    // Redirect to login page after successful registration
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }
                else
                {
                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewData["HideSidebar"] = true;

            return Page(); // Return the page with errors
        }
    }
}
