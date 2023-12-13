using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class Fees_Management
    {
        [Key]
        public int Challan_ID   { get; set; }
        public int Sem_Fee    { get; set; }
        public int Paid_Fee     { get; set; }
    }
}