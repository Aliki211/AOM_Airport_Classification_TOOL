using System.Windows;
using System.Windows.Controls;
using AOM_Airport_Classification.Model;
using MySql.Data.MySqlClient;
using AOM_Airport_Classification.ViewModel;
namespace AOM_Airport_Classification
{
    public partial class AddAirportWindow : Window
    {
        public string AirportName { get; private set; }

        public AddAirportWindow()
        {
            InitializeComponent();

            DataContext = new AirportViewModel();
        }

        private void Add_Airport_click(object sender, RoutedEventArgs e)
        {
            string Name = txt_name.Text;
            string Oaci = txt_oaci.Text;
            string Iata = txt_iata.Text;
            int NumberOfRunways = int.Parse(txt_number_of_runways.Text);
            int ShortestRunway = int.Parse(txt_shortest_runway.Text);
            int LongestRunway = int.Parse(txt_longest_runway.Text);
            try
            {
                int HighestMsa = int.Parse(txt_highest_msa.Text);
                int CirclingMinimas = int.Parse(txt_circling_minimas.Text);
                MessageBox.Show($"Name: {Name}, Oaci: {Oaci}, Iata: {Iata}, NumberOfRunways: {NumberOfRunways}, ShortestRunway: {ShortestRunway}, LongestRunway: {LongestRunway}, HighestMsa: {HighestMsa}, CirclingMinimas: {CirclingMinimas}");
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid integer value entered for HighestMsa or CirclingMinimas.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox? chkBox = sender as CheckBox;
            if (chkBox != null)
            {
                var viewModel = (AirportViewModel)DataContext;
                var airport = viewModel.Airport;

                if (chkBox.IsChecked == true)
                {
                    string checkBoxName = chkBox.Name;
                    bool isChecked = chkBox.IsChecked ?? false;
                    switch (chkBox.Name)
                    {
                        case "chkalsYes":
                            airport.Als = true;
                            chkalsNo.IsChecked = !isChecked;
                            break;
                        case "chkalsNo":
                            airport.Als = false;
                            chkalsYes.IsChecked = !isChecked;
                            break;
                        case "chkrcllYes":
                            airport.Rcll = true;
                            chkrcllNo.IsChecked = !isChecked;
                            break;
                        case "chkrcllNo":
                            airport.Rcll = false;
                            chkrcllYes.IsChecked = !isChecked;
                            break;
                        case "chkrlYes":
                            airport.Rl = true;
                            chkrlNo.IsChecked = !isChecked;
                            break;
                        case "chkrlNo":
                            airport.Rl = false;
                            chkrlYes.IsChecked = !isChecked;
                            break;
                        case "chkpapiYes":
                            airport.Papi = true;
                            chkpapiNo.IsChecked = !isChecked;
                            break;
                        case "chkpapiNo":
                            airport.Papi = false;
                            chkpapiYes.IsChecked = !isChecked;
                            break;
                        case "chkwxreportYes":
                            airport.WxReport = true;
                            chkwxreportNo.IsChecked = !isChecked;
                            break;
                        case "chkwxreportNo":
                            airport.WxReport = false;
                            chkwxreportYes.IsChecked = !isChecked;
                            break;
                        case "chkatcYes":
                            airport.Atc = true;
                            chkatcNo.IsChecked = !isChecked;
                            break;
                        case "chkatcNo":
                            airport.Atc = false;
                            chkatcYes.IsChecked = !isChecked;
                            break;
                        case "chkmeYes":
                            airport.MountainousEnvironnement = true;
                            chkmeNo.IsChecked = !isChecked;
                            break;
                        case "chkmeNo":
                            airport.MountainousEnvironnement = false;
                            chkmeYes.IsChecked = !isChecked;
                            break;
                        case "chkswxYes":
                            airport.SpecialWxPhenomenas = true;
                            chkswxNo.IsChecked = !isChecked;
                            break;
                        case "chkswxNo":
                            airport.SpecialWxPhenomenas = false;
                            chkswxYes.IsChecked = !isChecked;
                            break;
                        case "chknightoperationYes":
                            airport.NightOperation = true;
                            chknightoperationNo.IsChecked = !isChecked;
                            break;
                        case "chknightoperationNo":
                            airport.NightOperation = false;
                            chknightoperationYes.IsChecked = !isChecked;
                            break;
                    }
                }
            }
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox? listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                var viewModel = (AirportViewModel)DataContext;
                var airport = viewModel.Airport;

                string selectedContent = (listBox.SelectedItem as ListBoxItem).Content.ToString();

                if (listBox.Name == "lstApproach1")
                {
                    airport.Approach1 = selectedContent;
                }
                else if (listBox.Name == "lstApproach2")
                {
                    airport.Approach2 = selectedContent;
                }
            }
        }

        private void Classify_Airport_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AirportViewModel)DataContext;
            var airport = viewModel.Airport;

            // Classify the airport asynchronously
            viewModel.ClassifyAirport();
            StoreAirportData(airport);

            // No need to await since ClassifyAirport is not asynchronous

            // Show the classification result
            MessageBox.Show($"The airport class is: {viewModel.Classification}", "Airport classification is complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void StoreAirportData(Airport airport)
        {
            try
            {
                // Establish a connection to the MySQL database
                using (MySqlConnection connection = new MySqlConnection("server=localhost;user=ali;password=Aliki211112@;database=airportdb;"))
                {
                    // Open the connection
                    connection.Open();

                    // Create the SQL command to insert airport data into the database
                    string insertQuery = "INSERT INTO Airports (Name, Oaci, Iata, NumberOfRunways, ShortestRunway, LongestRunway, Als, Rcll, Rl, Papi, Approach1, Approach2, HighestMsa, CirclingMinimas, WxReport, Atc, MountainousEnvironnement, SpecialWxPhenomenas, NightOperation) " +
                                          "VALUES (@Name, @Oaci, @Iata, @NumberOfRunways, @ShortestRunway, @LongestRunway, @Als, @Rcll, @Rl, @Papi, @Approach1, @Approach2, @HighestMsa, @CirclingMinimas, @WxReport, @Atc, @MountainousEnvironnement, @SpecialWxPhenomenas, @NightOperation)";

                    // Create MySqlCommand object
                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@Name", txt_name.Text);
                    cmd.Parameters.AddWithValue("@Oaci", txt_oaci.Text);
                    cmd.Parameters.AddWithValue("@Iata", txt_iata.Text);
                    cmd.Parameters.AddWithValue("@NumberOfRunways", int.Parse(txt_number_of_runways.Text));
                    cmd.Parameters.AddWithValue("@ShortestRunway", int.Parse(txt_shortest_runway.Text));
                    cmd.Parameters.AddWithValue("@LongestRunway", int.Parse(txt_longest_runway.Text));
                    cmd.Parameters.AddWithValue("@Als", airport.Als ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@Rcll", airport.Rcll ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@Rl", airport.Rl ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@Papi", airport.Papi ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@Approach1", airport.Approach1);
                    cmd.Parameters.AddWithValue("@Approach2", airport.Approach2);
                    cmd.Parameters.AddWithValue("@HighestMsa", int.Parse(txt_highest_msa.Text));
                    cmd.Parameters.AddWithValue("@CirclingMinimas", int.Parse(txt_circling_minimas.Text));
                    cmd.Parameters.AddWithValue("@WxReport", airport.WxReport ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@Atc", airport.Atc ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@MountainousEnvironnement", airport.MountainousEnvironnement ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@SpecialWxPhenomenas", airport.SpecialWxPhenomenas ? "YES" : "NO");
                    cmd.Parameters.AddWithValue("@NightOperation", airport.NightOperation ? "YES" : "NO");
                    // Execute the command
                    cmd.ExecuteNonQuery();

                    // Display success message or perform other actions
                    MessageBox.Show("Airport data stored successfully in the database.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (MySqlException ex)
            {
                // Handle specific MySQL exceptions
                MessageBox.Show($"MySQL Error: {ex.Number} - {ex.Message}", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                MessageBox.Show($"An error occurred while storing airport data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}