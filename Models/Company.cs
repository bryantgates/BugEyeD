using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace BugEyeD.Models
{
	public class Company
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "Company Name")]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? Name { get; set; }
		[StringLength(1000, ErrorMessage = "The {0} can only be max {1} characters long.", MinimumLength = 0)]
		public string? Description { get; set; }

		[NotMapped]
		public IFormFile? ImageFormFile { get; set; }
		public byte[]? ImageFileData { get; set; }
		public string? ImageFileType { get; set; }

		// Navigation properties
		public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

		public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();

		public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
	}
}