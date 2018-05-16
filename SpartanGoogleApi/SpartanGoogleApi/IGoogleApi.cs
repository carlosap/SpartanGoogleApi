using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpartanGoogleApi
{
    public interface IGoogleApi
    {
        Task<List<AutoCompleteResponse>> GetAutoCompleteAsync(string token);
        Task<List<AutoCompleteResponse>> GetAutoCompleteNearLocationAsync(string lat, string lon, string token);
        Task<PlaceDetailResponse> GetPlaceDetailsByIdAsync(string id);
        Task<DistanceMatrix> GetDistanceMatrixByIdAsync(string lat, string lon, string id);
    }
}
