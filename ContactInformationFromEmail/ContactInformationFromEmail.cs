using System;
using NSFullContactAPI;
using System.Threading;
using System.Threading.Tasks;

namespace ContactInformationFromEmail
{
    class ContactInformationFromEmail
    {

        static void Main(string[] args)
        {
            IFullContactAPI api = new FullContactAPI("insert API key");

            Console.WriteLine("Pleas write an email adress to get contact information");
            while (true)
            {
                string email = Console.ReadLine();
                Task < FullContactPerson > lookup = api.LookupPersonByEmailAsync(email);
                AwaitAndWriteContactInfo(lookup);
            }
            
        }

        static async void AwaitAndWriteContactInfo(Task<FullContactPerson> lookup)
        {
            try
            {
                FullContactPerson fullContactPerson = await lookup;
                Console.WriteLine(fullContactPerson.Display());
            }
            catch(FullContactAPIException e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
