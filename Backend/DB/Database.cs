using System;
using System.Data.SqlClient;
using System.Data;
using Backend.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Backend.DB
{
    public class Database : IDatabase
    {
        private static readonly string connectionString = "Server=localhost;" +
                "Database=BugTracker2;" +
                "User Id=sa;" +
                "Password=#Bamse19918;" +
                "MultipleActiveResultSets=true";
        private SqlConnection conn = new SqlConnection(connectionString);
        private ILogger<Database> _logger;
        private List<SqlParameter> sqlParameters;

        public Database(ILogger<Database> logger)
        {
            _logger = logger;
        }

        #region basics

        private void connect()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                _logger.LogInformation("connection to database opened");
            }
        }

        private void disconnect()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                _logger.LogInformation("connection to database closed");
            }
        }

        private int execute(string query, List<SqlParameter> parameters = null)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            connect();
            _logger.LogInformation("executing non query from database");
            int rowsAffected = cmd.ExecuteNonQuery();
            disconnect();

            return rowsAffected;
        }

        private int executeScalar(string query, List<SqlParameter> parameters = null)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            foreach (SqlParameter parameter in parameters)
            {
                cmd.Parameters.Add(parameter);
            }

            connect();
            _logger.LogInformation("executing scalar from database");
            int count = (int)cmd.ExecuteScalar();
            disconnect();

            return count;
        }

        #endregion

        #region developers
        public bool createDeveloper(Developer developer)
        {
            if (developer.Name == null ||
                developer.Username == null ||
                developer.Email == null ||
                developer.Password == null ||
                checkDeveloperUsername(developer.Username))
                    return false;

            string query_start = "INSERT INTO Developers (";
            string query_end = "VALUES (";

            query_start += "Name, ";
            query_end += "@Name, ";

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = developer.Name;

            query_start += "Username, ";
            query_end += "@Username, ";

            SqlParameter usernameParameter = new SqlParameter("Username", SqlDbType.VarChar);
            usernameParameter.Value = developer.Username;

            query_start += "Email, ";
            query_end += "@Email, ";

            SqlParameter emailParameter = new SqlParameter("Email", SqlDbType.VarChar);
            emailParameter.Value = developer.Email;

            query_start += "Password) ";
            query_end += "@Password);";

            SqlParameter passwordParameter = new SqlParameter("Password", SqlDbType.VarChar);
            passwordParameter.Value = developer.Password;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                nameParameter,
                usernameParameter,
                emailParameter,
                passwordParameter
            };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        private bool checkDeveloperUsername(string username)
        {
            string query = $"SELECT COUNT(1) FROM Developers WHERE Username = @username;";

            SqlParameter nameParameter = new SqlParameter("username", SqlDbType.VarChar);
            nameParameter.Value = username;

            sqlParameters = new List<SqlParameter>() { nameParameter };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public Developer checkDeveloper(string username, string password)
        {
            string query = $"SELECT COUNT(1) FROM Developers WHERE " +
                $"Username = @Username AND " +
                $"Password = @Password;";

            SqlParameter usernameParameter = new SqlParameter("Username", SqlDbType.VarChar);
            usernameParameter.Value = username;

            SqlParameter passwordParameter = new SqlParameter("Password", SqlDbType.VarChar);
            passwordParameter.Value = password;

            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                usernameParameter,
                passwordParameter
            };

            if (executeScalar(query, sqlParameters) != 0)
                return getDeveloper(username);

            return null;
        }

        public Developer getDeveloper(string username)
        {
            Developer developer = null;

            string query = $"SELECT * FROM Developers WHERE Username = @Username;";
            SqlParameter usernameParameter = new SqlParameter("Username", SqlDbType.VarChar);
            usernameParameter.Value = username;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(usernameParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                developer = new Developer()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    Username = reader["Username"] as string,
                    Email = reader["Email"] as string,
                    Password = reader["Password"] as string,
                    Token = null
                };
            }

            disconnect();

            return developer;
        }

        public Developer getDeveloperById(int? id)
        {
            Developer developer = null;

            string query = $"SELECT * FROM Developers WHERE Id = @Id;";
            SqlParameter idParameter = new SqlParameter("Id", SqlDbType.Int);
            idParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(idParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                developer = new Developer()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    Username = reader["Username"] as string,
                    Email = reader["Email"] as string,
                    Password = reader["Password"] as string,
                    Token = null
                };
            }

            disconnect();

            return developer;
        }

        public IEnumerable<Developer> getDevelopers()
        {
            List<Developer> developers = new List<Developer>();
            Developer developer;

            string query = "SELECT * FROM Developers;";

            SqlCommand cmd = new SqlCommand(query, conn);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                developer = new Developer()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    Username = reader["Username"] as string,
                    Email = reader["Email"] as string,
                    Password = reader["Password"] as string,
                    Token = null
                };
                developers.Add(developer);
            }

            disconnect();

            return developers;
        }

        public bool updateDeveloper(Developer newDeveloper)
        {
            if (newDeveloper != null && newDeveloper.Username != null)
            {
                Developer oldDeveloper = getDeveloper(newDeveloper.Username);

                string name = oldDeveloper.Name;
                string username = newDeveloper.Username;
                string email = oldDeveloper.Email;
                string password = oldDeveloper.Password;

                if (!string.IsNullOrEmpty(newDeveloper.Name))
                    name = newDeveloper.Name;
                if (!string.IsNullOrEmpty(newDeveloper.Email))
                    email = newDeveloper.Email;
                if (!string.IsNullOrEmpty(newDeveloper.Password))
                    password = newDeveloper.Password;

                string query = "UPDATE Developers SET ";

                query += $"Name = @name, ";
                SqlParameter nameParameter = new SqlParameter("name", SqlDbType.VarChar);
                nameParameter.Value = name;

                query += $"Email = @email, ";
                SqlParameter emailParameter = new SqlParameter("email", SqlDbType.VarChar);
                emailParameter.Value = email;

                query += $"Password = @password ";
                SqlParameter passwordParameter = new SqlParameter("password", SqlDbType.VarChar);
                passwordParameter.Value = password;

                query += $"WHERE Username = @username;";
                SqlParameter usernameParameter = new SqlParameter("username", SqlDbType.VarChar);
                usernameParameter.Value = username;

                List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    nameParameter,
                    emailParameter,
                    passwordParameter,
                    usernameParameter
                };

                if (execute(query, sqlParameters) != 0)
                    return true;
            }

            return false;
        }

        #endregion

        #region bugs
        public IEnumerable<Bug> getBugs()
        {
            List<Bug> bugs = new List<Bug>();
            Bug bug;

            string query = "SELECT * FROM Bugs;";

            SqlCommand cmd = new SqlCommand(query, conn);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bug = new Bug()
                {
                    Id = (int)reader["Id"],
                    Title = reader["Title"] as string,
                    Description = reader["Description"] as string,
                    Priority = reader["Priority"] as string,
                    Category = reader["Category"] as string,
                    Area = reader["Area"] as string,
                    Product = reader["Product"] as string,
                    Release = reader["Release"] as string,
                    Developers = reader["Developers"] as string,
                    Customers = reader["Customers"] as string,
                    Resolution = reader["Resolution"] as string,
                    Created = (DateTime)reader["Created"]
                };
                bugs.Add(bug);
            }

            disconnect();

            return bugs;
        }

        public Bug getBug(int? id)
        {
            Bug bug = null;

            if (id != null)
            {
                string query = "SELECT * FROM Bugs WHERE Id = @Id;";
                SqlParameter idParameter = new SqlParameter("Id", SqlDbType.VarChar);
                idParameter.Value = id;

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(idParameter);

                connect();
                _logger.LogInformation("executing reader from database");

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bug = new Bug()
                    {
                        Id = (int)reader["Id"],
                        Title = reader["Title"] as string,
                        Description = reader["Description"] as string,
                        Priority = reader["Priority"] as string,
                        Category = reader["Category"] as string,
                        Area = reader["Area"] as string,
                        Product = reader["Product"] as string,
                        Release = reader["Release"] as string,
                        Developers = reader["Developers"] as string,
                        Customers = reader["Customers"] as string,
                        Resolution = reader["Resolution"] as string,
                        Created = (DateTime)reader["Created"]
                    };
                }

                disconnect();
            }

            return bug;
        }

        public Bug getBug(string title)
        {
            Bug bug = null;

            if (title != null)
            {
                string query = "SELECT * FROM Bugs WHERE Title = @Title;";
                SqlParameter titleParameter = new SqlParameter("Title", SqlDbType.VarChar);
                titleParameter.Value = title;

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(titleParameter);

                connect();
                _logger.LogInformation("executing reader from database");

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    bug = new Bug()
                    {
                        Id = (int)reader["Id"],
                        Title = title,
                        Description = reader["Description"] as string,
                        Priority = reader["Priority"] as string,
                        Category = reader["Category"] as string,
                        Area = reader["Area"] as string,
                        Product = reader["Product"] as string,
                        Release = reader["Release"] as string,
                        Developers = reader["Developers"] as string,
                        Customers = reader["Customers"] as string,
                        Resolution = reader["Resolution"] as string,
                        Created = (DateTime)reader["Created"]
                    };
                }

                disconnect();
            }

            return bug;
        }

        private bool checkBugTitle(string title)
        {
            string query = $"SELECT COUNT(1) FROM Bugs WHERE Title = @title;";

            SqlParameter titleParameter = new SqlParameter("title", SqlDbType.VarChar);
            titleParameter.Value = title;

            sqlParameters = new List<SqlParameter>() { titleParameter };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public int createBug(Bug bug)
        {
            if (bug.Title == null ||
                bug.Area == null ||
                bug.Category == null ||
                bug.Product == null ||
                bug.Priority == null ||
                bug.Created == null ||
                checkBugTitle(bug.Title))
                return 0;

            int returnId = 0;

            string query_start = "INSERT INTO Bugs (";
            string query_end = "OUTPUT INSERTED.ID VALUES (";

            query_start += "Title, ";
            query_end += "@Title, ";

            SqlParameter titleParameter = new SqlParameter("Title", SqlDbType.VarChar);
            titleParameter.Value = bug.Title;

            query_start += "Created, ";
            query_end += "@Created, ";

            SqlParameter createdParameter = new SqlParameter("Created", SqlDbType.DateTime);
            createdParameter.Value = bug.Created;

            query_start += "Description, ";
            query_end += "@Description, ";

            SqlParameter descriptionParameter = new SqlParameter("Description", SqlDbType.VarChar);
            descriptionParameter.Value = bug.Description;

            query_start += "Area, ";
            query_end += "@Area, ";

            SqlParameter areaParameter = new SqlParameter("Area", SqlDbType.VarChar);
            areaParameter.Value = bug.Area;

            query_start += "Category, ";
            query_end += "@Category, ";

            SqlParameter categoryParameter = new SqlParameter("Category", SqlDbType.VarChar);
            categoryParameter.Value = bug.Category;

            query_start += "Product, ";
            query_end += "@Product, ";

            SqlParameter productParameter = new SqlParameter("Product", SqlDbType.VarChar);
            productParameter.Value = bug.Product;

            query_start += "Release, ";
            query_end += "@Release, ";

            SqlParameter releaseParameter = new SqlParameter("Release", SqlDbType.VarChar);
            releaseParameter.Value = bug.Release;

            query_start += "Developers, ";
            query_end += "@Developers, ";

            SqlParameter developersParameter = new SqlParameter("Developers", SqlDbType.VarChar);
            developersParameter.Value = bug.Developers;

            query_start += "Customers, ";
            query_end += "@Customers, ";

            SqlParameter customersParameter = new SqlParameter("Customers", SqlDbType.VarChar);
            customersParameter.Value = bug.Customers;

            query_start += "Resolution, ";
            query_end += "@Resolution, ";

            SqlParameter resolutionParameter = new SqlParameter("Resolution", SqlDbType.VarChar);
            resolutionParameter.Value = bug.Resolution;

            query_start += "Priority) ";
            query_end += "@Priority);";

            SqlParameter priorityParameter = new SqlParameter("Priority", SqlDbType.VarChar);
            priorityParameter.Value = bug.Priority;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                titleParameter,
                createdParameter,
                descriptionParameter,
                areaParameter,
                categoryParameter,
                productParameter,
                releaseParameter,
                developersParameter,
                customersParameter,
                resolutionParameter,
                priorityParameter
            };

            returnId = executeScalar(query, sqlParameters);

            return returnId;
        }

        public bool updateBug(Bug newBug)
        {
            if (newBug != null && newBug.Id != null)
            {
                Bug oldBug = getBug(newBug.Id);

                int? id = newBug.Id;
                string title = oldBug.Title;
                string description = oldBug.Description;
                string area = oldBug.Area;
                string category = oldBug.Category;
                string product = oldBug.Product;
                string release = oldBug.Release;
                string developers = oldBug.Developers;
                string customers = oldBug.Customers;
                string resolution = oldBug.Resolution;
                string priority = oldBug.Priority;

                if (!string.IsNullOrEmpty(newBug.Title))
                    title = newBug.Title;
                if (!string.IsNullOrEmpty(newBug.Description))
                    description = newBug.Description;
                if (!string.IsNullOrEmpty(newBug.Area))
                    area = newBug.Area;
                if (!string.IsNullOrEmpty(newBug.Category))
                    category = newBug.Category;
                if (!string.IsNullOrEmpty(newBug.Product))
                    product = newBug.Product;
                if (!string.IsNullOrEmpty(newBug.Release))
                    release = newBug.Release;
                if (!string.IsNullOrEmpty(newBug.Developers))
                    developers = newBug.Developers;
                if (!string.IsNullOrEmpty(newBug.Customers))
                    customers = newBug.Customers;
                if (!string.IsNullOrEmpty(newBug.Resolution))
                    resolution = newBug.Resolution;
                if (!string.IsNullOrEmpty(newBug.Priority))
                    priority = newBug.Priority;

                string query = "UPDATE Bugs SET ";

                query += $"Title = @title, ";
                SqlParameter titleParameter = new SqlParameter("title", SqlDbType.VarChar);
                titleParameter.Value = title;

                query += $"Description = @description, ";
                SqlParameter descriptionParameter = new SqlParameter("description", SqlDbType.VarChar);
                descriptionParameter.Value = description;

                query += $"Area = @area, ";
                SqlParameter areaParameter = new SqlParameter("area", SqlDbType.VarChar);
                areaParameter.Value = area;

                query += $"Category = @category, ";
                SqlParameter categoryParameter = new SqlParameter("category", SqlDbType.VarChar);
                categoryParameter.Value = category;

                query += $"Product = @product, ";
                SqlParameter productParameter = new SqlParameter("product", SqlDbType.VarChar);
                productParameter.Value = product;

                query += $"Release = @release, ";
                SqlParameter releaseParameter = new SqlParameter("release", SqlDbType.VarChar);
                releaseParameter.Value = release;

                query += $"Developers = @developers, ";
                SqlParameter developersParameter = new SqlParameter("developers", SqlDbType.VarChar);
                developersParameter.Value = developers;

                query += $"Customers = @customers, ";
                SqlParameter customersParameter = new SqlParameter("customers", SqlDbType.VarChar);
                customersParameter.Value = customers;

                query += $"Resolution = @resolution, ";
                SqlParameter resolutionParameter = new SqlParameter("resolution", SqlDbType.VarChar);
                resolutionParameter.Value = resolution;

                query += $"Priority = @priority ";
                SqlParameter priorityParameter = new SqlParameter("priority", SqlDbType.VarChar);
                priorityParameter.Value = priority;

                query += $"WHERE Id = @id;";
                SqlParameter idParameter = new SqlParameter("id", SqlDbType.VarChar);
                idParameter.Value = id;

                List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    titleParameter,
                    descriptionParameter,
                    areaParameter,
                    categoryParameter,
                    productParameter,
                    releaseParameter,
                    developersParameter,
                    customersParameter,
                    resolutionParameter,
                    priorityParameter,
                    idParameter
                };

                if (execute(query, sqlParameters) != 0)
                    return true;
            }

            return false;
        }

        public bool deleteBug(int? id)
        {
            if (id == null)
                return false;

            string query = "DELETE FROM Bugs WHERE Id = @id";

            SqlParameter idParameter = new SqlParameter("id", SqlDbType.Int);
            idParameter.Value = id;

            sqlParameters = new List<SqlParameter>() { idParameter };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        #endregion

        #region custommers
        public IEnumerable<Customer> getCustomers()
        {
            List<Customer> customers = new List<Customer>();
            Customer customer;

            string query = "SELECT * FROM Customers;";

            SqlCommand cmd = new SqlCommand(query, conn);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                customer = new Customer()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    Company = reader["Company"] as string,
                    Email = reader["Email"] as string,
                };
                customers.Add(customer);
            }

            disconnect();

            return customers;
        }

        private bool checkCustomerEmail(string email)
        {
            string query = $"SELECT COUNT(1) FROM Customers WHERE Email = @email;";

            SqlParameter emailParameter = new SqlParameter("email", SqlDbType.VarChar);
            emailParameter.Value = email;

            sqlParameters = new List<SqlParameter>() { emailParameter };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public bool createCustomer(Customer customer)
        {
            if (customer.Name == null ||
                customer.Company == null ||
                customer.Email == null ||
                checkCustomerEmail(customer.Email))
                return false;

            string query_start = "INSERT INTO Customers (";
            string query_end = "VALUES (";

            query_start += "Name, ";
            query_end += "@Name, ";

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = customer.Name;

            query_start += "Company, ";
            query_end += "@Company, ";

            SqlParameter companyParameter = new SqlParameter("Company", SqlDbType.VarChar);
            companyParameter.Value = customer.Company;

            query_start += "Email) ";
            query_end += "@Email);";

            SqlParameter emailParameter = new SqlParameter("Email", SqlDbType.VarChar);
            emailParameter.Value = customer.Email;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                nameParameter,
                companyParameter,
                emailParameter,
            };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        #endregion

        #region products
        public Product getProduct(int? id)
        {
            Product product = null;

            string query = $"SELECT * FROM Products WHERE Id = @Id;";
            SqlParameter idParameter = new SqlParameter("Id", SqlDbType.Int);
            idParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(idParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                product = new Product()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                };
            }

            disconnect();

            return product;
        }

        public IEnumerable<Product> getProducts()
        {
            List<Product> products = new List<Product>();
            Product product;

            string query = "SELECT * FROM Products;";

            SqlCommand cmd = new SqlCommand(query, conn);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                product = new Product()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string
                };
                products.Add(product);
            }

            disconnect();

            return products;
        }

        private bool checkProductName(string name)
        {
            string query = $"SELECT COUNT(1) FROM Products WHERE Name = @name;";

            SqlParameter nameParameter = new SqlParameter("name", SqlDbType.VarChar);
            nameParameter.Value = name;

            sqlParameters = new List<SqlParameter>() { nameParameter };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public bool createProduct(Product product)
        {
            if (product == null || product.Name == null || checkProductName(product.Name))
                return false;

            string query_start = "INSERT INTO Products (";
            string query_end = "VALUES (";

            query_start += "Name) ";
            query_end += "@Name);";

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = product.Name;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>() { nameParameter };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        #endregion

        #region log
        public bool createLogEntry(int? bugId, string resolution)
        {
            if (bugId == null || resolution == null)
                return false;

            string query_start = "INSERT INTO Log (";
            string query_end = " VALUES (";

            query_start += "DateAndTime, ";
            query_end += "@DateAndTime, ";

            SqlParameter dateAndTimeParameter = new SqlParameter("DateAndTime", SqlDbType.DateTime);
            dateAndTimeParameter.Value = DateTime.Now;

            query_start += "BugId, ";
            query_end += "@BugId, ";

            SqlParameter bugIdParameter = new SqlParameter("BugId", SqlDbType.VarChar);
            bugIdParameter.Value = bugId;

            query_start += "Resolution)";
            query_end += "@Resolution);";

            SqlParameter resolutionParameter = new SqlParameter("Resolution", SqlDbType.VarChar);
            resolutionParameter.Value = resolution;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                dateAndTimeParameter,
                bugIdParameter,
                resolutionParameter
            };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public IEnumerable<Log> getLogsForId(int? id)
        {
            List<Log> logs = new List<Log>();
            Log log;

            string query = "SELECT * FROM Log WHERE BugId = @Id;";

            SqlParameter idParameter = new SqlParameter("Id", SqlDbType.VarChar);
            idParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(idParameter);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                log = new Log()
                {
                    Id = (int)reader["Id"],
                    DateAndTime = (DateTime)reader["DateAndTime"],
                    BugId = (int)reader["BugId"],
                    Resolution = reader["Resolution"] as string
                };
                logs.Add(log);
            }

            disconnect();

            return logs;
        }

        public IEnumerable<Log> getAllLogs()
        {
            List<Log> logs = new List<Log>();
            Log log;

            string query = "SELECT * FROM Log";

            SqlCommand cmd = new SqlCommand(query, conn);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                log = new Log()
                {
                    Id = (int)reader["Id"],
                    DateAndTime = (DateTime)reader["DateAndTime"],
                    BugId = (int)reader["BugId"],
                    Resolution = reader["Resolution"] as string
                };
                logs.Add(log);
            }

            disconnect();

            return logs;
        }

        public void deleteLogs(int? id)
        {
            if (id == null)
                return;

            string query = "DELETE FROM Log WHERE BugId = @bugId";

            SqlParameter bugIdParameter = new SqlParameter("bugId", SqlDbType.Int);
            bugIdParameter.Value = id;

            sqlParameters = new List<SqlParameter>() { bugIdParameter };

            execute(query, sqlParameters);
        }

        #endregion

        #region files
        private bool checkFileNameAndId(int? id, string name)
        {
            string query = $"SELECT COUNT(1) FROM Files WHERE " +
                $"Name = @Name AND " +
                $"BugId = @BugId;";

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = name;

            SqlParameter bugidParameter = new SqlParameter("BugId", SqlDbType.VarChar);
            bugidParameter.Value = id;

            List<SqlParameter> sqlParameters = new List<SqlParameter>() {
                nameParameter,
                bugidParameter
            };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public bool uploadFile(File file)
        {
            if (file == null ||
                file.BugId == null ||
                file.Name == null ||
                file.Contents == null ||
                file.ContentType == null ||
                checkFileNameAndId(file.BugId, file.Name))
                return false;

            string query_start = "INSERT INTO Files (";
            string query_end = "VALUES (";

            query_start += "Contents, ";
            query_end += "@Contents, ";

            SqlParameter contentsParameter = new SqlParameter("Contents", SqlDbType.VarBinary);
            contentsParameter.Value = file.Contents;

            query_start += "BugId, ";
            query_end += "@BugId, ";

            SqlParameter bugidParameter = new SqlParameter("BugId", SqlDbType.Int);
            bugidParameter.Value = file.BugId;

            query_start += "Name, ";
            query_end += "@Name, ";

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = file.Name;

            query_start += "ContentType) ";
            query_end += "@ContentType);";

            SqlParameter contenttypeParameter = new SqlParameter("ContentType", SqlDbType.VarChar);
            contenttypeParameter.Value = file.ContentType;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                contentsParameter,
                bugidParameter,
                nameParameter,
                contenttypeParameter
            };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public File getFile(int? id, string name)
        {
            File file = null;

            string query = $"SELECT * FROM Files WHERE BugId = @BugId AND Name = @Name;";

            SqlParameter bugidParameter = new SqlParameter("BugId", SqlDbType.VarChar);
            bugidParameter.Value = id;

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = name;

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.Add(bugidParameter);
            cmd.Parameters.Add(nameParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                file = new File()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    BugId = (int)reader["BugId"],
                    Contents = (byte[])reader["Contents"],
                    ContentType = reader["ContentType"] as string
                };
            }

            disconnect();

            return file;
        }

        public IEnumerable<File> getFiles(int? id)
        {
            List<File> files = new List<File>();
            File file;

            string query = "SELECT * FROM Files WHERE BugId = @BugId";

            SqlParameter idParameter = new SqlParameter("BugId", SqlDbType.VarChar);
            idParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(idParameter);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                file = new File()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    BugId = (int)reader["BugId"],
                    Contents = (byte[])reader["Contents"],
                    ContentType = reader["ContentType"] as string
                };
                files.Add(file);
            }

            disconnect();

            return files;
        }

        public void deleteFiles(int? id)
        {
            if (id == null)
                return;

            string query = "DELETE FROM Files WHERE BugId = @bugId";

            SqlParameter bugIdParameter = new SqlParameter("bugId", SqlDbType.Int);
            bugIdParameter.Value = id;

            sqlParameters = new List<SqlParameter>() { bugIdParameter };

            execute(query, sqlParameters);
        }

        #endregion

        #region comments
        public bool createComment(Comment comment)
        {
            if (comment.BugId == null || comment.DeveloperId == null)
                return false;

            string query_start = "INSERT INTO Comments (";
            string query_end = " VALUES (";

            query_start += "Created, ";
            query_end += "@Created, ";

            SqlParameter createdParameter = new SqlParameter("Created", SqlDbType.DateTime);
            createdParameter.Value = DateTime.Now;

            query_start += "BugId, ";
            query_end += "@BugId, ";

            SqlParameter bugIdParameter = new SqlParameter("BugId", SqlDbType.Int);
            bugIdParameter.Value = comment.BugId;

            query_start += "DeveloperId, ";
            query_end += "@DeveloperId, ";

            SqlParameter developerIdParameter = new SqlParameter("DeveloperId", SqlDbType.Int);
            developerIdParameter.Value = comment.DeveloperId;

            query_start += "Comment)";
            query_end += "@Comment);";

            SqlParameter commentParameter = new SqlParameter("Comment", SqlDbType.VarChar);
            commentParameter.Value = comment._Comment;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                createdParameter,
                bugIdParameter,
                developerIdParameter,
                commentParameter
            };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public IEnumerable<Comment> getCommentsForId(int? id)
        {
            List<Comment> comments = new List<Comment>();
            Comment comment;

            string query = "SELECT * FROM Comments WHERE BugId = @Id;";

            SqlParameter idParameter = new SqlParameter("Id", SqlDbType.VarChar);
            idParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(idParameter);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comment = new Comment()
                {
                    Id = (int)reader["Id"],
                    BugId = (int)reader["BugId"],
                    Created = (DateTime)reader["Created"],
                    DeveloperId = (int)reader["DeveloperId"],
                    _Comment = reader["Comment"] as string
                };
                comments.Add(comment);
            }

            disconnect();

            return comments;
        }

        public void deleteComments(int? bugId)
        {
            if (bugId == null)
                return;

            string query = "DELETE FROM Comments WHERE BugId = @bugId";

            SqlParameter bugIdParameter = new SqlParameter("bugId", SqlDbType.Int);
            bugIdParameter.Value = bugId;

            sqlParameters = new List<SqlParameter>() { bugIdParameter };

            execute(query, sqlParameters);
        }

        #endregion

        #region tasks
        public bool createTask(Task task)
        {
            if (task.ProductId == null ||
                task.Title == null ||
                task.Start == null ||
                task.End == null ||
                checkTaskTitle(task.Title))
                return false;

            List<string> bugTitles = new List<string>();
            foreach (Bug bug in task.TaskBugs)
            {
                bugTitles.Add(bug.Title);
            }
            string bugs = String.Join(",", bugTitles);

            string query_start = "INSERT INTO Tasks (";
            string query_end = "VALUES (";

            query_start += "ProductId, ";
            query_end += "@ProductId, ";

            SqlParameter productIdParameter = new SqlParameter("ProductId", SqlDbType.Int);
            productIdParameter.Value = task.ProductId;

            query_start += "Title, ";
            query_end += "@Title, ";

            SqlParameter titleParameter = new SqlParameter("Title", SqlDbType.VarChar);
            titleParameter.Value = task.Title;

            query_start += "DateStart, ";
            query_end += "@DateStart, ";

            SqlParameter datestartParameter = new SqlParameter("DateStart", SqlDbType.DateTime);
            datestartParameter.Value = task.Start;

            query_start += "DateEnd, ";
            query_end += "@DateEnd, ";

            SqlParameter dateendParameter = new SqlParameter("DateEnd", SqlDbType.DateTime);
            dateendParameter.Value = task.End;

            query_start += "Bugs) ";
            query_end += "@Bugs);";

            SqlParameter bugsParameter = new SqlParameter("Bugs", SqlDbType.VarChar);
            bugsParameter.Value = bugs;

            string query = query_start + query_end;
            sqlParameters = new List<SqlParameter>()
            {
                productIdParameter,
                titleParameter,
                datestartParameter,
                dateendParameter,
                bugsParameter
            };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        private bool checkTaskTitle(string title)
        {
            string query = $"SELECT COUNT(1) FROM Tasks WHERE Title = @title;";

            SqlParameter titleParameter = new SqlParameter("title", SqlDbType.VarChar);
            titleParameter.Value = title;

            sqlParameters = new List<SqlParameter>() { titleParameter };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public Task getTask(string title)
        {
            Task task = null;
            SimpleTask simpleTask = null;
            string bugs;
            string[] separatedBugs;
            List<string> separatedBugsList;
            List<Bug> bs = new List<Bug>();

            string query = $"SELECT * FROM Tasks WHERE Title = @Title;";
            SqlParameter titleParameter = new SqlParameter("Title", SqlDbType.VarChar);
            titleParameter.Value = title;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(titleParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                simpleTask = new SimpleTask()
                {
                    Id = (int)reader["Id"],
                    ProductId = (int)reader["ProductId"],
                    Title = reader["Title"] as string,
                    Start = (DateTime)reader["DateStart"],
                    End = (DateTime)reader["DateEnd"],
                    TaskBugs = reader["Bugs"] as string
                };
            }

            disconnect();

            if (simpleTask != null)
            {
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
                    bs.Add(getBug(bugTitle));
                }
            }

            task = new Task()
            {
                Id = simpleTask.Id,
                ProductId = simpleTask.ProductId,
                Title = simpleTask.Title,
                Start = simpleTask.Start,
                End = simpleTask.End,
                TaskBugs = bs
            };

            return task;
        }

        public Task getTask(int? id)
        {
            if (id == null)
                return null;

            Task task = null;
            SimpleTask simpleTask = null;
            string bugs;
            string[] separatedBugs;
            List<string> separatedBugsList;
            List<Bug> bs = new List<Bug>();

            string query = "SELECT * FROM Tasks WHERE Id = @id";

            SqlParameter idParameter = new SqlParameter("id", SqlDbType.Int);
            idParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(idParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                simpleTask = new SimpleTask()
                {
                    Id = (int)reader["Id"],
                    ProductId = (int)reader["ProductId"],
                    Title = reader["Title"] as string,
                    Start = (DateTime)reader["DateStart"],
                    End = (DateTime)reader["DateEnd"],
                    TaskBugs = reader["Bugs"] as string
                };
            }

            disconnect();

            if (simpleTask != null)
            {
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
                    bs.Add(getBug(bugTitle));
                }
            }

            task = new Task()
            {
                Id = simpleTask.Id,
                ProductId = simpleTask.ProductId,
                Title = simpleTask.Title,
                Start = simpleTask.Start,
                End = simpleTask.End,
                TaskBugs = bs
            };

            return task;
        }

        public IEnumerable<SimpleTask> getTasksForProductId(int? id)
        {
            List<SimpleTask> tasks = new List<SimpleTask>();
            SimpleTask task;

            string query = "SELECT * FROM Tasks WHERE ProductId = @ProductId;";
            SqlParameter productidParameter = new SqlParameter("ProductId", SqlDbType.Int);
            productidParameter.Value = id;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(productidParameter);

            connect();
            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                task = new SimpleTask()
                {
                    Id = (int)reader["Id"],
                    ProductId = (int)reader["ProductId"],
                    Title = reader["Title"] as string,
                    Start = (DateTime)reader["DateStart"],
                    End = (DateTime)reader["DateEnd"],
                    TaskBugs = reader["Bugs"] as string
                };
                tasks.Add(task);
            }

            disconnect();

            return tasks;
        }

        public IEnumerable<Task> getAllTasks()
        {
            List<SimpleTask> simpleTasks = new List<SimpleTask>();
            List<Task> tasks = new List<Task>();
            Task task;
            SimpleTask simpleTask;
            string bugs;
            string[] separatedBugs;
            List<string> separatedBugsList;
            List<Bug> bs;

            string query = $"SELECT * FROM Tasks;";

            SqlCommand cmd = new SqlCommand(query, conn);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                simpleTask = new SimpleTask()
                {
                    Id = (int)reader["Id"],
                    ProductId = (int)reader["ProductId"],
                    Title = reader["Title"] as string,
                    Start = (DateTime)reader["DateStart"],
                    End = (DateTime)reader["DateEnd"],
                    TaskBugs = reader["Bugs"] as string
                };

                simpleTasks.Add(simpleTask);
            }

            disconnect();

            foreach (SimpleTask simpletask in simpleTasks)
            {
                bugs = simpletask.TaskBugs;
                separatedBugs = bugs.Split(",");
                separatedBugsList = new List<string>(separatedBugs);
                bs = new List<Bug>();

                if (separatedBugs[separatedBugs.Count() - 1] == ", " ||
                    separatedBugs[separatedBugs.Count() - 1] == ",")
                {
                    separatedBugsList.RemoveAt(separatedBugsList.Count() - 1);
                }

                foreach (string bugTitle in separatedBugsList)
                {
                    bs.Add(getBug(bugTitle));
                }

                task = new Task()
                {
                    Id = simpletask.Id,
                    ProductId = simpletask.ProductId,
                    Title = simpletask.Title,
                    Start = simpletask.Start,
                    End = simpletask.End,
                    TaskBugs = bs
                };

                tasks.Add(task);
            }

            return tasks;
        }

        public bool updateTask(Task newtask)
        {
            if (newtask != null && newtask.Title != null && newtask.ProductId != null)
            {
                Task oldtask = getTask(newtask.Title);

                string title = newtask.Title;
                DateTime start = oldtask.Start;
                DateTime end = oldtask.End;
                string bugs = String.Join(",", newtask.TaskBugs.Select(bug => bug.Title));

                if (newtask.Start != null)
                    start = newtask.Start;
                if (newtask.End != null)
                    end = newtask.End;

                string query = "UPDATE Tasks SET ";

                query += $"DateStart = @start, ";
                SqlParameter startParameter = new SqlParameter("start", SqlDbType.DateTime);
                startParameter.Value = start;

                query += $"DateEnd = @end, ";
                SqlParameter endParameter = new SqlParameter("end", SqlDbType.DateTime);
                endParameter.Value = end;

                query += $"Bugs = @bugs ";
                SqlParameter bugsParameter = new SqlParameter("bugs", SqlDbType.VarChar);
                bugsParameter.Value = bugs;

                query += $"WHERE Title = @title;";
                SqlParameter titleParameter = new SqlParameter("title", SqlDbType.VarChar);
                titleParameter.Value = title;

                List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    startParameter,
                    endParameter,
                    bugsParameter,
                    titleParameter
                };

                if (execute(query, sqlParameters) != 0)
                    return true;
            }

            return false;
        }

        public bool deleteTask(int? id)
        {
            if (id == null)
                return false;

            string query = "DELETE FROM Tasks WHERE Id = @id";

            SqlParameter idParameter = new SqlParameter("id", SqlDbType.Int);
            idParameter.Value = id;

            sqlParameters = new List<SqlParameter>() { idParameter };

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        #endregion

        #region Release Plans

        public bool createReleasePlan(ReleasePlan releasePlan)
        {
            if (releasePlan.Name == null ||
                releasePlan.ProductName == null ||
                releasePlan.DateStart == null ||
                releasePlan.DateEnd == null ||
                checkReleaseName(releasePlan.Name, releasePlan.ProductName))
                return false;

            sqlParameters = new List<SqlParameter>();

            string query_start = "INSERT INTO ReleasePlans (";
            string query_end = "VALUES (";

            query_start += "Name, ";
            query_end += "@Name, ";

            SqlParameter nameParameter = new SqlParameter("Name", SqlDbType.VarChar);
            nameParameter.Value = releasePlan.Name;
            sqlParameters.Add(nameParameter);

            
            if (releasePlan.Objectives != null)
            {
                query_start += "Objectives, ";
                query_end += "@Objectives, ";

                SqlParameter objectivesParameter = new SqlParameter("Objectives", SqlDbType.VarChar);
                objectivesParameter.Value = releasePlan.Objectives;
                sqlParameters.Add(objectivesParameter);
            }

            if (releasePlan.Workload != null)
            {
                query_start += "Workload, ";
                query_end += "@Workload, ";

                SqlParameter workloadParameter = new SqlParameter("Workload", SqlDbType.VarChar);
                workloadParameter.Value = releasePlan.Workload;
                sqlParameters.Add(workloadParameter);
            }

            query_start += "DateStart, ";
            query_end += "@DateStart, ";

            SqlParameter datestartParameter = new SqlParameter("DateStart", SqlDbType.DateTime);
            datestartParameter.Value = releasePlan.DateStart;
            sqlParameters.Add(datestartParameter);

            query_start += "DateEnd, ";
            query_end += "@DateEnd, ";

            SqlParameter dateendParameter = new SqlParameter("DateEnd", SqlDbType.DateTime);
            dateendParameter.Value = releasePlan.DateEnd;
            sqlParameters.Add(dateendParameter);

            query_start += "ProductName) ";
            query_end += "@ProductName);";

            SqlParameter productnameParameter = new SqlParameter("ProductName", SqlDbType.VarChar);
            productnameParameter.Value = releasePlan.ProductName;
            sqlParameters.Add(productnameParameter);

            string query = query_start + query_end;

            if (execute(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public bool checkReleaseName(string release, string productName)
        {
            string query = $"SELECT COUNT(1) FROM ReleasePlans WHERE Name = @release AND ProductName = @product;";

            SqlParameter nameParameter = new SqlParameter("release", SqlDbType.VarChar);
            nameParameter.Value = release;

            SqlParameter productnameParameter = new SqlParameter("product", SqlDbType.VarChar);
            productnameParameter.Value = productName;

            sqlParameters = new List<SqlParameter>() { nameParameter, productnameParameter };

            if (executeScalar(query, sqlParameters) != 0)
                return true;

            return false;
        }

        public ReleasePlan getReleasePlan(string release, string productName)
        {
            ReleasePlan releasePlan = null;

            string query = $"SELECT * FROM ReleasePlans WHERE Name = @release AND ProductName = @product;";

            SqlParameter nameParameter = new SqlParameter("release", SqlDbType.VarChar);
            nameParameter.Value = release;

            SqlParameter productnameParameter = new SqlParameter("product", SqlDbType.VarChar);
            productnameParameter.Value = productName;

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(nameParameter);
            cmd.Parameters.Add(productnameParameter);

            connect();

            _logger.LogInformation("executing reader from database");

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                releasePlan = new ReleasePlan()
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    Objectives = reader["Objectives"] as string,
                    Workload = reader["Workload"] as string,
                    DateStart = (DateTime)reader["DateStart"],
                    DateEnd = (DateTime)reader["DateEnd"],
                    ProductName = reader["ProductName"] as string
                };
            }

            disconnect();

            return releasePlan;
        }

        #endregion
    }
}
