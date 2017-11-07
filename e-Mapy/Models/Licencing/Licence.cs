using System;

namespace eMapy.Models.Licencing
{
    public class Licence
    {
        public int Id { get; set; }
        public string KindOfLicence { get; set; }
        public int? AmountOfLoadedPoints { get; set; }
        public int? AmountOfQueries { get; set; }
        public int? AmountOfShownPoints { get; set; }
        public int? OptymalizationsLimit { get; set; }
        public int? OptymalizationPointsLimit { get; set; }
    }
}