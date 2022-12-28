using System;
using System.Data.SqlClient;
using System.Data;
using Backend.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Logging;
using Backend.DB;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Backend.Services
{
    public interface ISurveyService
    {
        Product GetProduct(int? id);
        IEnumerable<Product> GetProducts();
        IEnumerable<Bug> GetBugsWithCondition(int? id);
        IEnumerable<Bug> GetBugsWithConditions(int? id, string keyword);
        bool CreateTask(Task task);
        Task GetTask(int? id);
        bool EditTask(Task task);
        bool DeleteTask(int? id);
        List<DateAndTasks> GetRoadmapLists(int? id);
        OverallCompletion GetOverallCompletionStats();
        OverallPriority GetOverallPriorityStats();
        IEnumerable<CurrentOpen> GetCurrentOpenStats();
        IEnumerable<OpenAge> GetOpenAgeStats();
        IEnumerable<AverageDefectAge> GetAverageDefectAgeStats();
        IEnumerable<TaskOpenBugs> GetTaskOpenBugStats();
        IEnumerable<SubmittedOpenClosed> GetSubmittedOpenClosedStats();
        IEnumerable<DeveloperProductivity> GetDeveloperProductivityStats();
        IEnumerable<AreaBugs> GetAreaBugStats();
        IEnumerable<ProductAndReleases> GetReleases();
        IEnumerable<Bug> GetBugsForRelease(int productid, string release);
        IEnumerable<OpenReleaseBugs> GetOpenBugsForRelease(int productid, string release);
        IEnumerable<Task> GetTasksForRelease(int productid, string release);
        bool CreateReleasePlan(ReleasePlan releasePlan);
        bool CheckReleaseName(string release, string productName);
        ReleasePlan GetReleasePlan(string release, string productName);
    }

    public class SurveyService : ISurveyService
    {
        IDatabase database;
        IBugService bugService;

        public SurveyService(IDatabase _database, IBugService _bugService)
        {
            database = _database;
            bugService = _bugService;
        }

        public Product GetProduct(int? id)
        {
           return database.getProduct(id);
        }

        public IEnumerable<Product> GetProducts()
        {
            return database.getProducts();
        }

        public IEnumerable<Bug> GetBugsWithCondition(int? id)
        {
            Product product = database.getProduct(id);

            IEnumerable<Bug> bugs = database.getBugs();
            bugs = bugs.Where(bug => bug.Product == product.Name);

            bugs = bugs.Select(b => (b.Resolution == "NotRep") ?
                                                new Bug()
                                                {
                                                    Id = b.Id,
                                                    Title = b.Title,
                                                    Description = b.Description,
                                                    Area = b.Area,
                                                    Category = b.Category,
                                                    Product = b.Product,
                                                    Release = b.Release,
                                                    Developers = b.Developers,
                                                    Customers = b.Customers,
                                                    Resolution = "Cannot Reproduce",
                                                    Priority = b.Priority
                                                }
                                                : (b.Resolution == "NotFix") ?
                                                new Bug()
                                                {
                                                    Id = b.Id,
                                                    Title = b.Title,
                                                    Description = b.Description,
                                                    Area = b.Area,
                                                    Category = b.Category,
                                                    Product = b.Product,
                                                    Release = b.Release,
                                                    Developers = b.Developers,
                                                    Customers = b.Customers,
                                                    Resolution = "Will not Fix",
                                                    Priority = b.Priority
                                                }
                                                : (b.Resolution == "Test") ?
                                                new Bug()
                                                {
                                                    Id = b.Id,
                                                    Title = b.Title,
                                                    Description = b.Description,
                                                    Area = b.Area,
                                                    Category = b.Category,
                                                    Product = b.Product,
                                                    Release = b.Release,
                                                    Developers = b.Developers,
                                                    Customers = b.Customers,
                                                    Resolution = "Ready for Testing",
                                                    Priority = b.Priority
                                                }
                                                : b);

            return bugs;
        }

        public IEnumerable<Bug> GetBugsWithConditions(int? id, string keyword)
        {
            Product product = database.getProduct(id);
            IEnumerable<Bug> bugs = database.getBugs();
            bugs = bugs.Where(bug => bug.Product == product.Name &&
                                        bug.Title.ToLower().Contains(keyword.ToLower()));

            bugs = bugs.Select(b => (b.Resolution == "NotRep") ?
                                                new Bug()
                                                {
                                                    Id = b.Id,
                                                    Title = b.Title,
                                                    Description = b.Description,
                                                    Area = b.Area,
                                                    Category = b.Category,
                                                    Product = b.Product,
                                                    Release = b.Release,
                                                    Developers = b.Developers,
                                                    Customers = b.Customers,
                                                    Resolution = "Cannot Reproduce",
                                                    Priority = b.Priority
                                                }
                                                : (b.Resolution == "NotFix") ?
                                                new Bug()
                                                {
                                                    Id = b.Id,
                                                    Title = b.Title,
                                                    Description = b.Description,
                                                    Area = b.Area,
                                                    Category = b.Category,
                                                    Product = b.Product,
                                                    Release = b.Release,
                                                    Developers = b.Developers,
                                                    Customers = b.Customers,
                                                    Resolution = "Will not Fix",
                                                    Priority = b.Priority
                                                }
                                                : (b.Resolution == "Test") ?
                                                new Bug()
                                                {
                                                    Id = b.Id,
                                                    Title = b.Title,
                                                    Description = b.Description,
                                                    Area = b.Area,
                                                    Category = b.Category,
                                                    Product = b.Product,
                                                    Release = b.Release,
                                                    Developers = b.Developers,
                                                    Customers = b.Customers,
                                                    Resolution = "Ready for Testing",
                                                    Priority = b.Priority
                                                }
                                                : b);

            return bugs;
        }

        public bool CreateTask(Task task)
        {
            return database.createTask(task);
        }

        public Task GetTask(int? id)
        {
            return database.getTask(id);
        }

        public bool EditTask(Task task)
        {
            return database.updateTask(task);
        }

        public bool DeleteTask(int? id)
        {
            return database.deleteTask(id);
        }

        public List<DateAndTasks> GetRoadmapLists(int? id)
        {
            List<DateTime> dates = new List<DateTime>();
            List<Task> tasks = new List<Task>();
            string[] separatedBugs;
            List<string> separatedBugsList;
            List<Bug> bugs;
            string simpleDate = "January 2022";

            List<SimpleTask> simpletasks = database.getTasksForProductId(id).ToList();
            foreach (SimpleTask simpleTask in simpletasks)
            {
                separatedBugs = simpleTask.TaskBugs.Split(",");
                separatedBugsList = new List<string>(separatedBugs);

                bugs = new List<Bug>();
                foreach (string bugtitle in separatedBugsList)
                {
                    bugs.Add(database.getBug(bugtitle));
                }

                Task task = new Task()
                {
                    Id = simpleTask.Id,
                    ProductId = simpleTask.ProductId,
                    Title = simpleTask.Title,
                    Start = simpleTask.Start,
                    End = simpleTask.End,
                    TaskBugs = bugs
                };

                tasks.Add(task);
            }

            foreach (Task task in tasks)
            {
                dates.Add(new DateTime(task.End.Year, task.End.Month, 1));
            }

            dates = dates.Distinct().ToList();
            dates = dates.OrderBy(date => date.Date).ToList();

            List<DateAndTasks> dateAndTasks = new List<DateAndTasks>();
            foreach(DateTime date in dates)
            {
                switch (date.Month)
                {
                    case 1:
                        simpleDate = "January " + date.Year;
                        break;
                    case 2:
                        simpleDate = "February " + date.Year;
                        break;
                    case 3:
                        simpleDate = "March " + date.Year;
                        break;
                    case 4:
                        simpleDate = "April " + date.Year;
                        break;
                    case 5:
                        simpleDate = "May " + date.Year;
                        break;
                    case 6:
                        simpleDate = "June " + date.Year;
                        break;
                    case 7:
                        simpleDate = "July " + date.Year;
                        break;
                    case 8:
                        simpleDate = "August " + date.Year;
                        break;
                    case 9:
                        simpleDate = "September " + date.Year;
                        break;
                    case 10:
                        simpleDate = "October " + date.Year;
                        break;
                    case 11:
                        simpleDate = "November " + date.Year;
                        break;
                    case 12:
                        simpleDate = "December " + date.Year;
                        break;
                }

                dateAndTasks.Add(new DateAndTasks()
                {
                    ProductId = id,
                    simpleDate = simpleDate,
                    Date = date,
                    Tasks = new List<Task>()
                });
            }

            foreach (DateAndTasks dat in dateAndTasks)
            {
                foreach (Task task in tasks)
                {
                    if (dat.Date == new DateTime(task.End.Year, task.End.Month, 1))
                    {
                        dat.Tasks.Add(task);
                    }
                }
            }

            return dateAndTasks;
        }

        public OverallCompletion GetOverallCompletionStats()
        {
            IEnumerable<Bug> bugs = database.getBugs();

            List<Bug> openBugs = bugs.Where(bug => bug.Resolution == "Open").ToList();
            List<Bug> notRepBugs = bugs.Where(bug => bug.Resolution == "NotRep").ToList();
            List<Bug> notFixBugs = bugs.Where(bug => bug.Resolution == "NotFix").ToList();
            List<Bug> testBugs = bugs.Where(bug => bug.Resolution == "Test").ToList();
            List<Bug> resolvedBugs = bugs.Where(bug => bug.Resolution == "Resolved").ToList();

            OverallCompletion stats = new OverallCompletion()
            {
                numberOfOpenBugs = openBugs.Count(),
                numberOfNotReproduceBugs = notRepBugs.Count(),
                numberOfNotFixBugs = notFixBugs.Count(),
                numberOfTestBugs = testBugs.Count(),
                numberOfResolvedBugs = resolvedBugs.Count()
            };

            return stats;
        }

        public OverallPriority GetOverallPriorityStats()
        {
            IEnumerable<Bug> bugs = database.getBugs();

            List<Bug> criticalBugs = bugs.Where(bug => bug.Priority == "Critical").ToList();
            List<Bug> highBugs = bugs.Where(bug => bug.Priority == "High").ToList();
            List<Bug> normalBugs = bugs.Where(bug => bug.Priority == "Normal").ToList();
            List<Bug> LowBugs = bugs.Where(bug => bug.Priority == "Low").ToList();

            OverallPriority stats = new OverallPriority()
            {
                numberOfCriticalBugs = criticalBugs.Count(),
                numberOfHighBugs = highBugs.Count(),
                numberOfNormalBugs = normalBugs.Count(),
                numberOfLowBugs = LowBugs.Count()
            };

            return stats;
        }

        public IEnumerable<CurrentOpen> GetCurrentOpenStats()
        {
            IEnumerable<Bug> allBugs = database.getBugs();

            DateTime start = allBugs.OrderBy(bug => bug.Created).First().Created;
            DateTime end = DateTime.Now;
            List<DateTime> dates = AllDatesBetween(start, end);
            dates = dates.Distinct().ToList();
            dates = dates.OrderBy(date => date.Date).ToList();

            CurrentOpen stat;
            IEnumerable<Log> logs;
            Log log;
            int dateOpenBugs = 0;
            List<CurrentOpen> stats = new List<CurrentOpen>();
            foreach (DateTime date in dates)
            {
                dateOpenBugs = 0;

                foreach (Bug bug in allBugs)
                {
                    logs = database.getLogsForId(bug.Id);
                    log = logs.Where(log => log.DateAndTime.Date <= date.Date)
                                .OrderBy(log => log.DateAndTime).LastOrDefault();

                    if (log != null && log.Resolution != "Closed")
                        dateOpenBugs++;
                }

                stat = new CurrentOpen()
                {
                    numberOfOpenBugs = dateOpenBugs,
                    day = date
                };

                stats.Add(stat);
            }

            return stats;
        }

        private List<DateTime> AllDatesBetween(DateTime start, DateTime end)
        {
            var dates = new List<DateTime>();

            for (DateTime dt = start.Date; dt <= end.Date; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            return dates;
        }

        public IEnumerable<OpenAge> GetOpenAgeStats()
        {
            List<int> ages = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            IEnumerable<Bug> notClosedBugs = database.getBugs().Where(bug => bug.Resolution != "Closed");

            OpenAge openAge;
            List<OpenAge> openAges = new List<OpenAge>();
            foreach (int age in ages)
            {
                openAge = new OpenAge()
                {
                    numberOfNotClosedBugs = notClosedBugs.Where(bug =>
                        (DateTime.Now.Date - bug.Created.Date).Days >= (age - 1) * 7 &&
                        (DateTime.Now.Date - bug.Created.Date).Days <= age * 7).Count(),
                    ageInWeeks = age 
                };

                openAges.Add(openAge);
            }

            openAge = new OpenAge()
            {
                numberOfNotClosedBugs = notClosedBugs.Where(bug =>
                    (DateTime.Now.Date - bug.Created.Date).Days >= 8 * 7).Count(),
                ageInWeeks = 9
            };
            openAges.Add(openAge);

            return openAges;
        }

        public IEnumerable<AverageDefectAge> GetAverageDefectAgeStats()
        {
            List<AverageDefectAge> stats = new List<AverageDefectAge>();

            IEnumerable<Log> logs = database.getAllLogs();
            IEnumerable<Bug> allBugs = database.getBugs();
            IEnumerable<Bug> closedBugs = allBugs.Where(bug => bug.Resolution == "Closed");

            DateTime start = allBugs.OrderBy(bug => bug.Created).First().Created;
            DateTime end = DateTime.Now;
            List<DateTime> dates = AllDatesBetween(start, end);
            dates = dates.Distinct().ToList();
            dates = dates.OrderBy(date => date.Date).ToList();

            List<(int week, int year)> weeksAndYears = dates.Select(date => (ISOWeek.GetWeekOfYear(date), date.Year)).ToList();
            weeksAndYears = weeksAndYears.Distinct().ToList();

            List<Bug> bugs;
            foreach((int week, int year) weekAndYear in weeksAndYears)
            {
                bugs = new List<Bug>();
                foreach (Bug bug in closedBugs)
                {
                    DateTime closed = logs.Where(log => log.BugId == bug.Id).Last().DateAndTime;
                    if (ISOWeek.GetWeekOfYear(closed) == weekAndYear.week && closed.Year == weekAndYear.year)
                        bugs.Add(bug);
                }

                int sum = 0;
                foreach (Bug bug in bugs)
                {
                    DateTime closed = logs.Where(log => log.BugId == bug.Id).Last().DateAndTime;
                    sum += (closed - bug.Created).Days;
                }

                int average = 0;
                if (bugs.Count() < 0)
                    average = sum / bugs.Count();

                AverageDefectAge stat = new AverageDefectAge()
                {
                    averageDefectAge = average,
                    week = weekAndYear.week,
                    year = weekAndYear.year
                };
                stats.Add(stat);
            }

            return stats;
        }

        public IEnumerable<TaskOpenBugs> GetTaskOpenBugStats()
        {
            int iBugs;
            IEnumerable<Bug> allBugs = database.getBugs();
            TaskOpenBugs stat;
            List<TaskOpenBugs> stats = new List<TaskOpenBugs>();

            DateTime start = allBugs.OrderBy(bug => bug.Created).First().Created;

            IEnumerable<Task> closedTasks = database.getAllTasks().Where(task => task.End < DateTime.Now);
            closedTasks = closedTasks.OrderBy(task => task.End);
            IEnumerable<Task> closedTasksHigherThanStart = closedTasks.Where(task => task.End >= start);
            Task lastClosedTask = (closedTasksHigherThanStart.Count() > 0) ? closedTasksHigherThanStart.Last() : null;

            DateTime end = (lastClosedTask != null) ? lastClosedTask.End : DateTime.Now;
            List<DateTime> dates = AllDatesBetween(start, end);
            dates = dates.Distinct().ToList();
            dates = dates.OrderBy(date => date.Date).ToList();

            List<(int week, int year)> weeksAndYears = dates.Select(date => (ISOWeek.GetWeekOfYear(date), date.Year)).ToList();
            weeksAndYears = weeksAndYears.Distinct().ToList();

            foreach ((int week, int year) weekAndYear in weeksAndYears)
            {
                iBugs = 0;
                IEnumerable<Task> tasks = closedTasks.Where(task =>
                    ISOWeek.GetWeekOfYear(task.End) == weekAndYear.week && task.End.Year == weekAndYear.year);
                foreach (Task task in tasks)
                {
                    iBugs += task.TaskBugs.Count();
                }

                stat = new TaskOpenBugs()
                {
                    numberOfOpenBugs = iBugs,
                    week = weekAndYear.week,
                    year = weekAndYear.year
                };

                stats.Add(stat);
            }

            return stats;
        }

        public IEnumerable<SubmittedOpenClosed> GetSubmittedOpenClosedStats()
        {
            IEnumerable<Bug> submittedBugs;
            IEnumerable<Bug> closedBugs;
            IEnumerable<Bug> openBugs;

            SubmittedOpenClosed soc;
            List<SubmittedOpenClosed> socs = new List<SubmittedOpenClosed>();

            IEnumerable<Bug> bugs = database.getBugs();
            IEnumerable<Log> logs = database.getAllLogs();

            DateTime earliest = bugs.OrderBy(bug => bug.Created.Date).First().Created;
            List<(int week, int year)> weeksAndYears = getWeeksBetweenTwoDates(earliest, DateTime.Now);

            foreach ((int week, int year) weekAndYear in weeksAndYears)
            {
                submittedBugs = bugs.Where(bug =>
                        ISOWeek.GetWeekOfYear(bug.Created) == weekAndYear.week &&
                        bug.Created.Year == weekAndYear.year);

                closedBugs = logs.Where(log =>
                    log.Resolution == "Closed" &&
                    ISOWeek.GetWeekOfYear(log.DateAndTime) == weekAndYear.week &&
                    log.DateAndTime.Year == weekAndYear.year).Select(log => database.getBug(log.BugId));

                openBugs = logs.Where(log =>
                    log.Resolution != "Closed" &&
                    ISOWeek.GetWeekOfYear(log.DateAndTime) == weekAndYear.week &&
                    log.DateAndTime.Year == weekAndYear.year).Select(log => database.getBug(log.BugId));

                soc = new SubmittedOpenClosed()
                {
                    numberOfSubmittedBugs = submittedBugs.Count(),
                    numberOfClosedBugs = closedBugs.Count(),
                    numberOfOpenBugs = openBugs.Count(),

                    week = weekAndYear.week,
                    year = weekAndYear.year
                };

                socs.Add(soc);
            }

            return socs;
        }

        private List<(int week, int year)> getWeeksBetweenTwoDates(DateTime from, DateTime to)
        {
            List<(int week, int year)> weeksAndYears = new List<(int week, int year)>();

            int fromWeek = ISOWeek.GetWeekOfYear(from);
            int toWeek = ISOWeek.GetWeekOfYear(to);

            if (from.Year == to.Year)
            {
                for (int i = fromWeek; i <= toWeek; i++)
                {
                    weeksAndYears.Add((i, from.Year));
                }
            }
            else
            {
                for (int i = fromWeek; i <= ISOWeek.GetWeeksInYear(from.Year); i++)
                {
                    weeksAndYears.Add((i, from.Year));
                }

                for (int i = from.Year + 1; i <= to.Year - 1; i++)
                {
                    for (int j = 1; j <= ISOWeek.GetWeeksInYear(i); j++)
                    {
                        weeksAndYears.Add((j, i));
                    }
                }

                for (int i = 1; i <= toWeek; i++)
                {
                    weeksAndYears.Add((i, to.Year));
                }
            }

            return weeksAndYears;
        }

        public IEnumerable<DeveloperProductivity> GetDeveloperProductivityStats()
        {
            List<DeveloperProductivity> stats = new List<DeveloperProductivity>();

            IEnumerable<Bug> allBugs = database.getBugs();
            IEnumerable<Developer> developers = database.getDevelopers();

            IEnumerable<Bug> openBugs;
            IEnumerable<Bug> notFixBugs;
            IEnumerable<Bug> notRepBugs;
            IEnumerable<Bug> testBugs;
            IEnumerable<Bug> resolvedBugs;
            IEnumerable<Bug> developerBugs;
            DeveloperProductivity stat;

            List<(Developer dev, int numberOfBugs)> devBugs = new List<(Developer dev, int numberOfBugs)>();
            foreach (Developer d in developers)
            {
                developerBugs = allBugs.Where(bug =>
                    bug.Developers.ToLower().Contains(d.Username.ToLower()));

                devBugs.Add((d, developerBugs.Count()));
            }
            devBugs = devBugs.OrderByDescending(db => db.numberOfBugs).Take(4).ToList();

            foreach ((Developer developer, int numberOfBugs) in devBugs)
            {
                developerBugs = allBugs.Where(bug =>
                    bug.Developers.ToLower().Contains(developer.Username.ToLower()));

                openBugs = developerBugs.Where(bug => bug.Resolution == "Open");
                notFixBugs = developerBugs.Where(bug => bug.Resolution == "NotFix");
                notRepBugs = developerBugs.Where(bug => bug.Resolution == "NotRep");
                testBugs = developerBugs.Where(bug => bug.Resolution == "Test");
                resolvedBugs = developerBugs.Where(bug => bug.Resolution == "Resolved");

                stat = new DeveloperProductivity()
                {
                    developerUsername = developer.Username,

                    numberOfOpenBugs = openBugs.Count(),
                    numberOfNotFixBugs = notFixBugs.Count(),
                    numberOfNotReproduceBugs = notRepBugs.Count(),
                    numberOfTestBugs = testBugs.Count(),
                    numberOfResolvedBugs = resolvedBugs.Count()
                };

                stats.Add(stat);
            }

            return stats;
        }

        public IEnumerable<AreaBugs> GetAreaBugStats()
        {
            IEnumerable<Bug> bugs = database.getBugs();
            List<AreaBugs> stats = new List<AreaBugs>();

            stats.Add(new AreaBugs()
            {
                area = "DB",
                generatedBugs = bugs.Where(bug => bug.Area == "DB").Count(),
                openBugs = bugs.Where(bug => bug.Area == "DB" && bug.Resolution != "Closed").Count()
            });

            stats.Add(new AreaBugs()
            {
                area = "UI",
                generatedBugs = bugs.Where(bug => bug.Area == "UI").Count(),
                openBugs = bugs.Where(bug => bug.Area == "UI" && bug.Resolution != "Closed").Count()
            });

            stats.Add(new AreaBugs()
            {
                area = "Documentation",
                generatedBugs = bugs.Where(bug => bug.Area == "Documentation").Count(),
                openBugs = bugs.Where(bug => bug.Area == "Documentation" && bug.Resolution != "Closed").Count()
            });

            stats.Add(new AreaBugs()
            {
                area = "Frontend",
                generatedBugs = bugs.Where(bug => bug.Area == "Frontend").Count(),
                openBugs = bugs.Where(bug => bug.Area == "Frontend" && bug.Resolution != "Closed").Count()
            });

            stats.Add(new AreaBugs()
            {
                area = "Security",
                generatedBugs = bugs.Where(bug => bug.Area == "Security").Count(),
                openBugs = bugs.Where(bug => bug.Area == "Security" && bug.Resolution != "Closed").Count()
            });

            stats.Add(new AreaBugs()
            {
                area = "Backend",
                generatedBugs = bugs.Where(bug => bug.Area == "Backend").Count(),
                openBugs = bugs.Where(bug => bug.Area == "Backend" && bug.Resolution != "Closed").Count()
            });

            return stats;
        }

        public IEnumerable<ProductAndReleases> GetReleases()
        {
            List<ProductAndReleases> productAndReleases = new List<ProductAndReleases>();
            List<string> releases;
            IEnumerable<Bug> bugs;
            IEnumerable<Product> products = GetProducts();
            IEnumerable<Bug> allBugs = bugService.GetBugs();
            foreach (Product product in products)
            {
                releases = new List<string>();
                bugs = allBugs.Where(bug => bug.Product == product.Name);
                foreach (Bug bug in bugs)
                {
                    releases.Add(bug.Release);
                }
                releases = releases.Distinct().OrderBy(release => release).ToList();
                ProductAndReleases par = new ProductAndReleases()
                {
                    Product = product,
                    Releases = releases
                };
                productAndReleases.Add(par);
            }

            return productAndReleases;
        }

        public IEnumerable<Bug> GetBugsForRelease(int productid, string release)
        {
            Product product = GetProduct(productid);
            IEnumerable<Bug> bugs = bugService.GetBugs();
            bugs = bugs.Where(bug => bug.Product == product.Name && bug.Release == release);
            return bugs;
        }

        public IEnumerable<OpenReleaseBugs> GetOpenBugsForRelease(int productid, string release)
        {
            IEnumerable<Bug> bugs = GetBugsForRelease(productid, release);
            bugs = bugs.Where(bug => bug.Resolution != "Closed");

            List<DateTime> dates = new List<DateTime>();
            foreach (Bug bug in bugs)
            {
                dates.Add(bug.Created);
            }
            dates = dates.Distinct().OrderBy(date => date).ToList();

            int numberofbugs = 0;
            OpenReleaseBugs stat;
            List<OpenReleaseBugs> stats = new List<OpenReleaseBugs>();
            foreach (DateTime date in dates)
            {
                numberofbugs = 0;
                foreach (Bug bug in bugs)
                {
                    IEnumerable<Log> logs = database.getLogsForId(bug.Id).Where(log => log.DateAndTime < date);
                    Log log = null;
                    if (logs.Count() > 0)
                        log = logs.Last();

                    if (log != null)
                        numberofbugs++;
                }

                stat = new OpenReleaseBugs()
                {
                    day = date,
                    numberOfOpenBugs = numberofbugs
                };
                stats.Add(stat);
            }

            return stats;
        }

        public IEnumerable<Task> GetTasksForRelease(int productid, string release)
        {
            List<string> simplebugs;
            List<Bug> bugs = new List<Bug>();
            List<Task> tasks = new List<Task>();
            IEnumerable<SimpleTask> simpletasks = database.getTasksForProductId(productid);
            foreach (SimpleTask simpleTask in simpletasks)
            {
                simplebugs = simpleTask.TaskBugs.Split(',').ToList();
                foreach (string bug in simplebugs)
                {
                    if (bug == ", " || bug == ",")
                        simplebugs.Remove(bug);
                }

                simplebugs = simplebugs.Select(bug => Regex.Replace(bug, @"\s+", "")).ToList();
                bugs = new List<Bug>();
                foreach (string simplebug in simplebugs)
                {
                    Bug bug = database.getBug(simplebug);
                    if (bug != null && bug.Release == release)
                        bugs.Add(bug);
                }

                bugs = bugs.Where(bug => bug.Release == release).ToList();

                if (bugs.Count() > 0)
                {
                    Task task = new Task()
                    {
                        Id = simpleTask.Id,
                        ProductId = simpleTask.ProductId,
                        Title = simpleTask.Title,
                        Start = simpleTask.Start,
                        End = simpleTask.End,
                        TaskBugs = bugs
                    };
                    tasks.Add(task);
                }
            }

            return tasks;
        }

        public bool CreateReleasePlan(ReleasePlan releasePlan)
        {
            return database.createReleasePlan(releasePlan);
        }

        public bool CheckReleaseName(string release, string productName)
        {
            return database.checkReleaseName(release, productName);
        }

        public ReleasePlan GetReleasePlan(string release, string productName)
        {
            return database.getReleasePlan(release, productName);
        }
    }
}
