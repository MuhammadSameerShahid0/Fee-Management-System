using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FeeManagement.Models
{
    public class Request_Std
    {
        [Key]
        public int RequestId        { get; set; }
        public int SemesterId       { get; set; }
        public int CampusId         { get; set; }
        public int concessionid     { get; set; }
        public string StdEmail      { get; set; }
        public string Description   { get; set; }

        [NotMapped]
        public string Department    { get; set; }
        [NotMapped]
        public string Campus        { get; set; }
        [NotMapped]
        public string concessionn   { get; set; }
    }
}