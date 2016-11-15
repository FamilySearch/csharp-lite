using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gedcomx.Api.Lite
{
	public class FamilySearchSDK
	{
		private HttpClient _client = null;
		private Uri _baseUrl;
		private string _accessToken = null;

		/// <summary>
		/// Access Token for instancing other SDKs without having to authenticate
		/// </summary>
		public string AccessToken { get { return _accessToken; } }

		/// <summary>
		/// Main Constructor taking in credentials and supplying a client to make authenticated calls with.
		/// </summary>
		/// <param name="username">a user name</param>
		/// <param name="password">the password for the user</param>
		/// <param name="applicationKey">the application key, client key or developer key as refered to in other documentation</param>
		/// <param name="applicationName">the application name preferably from the dev registration site, but could be anything header friendly</param>
		/// <param name="version">the version of the application for debugging purposes such as 1.0.0</param>
		/// <param name="environment">the environment such as integration (was sandbox), beta and production</param>
		public FamilySearchSDK(string username, string password, string applicationKey, string applicationName, string version,
			Environment environment = Environment.Integration)
		{
			IDictionary<String, String> formData = new Dictionary<String, String>();
			formData.Add("grant_type", "password");
			formData.Add("username", username);
			formData.Add("password", password);
			formData.Add("client_id", applicationKey);

			GetAccessToken(formData, environment);

			InitClient(applicationName, version, applicationKey, environment);
		}

		/// <summary>
		/// Secondary constructor taking in the authentication token and supplying an internal authenticated client
		/// </summary>
		/// <param name="accessToken">the access token supplied by a another family search sdk instance</param>
		/// <param name="applicationKey">the application key, client key or developer key as refered to in other documentation</param>
		/// <param name="applicationName">the application name preferably from the dev registration site, but could be anything header friendly</param>
		/// <param name="version">the version of the application for debugging purposes such as 1.0.0</param>
		/// <param name="environment">the environment such as integration (was sandbox), beta and production</param>
		public FamilySearchSDK(string accessToken, string applicationKey, string applicationName, string version, 
			Environment environment = Environment.Integration)
		{
			_accessToken = accessToken;
			InitClient(applicationName, version, applicationKey, environment);
		}

		private void InitClient(string applicationName, string version, string applicationKey, Environment environment)
		{
			switch (environment)
			{
				case Environment.Production:
					_baseUrl = new Uri("https://familysearch.org/");
					break;
				case Environment.Beta:
					_baseUrl = new Uri("https://beta.familysearch.org/");
					break;
				case Environment.Integration:
					_baseUrl = new Uri("https://integration.familysearch.org/");
					break;
				default: // Do nothing
					throw new Exception("Unexpected environment");
			}

			var cookieContainer = new CookieContainer();
			_client = new HttpClient() { BaseAddress = _baseUrl };
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

			// Add User Agent Details
			_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(applicationName, version)));
			_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(applicationKey)));
		}

		private static string GetAcceptContentType(MediaType mediaType)
		{
			switch (mediaType)
			{
				case MediaType.APPLICATION_JSON:
					return "application/json";
				case MediaType.X_GEDCOMX_v1_JSON:
					return "application/x-gedcomx-v1+json";
				case MediaType.X_GEDCOMX_RECORDS_v1_JSON:
					return "application/x-gedcomx-records-v1+json";
				case MediaType.X_GEDCOMX_ATOM_JSON:
					return "application/x-gedcomx-atom+json";
				case MediaType.X_FS_v1_JSON:
					return "application/x-fs-v1+json";
				default:
					throw new Exception("Bad MediaType");
			}
		}

		private string SetMediaType(MediaType mediaType)
		{
			var type = GetAcceptContentType(mediaType);
			_client.DefaultRequestHeaders.Accept.Clear();
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(type));
			return type;
		}
	
		private void GetAccessToken(IDictionary<string, string> formData, Environment environment)
		{
			using (var httpClient = new HttpClient())
			{
				using (var content = new FormUrlEncodedContent(formData))
				{
					content.Headers.Clear();
					content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					string url = "";
					switch (environment)
					{
						case Environment.Production:
							//url = "https://integration.familysearch.org/cis-web/oauth2/v3/token";
							url = "https://ident.familysearch.org/cis-web/oauth2/v3/token";
							break;
						case Environment.Beta:
							url = "https://identbeta.familysearch.org/cis-web/oauth2/v3/token";
							break;
						case Environment.Integration:
							url = "https://identint.familysearch.org/cis-web/oauth2/v3/token";
							break;
						default: // Do nothing
							throw new Exception("Unexpected environment");
					}

					var response = httpClient.PostAsync(url, content).Result;

					if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
					{
						var accessToken = JsonConvert.DeserializeObject<IDictionary<string, object>>(response.Content.ReadAsStringAsync().Result);

						if (accessToken.ContainsKey("access_token"))
						{
							_accessToken = accessToken["access_token"] as string;
						}

						if (_accessToken == null && accessToken.ContainsKey("token"))
						{
							//workaround to accommodate providers that were built on an older version of the oauth2 specification.
							_accessToken = accessToken["token"] as string;
						}

						if (_accessToken == null)
						{
							throw new Exception("Illegal access token response: no access_token provided.");
						}
					}
					else
					{
						throw new Exception("Unable to obtain an access token. Check username, password and applicationKey");
					}
				}
			}
		}

		/// <summary>
		/// Adds the specified header and its values into the System.Net.Http.Headers.HttpHeaders collection
		/// </summary>
		/// <param name="name">the name of the value</param>
		/// <param name="value">the value of the name</param>
		public void AddClientHeader(string name, string value)
		{
			_client.DefaultRequestHeaders.Add(name, value);
		}

		/// <summary>
		/// Adds the specified header and its values into the System.Net.Http.Headers.HttpHeaders collection
		/// </summary>
		/// <param name="name">the name of the value</param>
		/// <param name="values">the values of the name</param>
		public void AddClientHeader(string name, IEnumerable<string> values)
		{
			_client.DefaultRequestHeaders.Add(name, values);
		}
		
		/// <summary>
		/// Gets any content as a dynamic from the api route passed in by media type
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>Task containing dynamic object ready for referencing</returns>
		public async Task<dynamic> Get(string apiRoute, MediaType mediaType = MediaType.APPLICATION_JSON)
		{
			SetMediaType(mediaType);
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var response = await _client.GetAsync(url).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject(s);
		}

		/// <summary>
		/// Gets any content as an object T from the api route passed in by media type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>Task containing object T ready for referencing</returns>
		public async Task<T> Get<T>(string apiRoute, MediaType mediaType = MediaType.APPLICATION_JSON)
		{
			SetMediaType(mediaType);
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var response = await _client.GetAsync(url).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(s);
		}

		public async Task<HttpResponseMessage> Head(string apiRoute, MediaType mediaType = MediaType.APPLICATION_JSON)
		{
			SetMediaType(mediaType);
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			return await _client.GetAsync(url).ConfigureAwait(false);
		}

		/// <summary>
		/// Puts any string content from the api route passed in by media type.
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>a dynamic object defined by the apiRoute</returns>
		public async Task<dynamic> Put(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			var response = await _client.PutAsync(url, body).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			if (string.IsNullOrEmpty(s))
			{
				return response; // No content is sent back, so send the response.
			}
			return JsonConvert.DeserializeObject<dynamic>(s);
		}

		/// <summary>
		/// Puts any string content from the api route passed in by media type.
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>an object T defined by the apiRoute</returns>
		public async Task<T> Put<T>(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			var response = await _client.PutAsync(url, body).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(s);
		}

		/// <summary>
		/// Puts any string content from the api route passed in by media type.
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>a dynamic object defined by the apiRoute</returns>
		public async Task<dynamic> Post(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			var response = await _client.PostAsync(url, body).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			if (string.IsNullOrEmpty(s))
			{
				return response; // No content is sent back, so send the response.
			}
			return JsonConvert.DeserializeObject<dynamic>(s);
		}

		/// <summary>
		/// Posts any string content from the api route passed in by media type.
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>an object T defined by the apiRoute</returns>
		public async Task<T> Post<T>(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			var response = await _client.PostAsync(url, body).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(s);
		}

		/// <summary>
		/// Deletes any content from the api route passed in
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <returns></returns>
		public async Task<dynamic> Delete(string apiRoute)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var response = await _client.DeleteAsync(url).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			if (string.IsNullOrEmpty(s))
			{
				return response; // No content is sent back, so send the response.
			}
			return JsonConvert.DeserializeObject<dynamic>(s);
		}

		#region Uncommon Calls

		/// <summary>
		/// (Uncommon) Posts any string content from the api route passed in by media type. 
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>only the response portion of the message</returns>
		public async Task<HttpResponseMessage> Post_GetResponse(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			return await _client.PostAsync(url, body).ConfigureAwait(false);
		}

		/// <summary>
		/// (Uncommon) Posts any string content from the api route passed in by media type. 
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>only the content portion of the message</returns>
		public async Task<HttpResponseMessage> Post_GetContent(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			var response = await _client.PostAsync(url, body).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<dynamic>(s);
		}

		/// <summary>
		/// (Uncommon) Puts any string content from the api route passed in by media type. 
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>only the response portion of the message</returns>
		public async Task<HttpResponseMessage> Put_GetResponse(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			return await _client.PutAsync(url, body).ConfigureAwait(false);
		}

		/// <summary>
		/// (Uncommon) Puts any string content from the api route passed in by media type. 
		/// </summary>
		/// <param name="apiRoute">any family search api route such as /platform/tree/persons/L11X-X11 refer to familysearch.org/developers/docs/api/resources</param>
		/// <param name="content">the string content likely as a JSON string</param>
		/// <param name="mediaType">the media type defining the type of content expected of and enumerable MediaType</param>
		/// <returns>only the content portion of the message</returns>
		public async Task<HttpResponseMessage> Put_GetContent(string apiRoute, string content, MediaType mediaType)
		{
			var url = new Uri(_baseUrl, apiRoute.Replace(_baseUrl.OriginalString, ""));
			var body = new StringContent(content, Encoding.UTF8, SetMediaType(mediaType));
			var response = await _client.PutAsync(url, body).ConfigureAwait(false);
			var s = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<dynamic>(s);
		}

		#endregion
	}
}
