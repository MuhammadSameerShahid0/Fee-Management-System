using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class BSCS_Details
    {
        [Key]
        public int BSCS_Details_Id  { get; set; }
        public int BSCS_First_ID    { get; set; }
        public int BSCS_Second_ID   { get; set; }
        public int BSCS_Third_ID    { get; set; }
        public int BSCS_Fourth_ID   { get; set; }
        public int BSCS_Fifth_ID    { get; set; }
        public int BSCS_Six_ID      { get; set; }
        public int BSCS_Seven_ID    { get; set; }
        public int BSCS_Eight_ID    { get; set; }

        [NotMapped]
        public string BSCS_First { get; set; }
    }
}