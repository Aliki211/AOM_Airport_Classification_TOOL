using System.Windows;
using System;
using System.Windows.Controls;
using AOM_Airport_Classification.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using AOM_Airport_Classification.ViewModel;
namespace AOM_Airport_Classification
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new AirportViewModel();
        }
        
        private Airport RetrieveAirportFromDatabase(string Oaci)
        {
            Airport airport = null;

            try
            {
                // Establish a connection to the MySQL database
                using (MySqlConnection connection = new MySqlConnection("server=localhost;user=ali;password=Aliki211112@;database=airportdb;"))
                {
                    // Open the database connection
                    connection.Open();

                    // Create a SQL query to retrieve the airport details based on the OACI code
                    string query = "SELECT * FROM Airports WHERE Oaci = @Oaci";

                    // Create a MySqlCommand object with the query and connection
                    MySqlCommand command = new MySqlCommand(query, connection);

                    // Add parameters to the command
                    command.Parameters.AddWithValue("@Oaci", Oaci);

                    // Execute the query and retrieve the result
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Check if there are any rows returned
                        if (reader.Read())
                        {
                            // Create a new Airport object and populate its properties from the database
                            airport = new Airport
                            {
                                Name = reader.GetString("Name"),
                                Oaci = reader.GetString("Oaci"),
                                Iata = reader.GetString("Iata"),
                                NumberOfRunways = reader.GetInt32("NumberOfRunways"),
                                ShortestRunway = reader.GetInt32("ShortestRunway"),
                                LongestRunway = reader.GetInt32("LongestRunway"),
                                Als = (reader.GetString("Als") == "YES"),
                                Rcll = (reader.GetString("Rcll") == "YES"),
                                Rl = (reader.GetString("Rl") == "YES"),
                                Papi = (reader.GetString("Papi") == "YES"),
                                Approach1 = reader.GetString("Approach1"),
                                Approach2 = reader.GetString("Approach2"),
                                HighestMsa = reader.GetInt32("HighestMsa"),
                                CirclingMinimas = reader.GetString("CirclingMinimas"),
                                WxReport = (reader.GetString("WxReport") == "YES"),
                                Atc = (reader.GetString("Atc") == "YES"),
                                MountainousEnvironnement = (reader.GetString("MountainousEnvironnement") == "YES"),
                                SpecialWxPhenomenas = (reader.GetString("SpecialWxPhenomenas") == "YES"),
                                NightOperation = (reader.GetString("NightOperation") == "YES")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle database connection or query errors
                MessageBox.Show($"Error retrieving airport details from the database: {ex.Message}");
            }

            return airport;
        }
        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddAirportWindow addAirportWindow = new AddAirportWindow();
            addAirportWindow.ShowDialog();
        }

        private void Classify_Airport_Click(object sender, RoutedEventArgs e)
        {
            string Oaci = txt_oaci.Text;
            string Name = txt_name.Text;
            MessageBox.Show($"Oaci: {Oaci}");
            Airport airport = RetrieveAirportFromDatabase(Oaci);
            if (airport != null)
            {
                // Pass the retrieved airport details to the ViewModel
                var viewModel = (AirportViewModel)DataContext;
                viewModel.Airport = airport;

                // Classify the airport asynchronously
                viewModel.ClassifyAirport();
                _ = GetAndDisplayLocalTime(Name);
                // Show the classification result
                MessageBox.Show($"The airport class is: {viewModel.Classification}", "Airport classification is complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Airport details not found in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task GetAndDisplayLocalTime(string airportName)
        {
            try
            {
                string apiKey = "AIzaSyAp8uJSaWdj_gah4v6moQEJeZnjfUqvRrc";
                DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
                long unixTimestamp = dateTimeOffset.ToUnixTimeSeconds();

                // Use the Google Geocoding API to get the coordinates of the airport
                string geocodingUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(airportName)}&key={apiKey}";

                using (HttpClient geocodingClient = new HttpClient())
                {
                    HttpResponseMessage geocodingResponse = await geocodingClient.GetAsync(geocodingUrl);
                    geocodingResponse.EnsureSuccessStatusCode();

                    string geocodingContent = await geocodingResponse.Content.ReadAsStringAsync();
                    dynamic geocodingJson = JsonConvert.DeserializeObject(geocodingContent);

                    if (geocodingJson.results.Count > 0)
                    {
                        double latitude = geocodingJson.results[0].geometry.location.lat;
                        double longitude = geocodingJson.results[0].geometry.location.lng;

                        // Use the obtained latitude and longitude to fetch local time from Time Zone API
                        string timezoneUrl = $"https://maps.googleapis.com/maps/api/timezone/json?location={latitude},{longitude}&timestamp={unixTimestamp}&key={apiKey}";

                        using (HttpClient timezoneClient = new HttpClient())
                        {
                            HttpResponseMessage timezoneResponse = await timezoneClient.GetAsync(timezoneUrl);
                            timezoneResponse.EnsureSuccessStatusCode();

                            string timezoneContent = await timezoneResponse.Content.ReadAsStringAsync();
                            dynamic timezoneJson = JsonConvert.DeserializeObject(timezoneContent);

                            // Log the timezone JSON response for debugging
                            Console.WriteLine("Timezone JSON Response:");
                            Console.WriteLine(timezoneContent);

                            // Check if the response contains valid timezone information
                            if (timezoneJson.status == "OK")
                            {
                                // Calculate local time using timezone offset
                                DateTime utcTime = DateTime.UtcNow;
                                TimeSpan timezoneOffset = TimeSpan.FromSeconds(timezoneJson.rawOffset + timezoneJson.dstOffset);
                                DateTime localTime = utcTime.Add(timezoneOffset);

                                // Display the local time and timezone information
                                MessageBox.Show($"Local time at {airportName}: {localTime.ToString()} (UTC{(timezoneOffset.TotalHours >= 0 ? "+" : "")}{timezoneOffset.TotalHours})");
                            }
                            else
                            {
                                MessageBox.Show("Error: Invalid timezone response.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: No results found for the given address.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request errors
                MessageBox.Show($"HTTP request error: {ex.Message}");
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                MessageBox.Show($"JSON parsing error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void OperationalRisk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem != null)
            {
                var viewModel = (AirportViewModel)DataContext;
                var airport = viewModel.Airport;

                string selectedContent = (listBox.SelectedItem as ListBoxItem).Content.ToString();

                airport.OperationalRisk = selectedContent;

            }
        }

       
    }
}