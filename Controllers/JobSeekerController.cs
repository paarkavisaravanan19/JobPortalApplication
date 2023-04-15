using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using MVC_Assignment_12_04_2023_.Models;

namespace MVC_Assignment_12_04_2023_.Controllers
{
    public class JobSeekerController : Controller
    {
        private readonly IConfiguration _configuration;
        public JobSeekerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private SqlConnection _connection;
        private List<JobModel> _jobList = new List<JobModel>();

        public bool ShowDiv = false;

        public void Connection()
        {
            string conn = _configuration.GetConnectionString("JobDB");
            _connection = new SqlConnection(conn);
            _connection.Open();
        }
        public IActionResult JobIndex()
        {
            ViewData["ShowDiv"] = ShowDiv;
            return View();
        }
        [HttpPost]
        public IActionResult JobIndex(string search) 
        {
            Connection();
            ShowDiv = true;
            ViewData["ShowDiv"] = ShowDiv;

            string searchQuery = $"SELECT * FROM JobDetailsTable WHERE JobTitle LIKE '{search}%' OR CompanyName LIKE '{search}%' OR JobDescription LIKE '{search}%';";
            Console.WriteLine(searchQuery);
            try
            {
                using(SqlCommand  cmd = new SqlCommand(searchQuery, _connection)) 
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        JobModel jobModel = new JobModel();
                        jobModel.JobId = (int)reader[0];
                        jobModel.JobTitle = reader[1].ToString().Trim();
                        jobModel.Salary = (decimal)reader[2];
                        jobModel.JobDescription = reader[3].ToString().Trim();
                        jobModel.CompanyName = reader[4].ToString().Trim();
                        _jobList.Add(jobModel);
                    }
                    ViewBag.jobList = _jobList;
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View(ViewBag);
        }

    }
}
