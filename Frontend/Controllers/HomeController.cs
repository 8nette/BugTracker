using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Frontend.Models;
using Backend.Services;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private IUserService userService;
        private IBugService bugService;
        private ISurveyService surveyService;

        public HomeController(ILogger<HomeController> _logger, IUserService _userService, IBugService _bugService, ISurveyService _surveyService)
        {
            logger = _logger;
            userService = _userService;
            bugService = _bugService;
            surveyService = _surveyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Bugs(BugsModel model)
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            BugSearch search = new BugSearch()
            {
                Title = model.Title,
                Area = model.Area,
                Category = model.Category,
                Product = model.Product,
                Release = model.Release,
                Developers = model.Developers,
                Customers = model.Customers,
                Resolution = model.Resolution,
                Priority = model.Priority,
                CreatedBefore = model.CreatedBefore,
                CreatedAfter = model.CreatedAfter,
                CreatedAt = model.CreatedAt
            };

            IEnumerable<Bug> bugs = bugService.GetSearchedBugs(search);

            model.Bugs = bugs;
                
            return View(model);
        }

        public IActionResult Bug(int? id)
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            Bug bug = bugService.GetBug(id);
            IEnumerable<Backend.Models.File> files = bugService.GetFiles(id);

            BugModel model = new BugModel()
            {
                Bug = bug,
                Files = files
            };

            return View(model);
        }

        public IActionResult NewBug()
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];
            return View();
        }

        [HttpPost]
        public IActionResult NewBug(NewBugModel model)
        {
            if (ModelState.IsValid == true)
            {
                if (!surveyService.CheckReleaseName(model.Release, model.Product))
                {
                    ReleasePlan releasePlan = new ReleasePlan()
                    {
                        Name = model.Release,
                        ProductName = model.Product,
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddMonths(2)
                    };

                    surveyService.CreateReleasePlan(releasePlan);
                }

                Bug bug = new Bug()
                {
                    Title = model.Title,
                    Description = model.Description,
                    Area = model.Area,
                    Category = model.Category,
                    Product = model.Product,
                    Release = model.Release,
                    Developers = model.Developers,
                    Customers = model.Customers,
                    Resolution = "Open",
                    Priority = model.Priority,
                    Created = DateTime.Now
                };

                int createdBugId = bugService.CreateBug(bug);
                bool createLog = false;
                if (createdBugId != 0)
                    createLog = bugService.CreateLogEntry(createdBugId, "Open");

                if (createdBugId != 0 && createLog)
                {
                    TempData["ReturnMessage"] = "The bug was created succesfully";
                    return RedirectToAction("Bugs");
                }
                else
                {
                    TempData["ReturnMessage"] = "An error ocurred or the bug title was already taken, please choose another one";
                    return RedirectToAction("NewBug");
                }
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "Title and Priority are required";
                return RedirectToAction("NewBug");
            }
        }

        public IActionResult EditBug(int? id)
        {
            return View(bugService.GetBug(id));
        }

        [HttpPost]
        public IActionResult EditBug(Bug bug)
        {
            if (bugService.UpdateBug(bug))
            {
                if ((!string.IsNullOrEmpty(bug.Resolution) &&
                    bugService.CreateLogEntry(bug.Id, bug.Resolution)) ||
                    string.IsNullOrEmpty(bug.Resolution))
                {
                    TempData["ReturnMessage"] = "The bug was updated succesfully";
                    return RedirectToAction("Bug", new { id = bug.Id });
                }
                else
                {
                    TempData["ReturnMessage"] = "An error ocurred and the log was not updated, please try again";
                    return RedirectToAction("Bug", new { id = bug.Id });
                }
            }
            else
            {
                TempData["ReturnMessage"] = "An error ocurred and the bug was not updated, please try again";
                return RedirectToAction("Bug", new { id = bug.Id });
            }
        }

        public IActionResult DeleteBug(int? id)
        {
            if (bugService.DeleteBug(id))
            {
                TempData["ReturnMessage"] = "The bug was deleted succesfully";
            }
            else
            {
                TempData["ReturnMessage"] = "An error ocurred and the bug was not deleted, please try again";
            }

            return RedirectToAction("Bugs");
        }

        public IActionResult NewProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewProduct(NewProductModel model)
        {
            if (ModelState.IsValid == true)
            {
                Product product = new Product()
                {
                    Name = model.Name
                };

                if (bugService.CreateProduct(product))
                {
                    TempData["ReturnMessage"] = "The product was created succesfully";
                    return RedirectToAction("Bugs");
                }
                else
                {
                    TempData["ReturnMessage"] = "The product name was already taken, please choose another one";
                    return RedirectToAction("NewProduct");
                }
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "Name is required";
                return RedirectToAction("NewProduct");
            }
        }

        [HttpPost]
        public JsonResult GetJsonProducts(string keyword)
        {
            var products = bugService.GetProductsWithCondition(keyword)
                                        .Select(p => new { label = p.Name, val = p.Name });

            return Json(products);
        }

        public IActionResult NewCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewCustomer(NewCustomerModel model)
        {
            if (ModelState.IsValid == true)
            {
                Customer customer = new Customer()
                {
                    Name = model.Name,
                    Company = model.Company,
                    Email = model.Email
                };

                if (bugService.CreateCustomer(customer))
                {
                    TempData["ReturnMessage"] = "The customer was created succesfully";
                    return RedirectToAction("Bugs");
                }
                else
                {
                    TempData["ReturnMessage"] = "The customer email was already taken, please choose another one";
                    return RedirectToAction("NewCustomer");
                }
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "Name, Company and Email are required";
                return RedirectToAction("NewProduct");
            }
        }

        [HttpPost]
        public JsonResult GetJsonCustomers(string keyword)
        {
            var customers = bugService.GetCustomersWithCondition(keyword)
                                        .Select(c => new { label = c.Name, val = c.Name });

            return Json(customers);
        }

        [HttpPost]
        public JsonResult GetJsonDevelopers(string keyword)
        {
            var developers = bugService.GetDevelopersWithCondition(keyword)
                                        .Select(d => new { label = d.Name, val = d.Name });

            return Json(developers);
        }

        public IActionResult Log(int? id)
        {
            LogModel model = new LogModel()
            {
                bugId = id,
                bugTitle = bugService.GetBug(id).Title,
                logs = bugService.GetLogsForId(id)
            };

            return View(model);
        }

        public IActionResult File(int? id)
        {
            IdModel model = new IdModel()
            {
                Id = id,
                BugTitle = bugService.GetBug(id).Title
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UploadFile(IdModel model)
        {
            if (HttpContext.Request.Form.Files.Count() > 0)
            {
                IFormFile file = HttpContext.Request.Form.Files[0];

                Backend.Models.File filemodel = new Backend.Models.File()
                {
                    BugId = model.Id,
                    Name = Path.GetFileNameWithoutExtension(file.FileName),
                    ContentType = file.ContentType
                };

                using (MemoryStream stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    filemodel.Contents = stream.ToArray();
                }

                if (bugService.UploadFile(filemodel))
                {
                    TempData["ReturnMessage"] = "The file was uploaded succesfully";
                    return RedirectToAction("Bug", new { model.Id });
                }

                else
                {
                    TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "a file is required, and the filename has to be unique";

                    return RedirectToAction("File", new { model.Id });
                }
                    
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "a file is required";

                return RedirectToAction("File", new { model.Id });
            }
        }

        public FileResult DownloadFile(int? id, string name)
        {
            Backend.Models.File file = bugService.GetFile(id, name);

            return File(file.Contents, file.ContentType, name); 
        }

        public IActionResult Comments(int? id)
        {
            Bug bug = bugService.GetBug(id);
            Comment comment = new Comment()
            {
                BugId = id
            };

            Developer developer;
            List<(string, Comment)> developerComments = new List<(string, Comment)>();
            IEnumerable<Comment> comments = bugService.GetCommentsForId(id);
            foreach (Comment c in comments)
            {
                developer = userService.GetUserById(c.DeveloperId);
                developerComments.Add((developer.Username, c));
            }

            CommentsModel model = new CommentsModel()
            {
                BugTitle = bug.Title,
                Comment = comment,
                Comments = developerComments
            };

            ViewBag.ReturnMessage = TempData["ReturnMessage"];
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateComment(CommentsModel model)
        {
            Comment comment = new Comment()
            {
                BugId = model.Comment.BugId,
                DeveloperId = userService.GetUser(HttpContext.Session.GetString("Username")).Id,
                Created = DateTime.Now,
                _Comment = model.Comment._Comment
            };

            if (bugService.CreateComment(comment))
            {
                TempData["ReturnMessage"] = "The comment was created succesfully";
                return RedirectToAction("Comments", new { id = model.Comment.BugId });
            }
            else
            {
                TempData["ReturnMessage"] = "An error occurred and the comment was not created, please try again";
                return RedirectToAction("Comments", new { id = model.Comment.BugId });
            }
        }
    }
}
