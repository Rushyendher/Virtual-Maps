using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VirtualMaps.Resources;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Controls;
using Coding4Fun.Toolkit.Controls;
using Windows.Phone.Speech.Synthesis;
using Windows.Phone.Speech.Recognition;
using System.Text;
using Microsoft.Phone.Maps.Toolkit;
using System.Windows.Media;
using System.Windows.Shapes;
using VirtualMaps.Model;
using VirtualMaps.ViewModel;
using System.Collections.ObjectModel;
using SQLite;

namespace VirtualMaps
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<GeoCoordinate> myRouteCoordinates = new List<GeoCoordinate>();
        Geolocator MyGeolocator;
        Geoposition MyGeoPosition;
        RouteQuery myRouteQuery;
        GeocodeQuery mygeocodeQuery;
        InputPrompt searchloc;
        GeocodeQuery mysearchgeoquery;
        SpeechRecognizerUI recog;
        SpeechRecognitionUIResult recogResult;
        SpeechSynthesizer synth;
        InputPrompt searchoffline;
        GeoCoordinate geocordcurr;
        GeoCoordinate geocorddest;
        GeoCoordinate contactnumbercorrd;

        double distanceinmeters;
        int distancerounded;
        string contactlocation;
        string contactname;
        int rows;

        SpeechRecognizerUI recogsrch;
        SpeechRecognitionUIResult recogsrchResult;

        DatabaseHelperClass Db_Helper = new DatabaseHelperClass();
        Contacts currentcontact = new Contacts();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
                //GetCurrentLocation();
        }



        private void GetPhoneContactToMap()
        {
            rows = Db_Helper.TotalRows();

            for (int i = 1; i <= rows; i++)
            {

                currentcontact = Db_Helper.ReadContact(i);
                name.Text = currentcontact.Name;
                loc.Text = currentcontact.Location;

                contactlocation = loc.Text;
                contactname = name.Text;

                mysearchgeoquery = new GeocodeQuery();
                mysearchgeoquery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                mysearchgeoquery.SearchTerm = contactlocation;
                mysearchgeoquery.QueryCompleted += ContactNumberBinding_QueryCompleted;
                mysearchgeoquery.QueryAsync();
                sayingInfo(contactname);
            }
        }

        private void ContactNumberBinding_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
           
            if (e.Error == null)
            {

                foreach (var cor in e.Result)
                {
                    contactnumbercorrd = cor.GeoCoordinate;
                }
                MapLayer layer = new MapLayer();
                MyMaps.Layers.Add(layer);
                Pushpin cntpp = new Pushpin();
                cntpp.Content = contactname;

                MapOverlay overlay = new MapOverlay
                {
                    GeoCoordinate = contactnumbercorrd
                };

                overlay.Content = cntpp;
                cntpp.Tap += cntpp_Tap;
                layer.Add(overlay);
            }
        }


        void cntpp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ContactInfo.xaml?parameter=" + contactname, UriKind.Relative));
        }

        private async void GetCurrentLocation()
        {
            sayingInfo("Please wait, Finding the location");


            MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 50;
            try
            {
                MapLayer layer = new MapLayer();
                MyMaps.Layers.Add(layer);
                Pushpin pp = new Pushpin();
                pp.Content = "My position";

                MyGeoPosition = await MyGeolocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
                geocordcurr = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                myRouteCoordinates.Add(geocordcurr);
                MyMaps.SetView(geocordcurr, 14, MapAnimationKind.Parabolic);

                MapOverlay overlay = new MapOverlay
                {
                    GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude)
                };

                overlay.Content = pp;

                layer.Add(overlay);
                pp.Tap += pp_Tap;

                speakInfoAboutLocation(geocordcurr);
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error :(", MessageBoxButton.OK);
            }
        }

        void pp_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var coordinates = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
            speakInfoAboutLocation(coordinates);
        }

        void speakInfoAboutLocation(GeoCoordinate geocord)
        {
            ReverseGeocodeQuery query = new ReverseGeocodeQuery();
            query.GeoCoordinate = geocord;
            query.QueryCompleted += query_QueryCompleted;
            query.QueryAsync();
        }

        async void query_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cor in e.Result)
            {
                sb.AppendLine("You are in " + cor.Information.Address.City);
                sb.AppendLine("The district is " + cor.Information.Address.District);
                sb.AppendLine("The ppostal code is " + cor.Information.Address.PostalCode);
                sb.AppendLine("The country code is " + cor.Information.Address.Country);
            }

            SpeechSynthesizer talkInfo = new SpeechSynthesizer();
            await talkInfo.SpeakTextAsync(sb.ToString());
        }

        //all commands function callings
        private async void saycmnd_Click(object sender, EventArgs e)
        {
            this.recog = new SpeechRecognizerUI();
            this.recogResult = await recog.RecognizeWithUIAsync();
            if (recogResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                if (recogResult.RecognitionResult.Text == "Where am I?")
                {
                    GetCurrentLocation();
                }
                else if (recogResult.RecognitionResult.Text == "Navigation.")
                {
                    SpeakCommand();
                }
                else if (recogResult.RecognitionResult.Text == "Change my map.")
                {
                    SpeechSynthesizer speechdir = new SpeechSynthesizer();
                    await speechdir.SpeakTextAsync("Which mode you want me to change, Hybrid, Terrain or Road?");
                    ChangeMapMode();
                }
                else if(recogResult.RecognitionResult.Text == "Search place.")
                {
                    SearchPlace();
                }
                else if (recogResult.RecognitionResult.Text == "Search people.")
                {
                    GetPhoneContactToMap();
                }
                else if (recogResult.RecognitionResult.Text == "Call nearby.")
                {
                    MessageBox.Show("The code is in progress...");
                }
                else
                {
                    SpeechSynthesizer speecherr = new SpeechSynthesizer();
                    await speecherr.SpeakTextAsync("Sorry, couldn't find that command");
                }
            }
        }

        //Change map modes
        private async void ChangeMapMode()
        {
            SpeechRecognizerUI recog3 = new SpeechRecognizerUI();
            SpeechRecognitionUIResult recogResult3 = await recog3.RecognizeWithUIAsync();
            if (recogResult3.RecognitionResult.Text == "Hybrid.")
            {
                MyMaps.CartographicMode = MapCartographicMode.Hybrid;
            }
            else if (recogResult3.RecognitionResult.Text == "Terrain.")
            {
                MyMaps.CartographicMode = MapCartographicMode.Terrain;
            }
            else if (recogResult3.RecognitionResult.Text == "Road.")
            {
                MyMaps.CartographicMode = MapCartographicMode.Road;
            }
            else
            {
                sayingInfo("Sorry, we couldn't recognize that mode.");
            }
        }

        //Navigation command function
        async void SpeakCommand()
        {

            SpeechSynthesizer commandnav = new SpeechSynthesizer();
            await commandnav.SpeakTextAsync("Where would you like to navigate?");

            SpeechRecognizerUI recog2 = new SpeechRecognizerUI();
            SpeechRecognitionUIResult recogResult2 = await recog2.RecognizeWithUIAsync();

            mygeocodeQuery = new GeocodeQuery();
            mygeocodeQuery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
            mygeocodeQuery.SearchTerm = recogResult2.RecognitionResult.Text;
            mygeocodeQuery.QueryCompleted += mygeocodeQuery_QueryCompleted;
            mygeocodeQuery.QueryAsync();
        }

        void mygeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {

            if (e.Error == null)
            {
                myRouteQuery = new RouteQuery();
                myRouteCoordinates.Add(e.Result[0].GeoCoordinate);
                myRouteQuery.Waypoints = myRouteCoordinates;
                myRouteQuery.QueryCompleted += myRouteQuery_QueryCompleted;
                myRouteQuery.QueryAsync();
                mygeocodeQuery.Dispose();
            }
        }

        void myRouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route MyRoute = e.Result;
                MapRoute MyMapRoute = new MapRoute(MyRoute);
                MyMaps.AddRoute(MyMapRoute);
                foreach (RouteLeg leg in MyRoute.Legs)
                {
                    foreach (RouteManeuver maneuver in leg.Maneuvers)
                    {
                        directions.Text += maneuver.InstructionText;
                    }
                    sayingInfo(directions.Text);
                }
                myRouteQuery.Dispose();
            }
        }

        //getting the current location
        private void gps_Click(object sender, EventArgs e)
        {
            GetCurrentLocation();
        }

        async void sayingInfo(string command)
        {
            synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(command);
        }

        //offline navigation function
        private void route_Click(object sender, EventArgs e)
        {
            searchloc = new InputPrompt();
            searchloc.Message = "Enter the location";
            searchloc.Completed += searchloc_Completed;
            searchloc.Show();
        }

        void searchloc_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            mygeocodeQuery = new GeocodeQuery();
            mygeocodeQuery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
            mygeocodeQuery.SearchTerm = searchloc.Value;
            mygeocodeQuery.QueryCompleted += mygeocodeQuery_QueryCompleted;
            mygeocodeQuery.QueryAsync();
        }

        //offline search button function
        private void srch_Click(object sender, EventArgs e)
        {
            searchoffline = new InputPrompt();
            searchoffline.Message = "Enter the location";
            searchoffline.Completed += searchoffline_Completed;
            searchoffline.Show();
        }

        void searchoffline_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            mysearchgeoquery = new GeocodeQuery();
            mysearchgeoquery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
            mysearchgeoquery.SearchTerm = searchoffline.Value;
            mysearchgeoquery.QueryCompleted += myofflinesearchgeoquery_QueryCompleted;
            mysearchgeoquery.QueryAsync();
        }

        private async void myofflinesearchgeoquery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                foreach (var cor in e.Result)
                {
                    geocorddest = cor.GeoCoordinate;
                }
                MyMaps.SetView(geocorddest, 15, MapAnimationKind.Parabolic);

                distanceinmeters = geocordcurr.GetDistanceTo(geocorddest);
                distancerounded = Convert.ToInt32(distanceinmeters);
                MessageBox.Show("You are at " + distancerounded.ToString() + " meters from your current position", "Distance", MessageBoxButton.OK);
            }
        }


        private async void mysearchgeoquery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                foreach (var cor in e.Result)
                {
                    geocorddest = cor.GeoCoordinate;
                }
                MyMaps.SetView(geocorddest, 15, MapAnimationKind.Parabolic);
                
                distanceinmeters = geocordcurr.GetDistanceTo(geocorddest);
                distancerounded = Convert.ToInt32(distanceinmeters);
                sayingInfo("You are at " + distancerounded.ToString() + " meters from your current position");

                SpeechSynthesizer takingToDest = new SpeechSynthesizer();
                await takingToDest.SpeakTextAsync("Do you want me to take you there?");

                SpeechRecognizerUI takeme = new SpeechRecognizerUI();
                SpeechRecognitionUIResult takemeResult = await takeme.RecognizeWithUIAsync();

                if (takemeResult.RecognitionResult.Text == "Yes.")
                {
                    AskMeToTakeDest();
                }
                else if (takemeResult.RecognitionResult.Text == "No.")
                {
                    return;
                }
                else
                {
                    sayingInfo("Sorry didn't catch that");
                }

            }
        }

        //Voice search through a command
        private async void SearchPlace()
        {
            SpeechSynthesizer commandsrch = new SpeechSynthesizer();
            await commandsrch.SpeakTextAsync("Which place do you want me to search?");

            recogsrch = new SpeechRecognizerUI();
            recogsrchResult = await recogsrch.RecognizeWithUIAsync();
            if (recogsrchResult.ResultStatus == SpeechRecognitionUIStatus.Succeeded)
            {
                mysearchgeoquery = new GeocodeQuery();
                mysearchgeoquery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                mysearchgeoquery.SearchTerm = recogsrchResult.RecognitionResult.Text;
                mysearchgeoquery.QueryCompleted += mysearchgeoquery_QueryCompleted;
                mysearchgeoquery.QueryAsync();
            }
        }

        //Confiramtion finding the route for a voice search place

        void AskMeToTakeDest()
        {
            try
            {
                mygeocodeQuery = new GeocodeQuery();
                mygeocodeQuery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                mygeocodeQuery.SearchTerm = recogsrchResult.RecognitionResult.Text;
                mygeocodeQuery.QueryCompleted += mygeocodeQuery_QueryCompleted;
                mygeocodeQuery.QueryAsync();
            }
            catch (Exception ex)
            {
                sayingInfo("sorry, out of bounds");
            }

        }

        //Commands information.
        private void allcmnds_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Commands.xaml",UriKind.RelativeOrAbsolute));
        }

        private void saveanum_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SavePhoneNumber.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}