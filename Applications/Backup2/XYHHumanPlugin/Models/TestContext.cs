using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace XYHHumanPlugin.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
    class TestContext : DbContext
    {
        public TestContext(DbContextOptions<TestContext> options)
                : base(options)
        {
        }
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
