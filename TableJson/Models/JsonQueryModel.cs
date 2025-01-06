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
        public string Name { get; set; }
        public JsonQuery(int id)
        {
            Id = id;
        }
        public JsonQuery(int id, string query, string name)
        {
            Id = id;
            Query = query;
            Name = name;
        }
    }
}
