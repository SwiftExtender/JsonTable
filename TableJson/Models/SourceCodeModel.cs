using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TableJson.Models
{
    //public enum MacrosTypes
    //{
    //    RightClick = 1,
    //    Manually = 2,
    //    Combined = 3,
    //}
    public class Macros
    {
        [Key, Required]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Name { get; set; }
        //public string MacrosType { get; set; }
        public string? SourceCode { get; set; } = "";
        public string? Checksum { get; set; } = "";
        public byte[]? BinaryExecutable { get; set; }
        [NotMapped]
        public bool IsSaved { get; set; } = true; //false = temp, true = in DB
        public Macros(bool isSaved)
        {
            IsSaved = isSaved;
        }
        public Macros(int id, bool isActive, string name)
        {
            Id = id;
            IsActive = isActive;
            Name = name;
        }
    }
    public class HelpContext : DbContext
    {
        public DbSet<Macros>? MacrosTable { get; set; }
        public DbSet<JsonQuery>? JsonQueryTable { get; set; }
        private string DbPath { get; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
        public HelpContext()
        {
            DbPath = System.IO.Path.Join(AppDomain.CurrentDomain.BaseDirectory, "scripts.db");
        }
    }
}