using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;

namespace CardsFullStack.Models
{
   //importatnt---some of the nuGet is in microsoft.aspnet.webapi.client

    public class DAL
    {
        public static MySqlConnection DB;


        //============================
        //
        //  HIGHER LEVEL DB HELPER FUNCTIONS
        //
        //from a user perspective, what do we need to do
        //ideally this section is the only place where
        //  DeckResponse and CardResponse classes r used
        // the rest of the app uses Deck and Card Classes
        //
        //===================

        //Initialize a Deck
        //  Get back a deck from the API
        //  Save the deck into our own DB
        //  Draw two cards 
        //  Save those cards in our own DB
        //  Return those cards

        public static async Task<IEnumerable<Card>> InitializeDeck()
        {
            //step 1: call the api for a new shuffled deck. grab the deck id
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://deckofcardsapi.com");
            var response = await client.GetAsync("/api/deck/new/shuffle/?deck_count=1");
            DeckResponse deckresp = await response.Content.ReadAsAsync<DeckResponse>();

            //step 2: save the deck into the db(we have a func that does that)
            //a. create a deck instance from the deckresponse. 
            //b. save that instance into the db
            //c. grab the deck id and save into a local var
            //instead of abc use one of our methoeds

            //add the username later
            Deck mydeck = saveDeck(deckresp.deck_id, "user100");

            //step 3: call the api to get 2 cards for that deck
                     
            response = await client.GetAsync($"https://deckofcardsapi.com/api/deck/{mydeck.deck_id}/draw/?count=2");
            DeckResponse deckresp2 = await response.Content.ReadAsAsync<DeckResponse>();

            //step 4: save those cards into the db (we have a funct that does that)
            foreach(CardResponse cardresp in deckresp2.cards)
            {
                saveCard(mydeck.deck_id, cardresp.image, cardresp.code, "user100");
            }

            //step 5: return that list of card instances (not a list of cardResponse instances)

            //we have a func for that

            return getCardsForDeck(mydeck.deck_id);

        }

        //Get More Cards
        //  Draw 2 cards (from which deck?- this will be a parameter)
        //  SAve those cards in our own DB
        //  Return those cards


        //===========================
        //
        //LOWER LEVEL DB Function Methods

        //the code below has no api calls, therefore no knowledge of the api classes.
        //the code below will not use a deckresponse or CardResponse
        //we call this 'separation of concerns'
        //
        //=============================
        //

        //add a new deck to the deck table
        public static Deck saveDeck(string deck_id, string username) 
        {
            Deck theDeck = new Deck() { deck_id = deck_id, username = username, created_at = DateTime.Now };
            DB.Insert(theDeck);
            return theDeck;
        }
        

        //get the latest deck from the deck table

        public static Deck getLatestDeck()
        {
            //to make this work, we have to also add in the dapper using statement...
           IEnumerable<Deck> result = DB.Query<Deck>("select * from Deck order by created_at desc limit 1");    
            if(result.Count() == 0)
            {
                return null;
            }
            else
            {
              return  result.First();
            }  
        }

        //save a set of cards to the Card table for a particular deck

        public static void saveCards(IEnumerable<Card> cards)
        {

            foreach (Card card in cards)
            {
                DB.Insert(card);
            }
        }
        //save a single card
        public static Card saveCard(string deck_id, string image, string cardcode, string username)
        {
            Card thecard = new Card()
            {
                deck_id = deck_id,
                image = image,
                cardcode = cardcode,
                username = username,
                created_at = DateTime.Now
            };
            DB.Insert(thecard);
            return thecard;
        }

        //read all the current  cards in a particular deck

        public static IEnumerable<Card> getCardsForDeck(string deck_id_param)
        {
            var p = new
            {
                //our var equals the deck_id above (from teh db)
                deck_id = deck_id_param
            };
            IEnumerable<Card> result = DB.Query<Card>("select * from Card where deck_id = @deck_id", p);
            return result;
        }
    }
}
