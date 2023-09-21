using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondStealer.Models
{
    internal class Promo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int promo { get; set; }
        [Required]
        public bool status { get; set; }
        [Required]
        public int activatedByUserId { get; set; } = 0;
        [Required]
        public string expireTime { get; set; } = "NONE";
    }
}
