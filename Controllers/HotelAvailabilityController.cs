using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebBedsTask.Models;
using WebBedsTask.Utils;

namespace WebBedsTask.Controllers
{
    [ApiController]
    [Route("api/HotelAvailability")]
    public class HotelAvailabilityController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string _bargainsForCouplesApiBaseUrl = "https://webbedsdevtest.azurewebsites.net/api";
        private const string _dummyBargainsForCouplesApiBaseUrl = "http://demo4413177.mockable.io/api";

        public HotelAvailabilityController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Get the hotels availabilities using bargainsForCouples API
        /// </summary>
        /// <param name="destinationId">Destination id</param>
        /// <param name="nights">Nights count</param>
        /// <param name="code">Auth code</param>
        /// <returns>
        /// list of hotels with rates. 
        ///The rates have a Board Type, Value and Rate Type(Per Night or Stay). 
        ///If rate type is Per Night, you must calculate the final price(Value x Number of nights). 
        ///If rate type is Stay, value is already the final price.
        /// </returns>

        [HttpGet]
        public ActionResult<IEnumerable<Bargain>> GetHotelsAvailabilities(long destinationId, int nights, string code, bool useDummyApi = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    throw new UnauthorizedAccessException();

                List<Bargain> result = new List<Bargain>();

                string _ContentType = "application/json";

                string baseUrl = useDummyApi ? _dummyBargainsForCouplesApiBaseUrl : _bargainsForCouplesApiBaseUrl;
                var findBargainRequest = new HttpRequestMessage(HttpMethod.Get,
                    $"{baseUrl}/findBargain?destinationId={destinationId}&nights={nights}&code={code}");
                //findBargainRequest.Headers.Add("Content-Type", "application/json");
               
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));
                
                var response = client.SendAsync(findBargainRequest).Result;
                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = response.Content.ReadAsStreamAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    result = JsonSerializer.DeserializeAsync<List<Bargain>>(responseStream, options).Result;
                    GetCalculatedBargain(bargins: result, nights: nights);   
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

        private void GetCalculatedBargain(IEnumerable<Bargain> bargins, int nights)
        { 
            if (bargins != null)
            {
                foreach (var bargin in bargins)
                {
                    if (bargin.Rates != null)
                    {
                        foreach (var rate in bargin.Rates)
                        {
                            if (rate.RateType == "PerNight")
                                rate.TotalPrice = nights * rate.Value;
                            else
                                rate.TotalPrice = rate.Value;

                        }
                    }
                }
            }
        }
    }
}
