using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace CardsFullStack.Models
{
    [Table("Card")]
    public class Card
    {
        //we let the db create
        [Key]
        public int id { get; set; }
        public string deck_id { get; set;}
        public string image { get; set; }
        public string cardcode { get; set; }
        public string username { get; set; }
        public DateTime created_at { get; set; }


    }
}
