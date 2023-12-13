using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace FeeManagement.Models
{
    public class BSCS_Six
    {
        [Key]
        public int BSCS_Six_ID { get; set; }
        public string Six_cn { get; set; }
        public int Six_credit_hours { get; set; }
        public int Six_fees { get; set; }
    }
}