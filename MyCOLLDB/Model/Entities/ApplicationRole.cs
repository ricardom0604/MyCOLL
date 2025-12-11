using Microsoft.AspNetCore.Identity;


namespace MyCOLLDB.Model.Entities;

public class ApplicationRole : IdentityRole
{
	public string? Description { get; set; }
}
