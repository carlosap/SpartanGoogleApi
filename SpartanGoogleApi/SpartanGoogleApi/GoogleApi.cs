using SpartanExtensions.Strings;
using SpartanExtensions.Objects;
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
        private static PlaceDetailResponse _placeDetail;
        private static List<string> RemovePhrases = new List<string>() { " at ", " on ", " - ", ", " };
        private static List<AutoCompleteResponse> _predictions;
        public string Key { get; set; }
        public string AutoCompleteUrl { get; set; } = "https://maps.googleapis.com/maps/api/place/autocomplete/json?location=0,0&radius=20000000&input={0}&key={1}";
        public string PlaceDetailUrl { get; set; } = "https://maps.googleapis.com/maps/api/place/details/json?placeid={0}&key={1}";
        public string DistanceMatrix { get; set; } = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&origins={0},{1}&destinations=place_id:{2}&mode=walking&key={3}";
        public string Near { get; set; } = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&keyword={3}&key={4}";
        public static int RadiusInMeter { get; set; } = 1000;

        public GoogleApi()
        {
            _setting = new Setting();
            _logging = new Logging();
            _predictions = new List<AutoCompleteResponse>();
            _configPath = _setting.CreateModuleSetting("google");
            Key = GetGoolgeApiKey();

        }

        /// <summary>
        /// returns google key value from the google.json file
        /// this is part of your setting during the bootstrap process
        /// </summary>
        /// <returns>google api key value</returns>
        private string GetGoolgeApiKey()
        {
            var retVal = string.Empty;
            var keyErrorMsg = $"A google configuration file was created in {_configPath}. This configuration is missing a Google Key";
            if (!string.IsNullOrEmpty(_configPath))
            {
                try
                {

                    var cTemp = _configPath.LoadAsJsonType();
                    retVal = cTemp.ContainsKey("Key") ? (string)cTemp.GetValue("Key") : "";
                    if (string.IsNullOrWhiteSpace(retVal))
                    {
                        _configPath.SaveTo(this.SerializeToJson());
                        _logging.Error("SpartanGoogleApi:GetGoolgeApiKey", keyErrorMsg);
                    }
                }
                catch (Exception ex)
                {
                    
                    _configPath.SaveTo(this.SerializeToJson());
                    _logging.Error("SpartanGoogleApi:GetGoolgeApiKey", keyErrorMsg);
                    retVal = string.Empty;

                }
            }
            return retVal;
        }

        /// <summary>
        /// retrieves auto complete list
        /// ideal for listboxes or inteli.
        /// </summary>
        /// <param name="token">search text</param>
        /// <returns>List of Suggestions from Google</returns>
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

        /// <summary>
        /// returns suggestions by near locations
        /// </summary>
        /// <param name="lat">lat</param>
        /// <param name="lon">lon</param>
        /// <param name="token">search key word</param>
        /// <returns>list of suggestions near by your location</returns>
        public async Task<List<AutoCompleteResponse>> GetAutoCompleteNearLocationAsync(string lat, string lon, string token)
        {
            _predictions = new List<AutoCompleteResponse>();
            if (string.IsNullOrWhiteSpace(lat))
            {
                _logging.Error("GoogleApi:GetAutoCompleteNearLocationAsync", "The lat can not be empty");
                return _predictions;
            }

            if (string.IsNullOrWhiteSpace(Key))
            {
                _logging.Error("GoogleApi:GetAutoCompleteNearLocationAsync", "The lon can not be empty");
                return _predictions;
            }

            if (string.IsNullOrWhiteSpace(Key))
            {
                _logging.Error("GoogleApi:GetAutoCompleteNearLocationAsync", "The token value can not be empty");
                return _predictions;
            }

            return await Task.Run(async () =>
            {
                try
                {
                    _predictions = new List<AutoCompleteResponse>();
                    string url = string.Format(Near, lat, lon, RadiusInMeter, token, Key);
                    var rawJson = await url.GetUrlResponseString();
                    var nearplaces = rawJson.DeserializeObject<Near>();
                    if (nearplaces != null && nearplaces.status == "OK" && nearplaces.results.Count > 0)
                    {
                        foreach (Result nearplace in nearplaces.results)
                        {
                            string placeOfInterest = GetLeftSplitValue(nearplace.name);
                            bool hasDescription = _predictions.Any(x => x.ShortName.Contains(placeOfInterest));
                            if (!hasDescription)
                            {
                                _predictions.Add(new AutoCompleteResponse
                                {
                                    ShortName = placeOfInterest.Trim(),
                                    LongName = nearplace.name,
                                    Id = nearplace.place_id,
                                    PredictionTypes = nearplace.types
                                });
                            }
                        }
                    }
                    return _predictions;
                }
                catch (Exception ex)
                {
                    _logging.Error("GoogleApi:GetAutoCompleteNearLocationAsync", ex.ToString());
                    _predictions = null;
                }
                return _predictions;
            });
        }


        /// <summary>
        /// provides additional details from the autocomplete search id
        /// </summary>
        /// <param name="id">place id</param>
        /// <returns>details</returns>
        public async Task<PlaceDetailResponse> GetPlaceDetailsByIdAsync(string id)
        {
            _placeDetail = new PlaceDetailResponse();
            if (string.IsNullOrWhiteSpace(id))
            {
                _logging.Error("GoogleApi:GetPlaceDetailsByIdAsync", "The id value is missing");
                return _placeDetail;
            }

            return await Task.Run(async () =>
            {

                try
                {

                    _placeDetail = new PlaceDetailResponse();
                    string url = string.Format(PlaceDetailUrl, id, Key);
                    var rawJson = await url.GetUrlResponseString();
                    var placeDetail = rawJson.DeserializeObject<PlaceDetail>();
                    if (placeDetail != null && placeDetail.status == "OK")
                    {
                        _placeDetail.Name = placeDetail.result.name;
                        _placeDetail.Location = placeDetail.result.geometry.location;
                        _placeDetail.Url = placeDetail.result.url;
                        _placeDetail.Website = placeDetail.result.website;
                        _placeDetail.Address = placeDetail.result.formatted_address;
                        _placeDetail.PhoneNumber = placeDetail.result.formatted_phone_number;
                        _placeDetail.LocationTypes = placeDetail.result.types;
                        _placeDetail.isOpen = placeDetail.result.opening_hours.open_now;
                    }

                    return _placeDetail;
                }
                catch (Exception ex)
                {
                    _logging.Error("GoogleApi:GetPlaceDetailsByIdAsync", ex.ToString());
                    _placeDetail = null;
                }
                return _placeDetail;
            });
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
