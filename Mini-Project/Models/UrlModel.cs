using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Models
{
    public class UrlModel
    {
        [Required(ErrorMessage = "Please enter a valid url")]
        public Uri Url { get; set; }
    }
}
