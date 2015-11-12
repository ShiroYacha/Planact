using Microsoft.Data.Entity;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planact.Data.Source
{
    public class SampleData
    {
        public int Id
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }

    public class MyContext : DbContext
    {
        public DbSet<SampleData> SampleData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "test.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

    }
}
