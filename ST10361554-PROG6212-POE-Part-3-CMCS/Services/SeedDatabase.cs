using Microsoft.AspNetCore.Identity;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Services
{
    public class SeedDatabase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SeedDatabase> _logger;

        // Code Attribution:
        // How To Seed Users And Roles With Code In ASP.NET Core Identity
        // Rajesh Shirsagar
        // 9 October 2024
        // https://www.linkedin.com/pulse/overview-how-seed-users-roles-code-aspnet-core-rajesh-shirsagar/

        public SeedDatabase(
            UserManager<ApplicationUser> userManager,
            ILogger<SeedDatabase> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedHRUserAsync()
        {
            try
            {
                // check if the Hr user already exists
                var existingUser = await _userManager.FindByEmailAsync("hr@mail.com");

                if (existingUser == null)
                {
                    var hrUser = new ApplicationUser
                    {
                        UserName = "hr@mail.com",
                        Email = "hr@mail.com"
                    };

                    // seed the HR user with a password
                    var result = await _userManager.CreateAsync(hrUser, "Hr@123");

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("HR user seeded successfully.");

                        // Attempt to add the HR user to the HR role
                        var roleResult = await _userManager.AddToRoleAsync(hrUser, "HR");

                        if (roleResult.Succeeded)
                        {
                            _logger.LogInformation("HR user added to HR role successfully.");
                            return;
                        }
                        else
                        {
                            // Log errors that occur during seeding
                            foreach (var error in roleResult.Errors)
                            {
                                _logger.LogError($"Error adding HR user to HR role: {error.Description}");
                            }

                            return;
                        }
                    }
                    else
                    {
                        // Log errors that occur during seeding
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError($"Error seeding HR user: {error.Description}");
                        }

                        return;
                    }
                }
                else
                {
                    _logger.LogInformation("HR user already exists.");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An unexpected error occurred while seeding the HR user.");
                return;
            }
        }

        public async Task SeedLecturerUsersAsync()
        {
            try
            {
                // Check if any Lecturer user already exists in the database
                var lecturerUsers = await _userManager.GetUsersInRoleAsync("Lecturer");

                if (lecturerUsers.Any())
                {
                    _logger.LogInformation("Lecturer users already exist. Seeding is not required.");
                    return; // Exit if lecturers already exist
                }

                // Create a list to hold the lecturer users to be seeded
                var lecturersToSeed = new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        FirstName = "Sandra",
                        Surname = "Hicks",
                        PhoneNumber = "082 965 3675",
                        UserName = "lecturer@mail.com",
                        Email = "lecturer@mail.com",
                        StreetAddress = "65 Maple Lane",
                        AreaAddress = "Bryanston",
                        City = "Johannesburg",
                        Province = "Gauteng",
                        Faculty = "Education",
                        Module = "ChildPsyche201",
                        AccountNumber = "484621046320",
                        BankName = "Investec",
                        BranchCode = "452329",
                        IsLecturerApproved = true,
                        IsManagerApproved = false,
                        HourlyRate = 355.00m
                    },
                    new ApplicationUser
                    {
                        FirstName = "Ellen",
                        Surname = "Rodgers",
                        PhoneNumber = "082 420 0560",
                        UserName = "lecturer2@mail.com",
                        Email = "lecturer2@mail.com",
                        StreetAddress = "78 Oak Avenue",
                        AreaAddress = "Mount Edgecombe",
                        City = "Durban",
                        Province = "KwaZulu-Natal",
                        Faculty = "Computer Science",
                        Module = "C#Programming101",
                        AccountNumber = "14984813061984",
                        BankName = "Standard Bank",
                        BranchCode = "44156",
                        IsLecturerApproved = false,
                        IsManagerApproved = false,
                        HourlyRate = 280.00m
                    },
                    new ApplicationUser
                    {
                        FirstName = "Suzanne",
                        Surname = "Adler",
                        PhoneNumber = "082 471 8286",
                        UserName = "lecturer3@mail.com",
                        Email = "lecturer3@mail.com",
                        StreetAddress = "387 Telford Drive",
                        AreaAddress = "Umhlanga",
                        City = "Durban",
                        Province = "KwaZulu-Natal",
                        Faculty = "Finance",
                        Module = "Financial Accounting 302",
                        AccountNumber = "8548946165219",
                        BankName = "NedBank",
                        BranchCode = "98986",
                        IsLecturerApproved = false,
                        IsManagerApproved = false,
                        HourlyRate = 320.00m
                    },
                };

                // Define a default password for all users
                const string defaultPassword = "Lecturer@123";

                // Loop through each lecturer user and seed them
                foreach (var lecturerUser in lecturersToSeed)
                {
                    // Check if the user already exists
                    var existingUser = await _userManager.FindByEmailAsync(lecturerUser.Email!);
                    if (existingUser != null)
                    {
                        _logger.LogWarning($"User {lecturerUser.Email} already exists. Skipping.");
                        continue;
                    }

                    var result = await _userManager.CreateAsync(lecturerUser, defaultPassword);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Lecturer {lecturerUser.Email} seeded successfully.");

                        // Assign the Lecturer role to the user
                        var roleResult = await _userManager.AddToRoleAsync(lecturerUser, "Lecturer");

                        if (roleResult.Succeeded)
                        {
                            _logger.LogInformation($"Lecturer {lecturerUser.Email} assigned to Lecturer role successfully.");
                        }
                        else
                        {
                            // Log errors that occur during seeding
                            foreach (var error in roleResult.Errors)
                            {
                                _logger.LogError($"Error adding {lecturerUser.Email} to Lecturer role: {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        // Log errors that occur during seeding
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError($"Error seeding {lecturerUser.Email}: {error.Description}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An unexpected error occurred while seeding lecturer users.");
                return;
            }
        }

        public async Task SeedAcademicManagerUsersAsync()
        {
            try
            {
                // Check if any manager user already exists in the database
                var managerUsers = await _userManager.GetUsersInRoleAsync("Academic Manager");

                if (managerUsers.Any())
                {
                    _logger.LogInformation("Academic Manager users already exist. Seeding is not required.");
                    return; // Exit if Academic Managers already exist
                }

                // Create a list to hold the Academic Manager users to be seeded
                var managersToSeed = new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        FirstName = "Grace",
                        Surname = "Arnold",
                        PhoneNumber = "084 983 2867",
                        UserName = "academicmanager@mail.com",
                        Email = "academicmanager@mail.com",
                        StreetAddress = "786 Yale Lane",
                        AreaAddress = "Durban North",
                        City = "Durban",
                        Province = "KwaZulu-Natal",
                        IsLecturerApproved = false,
                        IsManagerApproved = true
                    },
                    new ApplicationUser
                    {
                        FirstName = "Dean",
                        Surname = "Thomas",
                        PhoneNumber = "082 569 3486",
                        UserName = "academicmanager2@mail.com",
                        Email = "academicmanager2@mail.com",
                        StreetAddress = "34 Oxford Avenue",
                        AreaAddress = "La Lucia",
                        City = "Durban",
                        Province = "KwaZulu-Natal",
                        IsLecturerApproved = false,
                        IsManagerApproved = false
                    },
                    new ApplicationUser
                    {
                        FirstName = "Michael",
                        Surname = "Duvall",
                        PhoneNumber = "083 187 6281",
                        UserName = "academicmanager3@mail.com",
                        Email = "academicmanager3@mail.com",
                        StreetAddress = "1894 Station Road",
                        AreaAddress = "Durban North",
                        City = "Durban",
                        Province = "KwaZulu-Natal",
                        IsLecturerApproved = false,
                        IsManagerApproved = false
                    },
                };

                // Define a default password for all users
                const string defaultPassword = "AcademicM@123";

                // Loop through each lecturer user and seed them
                foreach (var managerUser in managersToSeed)
                {
                    // Check if the user already exists
                    var existingUser = await _userManager.FindByEmailAsync(managerUser.Email!);
                    if (existingUser != null)
                    {
                        _logger.LogWarning($"User {managerUser.Email} already exists. Skipping.");
                        continue;
                    }

                    var result = await _userManager.CreateAsync(managerUser, defaultPassword);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Academic Manager {managerUser.Email} seeded successfully.");

                        // Assign the Academic Manager role to the user
                        var roleResult = await _userManager.AddToRoleAsync(managerUser, "Academic Manager");

                        if (roleResult.Succeeded)
                        {
                            _logger.LogInformation($"Academic Manager {managerUser.Email} assigned to Academic Manager role successfully.");
                        }
                        else
                        {
                            // Log errors that occur during seeding
                            foreach (var error in roleResult.Errors)
                            {
                                _logger.LogError($"Error adding {managerUser.Email} to Academic Manager role: {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        // Log errors that occur during seeding
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError($"Error seeding {managerUser.Email}: {error.Description}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An unexpected error occurred while seeding Academic Manager users.");
                return;
            }
        }
    }
}
