using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ChildernOfTheBible.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1. Build the configuration to read from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Create the DbContextOptionsBuilder
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // 3. Get the connection string from the configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 4. Configure the builder to use SQL Server with the connection string
            builder.UseSqlServer(connectionString);

            // 5. Return a new instance of your DbContext with the configured options
            return new ApplicationDbContext(builder.Options);
        }
    }
}
