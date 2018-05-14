using SpartanExtensions.Strings;
using SpartanLogging;
using SpartanSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpartanGoogleApi
{
    public class GoogleApi : IGoogleApi
    {
        private static ISetting _setting;
        private static ILogging _logging;
        private static string _configPath;
        private static List<string> RemovePhrases = new List<string>() { " at ", " on ", " - ", ", " };
        private static List<AutoCompleteResponse> _predictions;
        public string Key { get; set; }
        public string AutoCompleteUrl { get; set; } = "https://maps.googleapis.com/maps/api/place/autocomplete/json?location=0,0&radius=20000000&input={0}&key={1}";
        public string PlaceDetailUrl { get; set; } = "https://maps.googleapis.com/maps/api/place/details/json?placeid={0}&key={1}";
        public string DistanceMatrix { get; set; } = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={0},{1}&destinations=place_id:{2}&mode=walking&key={3}";
        public string Near { get; set; } = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&keyword={3}&key={4}";

        public GoogleApi()
        {
            _setting = new Setting();
            _logging = new Logging();
            _predictions = new List<AutoCompleteResponse>();
            _configPath = _setting.CreateModuleSetting("google");
            Setkey();

        }

        private void Setkey()
        {
            var keyErrorMsg = $"A google configuration file was created in {_configPath}. This configuration is missing a Google Key";
            if (!string.IsNullOrEmpty(_configPath))
            {
                try
                {

                    var cTemp = _configPath.LoadAsJsonType();
                    Key = cTemp.ContainsKey("Key") ? (string)cTemp.GetValue("Key") : "";
                    if (string.IsNullOrWhiteSpace(Key))
                    {
                        _configPath.SaveTo(this.SerializeObject());
                        _logging.Error("SpartanGoogleApi:Setkey", keyErrorMsg);
                    }
                }
                catch (Exception ex)
                {

                    _configPath.SaveTo(this.SerializeObject());
                    _logging.Error("SpartanGoogleApi:Setkey", keyErrorMsg);
                    throw new InvalidOperationException(keyErrorMsg);

                }
            }

        }

        public async Task<List<AutoCompleteResponse>> GetAutoCompleteAsync(string token)
        {
            _predictions = new List<AutoCompleteResponse>();
            if (string.IsNullOrWhiteSpace(token))
            {
                _logging.Error("GoogleApi:GetAutoCompleteAsync", "The search token can not be empty");
                return _predictions;
            }

            if (string.IsNullOrWhiteSpace(Key))
            {
                _logging.Error("GoogleApi:GetAutoCompleteAsync", "The Google Key value is missing");
                return _predictions;
            }

            return await Task.Run(async () =>
            {
                try
                {
                    string url = string.Format(AutoCompleteUrl, token, Key);
                    var rawJson = await url.GetUrlResponseString();
                    var autoCompleteItems = rawJson.DeserializeObject<AutoComplete>();
                    if (autoCompleteItems != null && autoCompleteItems.predictions.Count > 0)
                    {
                        foreach (Prediction prediction in autoCompleteItems.predictions)
                        {
                            string placeOfInterest = GetLeftSplitValue(prediction.description);
                            bool hasDescription = _predictions.Any(x => x.ShortName.Contains(placeOfInterest));
                            if (!hasDescription)
                            {
                                _predictions.Add(new AutoCompleteResponse
                                {
                                    ShortName = placeOfInterest.Trim(),
                                    LongName = prediction.description,
                                    Id = prediction.place_id,
                                    PredictionTypes = prediction.types
                                });
                            }
                        }
                    }
                    return _predictions;
                }
                catch (Exception ex)
                {
                    _logging.Exception("AutoCompleteRepository:Get", ex);
                    _predictions = null;
                }
                return _predictions;
            });
        }

        public Task<List<AutoCompleteResponse>> GetAutoCompleteNearLocationAsync(string lat, string lon, string token)
        {
            throw new NotImplementedException();
        }

        public Task<PlaceDetailResponse> GetPlaceDetailsByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns left side of string if phrases found
        /// in Config.RemovePhrases. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>value or left side of string value</returns>
        private string GetLeftSplitValue(string value)
        {
            foreach (string item in RemovePhrases)
            {
                if (value.Contains(item))
                {
                    return value.Split(item)[0];
                }
            }

            return value;
        }
    }
}
