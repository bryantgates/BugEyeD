﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugEyeD.Models
{
	public class TicketAttachment
	{
		public int? Id { get; set; }

		public string? Description { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime? Created { get; set; }

		public int? TicketId { get; set; }

		[Required]
		public string? BTUserId { get; set; }

		[NotMapped]
		public IFormFile? FormFile { get; set; }
		public byte[]? FileData { get; set; }
		public string? FileType { get; set; }

		// Navigation properties
		public virtual Ticket? Ticket { get; set; }

		public virtual BTUser? BTUser { get; set; }
	}
}
