using Gedcomx.Api.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example2.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var ft = new FamilySearchSDK("username", "password", "applicationKey", "Example2.Core", "1.0.0", Gedcomx.Api.Lite.Environment.Integration);

			// Search for a Person
			var searchString = $" surname:Smith~ givenName:John~";
			var encoded = Uri.EscapeDataString(searchString);
			var searchResult = ft.Get("/platform/tree/search?q=" + encoded, MediaType.X_GEDCOMX_ATOM_JSON).Result;

			var stopCount = 1000;
			var totalFetched = 0;
			while (searchResult != null &&
				(totalFetched <= searchResult.results.Value || totalFetched > stopCount))
			{
				totalFetched = (searchResult.index + searchResult.entries.Count);
				foreach (var e in searchResult.entries)
				{
					var p = e.content.gedcomx.persons[0];
					Console.WriteLine($"{p.id} - {p.display.name} birthDate {p.display.birthDate} birthPlace {p.display.birthPlace}");
				}

				// Advance & get the next search results if there.
				if (searchResult.results > (searchResult.index + searchResult.entries.Count))
				{
					Console.WriteLine($"fetching another. total={totalFetched}");
					searchResult = ft.Get(searchResult.links.next.href.Value, MediaType.X_GEDCOMX_ATOM_JSON).Result;
				}
			}

			Console.ReadLine();
		}
    }
}
