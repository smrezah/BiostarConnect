using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using webRestaurantBS.Models;

namespace webRestaurantBS.Services
{
    public interface IMaherAfzarApiService
    {
        string Message { get; }

        Task<bool> ConnectAsync();
        Task<bool> DisconnectAsync();
        Task<bool> TestConnectionAsync();

        Task<DefaultsInfo> GetDataDefaultsAsync();
        Task<List<Reservation>> GetInfoReservationAsync(string personalId, DateTime fromDate, DateTime toDate);
        Task<Restaurant> GetReservationDetailsAsync(string restaurantId);

        Task<bool> CreateReservationAsync(string personalId, DateTime date,
            string restaurantOid, string mealOid,
            string selectedFoodOid, string selectedWayOfServeOid);

        Task<bool> DeleteReservationAsync(string reservationId);

        List<Reservation> SyncDataReservation(List<Reservation> listReservation, DefaultsInfo defaultsInfo);

        Task<Image> GetImage(string personalId);
    }
}