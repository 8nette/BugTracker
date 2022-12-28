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
using Newtonsoft.Json;

namespace Frontend.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private IUserService userService;
        private IBugService bugService;
        private ISurveyService surveyService;

        public SurveyController(ILogger<HomeController> _logger, IUserService _userService, IBugService _bugService, ISurveyService _surveyService)
        {
            logger = _logger;
            userService = _userService;
            bugService = _bugService;
            surveyService = _surveyService;
        }

        public IActionResult ProductRoadMaps()
        {
            return View(surveyService.GetProducts());
        }

        public IActionResult ProductRoadmap(int? id)
        {
            Product product = surveyService.GetProduct(id);
            List<DateAndTasks> tasks = surveyService.GetRoadmapLists(id);

            RoadmapModel model = new RoadmapModel()
            {
                product = product,
                productBugs = surveyService.GetBugsWithCondition(id),
                tasks = tasks.ToList()
            };

            return View(model);
        }

        public JsonResult GetJsonTasks(int? id)
        {
            List<DateAndTasks> tasks = surveyService.GetRoadmapLists(id);
            string json = JsonConvert.SerializeObject(tasks);
            return Json(json);
        }

        public IActionResult NewTask(int? id)
        {
            NewTaskModel task = new NewTaskModel()
            {
                productId = id
            };

            ViewBag.ReturnMessage = TempData["ReturnMessage"];
            return View(task);
        }

        public IActionResult Task(int? id)
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            Backend.Models.Task task = surveyService.GetTask(id);

            TaskModel model = new TaskModel()
            {
                Task = task
            };

            return View(model);
        }

        public IActionResult EditTask(int? id)
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            Backend.Models.Task task = surveyService.GetTask(id);

            List<string> bugTitles = new List<string>();
            foreach (Bug bug in task.TaskBugs)
            {
                bugTitles.Add(bug.Title);
            }
            string bugs = String.Join(",", bugTitles);

            SimpleTask simpleTask = new SimpleTask()
            {
                Id = task.Id,
                ProductId = task.ProductId,
                Title = task.Title,
                Start = task.Start,
                End = task.End,
                TaskBugs = bugs
            };

            return View(simpleTask);
        }

        [HttpPost]
        public IActionResult EditTask(SimpleTask simpleTask)
        {
            Backend.Models.Task task = null;
            string bugs;
            string[] separatedBugs;
            List<string> separatedBugsList;
            List<Bug> bs = new List<Bug>();

            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            bugs = simpleTask.TaskBugs;
            separatedBugs = bugs.Split(",");
            separatedBugsList = new List<string>(separatedBugs);

            if (separatedBugs[separatedBugs.Count() - 1] == ", " ||
                separatedBugs[separatedBugs.Count() - 1] == ",")
            {
                separatedBugsList.RemoveAt(separatedBugsList.Count() - 1);
            }

            foreach (string bugTitle in separatedBugsList)
            {
                bs.Add(bugService.GetBug(bugTitle));
            }

            task = new Backend.Models.Task()
            {
                Id = simpleTask.Id,
                ProductId = simpleTask.ProductId,
                Title = simpleTask.Title,
                Start = simpleTask.Start,
                End = simpleTask.End,
                TaskBugs = bs
            };

            if (surveyService.EditTask(task))
            {
                TempData["ReturnMessage"] = "The task was updated succesfully";
                return RedirectToAction("ProductRoadmap", new { id = task.ProductId });
            }
            else
            {
                TempData["ReturnMessage"] = "An error occurred and your task was not update, please try again";
                return RedirectToAction("Task", new { task.Id });
            }
        }

        public IActionResult DeleteTask(int? id)
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];

            Backend.Models.Task task = surveyService.GetTask(id);

            if (surveyService.DeleteTask(id))
            {
                TempData["ReturnMessage"] = "The task was deleted succesfully";
                return RedirectToAction("ProductRoadmap", new { id = task.ProductId });
            }
            else
            {
                TempData["ReturnMessage"] = "An error occurred and the task was not deleted, please try again";
                return RedirectToAction("Task", new { task.Id });
            }
        }

        public JsonResult GetJsonBugs(int? id, string keyword)
        {
            var bugs = surveyService.GetBugsWithConditions(id, keyword)
                                    .Select(bug => new { label = bug.Title, val = bug.Title });
            return Json(bugs);
        }

        [HttpPost]
        public IActionResult NewTask(NewTaskModel model)
        {
            ViewBag.ReturnMessage = TempData["ReturnMessage"];
            if (ModelState.IsValid == true)
            {
                List<Bug> bugs = new List<Bug>();
                string[] separatedBugs = model.Bugs.Split(", ");
                List<string> separatedBugsList = new List<string>(separatedBugs);

                if (separatedBugs[separatedBugs.Count() - 1] == ", " ||
                    separatedBugs[separatedBugs.Count() - 1] == "," ||
                    String.IsNullOrWhiteSpace(separatedBugs[separatedBugs.Count() - 1]))
                {
                    separatedBugsList.RemoveAt(separatedBugsList.Count() - 1);
                }

                foreach (string bugTitle in separatedBugsList)
                {
                    bugs.Add(bugService.GetBug(bugTitle));
                }

                Backend.Models.Task task = new Backend.Models.Task()
                {
                    ProductId = model.productId,
                    Title = model.Title,
                    Start = model.Start,
                    End = model.End,
                    TaskBugs = bugs
                };

                if (surveyService.CreateTask(task))
                {
                    TempData["ReturnMessage"] = "The task was created succesfully";
                    return RedirectToAction("ProductRoadmap", new { id = model.productId });
                }
                else
                {
                    TempData["ReturnMessage"] = "The task title was already taken, please choose another one";
                    return RedirectToAction("NewTask", new { model.productId });
                }
            }
            else
            {
                TempData["ReturnMessage"] = "The form was not filled out correctly, " +
                    "Title, Start and End are required";
                return RedirectToAction("NewTask", new { id = model.productId });
            }
            
        }

        public IActionResult Statistics()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetJsonStatCompletion()
        {
            OverallCompletion overallCompletion = surveyService.GetOverallCompletionStats();
            return Json(overallCompletion);
        }

        [HttpGet]
        public JsonResult GetJsonStatPriority()
        {
            OverallPriority overallPriority = surveyService.GetOverallPriorityStats();
            return Json(overallPriority);
        }

        [HttpGet]
        public JsonResult GetJsonStatCurrentOpen()
        {
            IEnumerable<CurrentOpen> currentOpens = surveyService.GetCurrentOpenStats();
            return Json(currentOpens);
        }

        [HttpGet]
        public JsonResult GetJsonStatOpenAges()
        {
            IEnumerable<OpenAge> openAges = surveyService.GetOpenAgeStats();
            return Json(openAges);
        }

        [HttpGet]
        public JsonResult GetJsonStatAverageDefectAges()
        {
            IEnumerable<AverageDefectAge> averageDefectAges = surveyService.GetAverageDefectAgeStats();
            return Json(averageDefectAges);
        }

        [HttpGet]
        public JsonResult GetJsonStatTaskOpenBugs()
        {
            IEnumerable<TaskOpenBugs> taskOpenBugs = surveyService.GetTaskOpenBugStats();
            return Json(taskOpenBugs);
        }

        [HttpGet]
        public JsonResult GetJsonSubmittedOpenClosedStats()
        {
            IEnumerable<SubmittedOpenClosed> submittedOpenClosed = surveyService.GetSubmittedOpenClosedStats();
            return Json(submittedOpenClosed);
        }

        [HttpGet]
        public JsonResult GetJsonDeveloperProductivityStats()
        {
            IEnumerable<DeveloperProductivity> developerProductivities = surveyService.GetDeveloperProductivityStats();
            return Json(developerProductivities);
        }

        [HttpGet]
        public JsonResult GetJsonAreaBugs()
        {
            IEnumerable<AreaBugs> areaBugs = surveyService.GetAreaBugStats();
            return Json(areaBugs);
        }

        public IActionResult ReleasePlanning()
        {
            return View(surveyService.GetReleases());
        }

        [HttpGet]
        public JsonResult GetJsonOpenReleaseBugs(int id, string release)
        {
            IEnumerable<OpenReleaseBugs> orb = surveyService.GetOpenBugsForRelease(id, release);
            return Json(orb);
        }

        public IActionResult Release(int id, string release)
        {
            Product product = surveyService.GetProduct(id);

            ReleasePlan releasePlan = surveyService.GetReleasePlan(release, product.Name);

            IEnumerable<Bug> bugs = surveyService.GetBugsForRelease(id, release);
            IEnumerable<Bug> features = bugs.Where(bug => bug.Category == "Feature");
            IEnumerable<Bug> noFeatures = bugs.Where(bug => bug.Category != "Feature");

            IEnumerable<Backend.Models.Task> tasks = surveyService.GetTasksForRelease(id, release);

            ReleaseModel model = new ReleaseModel()
            {
                product = product,
                release = release,
                releasePlan = releasePlan,
                features = features,
                bugs = noFeatures,
                tasks = tasks
            };

            return View(model);
        }
    }
}
