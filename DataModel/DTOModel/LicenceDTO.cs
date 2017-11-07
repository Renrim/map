using System;
using System.Collections;
using System.Collections.Generic;

namespace DataModel
{
    public class LicenceDTO
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