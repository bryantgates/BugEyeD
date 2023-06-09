﻿using System.ComponentModel.DataAnnotations;

namespace BugEyeD.Models
{
	public class TicketHistory
	{
		public int Id { get; set; }

		public int TicketId { get; set; }
		[Display(Name = "Property Name")]
		public string? PropertyName { get; set; }
		[StringLength(500, ErrorMessage = "The {0} can only be max {1} characters long.", MinimumLength = 0)]
		public string? Description { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime Created { get; set; }

		public string? OldValue { get; set; }

		public string? NewValue { get; set; }

		[Required]
		public string? UserId { get; set; }

		// Navigation properties
		public virtual Ticket? Ticket { get; set; }

		public virtual BTUser? User { get; set; }
	}
}
