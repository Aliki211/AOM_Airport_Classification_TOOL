using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AOM_Airport_Classification.Model;

namespace AOM_Airport_Classification.ViewModel
{
    public class AirportViewModel : INotifyPropertyChanged
    {
        private Airport _airport;
        private string _classification;

        public Airport Airport
        {
            get { return _airport; }
            set
            {
                _airport = value;
                OnPropertyChanged();
            }
        }
        public AirportViewModel()
        {
            // Initialize the Airport object here
            Airport = new Airport(); // Assuming Airport has a default constructor
        }

        public string Classification
        {
            get { return _classification; }
            set
            {
                _classification = value;
                OnPropertyChanged();
            }
        }

        public void ClassifyAirport()
        {
            if (_airport == null)
                return;

            int runwayScore = CalculateRunwayScore(_airport.ShortestRunway, _airport.LongestRunway);
            int lightingScore = CalculateLightingScore(_airport.Als, _airport.Rcll, _airport.Rl, _airport.Papi);
            int approachScore = CalculateApproachScore(_airport.Approach1, _airport.Approach2);
            int msaScore = CalculateMSAScore(_airport.HighestMsa);
            int circlingScore = CalculateCirclingScore(_airport.CirclingMinimas);
            int wxAtcScore = CalculateWXATCScore(_airport.WxReport, _airport.Atc);
            int specificityScore = CalculateSpecificityScore(_airport.SpecialWxPhenomenas, _airport.MountainousEnvironnement, _airport.NightOperation);
            int OperationalRiskScore = CalculateOperationalRiskScore(_airport.OperationalRisk);
            int totalScore = runwayScore + lightingScore + approachScore + msaScore + circlingScore + wxAtcScore + specificityScore;

            if (totalScore <= 18)
                Classification = "A";
            else if (totalScore <= 23)
                Classification = "B";
            else
                Classification = "C";
        }
        private int CalculateOperationalRiskScore(string OperationalRisk)
        {
            if (OperationalRisk == "High")
                return 6;
            else if (OperationalRisk == "Medium")
                return 3;
            else
                return 0;
        }

        private int CalculateRunwayScore(int shortest, int longest)
        {
            if (shortest <= 800)
                return 5;
            else if (shortest <= 1000)
                return 4;
            else if (shortest <= 1400)
                return 3;
            else if (shortest <= 2000)
                return 2;
            else
                return 1;
        }

        private int CalculateLightingScore(bool als, bool rcll, bool rl, bool papi)
        {
            if (!als && !rcll && !rl && !papi)
                return 5;
            else if (!rcll && !rl && !als)
                return 4;
            else if (!rcll && !als)
                return 3;
            else if (!rcll)
                return 2;
            else
                return 1;
        }

        private int CalculateApproachScore(string approach1, string approach2)
        {
            List<string> twodApproach = new List<string> { "RNP", "vor", "RNAV", "NDB" };
            if (approach1 == "UNK" || approach2 == "UNK")
                return 5;
            else if (approach1 == "VIS" || approach2 == "VIS")
                return 4;
            else if (approach1 != null && approach2 == null)
                return 3;
            else if (twodApproach.Contains(approach1) || twodApproach.Contains(approach2))
                return 2;
            else
                return 1;
        }

        private int CalculateMSAScore(int msa)
        {
            if (msa >= 10000)
                return 5;
            else if (msa >= 8000)
                return 4;
            else if (msa >= 6000)
                return 3;
            else if (msa >= 4000)
                return 2;
            else
                return 1;
        }

        private int CalculateCirclingScore(string circling)
        {
            bool Circling_numerical = long.TryParse(circling, out _);
            if (Circling_numerical)
                return 2;
            else if (circling == "UNK")
                return 5;
            else if (circling == "NIL")
                return 4;
            else
                return 5;
        }

        private int CalculateWXATCScore(bool wxReport, bool atc)
        {
            if (!wxReport && !atc)
                return 5;
            else if (!wxReport && atc || wxReport && !atc)
                return 3;
            else
                return 1;
        }

        private int CalculateSpecificityScore(bool specialWxPhenomena, bool mountainous, bool nightOperation)
        {
            if (specialWxPhenomena && mountainous && nightOperation)
                return 5;
            else if (specialWxPhenomena && mountainous && !nightOperation)
                return 4;
            else if (mountainous && nightOperation)
                return 3;
            else if (mountainous && !nightOperation)
                return 2;
            else
                return 1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}