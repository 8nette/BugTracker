using System;
using System.Data.SqlClient;
using System.Data;
using Backend.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Logging;
using Backend.DB;

namespace Backend.Services
{
    public interface IBugService
    {
        Bug GetBug(int? id);
        Bug GetBug(string title);
        IEnumerable<Bug> GetBugs();
        IEnumerable<Bug> GetSearchedBugs(BugSearch search);
        int CreateBug(Bug bug);
        bool UpdateBug(Bug bug);
        bool DeleteBug(int? id);
        IEnumerable<Product> GetProductsWithCondition(string keyword);
        bool CreateProduct(Product product);
        IEnumerable<Customer> GetCustomersWithCondition(string keyword);
        bool CreateCustomer(Customer customer);
        IEnumerable<Developer> GetDevelopersWithCondition(string keyword);
        bool CreateLogEntry(int? bugId, string resolution);
        IEnumerable<Log> GetLogsForId(int? id);
        IEnumerable<Log> GetAllLogs();
        bool UploadFile(File file);
        IEnumerable<File> GetFiles(int? id);
        File GetFile(int? id, string name);
        bool CreateComment(Comment comment);
        IEnumerable<Comment> GetCommentsForId(int? id);
    }

    public class BugService : IBugService
    {
        IDatabase database;

        public BugService(IDatabase _database)
        {
            database = _database;
        }

        public Bug GetBug(int? id)
        {
            Bug bug = database.getBug(id);
            Bug newBug;

            if (bug.Resolution == "NotRep")
            {
                newBug = new Bug()
                {
                    Id = bug.Id,
                    Title = bug.Title,
                    Description = bug.Description,
                    Area = bug.Area,
                    Category = bug.Category,
                    Product = bug.Product,
                    Release = bug.Release,
                    Developers = bug.Developers,
                    Customers = bug.Customers,
                    Resolution = "Cannot Reproduce",
                    Priority = bug.Priority
                };
            }
            else if (bug.Resolution == "NotFix")
            {
                newBug = new Bug()
                {
                    Id = bug.Id,
                    Title = bug.Title,
                    Description = bug.Description,
                    Area = bug.Area,
                    Category = bug.Category,
                    Product = bug.Product,
                    Release = bug.Release,
                    Developers = bug.Developers,
                    Customers = bug.Customers,
                    Resolution = "Will not Fix",
                    Priority = bug.Priority
                };
            }
            else if (bug.Resolution == "Test")
            {
                newBug = new Bug()
                {
                    Id = bug.Id,
                    Title = bug.Title,
                    Description = bug.Description,
                    Area = bug.Area,
                    Category = bug.Category,
                    Product = bug.Product,
                    Release = bug.Release,
                    Developers = bug.Developers,
                    Customers = bug.Customers,
                    Resolution = "Ready for Testing",
                    Priority = bug.Priority
                };
            }
            else
                newBug = bug;

            return newBug;

        }

        public Bug GetBug(string title)
        {
            return database.getBug(title);
        }

        public IEnumerable<Bug> GetBugs()
        {
            IEnumerable<Bug> bugs = database.getBugs();

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

        public IEnumerable<Bug> GetBugsByRelease(string release)
        {
            IEnumerable<Bug> allBugs = GetBugs();
            IEnumerable<Bug> releaseBugs = allBugs.Where(bug => bug.Release == release);
            releaseBugs = releaseBugs.Select(b => (b.Resolution == "NotRep") ?
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

            return releaseBugs;
        }

        public IEnumerable<Bug> GetSearchedBugs(BugSearch search)
        {
            IEnumerable<Bug> bugs = GetBugs();

            if (!String.IsNullOrEmpty(search.Title))
                bugs = bugs.Where(bug => bug.Title.ToLower().Contains(search.Title.ToLower()));
            if (!String.IsNullOrEmpty(search.Area))
                bugs = bugs.Where(bug => bug.Area == search.Area);
            if (!String.IsNullOrEmpty(search.Category))
                bugs = bugs.Where(bug => bug.Category == search.Category);
            if (!String.IsNullOrEmpty(search.Product))
                bugs = bugs.Where(bug => bug.Product.ToLower().Contains(search.Product.ToLower()));
            if (!String.IsNullOrEmpty(search.Release))
                bugs = bugs.Where(bug => bug.Release.Contains(search.Release));

            if (!String.IsNullOrEmpty(search.Developers))
            {
                List<string> developers = search.Developers.Split(',').ToList();
                foreach (string developer in developers)
                {
                    if (developer != ", " && developer != ",")
                        bugs = bugs.Where(bug => bug.Developers.ToLower().Contains(developer.ToLower()));
                }
            }

            if (!String.IsNullOrEmpty(search.Customers))
            {
                List<string> customers = search.Customers.Split(',').ToList();
                foreach (string customer in customers)
                {
                    if (customer != ", " && customer != ",")
                        bugs = bugs.Where(bug => bug.Customers.ToLower().Contains(customer.ToLower()));
                }
            }

            if (!String.IsNullOrEmpty(search.Resolution))
                bugs = bugs.Where(bug => bug.Resolution == search.Resolution);

            if (!String.IsNullOrEmpty(search.Priority))
                bugs = bugs.Where(bug => bug.Priority == search.Priority);

            if (search.CreatedBefore != DateTime.MinValue)
                bugs = bugs.Where(bug => bug.Created <= search.CreatedBefore);
            if (search.CreatedAfter != DateTime.MinValue)
                bugs = bugs.Where(bug => bug.Created >= search.CreatedAfter);
            if (search.CreatedAt != DateTime.MinValue)
                bugs = bugs.Where(bug => bug.Created == search.CreatedAt);

            return bugs;
        }

        public int CreateBug(Bug bug)
        {
            return database.createBug(bug);
        }

        public bool UpdateBug(Bug bug)
        {
            return database.updateBug(bug);
        }

        public bool DeleteBug(int? id)
        {
            database.deleteComments(id);
            database.deleteFiles(id);
            database.deleteLogs(id);

            Bug bug = database.getBug(id);

            IEnumerable<Task> tasks = database.getAllTasks();
            foreach (Task task in tasks)
            {
                IEnumerable<Bug> newBugList = task.TaskBugs.Where(b => b.Title != bug.Title);
                task.TaskBugs = newBugList;
                database.updateTask(task);
            }

            return database.deleteBug(id);
        }

        public IEnumerable<Product> GetProductsWithCondition(string keyword)
        {
            List<Product> products = database.getProducts().ToList();

            if (keyword != null)
                products = products.Where(p => p.Name.ToLower().Contains(keyword.ToLower())).ToList();

            return products;
        }

        public bool CreateProduct(Product product)
        {
            return database.createProduct(product);
        }

        public IEnumerable<Customer> GetCustomersWithCondition(string keyword)
        {
            List<Customer> customers = database.getCustomers().ToList();

            if (keyword != null)
            {
                customers = customers.Where(c => c.Name.ToLower().Contains(keyword.ToLower()) ||
                                                    c.Company.ToLower().Contains(keyword.ToLower()) ||
                                                    c.Email.ToLower().Contains(keyword.ToLower())).ToList();
            }

            return customers;
        }

        public bool CreateCustomer(Customer customer)
        {
            return database.createCustomer(customer);
        }

        public IEnumerable<Developer> GetDevelopersWithCondition(string keyword)
        {
            List<Developer> developers = database.getDevelopers().ToList();

            if (keyword != null)
            {
                developers = developers.Where(d => d.Name.ToLower().Contains(keyword.ToLower()) ||
                                                    d.Username.ToLower().Contains(keyword.ToLower()) ||
                                                    d.Email.ToLower().Contains(keyword.ToLower())).ToList();
            }

            return developers;
        }

        public bool CreateLogEntry(int? bugId, string resolution)
        {
            return database.createLogEntry(bugId, resolution);
        }

        public IEnumerable<Log> GetLogsForId(int? id)
        {
            return database.getLogsForId(id);
        }

        public IEnumerable<Log> GetAllLogs()
        {
            return database.getAllLogs();
        }

        public bool UploadFile(File file)
        {
            return database.uploadFile(file);
        }

        public IEnumerable<File> GetFiles(int? id)
        {
            return database.getFiles(id);
        }

        public File GetFile(int? id, string name)
        {
            return database.getFile(id, name);
        }

        public bool CreateComment(Comment comment)
        {
            return database.createComment(comment);
        }

        public IEnumerable<Comment> GetCommentsForId(int? id)
        {
            return database.getCommentsForId(id);
        }
    }
}
