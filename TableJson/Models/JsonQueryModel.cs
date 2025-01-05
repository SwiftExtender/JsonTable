using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TableJson.Models
{
    public class JsonQuery
    {
        [Key, Required]
        public int Id { get; set; }
        public string Query { get; set; }
        public string? Description { get; set; }
        public JsonQuery(int id)
        {
            Id = id;
        }
        public JsonQuery(int id, string query, string description)
        {
            Id = id;
            Query = query;
            Description = description;
        }
    }
}
