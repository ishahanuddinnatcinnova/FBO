using FBO.ViewModels;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FBO.Services
{
    public class BillingService
    {
        private static readonly HttpClient client = new HttpClient();

        public static bool ValidateCreditExpiration(string expMonth, string expYear)
        {
            bool result = true;
            DateTime dateTime = DateTime.Now.AddMonths(1);
            string text = dateTime.Month.ToString();
            string text2 = dateTime.Year.ToString();
            string value = text + "/01/" + text2;
            DateTime dateTime2 = default(DateTime);
            dateTime2 = Convert.ToDateTime(value).AddDays(-1.0);
            string value2 = expMonth + "/01/" + expYear;
            DateTime dateTime3 = default(DateTime);
            dateTime3 = Convert.ToDateTime(value2).AddMonths(1);
            if (DateTime.Compare(dateTime3, dateTime2) < 0)
            {
                result = false;
            }

            return result;
        }

        public static CreditCardResponse ChargeCreditCard(string name, string address, string city, string regionOrState, string country, string postal, string ccNumber, string ccExpiry, int cvv, decimal amount, string chargeDescription)
        {
            JObject jObject = new JObject();
            jObject.Add("merchid", (JToken)"496213696883");
            jObject.Add("account", (JToken)ccNumber);
            jObject.Add("expiry", (JToken)ccExpiry);
            jObject.Add("cvv2", (JToken)cvv);
            jObject.Add("amount", (JToken)amount);
            jObject.Add("currency", (JToken)"USD");
            jObject.Add("name", (JToken)name);
            jObject.Add("address", (JToken)address);
            jObject.Add("city", (JToken)city);
            jObject.Add("region", (JToken)regionOrState);
            jObject.Add("country", (JToken)country);
            jObject.Add("postal", (JToken)postal);
            var o = new[]
            {
                new
                {
                    Description = chargeDescription
                }
            };
            jObject.Add("userfields", JToken.FromObject(o));
            jObject.Add("capture", (JToken)"Y");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Basic {" + Convert.ToBase64String(Encoding.ASCII.GetBytes("instantaccept:Yl10Ih7F@8v6p")) + "}");
            StringContent content = new StringContent(jObject.ToString(), Encoding.ASCII, "application/json");
            HttpResponseMessage result = client.PutAsync("https://fts.cardconnect.com:8443/cardconnect/rest/auth", content).Result;
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return new CreditCardResponse(result.Content.ReadAsStringAsync().Result);
            }
            Log.Error("Error calling CardPointe API: " + result.Content.ReadAsStringAsync().Result);
            throw new Exception("Error calling CardPointe API: " + result.Content.ReadAsStringAsync().Result);
        }
    }
}
