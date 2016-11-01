using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace SKEMD_WWW.Models
{
    public class APKContext
    {
        static List<APK> list = null;
        static int status = 0;
        static int type = 0;
        static string connString = "SKEMDWebConnection";

        private static string GetDatabaseConnection(string name)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            return settings.ConnectionString;
        }
        
        public static List<APK> GetAPKList()
        {
            if (list == null)
                GenerateAPKList();
            return list;
        }

        public static List<APK> GenerateAPKList()
        {
            //if (list == null)
            {
                list = new List<APK>();

                using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
                {
                    SqlCommand command = new SqlCommand("SELECT * FROM Posts", connection);
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        return list;
                    }
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        APK apk = new APK();
                        apk.ID = reader.GetInt32(0);
                        apk.Number = reader.GetString(4);
                        apk.Name = reader.GetString(5);
                        apk.Lat = reader.GetDouble(8);
                        apk.Lan = reader.GetDouble(9);
                        /*apk.Status = (Statuses)status;//зв≥дки т€гнути?????
                        status++; 
                        if (status == 4) 
                            status = 0;*/
                        int st = (int)reader["PostStatus"];
                        if (st == 1)//active
                            apk.Status = Statuses.normal;
                        else if (st == 0)
                            apk.Status = Statuses.inactive;
                        else if (st == 2)
                            apk.Status = Statuses.repair;

                        apk.Description = reader.GetString(6);
                        /*apk.Type = (Types)type; //зв≥дки т€гнути?????
                        type++;
                        if (type == 6)
                            type = 0;*/
                        
                        if (apk.ID == 90 || apk.ID == 91 || apk.ID == 95 || apk.ID == 99)
                            apk.Type = Types.chem;
                        else if (apk.ID == 92)
                            apk.Type = Types.radio;
                        else if (apk.ID == 93)
                            apk.Type = Types.gamaANDcontrol;
                        else if (apk.ID == 94 || apk.ID == 97 || apk.ID == 100 || apk.ID == 101)
                            apk.Type = Types.gama;
                        else if (apk.ID == 96)
                            apk.Type = Types.meteo;
                        else if (apk.ID == 98)
                            apk.Type = Types.gamaANDchem;

                        apk.ParameterIDs = new List<int>();
                        apk.IndexNames = new List<string>();
                        apk.IndexValues = new List<double>();
                        apk.IndexValuesName = new List<string>();
                        apk.IsIndexVisible = new List<bool>();
                        apk.IndexIdQuery = new List<int>();
                        apk.IsIndexVisible.Add(true);//назва поста
                        apk.IsIndexVisible.Add(true);//номер поста
                        
                        list.Add(apk);
                    }
                    reader.Close();

                    //connection.Close();
                    //connection.Open();

                    for (int i = 0; i < list.Count; i++) //список параметр≥в
                    {
                    
                        /*command.CommandText = "SELECT " + "SensorOutput.ParameterId, Parameters.ParameterName, Parameters.ParameterUnit, DataOutput.Value " +//, DataOutput.VerifyDate " +
                                                "FROM PostSensors " + 
                                                "left join SensorOutput on PostSensors.SensorId = SensorOutput.SensorId " +
                                                "inner join Parameters on SensorOutput.ParameterId = Parameters.ParameterId " +
                                                "left join DataOutput on PostSensors.PSId = DataOutput.IdPS AND SensorOutput.ParameterId = DataOutput.IdSP " + 
                                                "Where PostSensors.PostId = " + list[i].ID + 
                                                " ORDER BY VerifyDate DESC";*/

                        /*command.CommandText = "SELECT t.IdSP, ParameterName, ParameterUnit, t.Value " +
                                                "FROM DataOutput t " +
                                                "INNER JOIN Parameters on t.IdSP = ParameterId " +
                                                "INNER JOIN PostSensors ON t.IdPS = PSId " +
                                                "INNER JOIN ( " +
                                                "    SELECT IdSP, MAX(VerifyDate) AS MaxDate " +
                                                "    FROM DataOutput " +
                                                "    WHERE " +
                                                "    [VerifyAuto] = 1 AND " +
                                                "    [Verify] = 1 " +
                                                "    GROUP BY IdSP " +
                                                ") tm ON t.IdSP = tm.IdSP and t.VerifyDate = tm.MaxDate " +
                                                "where PostId = " + list[i].ID;*/

                        command.CommandText = "SELECT dt.IdSP, ParameterName, ParameterUnit, dt.Value, dt.IdQuery, VerifyDate " +
                                                "FROM DataOutput dt " +
                                                "    INNER JOIN PostSensors ON dt.IdPS = PSId " +
                                                "    INNER JOIN Parameters ON dt.IdSP = ParameterId " +
                                                "    INNER JOIN SensorTypes ON SensorId = TypeId " +
                                                "    INNER JOIN " +
                                                "    ( " +
                                                "SELECT IdSP, MAX(VerifyDate) As MaxDate " +
                                                "FROM DataOutput td " +
                                                "INNER JOIN PostSensors ON td.IdPS = PSId " +
                                                "WHERE PostId=" + list[i].ID +
                                                " GROUP BY IdSP " +
                                                "    ) ddt ON dt.IdSP = ddt.IdSP and dt.VerifyDate=ddt.MaxDate " +
                                                "WHERE PostId=" + list[i].ID +
                                                " ORDER BY VerifyDate DESC";

                        reader = command.ExecuteReader();
                        List<DateTime> dts = new List<DateTime>();
                        while (reader.Read()) //список датчик≥в
                        {
                            //var PSId = reader.GetInt32(0);
                            var SPId = reader.GetInt32(0);
                            if (/*(list[i].PostSensorIDs.Contains(PSId) == false) || */(list[i].ParameterIDs.Contains(SPId) == false))
                            {
                                /*if (list[i].PostSensorIDs.Contains(PSId) == false)//no sensor
                                    list[i].PostSensorIDs.Add(PSId);*/
                                //if (list[i].ParameterIDs.Contains(SPId) == false)//no sensor
                                    list[i].ParameterIDs.Add(SPId);
                                var indexName = reader.GetString(1);
                                var indexUnit = reader.GetString(2);
                                double indexVal = 0;
                                if (!reader.IsDBNull(3))
                                    indexVal = reader.GetDouble(3);
                                int indexIdQuery = reader.GetInt32(4);
                                DateTime dt = reader.GetDateTime(5);
                                //if (list[i].IndexNames.Contains(indexName) == false)
                                {
                                    list[i].IndexNames.Add(indexName);
                                    list[i].IndexValues.Add(indexVal);
                                    list[i].IndexValuesName.Add(indexUnit);
                                    list[i].IsIndexVisible.Add(true);
                                    list[i].IndexIdQuery.Add(indexIdQuery);
                                    dts.Add(dt);
                                }
                            }
                        }
                        if (dts.Count > 0)
                        {
                            DateTime dt = dts.Max();
                            list[i].LastUpdateDateDate = dt.Day;
                            list[i].LastUpdateDateMonth = dt.Month;
                            list[i].LastUpdateDateYear = dt.Year;
                            list[i].LastUpdateDateHours = dt.Hour;
                            list[i].LastUpdateDateMinutes = dt.Minute;
                        }
                        reader.Close();
                    }
                }
            }

            return list;
        }

        public static List<List<string>> GetListOfParameterValuesReport(int PostId, List<int> list, string day, string month, string year)
        {
            List<List<string>> result = new List<List<string>>();
            using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    return result;
                }
                SqlCommand command = new SqlCommand("", connection);
                bool[] FilledDates = new bool[24];
                foreach (var item in list)
                {
                    command.CommandText = "SELECT PostSensors.PSId, SensorOutput.ParameterId, DataOutput.Value, DataOutput.VerifyDate " +
                                                    "FROM PostSensors " +
                                                    "left join SensorOutput on PostSensors.SensorId = SensorOutput.SensorId " +
                                                    "inner join Parameters on SensorOutput.ParameterId = Parameters.ParameterId " +
                                                    "left join DataOutput on PostSensors.PSId = DataOutput.IdPS AND SensorOutput.ParameterId = DataOutput.IdSP " +
                                                    "Where PostSensors.PostId = " + PostId + " AND DataOutput.IdSP = " + item +
                                                    "AND (year(VerifyDate)='" + year + "') AND " +
                                                    "(month(VerifyDate)='" + month + "') AND " +
                                                    "(day(VerifyDate)='" + day + "') "+
                                                    " ORDER BY VerifyDate DESC";

                    SqlDataReader reader = command.ExecuteReader();
                    List<string> values = new List<string>();
                    List<DateTime> dts = new List<DateTime>();
                    
                    for (int i = 0; i < 24; i++)
                    {
                        values.Add("-");
                        FilledDates[i] = false;
                    }
                    while (reader.Read())
                    {
                        DateTime dt = reader.GetDateTime(3);
                        double value = reader.GetDouble(2);
                        if (!FilledDates[dt.Hour])// дл€ такоњ години ще не заповнювали значенн€
                        {
                            FilledDates[dt.Hour] = true;
                            values[dt.Hour] = value.ToString();
                        }
                        
                        
                        /*if (dts.Count == 0)
                        {
                            dts.Add(dt);
                            values.Add(value);
                            continue;
                        }
                        if (dt.AddHours(1) < dts[dts.Count - 1])
                        {
                            dts.Add(dt);
                            values.Add(value);
                            if (dts.Count > 23)
                                break;
                        }*/
                    }
                    reader.Close();
                    result.Add(values);
                }
            }
            return result;
        }

        public static Dictionary<List<DateTime>, List<double>> GetParametersValuesGraph(int PostId, DateTime From, DateTime To, List<int> list)
        {
            Dictionary<List<DateTime>, List<double>> result = new Dictionary<List<DateTime>, List<double>>();
            using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    return result;
                }
                SqlCommand command = new SqlCommand("", connection);
                string sFrom = String.Format("{0:dd/MM/yyyy}", From);
                string sTo = String.Format("{0:dd/MM/yyyy}", To);
                foreach (var item in list)
                {
                    command.CommandText = "SELECT DataOutput.Value, DataOutput.VerifyDate " +
                                                    "FROM PostSensors " +
                                                    "left join SensorOutput on PostSensors.SensorId = SensorOutput.SensorId " +
                                                    "inner join Parameters on SensorOutput.ParameterId = Parameters.ParameterId " +
                                                    "inner join DataOutput on PostSensors.PSId = DataOutput.IdPS AND SensorOutput.ParameterId = DataOutput.IdSP " +
                                                    "Where PostSensors.PostId = " + PostId + " AND DataOutput.IdSP = " + item +
                                                    "AND DataOutput.VerifyDate BETWEEN '" + sFrom + "' AND '" + sTo + "'" +
                                                    " ORDER BY VerifyDate DESC";

                    SqlDataReader reader = command.ExecuteReader();
                    List<double> values = new List<double>();
                    List<DateTime> dts = new List<DateTime>();
                    while (reader.Read())
                    {
                        dts.Add(reader.GetDateTime(1));
                        values.Add(reader.GetDouble(0));
                        /*DateTime dt = reader.GetDateTime(1);
                        double value = reader.GetDouble(0);

                        if (dt > To)
                            break;

                        if (From < dt && dt < To)
                        {
                            
                        }*/
                    }
                    reader.Close();
                    result.Add(dts, values);
                }
            }
            return result;
        }

        public static Dictionary<List<DateTime>, List<double>> GetParametersValuesGraph2(List<int> PostIds, DateTime From, DateTime To, int parameterId)
        {
            Dictionary<List<DateTime>, List<double>> result = new Dictionary<List<DateTime>, List<double>>();
            using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    return result;
                }
                SqlCommand command = new SqlCommand("", connection);
                string sFrom = String.Format("{0:dd/MM/yyyy}", From);
                string sTo = String.Format("{0:dd/MM/yyyy}", To);
                foreach (var id in PostIds)
                {
                    command.CommandText = "SELECT DataOutput.Value, DataOutput.VerifyDate " +
                                                    "FROM PostSensors " +
                                                    "left join SensorOutput on PostSensors.SensorId = SensorOutput.SensorId " +
                                                    "inner join Parameters on SensorOutput.ParameterId = Parameters.ParameterId " +
                                                    "inner join DataOutput on PostSensors.PSId = DataOutput.IdPS AND SensorOutput.ParameterId = DataOutput.IdSP " +
                                                    "Where PostSensors.PostId = " + id + " AND DataOutput.IdSP = " + parameterId +
                                                    "AND DataOutput.VerifyDate BETWEEN '" + sFrom + "' AND '" + sTo + "'" +
                                                    " ORDER BY VerifyDate DESC";

                    SqlDataReader reader = command.ExecuteReader();
                    List<double> values = new List<double>();
                    List<DateTime> dts = new List<DateTime>();
                    while (reader.Read())
                    {
                        dts.Add(reader.GetDateTime(1));
                        values.Add(reader.GetDouble(0));
                    }
                    reader.Close();
                    result.Add(dts, values);
                }
            }
            return result;
        }
        
        public static Dictionary<int, string> GetListOfPostParametersGraph(int PostId)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    return result;
                }
                SqlCommand command = new SqlCommand("", connection);
                command.CommandText = "SELECT SensorOutput.ParameterId, Parameters.ParameterName FROM PostSensors " +
                                      "inner join SensorOutput on PostSensors.SensorId = SensorOutput.SensorId " +
                                      "inner join Parameters on Parameters.ParameterId = SensorOutput.ParameterId " +
                                      "Where PostSensors.PostId = " + PostId;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int paramId = reader.GetInt32(0);
                    string paramName = reader.GetString(1);
                    if (!result.Keys.Contains(paramId))
                    {
                        result.Add(paramId, paramName);
                    }
                }
                reader.Close();
            }
            return result;
        }
        
        public static Dictionary<string, int> GetListOfSensorParametersGraph(int SensorId)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            List<int> IDs = new List<int>();
            using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    return result;
                }
                SqlCommand command = new SqlCommand("", connection);
                command.CommandText =   "SELECT Parameters.ParameterId, ParameterName from SensorOutput " +
                                        "inner join Parameters on SensorOutput.ParameterId = Parameters.ParameterId " +
                                        "where SensorOutput.SensorId = " + SensorId;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int paramId = reader.GetInt32(0);
                    string paramName = reader.GetString(1);
                    if (!IDs.Contains(paramId))
                    {
                        IDs.Add(paramId);
                        result.Add(paramName, paramId);
                    }
                }
                reader.Close();
            }
            return result;
        }

        public static bool CheckDBConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(GetDatabaseConnection(connString)))
                {
                    SqlCommand command = new SqlCommand("SELECT * FROM Posts", connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
    public class ReportModel
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}