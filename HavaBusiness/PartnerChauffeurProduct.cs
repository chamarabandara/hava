//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HavaBusiness
{
    using System;
    using System.Collections.Generic;
    
    public partial class PartnerChauffeurProduct
    {
        public int Id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> PartnerId { get; set; }
        public Nullable<decimal> HavaPrice { get; set; }
        public Nullable<decimal> MarketPrice { get; set; }
        public Nullable<decimal> PartnerSellingPrice { get; set; }
        public Nullable<bool> IsMarkUp { get; set; }
        public Nullable<decimal> Markup { get; set; }
        public Nullable<decimal> Percentage { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    
        public virtual Partner Partner { get; set; }
        public virtual Product Product { get; set; }
    }
}
