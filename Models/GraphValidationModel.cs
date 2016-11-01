using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SKEMD_WWW.Models
{
    public class GraphValidationModel
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public DateTime TimeFrom { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public DateTime TimeTo { get; set; }
        public int APKID { get; set; }
    }
}