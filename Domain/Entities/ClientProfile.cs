using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ClientProfile
    {

        [Key]
        
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Sex { get; set; }
        public DateTime? Date { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ExternalUrl { get; set; }
        public string InternalUrl { get; set; }
            }
}
