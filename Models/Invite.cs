using System.ComponentModel.DataAnnotations;

namespace BugEyeD.Models
{
	public class Invite
	{
		public int Id { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime InviteDate { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime? JoinDate { get; set; }

		public Guid CompanyToken { get; set; }

		public int CompanyId { get; set; }

		public int? ProjectId { get; set; }

		[Required]
		public string? InvitorId { get; set; }

		public string? InviteeId { get; set; }

		[Required]
		public string? InviteeEmail { get; set; }

		[Required]
		[Display(Name = "First Name")]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? InviteeFirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? InviteeLastName { get; set; }
		[StringLength(500, ErrorMessage = "The {0} can only be max {1} characters long.", MinimumLength = 0)]
		public string? Message { get; set; }

		public bool IsValid { get; set; }

		// Navigation properties
		public virtual Company? Company { get; set; }

		public virtual Project? Project { get; set; }

		public virtual BTUser? Invitor { get; set; }

		public virtual BTUser? Invitee { get; set; }
	}
}
