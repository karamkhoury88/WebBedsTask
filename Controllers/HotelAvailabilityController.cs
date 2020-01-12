using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// <param name="useDummyApi">If set to false, the service will use the real Bargains for Couples API, otherwise, it will use a dummy API created by me that ignores both destinationId and code parameters just for testing purpose</param>
        /// <returns>
        /// list of hotels with rates. 
        ///The rates have a Board Type, Value and Rate Type(Per Night or Stay). 
        ///If rate type is Per Night, you must calculate the final price(Value x Number of nights). 
        ///If rate type is Stay, value is already the final price.
        /// </returns>

        [HttpGet]
        public ActionResult<IEnumerable<HotelAvailability>> GetHotelsAvailabilities(long destinationId, int nights, string code, bool useDummyApi = false)
        {
            try
            {
                List<HotelAvailability> result = new List<HotelAvailability>();

                string _ContentType = "application/json";

                string baseUrl = useDummyApi ? _dummyBargainsForCouplesApiBaseUrl : _bargainsForCouplesApiBaseUrl;

                var findBargainRequest = new HttpRequestMessage(HttpMethod.Get,
                    $"{baseUrl}/findBargain?destinationId={destinationId}&nights={nights}&code={code}");

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
                    result = JsonSerializer.DeserializeAsync<List<HotelAvailability>>(responseStream, options).Result;
                    CalculateTotalPrice(hotelAvailabilities: result, nights: nights);   
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ExceptionHandler.HandleException(ex);
            }
        }

        private void CalculateTotalPrice(IEnumerable<HotelAvailability> hotelAvailabilities, int nights)
        { 
            if (hotelAvailabilities != null)
            {
                foreach (var bargin in hotelAvailabilities)
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
