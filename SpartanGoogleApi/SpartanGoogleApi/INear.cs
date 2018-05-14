using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpartanGoogleApi
{
    public interface INear
    {
        Task<List<AutoCompleteResponse>> Get(string lat, string lon, string token);
    }
}
