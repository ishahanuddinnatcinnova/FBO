using Newtonsoft.Json.Linq;

namespace FBO.ViewModels
{
    public class CreditCardResponse
    {
        public enum StatusCodes
        {
            Success,
            Fail,
            Retry
        }

        public CreditCardResponse(string json)
        {
            var jsonResp = JObject.Parse(json);

            // Get the status code
            if (jsonResp["respstat"] != null)
            {
                var jsonStatusCode = jsonResp["respstat"].Value<string>();

                if (jsonStatusCode == "A") StatusCode = CreditCardResponse.StatusCodes.Success;
                else if (jsonStatusCode == "B") StatusCode = CreditCardResponse.StatusCodes.Retry;
                else StatusCode = CreditCardResponse.StatusCodes.Fail;
            }

            // Get Response
            if (jsonResp["resptext"] != null)
            {
                ResponseText = jsonResp["resptext"].Value<string>();
            }
        }

        public StatusCodes StatusCode { get; set; }
        public string ResponseText { get; set; }
    }
}
