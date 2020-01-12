# WebBeds Developers Coding Exercise

Done By Karam Khoury


## Usage
When you run the application on IIS Express, you can use this link to get the hotels availabilities:

[https://localhost:[Port Number]/api/HotelAvailability?destinationId=1&nights=2&code=3&useDummyApi=false](https://localhost:44309/api/HotelAvailability?destinationId=1&nights=2&code=3&useDummyApi=false)

## Parameters
**destinationId (Integer):** Destination ID

**nights (Integer):** Number of nights

**code (String):** Secret authentication code

**useDummyApi (bool):** If set to false, the service will use the real Bargains for Couples API,
otherwise, it will use a dummy API created by me that ignores both destinationId and code parameters **just for testing purpose**




