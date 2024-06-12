using MySql.Data.MySqlClient;

namespace AOM_Airport_Classification.Model
{
    public class Airport
    {
        public string Name { get; set; }
        public string Oaci { get; set; }
        public string Iata { get; set; }
        public int NumberOfRunways { get; set; }
        public int ShortestRunway { get; set; }
        public int LongestRunway { get; set; }
        public bool Als { get; set; }
        public bool Rcll { get; set; }
        public bool Rl { get; set; }
        public bool Papi { get; set; }
        public string Approach1 { get; set; }
        public string Approach2 { get; set; }
        public int HighestMsa { get; set; }
        public string CirclingMinimas { get; set; }
        public bool WxReport { get; set; }
        public bool Atc { get; set; }
        public bool MountainousEnvironnement { get; set; }
        public bool SpecialWxPhenomenas { get; set; }
        public bool NightOperation { get; set; }
        public string OperationalRisk { get; set; }

        public void InsertIntoDatabase()
        {
            using (var connection = new AirportDbConnection().GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO Airports (Name, Oaci, Iata, NumberOfRunways, ShortestRunway, LongestRunway, Als, Rcll, Rl, Papi, Approach1, Approach2, HighestMsa, CirclingMinimas, WxReport, Atc, MountainousEnvironnement, SpecialWxPhenomenas, NightOperation) " +
                                          "VALUES (@Name, @Oaci, @Iata, @NumberOfRunways, @ShortestRunway, @LongestRunway, @Als, @Rcll, @Rl, @Papi, @Approach1, @Approach2, @HighestMsa, @CirclingMinimas, @WxReport, @Atc, @MountainousEnvironnement, @SpecialWxPhenomenas, @NightOperation)";

                    AddParameters(command);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateInDatabase()
        {
            using (var connection = new AirportDbConnection().GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE Airports SET NumberOfRunways = @NumberOfRunways, ShortestRunway = @ShortestRunway, LongestRunway = @LongestRunway, " +
                                          "Als = @Als, Rcll = @Rcll, Rl = @Rl, Papi = @Papi, Approach1 = @Approach1, Approach2 = @Approach2, HighestMsa = @HighestMsa, CirclingMinimas = @CirclingMinimas, " +
                                          "WxReport = @WxReport, Atc = @Atc, MountainousEnvironnement = @MountainousEnvironnement, SpecialWxPhenomenas = @SpecialWxPhenomenas, NightOperation = @NightOperation " +
                                          "WHERE Oaci = @Oaci";

                    AddParameters(command);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameters(MySqlCommand command)
        {
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Oaci", Oaci);
            command.Parameters.AddWithValue("@Iata", Iata);
            command.Parameters.AddWithValue("@NumberOfRunways", NumberOfRunways);
            command.Parameters.AddWithValue("@ShortestRunway", ShortestRunway);
            command.Parameters.AddWithValue("@LongestRunway", LongestRunway);
            command.Parameters.AddWithValue("@Als", Als);
            command.Parameters.AddWithValue("@Rcll", Rcll);
            command.Parameters.AddWithValue("@Rl", Rl);
            command.Parameters.AddWithValue("@Papi", Papi);
            command.Parameters.AddWithValue("@Approach1", Approach1);
            command.Parameters.AddWithValue("@Approach2", Approach2);
            command.Parameters.AddWithValue("@HighestMsa", HighestMsa);
            command.Parameters.AddWithValue("@CirclingMinimas", CirclingMinimas);
            command.Parameters.AddWithValue("@WxReport", WxReport);
            command.Parameters.AddWithValue("@Atc", Atc);
            command.Parameters.AddWithValue("@MountainousEnvironnement", MountainousEnvironnement);
            command.Parameters.AddWithValue("@SpecialWxPhenomenas", SpecialWxPhenomenas);
            command.Parameters.AddWithValue("@NightOperation", NightOperation);
        }
    }

    public class AirportDbConnection
    {
        private readonly string _connectionString;

        public AirportDbConnection()
        {
            _connectionString = "server=localhost;user=ali;password=Aliki211112@;database=airportdb;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}