using System.ComponentModel.DataAnnotations;
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

		public string Location { get; set; }

		public TFItemType LocationType { get; set; }
	}
}
