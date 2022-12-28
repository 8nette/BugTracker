using System;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.DB
{
    public interface IDatabase
    {
        bool createDeveloper(Developer developer);
        Developer checkDeveloper(string username, string password);
        Developer getDeveloper(string username);
        Developer getDeveloperById(int? id);
        IEnumerable<Developer> getDevelopers();
        bool updateDeveloper(Developer newDeveloper);

        IEnumerable<Bug> getBugs();
        Bug getBug(int? id);
        Bug getBug(string title);
        int createBug(Bug bug);
        bool updateBug(Bug newBug);
        bool deleteBug(int? id);

        IEnumerable<Customer> getCustomers();
        bool createCustomer(Customer customer);

        Product getProduct(int? id);
        IEnumerable<Product> getProducts();
        bool createProduct(Product product);

        bool createLogEntry(int? bugId, string resolution);
        IEnumerable<Log> getLogsForId(int? id);
        IEnumerable<Log> getAllLogs();
        void deleteLogs(int? id);

        bool uploadFile(File file);
        IEnumerable<File> getFiles(int? id);
        File getFile(int? id, string name);
        void deleteFiles(int? id);

        bool createComment(Comment comment);
        IEnumerable<Comment> getCommentsForId(int? id);
        void deleteComments(int? id);

        bool createTask(Task task);
        Task getTask(string title);
        Task getTask(int? id);
        IEnumerable<SimpleTask> getTasksForProductId(int? id);
        IEnumerable<Task> getAllTasks();
        bool updateTask(Task newtask);
        bool deleteTask(int? id);

        bool createReleasePlan(ReleasePlan releasePlan);
        bool checkReleaseName(string release, string productName);
        ReleasePlan getReleasePlan(string release, string productName);
    }
}
