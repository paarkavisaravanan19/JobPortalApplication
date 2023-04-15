using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MVC_Assignment_12_04_2023_.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
	public string firstName { get; set; } = null!;
	public string lastName { get; set; } = null!;
}

