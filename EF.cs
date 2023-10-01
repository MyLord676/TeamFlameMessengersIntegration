using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace TelegramBotExperiments 
{
    public class DataBaseContext : DbContext
	{
		/*public DataBaseContext()
			: base("server=127.0.0.1;uid=root;pwd=12345;database=test")//"data source=(localdb)\\MSSQLLocalDB;Initial Catalog=userstore;Integrated Security=True;"
		{ }*/
		static readonly string connectionString = "Server=localhost; User ID=root; Password=Alex9429Alex; Database=teamflame";
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
		}
		public DbSet<Session> Sessions { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Procedure> Procedures { get; set; }
	}
	public class User
	{
		[Key]
		public string MediaUserId { get; set; }
		public string token { get; set; }
	}
	public class Session
	{
		[Key]
		public string MediaUserId { get; set; }
		public string? Location { get; set; }
		public TFItemType? LocationType { get; set; }
		public string? ProcedureName { get; set; }
		public int? CurrentState { get; set; }
	}
	[PrimaryKey ("MediaUserId", "State")]
	public class Procedure
	{
		public string MediaUserId { get; set; }
		public int State { get; set; }
		public string? Answer { get; set; }
	}
}