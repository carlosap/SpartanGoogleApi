using System.Threading.Tasks;
namespace SpartanGoogleApi
{
    public interface IPlaceDetail
    {
        Task<PlaceDetailResponse> Get(string id);
    }
}
