using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace SpartanGoogleApi.Test
{
    [TestClass]
    public class GoogleApiTest
    {
        [TestMethod]
        public async System.Threading.Tasks.Task TestGetAutoCompleteAsync()
        {
            //localhost:9000/autocomplete?q=publix
            var _google = new GoogleApi();
            var respList = await _google.GetAutoCompleteAsync("publix");
            Assert.IsTrue(respList.Count > 0, "No records found");

        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestGetAutoCompleteNearLocationAsync()
        {
            //localhost:9000/near?q=cvs&lat=27.7702036&lon=-82.6864508
            var lat = "27.7702036";
            var lon = "-82.6864508";
            var token = "cvs";
            var _google = new GoogleApi();
            var respList = await _google.GetAutoCompleteNearLocationAsync(lat, lon, token);
            Assert.IsTrue(respList.Count > 0, "No records found");

        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestGetPlaceDetailsByIdAsync()
        {
            //localhost:9000/place?q=ChIJ5YQQf1GHhYARPKG7WLIaOko
            var id = "ChIJh7E5A2mA3YgR2IsjfOXPfx8";
            var _google = new GoogleApi();
            var dPlace = await _google.GetPlaceDetailsByIdAsync(id);
            Assert.IsTrue((dPlace != null && dPlace.Name != null), "Id was not found");

        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestGetDistanceMatrixByIdAsync()
        {
            //localhost:9000/distance?q=ChIJ5YQQf1GHhYARPKG7WLIaOko&lat=27.7702036&lon=-82.6864508
            var id = "ChIJh7E5A2mA3YgR2IsjfOXPfx8";
            var lat = "27.7702036";
            var lon = "-82.6864508";
            var _google = new GoogleApi();
            var _distance = await _google.GetDistanceMatrixByIdAsync(lat, lon, id);
            Assert.IsTrue((_distance != null && _distance.status != "ok"), "Id was not found");

        }
    }
}
