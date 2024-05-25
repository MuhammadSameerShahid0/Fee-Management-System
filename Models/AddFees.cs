using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class AddFees
    {
        [Key]
        public int Add_Fee_Id{ get; set; }
        public int StdId { get; set; }
        public DateTime DOT { get; set; }
        public int Payable_Fees { get; set; }
        public string StdName { get; set; }
        public int StdRollNo { get; set; }
        public string Email { get; set; }
        public string StdFName { get; set; }
        public int Semester { get; set; }
        public int TotalFees { get; set; }
        public int RemainFees { get; set; }

        [NotMapped]
        public string Department { get; set; }
        [NotMapped]
        public int previousRemainFees { get; set; }
    }
}