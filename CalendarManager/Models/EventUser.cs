//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CalendarManager.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventUser
    {
        public string Email { get; set; }
        public Nullable<bool> Approved { get; set; }
        public long Id { get; set; }
    
        public virtual Event Event { get; set; }
    }
}
