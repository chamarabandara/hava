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
    
    public partial class ProductFeature
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int ProdcutId { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
