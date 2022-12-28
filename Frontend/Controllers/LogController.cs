using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;
using Backend.Models;
using Backend.Services;
using Microsoft.Extensions.Options;
using Frontend.Helpers;
using Microsoft.AspNetCore.Http;

namespace Frontend.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogger<LogController> logger;
        private IUserService userService;
        private readonly AppSettings appSettings;

        public LogController(ILogger<LogController> _logger, IOptions<AppSettings> _appSettings, IUserService _userService)
        {
            logger = _logger;
            appSettings = _appSettings.Value;
            userService = _userService;
        }

        public IActionResult LogIn()
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(AuthenticateModel model)
        {
            if (ModelState.IsValid == true)
            {
                Developer user = userService.Authenticate(model.Username, model.Password, appSettings.Secret);

                if (user == null)
                {
                    TempData["ReturnMessage"] = "The log in failed, " +
                        "Username or Password was not correct. Please try again.";
                    return RedirectToAction("LogIn");
                }

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Token", user.Token);

                return RedirectToAction("Statistics", "Survey");
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "Username and Password are required";
                return RedirectToAction("LogIn");
            }
        }

        public IActionResult Register()
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid == true)
            {
                Developer user = new Developer()
                {
                    Name = model.Name,
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password
                };

                if (userService.Register(user))
                {
                    TempData["ReturnMessage"] = "You were registered succesfully, please log in";
                    return RedirectToAction("LogIn");
                }
                else
                {
                    TempData["ReturnMessage"] = "Your username is already taken, " +
                        "please choose another one";
                    return RedirectToAction("Register");
                }

            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "Name, Username, Email and Password are required";
                return RedirectToAction("Register");
            }
        }

        public IActionResult Update()
        {
            string username = HttpContext.Session.GetString("Username");

            ViewData["Username"] = username;
            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            Developer oldDeveloper = userService.GetUser(username);
            UpdateDeveloperModel model = new UpdateDeveloperModel()
            {
                Name = oldDeveloper.Name,
                Email = oldDeveloper.Email,
                Password = oldDeveloper.Password
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(UpdateDeveloperModel model)
        {
            if (ModelState.IsValid == true)
            {
                Developer developer = new Developer()
                {
                    Name = model.Name,
                    Username = HttpContext.Session.GetString("Username"),
                    Email = model.Email,
                    Password = model.Password
                };

                if (userService.UpdateUser(developer))
                    TempData["ReturnMessage"] = "Your user profile was updated succesfully";
                else
                    TempData["ReturnMessage"] = "An error occurred, your profile was not updated, please try again";

                return RedirectToAction("Update");
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly";
                return RedirectToAction("Update");
            }
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.SetString("Username", "");
            HttpContext.Session.SetString("Token", "");

            return RedirectToAction("Index", "Home");
        }
    }
}
