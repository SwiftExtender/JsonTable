using System.ComponentModel.DataAnnotations;

namespace TableJson.Models
{
    public class JsonQuery
    {
        [Key, Required]
        public int Id { get; set; }
        public string Query { get; set; }
        public string Description { get; set; }
        public JsonQuery(string query, string description)
        {
            Query = query;
            Description = description;
        }
    }
}
