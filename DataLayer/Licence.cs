//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Licence
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Licence()
        {
            this.Users = new HashSet<Users>();
        }
    
        public int Id { get; set; }
        public string TypeOfLicence { get; set; }
        public Nullable<int> AmountOfLoadedPoints { get; set; }
        public Nullable<int> AmountOfQueries { get; set; }
        public Nullable<int> AmountOfShownPoints { get; set; }
        public Nullable<int> AmountOfOptymalization { get; set; }
        public Nullable<int> AmountOfPointsToOptymalization { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
    }
}
