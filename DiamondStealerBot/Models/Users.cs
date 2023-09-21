using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondStealer.Models
{
    internal class Users
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public long userId { get; set; }
        [Required]
        public bool access { get; set; }
        [Required]
        public bool adminAccess { get; set; }
        [Required]
        public bool requestOnAccess { get; set; }
        [Required]
        public string productType { get; set; }
        [Required]
        public string currencyType { get; set; }
        public string photoURL { get; set; }
    }
}
