using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsFullStack.Models
{
    public class DeckResponse
    {
        public string deck_id { get; set; }
        public int remaining { get; set; }
        //cards list name has to match the api name
        public List<CardResponse> cards { get; set; }

    }
}
