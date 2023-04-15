using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Graph.Groups.Item;
using Microsoft.IdentityModel.Tokens;
using MVC_Assignment_12_04_2023_.Models;
namespace MVC_Assignment_12_04_2023_.Controllers
{
	public class AdminController : Controller
	{
		private readonly IConfiguration _configuration;
		public AdminController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		private SqlConnection _connection;
		private List<JobModel> _jobList = new List<JobModel>();

		public void Connection()
		{
			string conn = _configuration.GetConnectionString("JobDB");
			_connection = new SqlConnection(conn);
			_connection.Open();
		}

		[HttpGet]
		public IActionResult AdminIndex()
		{
			Connection();
			string selectQuery = "SELECT * FROM JobDetailsTable";

			string opnMode = "Normal";
			string jobIdStr = "";


			if(!Request.Query.IsNullOrEmpty())
			{
				opnMode = Request.Query["opnMode"];
				int jobId = int.Parse(Request.Query["jobId"].ToString());
				if(opnMode == "Delete")
				{
					string deleteJobQuery = $"DELETE FROM JobDetailsTable WHERE JobId = {jobId}";
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(deleteJobQuery, _connection))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
				else if(opnMode == "Update")
				{
					opnMode = "Update";
					jobIdStr = Request.Query["jobId"].ToString();

				}
			}

			using (SqlCommand cmd = new SqlCommand(selectQuery, _connection))
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
				reader.Close();
			}

			ViewBag.jobList = _jobList;
			ViewBag.status = opnMode;
			ViewBag.jobId = jobIdStr;

			return View(ViewBag);
		}

		[HttpPost]
		public IActionResult AdminIndex(JobModel jobModel)
		{
			Connection();


			string updateQuery = $"UPDATE JobDetailsTable SET JobTitle = '{jobModel.JobTitle}'," +
					$"JobDescription = '{jobModel.JobDescription}', Salary = {jobModel.Salary}, CompanyName = '{jobModel.CompanyName}' WHERE JobID = {jobModel.JobId};";

			try
			{
				using(SqlCommand cmd = new SqlCommand(updateQuery, _connection))
				{
					var result = cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return RedirectToAction("AdminIndex", "Admin");

		}


		public IActionResult CreateJob()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateJob(JobModel jobModel)
		{
			Connection();
			string addJobQuery = $"INSERT INTO JobDetailsTable VALUES('{jobModel.JobTitle}',{jobModel.Salary},'{jobModel.JobDescription}','{jobModel.CompanyName}')";

			try
			{
				using(SqlCommand cmd = new SqlCommand(addJobQuery, _connection))
				{
					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return RedirectToAction("AdminIndex");
		}

	}
}
