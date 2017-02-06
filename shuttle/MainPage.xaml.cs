using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.UI.Xaml.Controls.Maps;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace shuttle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string apiUrl = "http://www.iconapis.com/icon_pis_server/VlpData";

        DispatcherTimer dispatcherTimer;
        private BusPin busPin;
        public MainPage()
        {
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            busPin = new BusPin();
            mapControl.Children.Add(busPin);
            var first = mapControl.Children.First();
            var mapIcon = new MapIcon();
            var imageUri = new Uri("ms-appx:///Assets/StoreLogo.png");
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(imageUri);
            mapIcon.Location = new Geopoint(new BasicGeoposition()
            {
                Longitude = -1.826189,
                Latitude = 51.178840,
            });
            mapControl.MapElements.Add(mapIcon);

            //map.Children.Add()

        }

        private const string belvalRouteId = "2_1451404159733";
        private const string kirchbergRouteId = "";
        private async void DispatcherTimer_Tick(object sender, object e)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                //var response = await hc.PostAsync(url,new StringContent (yourString));
                var request = (HttpWebRequest)WebRequest.Create(apiUrl);

                var keyValuePairs = new Dictionary<string, string>()
                {
                    { "query_name", "get_closest_vehicle_on_route" },
                    { "user_id", "-1" },
                    {"route_id",belvalRouteId },
                    {"target_index_in_route","1" },
                    {"route_location_sernum","46" },
                    {"container_sernum","84" },
                };
                // Fill keyValuePairs

                var content = new FormUrlEncodedContent(keyValuePairs);

                var response = await client.PostAsync(apiUrl, content);
                var result = response.Content.ReadAsStringAsync().Result;

                var json = JsonConvert.DeserializeObject(result);

                var vehicles = json.GetType().GetProperty("vehiclesList").GetValue(json);
                var plate = vehicles.GetType().GetProperty("plate").GetValue(vehicles);
                if ((string)plate == "NOT FOUND")
                {

                }
                else
                {

                }

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            //dispatcherTimer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //dispatcherTimer.Stop();
        }
    }
}
