using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		[Required]
		public string Username { get; set; } = string.Empty;
		[Required]
		public string PasswordHash { get; set; } = string.Empty; 
		public int FailedLoginAttempts { get; set; } = 0;
		public bool IsLockedOut { get; set; } = false;
		public DateTime? LockoutEnd { get; set; }
	}

}
