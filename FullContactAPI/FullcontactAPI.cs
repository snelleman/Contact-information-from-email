using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace NSFullContactAPI
{
    public interface IFullContactAPI
    {
        Task<FullContactPerson> LookupPersonByEmailAsync(string email);
    }
    public class FullContactAPI : IFullContactAPI
    {
        private readonly string APIKEY;
        private readonly string ADRESS = "https://api.fullcontact.com/v2/person.json?email=";
        private readonly string HEADER = "X-FullContact-APIKey";
        public FullContactAPI(string APIKey)
        {
            APIKEY = APIKey;
        }

        public async Task<FullContactPerson> LookupPersonByEmailAsync(string email)
        {
            string page = ADRESS + email;
            
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(HEADER, APIKEY);
            HttpResponseMessage response = await client.GetAsync(page);
            HttpContent content = response.Content;

            string jsonData = await content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(jsonData);
            int status = (int)jObject.SelectToken("status");
            if (status != 200)
            {
                throw new FullContactAPIException(status);
            }

            FullContactPerson fullContactPerson = JsonConvert.DeserializeObject<FullContactPerson>(jsonData);
            
            return fullContactPerson;
        }
    }

    public class FullContactPerson
    {
        public double likelihood { get; set; }
        public ContactInfo contactInfo { get; set; }
        public List<SocialProfile> socialProfiles { get; set; }

        public class ContactInfo
        {
            public string familyName { get; set; }
            public string givenName { get; set; }
            public string fullName { get; set; }
            public List<Website> websites { get; set; }

            public class Website
            {
                public string url { get; set; }
            }
        }

        public class SocialProfile
        {
            public string typeId { get; set; }
            public string typeName { get; set; }
            public string url { get; set; }
            public string id { get; set; }
            public string username { get; set; }
            public string bio { get; set; }
            public int followers { get; set; }
            public int following { get; set; }
            public string rss { get; set; }
        }

        // Return a string to display the contact person
        public string Display()
        {
            return JsonConvert.SerializeObject(this, 
                                               Formatting.Indented, 
                                               new JsonSerializerSettings
                                               {
                                                    NullValueHandling = NullValueHandling.Ignore
                                               });
        }
    }

    public class FullContactAPIException: Exception
    {
        public readonly int statusCode;
        public FullContactAPIException(int statusCode) : base(MessageFromStatus(statusCode))
        {
            this.statusCode = statusCode;
        }

        // Set a message based on the status code
        private static string MessageFromStatus(int statusCode)
        {
            string message;
            switch (statusCode)
            {
                case 403:
                    message = "The API key is invalid";
                    break;
                default:
                    message = "There was an error";
                    break;
            }
            return message;
        }
    }

}
