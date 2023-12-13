using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class BSCS_Fifth
    {
        [Key]
        public int BSCS_Fifth_ID { get; set; }
        public string Fifthh_cn { get; set; }
        public int Fifth_credit_hours { get; set; }
        public int Fifth_fees { get; set; }
    }
}