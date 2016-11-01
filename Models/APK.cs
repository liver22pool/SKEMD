using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKEMD_WWW.Models
{
    public enum Statuses
    {
        repair,
        normal,
        over,
        noconnection,
        inactive
    }
    
    public enum Types
    {
        chem,
        gama,
        gamaANDcontrol,
        radio,
        meteo,
        gamaANDchem
    }

    public class APK
    {
        public int ID { get; set; }
        public String Number { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lan { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public Statuses Status { get; set; }
        public Types Type { get; set; }
        //public List<int> PostSensorIDs { get; set; }
        public List<int> ParameterIDs { get; set; }
        public List<string> IndexNames { get; set; }
        public List<double> IndexValues { get; set; }
        public List<string> IndexValuesName { get; set; }
        public List<bool> IsIndexVisible { get; set; }
        public List<int> IndexIdQuery { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public int LastUpdateDateDate { get; set; }
        public int LastUpdateDateMonth { get; set; }
        public int LastUpdateDateYear { get; set; }
        public int LastUpdateDateHours { get; set; }
        public int LastUpdateDateMinutes { get; set; }
    }
}