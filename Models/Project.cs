﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugEyeD.Models
{
	public class Project
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		[Required]
		[Display(Name = "Project Name")]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? Name { get; set; }
		[Required]
		public string? Description { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime Created { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }

		[Display(Name = "Project Priority")]
		public int ProjectPriorityId { get; set; }

		[NotMapped]
		public IFormFile? ImageFormFile { get; set; }
		public byte[]? ImageFileData { get; set; }
		public string? ImageFileType { get; set; }

		public bool Archived { get; set; }

		// Navigation properties
		public virtual Company? Company { get; set; }
		public virtual ProjectPriority? ProjectPriority { get; set; }
		public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
		public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
	}
}
