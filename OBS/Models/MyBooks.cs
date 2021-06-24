using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OBS.Models
{
    public class MyBooks
    {
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        public string UserId { get; set; }

        [Range(0, 10)]
        public int? Rating { get; set; }

        public int? Bought { get; set; }
    }
}
