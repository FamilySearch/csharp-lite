using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gedcomx.Api.Lite.Tests.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Net;

namespace Gedcomx.Api.Lite.Tests
{
	[TestClass]
	public class PersonTests
	{
		[TestMethod]
		public void PostAPerson()
		{
			var ft = new FamilySearchSDK(Settings.Default.UserName, Settings.Default.Password, Settings.Default.ApplicationKey,
				TestBacking.AppName, TestBacking.AppVersion, Gedcomx.Api.Lite.Environment.Integration);

			// Create a Person
			var gedcomx = new Gx.Gedcomx();
			gedcomx.AddPerson(TestBacking.GetCreateMalePerson());
			var postResults = ft.Post("/platform/tree/persons", JsonConvert.SerializeObject(gedcomx), MediaType.X_GEDCOMX_v1_JSON).Result;

			// Now get the new person.
			string personId = ((string[])postResults.Headers.Location.ToString().Split('/')).Last();
			var response = ft.Get("/platform/tree/persons/" + personId).Result;

			Assert.IsTrue(postResults.StatusCode == HttpStatusCode.Created, "Posting should flag the reply as created");
		}

		[TestMethod]
		public void GetAPerson()
		{
			var ft = new FamilySearchSDK(Settings.Default.UserName, Settings.Default.Password, Settings.Default.ApplicationKey,
				TestBacking.AppName, TestBacking.AppVersion, Gedcomx.Api.Lite.Environment.Integration);

			// Create a Person
			var gedcomx = new Gx.Gedcomx();
			gedcomx.AddPerson(TestBacking.GetCreateMalePerson());
			var postResults = ft.Post("/platform/tree/persons", JsonConvert.SerializeObject(gedcomx), MediaType.X_GEDCOMX_v1_JSON).Result;

			// Now get the new person.
			string personId = ((string[])postResults.Headers.Location.ToString().Split('/')).Last();
			var response = ft.Get("/platform/tree/persons/" + personId).Result;

			Assert.IsNotNull(response.persons, "resonse should contain a person");
		}

		[TestMethod]
		public void DeleteAPerson()
		{
			var ft = new FamilySearchSDK(Settings.Default.UserName, Settings.Default.Password, Settings.Default.ApplicationKey,
				TestBacking.AppName, TestBacking.AppVersion, Gedcomx.Api.Lite.Environment.Integration);

			// Create a Person
			var gedcomx = new Gx.Gedcomx();
			gedcomx.AddPerson(TestBacking.GetCreateMalePerson());
			var postResults = ft.Post("/platform/tree/persons", JsonConvert.SerializeObject(gedcomx), MediaType.X_GEDCOMX_v1_JSON).Result;

			// Now get the new person.
			string personId = ((string[])postResults.Headers.Location.ToString().Split('/')).Last();
			var response = ft.Delete("/platform/tree/persons/" + personId).Result;

			Assert.IsTrue(postResults.StatusCode == HttpStatusCode.NoContent, "Resonse should indicate deletion successfully");
		}

		[TestMethod]
		public void ReadHeadForAPerson()
		{
			var ft = new FamilySearchSDK(Settings.Default.UserName, Settings.Default.Password, Settings.Default.ApplicationKey,
				TestBacking.AppName, TestBacking.AppVersion, Gedcomx.Api.Lite.Environment.Integration);

			// Create a Person
			var gedcomx = new Gx.Gedcomx();
			gedcomx.AddPerson(TestBacking.GetCreateMalePerson());
			var postResults = ft.Post("/platform/tree/persons", JsonConvert.SerializeObject(gedcomx), MediaType.X_GEDCOMX_v1_JSON).Result;

			// Now get the new person.
			string personId = ((string[])postResults.Headers.Location.ToString().Split('/')).Last();
			var response = ft.Head("/platform/tree/persons/" + personId, MediaType.APPLICATION_JSON).Result;

			Assert.IsTrue(response.StatusCode == HttpStatusCode.OK, "Resonse should indicate header retreived successfully");
		}
	}
}
