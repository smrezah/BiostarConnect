using System.Net.Http.Headers;
using webRestaurantBS.Models;

namespace webRestaurantBS.Services
{
    public class MaherAfzarApiService : IMaherAfzarApiService
    {
        private readonly string _BaseApiUrl;
        private readonly string _USERNAME;
        private readonly string _PASSWORD;
        private string _TOKEN;
        private readonly HttpClient _Client;

        public string Message { get; private set; }

        public MaherAfzarApiService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _BaseApiUrl = configuration["MaherApi:BaseUrl"];
            _USERNAME = configuration["MaherApi:Username"];
            _PASSWORD = configuration["MaherApi:Password"];

            _Client = httpClientFactory.CreateClient();
            _Client.BaseAddress = new Uri(_BaseApiUrl);
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                var data = new Dictionary<string, string>
                {
                    { "UserName", _USERNAME },
                    { "Password", _PASSWORD }
                };

                var response = await _Client.PostAsync("token/auth", new FormUrlEncodedContent(data));

                if (response.IsSuccessStatusCode)
                {
                    if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                    {
                        foreach (var cookie in cookies)
                        {
                            var tokenStartIndex = cookie.IndexOf("access-token=") + 13;
                            var tokenEndIndex = cookie.IndexOf(";", tokenStartIndex);
                            _TOKEN = cookie.Substring(tokenStartIndex, tokenEndIndex - tokenStartIndex);
                            Message = "Success";
                            return true;
                        }
                    }
                }

                Message = "Failed to get token from API.";
                return false;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public async Task<bool> DisconnectAsync()
        {
            try
            {
                var response = await _Client.DeleteAsync("token");
                Message = response.IsSuccessStatusCode ? "Success" : "Failed";
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_TOKEN))
                    await ConnectAsync();

                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var response = await _Client.GetAsync("value");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (result.Contains("User apiuser athenticated!"))
                    {
                        Message = "Success";
                        return true;
                    }
                    Message = result;
                }

                return false;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public async Task<DefaultsInfo> GetDataDefaultsAsync()
        {
            try
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var response = await _Client.GetAsync("restaurant/defaults");

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadAsStringAsync();
                Message = "Success";
                return JsonConvert.DeserializeObject<DefaultsInfo>(result);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public async Task<List<Reservation>> GetInfoReservationAsync(string personalId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var response = await _Client.GetAsync(
                    $"restaurant/reservations?from={fromDate:yyyy-MM-ddTHH:mm:ss}&to={toDate:yyyy-MM-ddTHH:mm:ss}&employeeid={personalId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadAsStringAsync();
                Message = "Success";
                return JsonConvert.DeserializeObject<List<Reservation>>(result);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public async Task<Restaurant> GetReservationDetailsAsync(string restaurantId)
        {
            try
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var response = await _Client.GetAsync($"restaurant/reservations/{restaurantId}/details");

                if (!response.IsSuccessStatusCode)
                    return null;

                var result = await response.Content.ReadAsStringAsync();
                Message = "Success";
                return JsonConvert.DeserializeObject<Restaurant>(result);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        public async Task<bool> CreateReservationAsync(string personalId, DateTime date,
            string restaurantOid, string mealOid,
            string selectedFoodOid, string selectedWayOfServeOid)
        {
            try
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var formData = new Dictionary<string, string>
                {
                    { "EmployeeId", personalId },
                    { "Date", date.ToString("yyyy-MM-ddTHH:mm:ss") },
                    { "RestaurantOid", restaurantOid },
                    { "MealOid", mealOid },
                    { "SelectdFoodOid", selectedFoodOid },
                    { "SeletedWayOfServeOid", selectedWayOfServeOid }
                };

                var response = await _Client.PostAsync("restaurant/reservations",
                    new FormUrlEncodedContent(formData));

                Message = response.IsSuccessStatusCode ? "Success" : "Failed";
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public async Task<bool> DeleteReservationAsync(string reservationId)
        {
            try
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var response = await _Client.DeleteAsync($"restaurant/reservations/{reservationId}");
                Message = response.IsSuccessStatusCode ? "Success" : "Failed";
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return false;
            }
        }

        public List<Reservation> SyncDataReservation(List<Reservation> listReservation, DefaultsInfo defaultsInfo)
        {
            foreach (var reservation in listReservation)
            {
                reservation.MealName =
                    defaultsInfo.Meals.TryGetValue(reservation.MealOid, out var meal)
                    ? meal : string.Empty;

                reservation.RestaurantName =
                    defaultsInfo.Restaurants.TryGetValue(reservation.RestaurantOid, out var rest)
                    ? rest : string.Empty;

                reservation.ServeMethodName =
                    defaultsInfo.ServeMethods.TryGetValue(reservation.ServeMethodOid, out var serve)
                    ? serve : string.Empty;
            }

            return listReservation;
        }

        public async Task<Image> GetImage(string personalId)
        {
            try
            {
                _Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _TOKEN);

                var response = await _Client.GetAsync($"systems/images?id={personalId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                using var ms = new MemoryStream(imageBytes);
                Message = "Success";
                return Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }
    }
}
