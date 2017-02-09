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
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace shuttle
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer dispatcherTimer;

        private MapIcon bus1Icon = new MapIcon() { NormalizedAnchorPoint = new Point(0.5, 1) };
        private MapIcon bus2Icon = new MapIcon();
        private Uri busLogoUri = new Uri("ms-appx:///Assets/images/bus.png");
        private Geopoint basicPosition = new Geopoint(new BasicGeoposition()
        {
            Latitude = 49.548156,
            Longitude = 6.045190
        });

        public MainPage()
        {
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            bus1Icon.Image = RandomAccessStreamReference.CreateFromUri(busLogoUri);
            bus2Icon.Image = RandomAccessStreamReference.CreateFromUri(busLogoUri);

            //mapIcon.Location = new Geopoint(new BasicGeoposition()
            //{
            //    Longitude = -1.826189,
            //    Latitude = 51.178840,
            //});
            mapControl.MapElements.Add(bus1Icon);
            mapControl.MapElements.Add(bus2Icon);
            mapControl.ZoomLevel = 11;
            mapControl.Center = basicPosition;
            //map.Children.Add()

        }

        private const string belvalRouteId = "2_1451404159733";
        private const string kirchbergRouteId = "";
        private async void DispatcherTimer_Tick(object sender, object e)
        {
            //query_name=get_closest_vehicle_on_route&user_id=-1&route_id=2_1451404159733&target_index_in_route=3&route_location_sernum=48&container_sernum=85
            //query_name=get_closest_vehicle_on_route&user_id=-1&route_id=2_1451404159733&target_index_in_route=7&route_location_sernum=75&container_sernum=92
            //http://www.iconapis.com/icon_pis_server/VlpData

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //dispatcherTimer.Start();
            await GetInfo();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //dispatcherTimer.Stop();
        }

        private async Task GetInfo()
        {
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"http://www.iconapis.com/");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

                string endpoint = @"/icon_pis_server/VlpData";
                var keyValuePairs = new Dictionary<string, string>()
                {
                    { "query_name", "get_closest_vehicle_on_route" },
                    { "user_id", "-1" },
                    {"route_id",belvalRouteId },
                    {"target_index_in_route","7" },
                    {"route_location_sernum","75" },
                    {"container_sernum","92" },
                };

                try
                {
                    var content = new FormUrlEncodedContent(keyValuePairs);
                    var response = await httpClient.PostAsync(endpoint, content);


                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        //var json = JsonConvert.DeserializeObject(result);
                        var vehicles = JsonObject.Parse(json)["vehiclesList"].GetArray();
                        //vehicles.GetArray()[0].GetObject()["plate"].GetString()
                        for(int i=0;i<vehicles.Count;i++)
                        {
                            var veh = vehicles[i].GetObject();
                            var plateString = veh["plate"].GetString();
                            if (plateString == "NOT FOUND")
                            {
                                bus1Icon.Visible = false;
                            }
                            else
                            {

                                var lat = veh["lastLat"].GetNumber();
                                var lng = veh["lastLng"].GetNumber();
                                bus1Icon.Location = new Geopoint(new BasicGeoposition()
                                {
                                    Latitude = lat,
                                    Longitude = lng
                                });
                                bus1Icon.Visible = true;

                            }

                        }

                        //var vehicles = json.GetType().GetProperty("vehiclesList").GetValue(json);
                        //var plate = vehicles.GetType().GetProperty("plate").GetValue(vehicles);
                        /*if ((string)plate == "NOT FOUND")
                        {
                            bus1Icon.Visible = false;
                        }
                        else
                        {
                            //bus1Icon.Visible = p;
                            var locX = plate.GetType().GetProperty("locationX");
                            var locY = plate.GetType().GetProperty("locationY");
                            //bus1Icon.Location = new Geopoint
                        }*/
                    }
                }
                catch (Exception ex)
                {
                    //Could not connect to server
                    //Use more specific exception handling, this is just an example
                }
            }

            await Task.CompletedTask;
        }
    }
}
