using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondStealer.Models
{
    internal class UserAccess
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public long userId { get; set; }
        public DateTime timeAccess { get; internal set; }
    }
}
