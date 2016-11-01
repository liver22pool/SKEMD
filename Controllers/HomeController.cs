using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Drawing;
using System.Web.Security;
using WebMatrix.WebData;
using ExcelLibrary.CompoundDocumentFormat;
using ExcelLibrary.SpreadSheet;
using SKEMD_WWW.Models;
using System.IO;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace SKEMD_WEB.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            GraphValidationModel model = new GraphValidationModel();
            APKContext.GenerateAPKList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(GraphValidationModel model, string actionName)
        {
            CheckGraphValidationModel(ref model);

            TempData["model"] = model;
            if (actionName == "graph")
            {
                return RedirectToAction("Graph"/*, "Home", model*/);    
            }
            else if (actionName == "report")
            {
                return RedirectToAction("Report");   
            }
            return View();
        }

        private void CheckGraphValidationModel(ref GraphValidationModel model)
        {
            if(model.DateFrom == DateTime.Parse("01.01.0001"))
                model.DateFrom = DateTime.Now;
            if (model.TimeFrom == DateTime.Parse("01.01.0001 00:00"))
                model.TimeFrom = DateTime.Now;
            if (model.DateTo == DateTime.Parse("01.01.0001"))
                model.DateTo = DateTime.Now;
            if (model.TimeTo == DateTime.Parse("01.01.0001 00:00"))
                model.TimeTo = DateTime.Now;
        }

        public ActionResult Graph(/*GraphValidationModel model*/)
        {
            var model = (GraphValidationModel)TempData["model"];
            /*if (model != null)
            {
                @ViewBag.FromAccordeon = true;
            }*/
            return View(model);
        }

        public ActionResult Report()
        {
            var model = (GraphValidationModel)TempData["model"];
            
            List<APK> apklst = APKContext.GetAPKList();
            List<ReportModel> list = new List<ReportModel>();
            for (int i = 0; i < apklst.Count; i++)
            {
                ReportModel rep = new ReportModel();
                rep.Description = apklst[i].Description;
                rep.Name = apklst[i].Name;
                rep.Type = (int)apklst[i].Type;
                if (model == null)
                {
                    rep.Day = DateTime.Now.Day;
                    rep.Month = DateTime.Now.Month;
                    rep.Year = DateTime.Now.Year;
                }
                else
                {
                    rep.Day = model.DateFrom.Day;
                    rep.Month = model.DateFrom.Month;
                    rep.Year = model.DateFrom.Year;
                }
                list.Add(rep);
            }
            return View(list);
        }

        public ActionResult Help()
        {
            return View();
        }

        public ActionResult Developer()
        {
            return View();
        }

        public ActionResult ContactInfo()
        {
            return View();
        }
        
        public JsonResult APKSearch(int byWhom, string what)
        {
            JsonResult jr = null;
            List<APK> list = APKContext.GetAPKList();
            if (byWhom == 0) //by number
            {
                //int i = -1;
                APK apk = null;
                //if(int.TryParse(what, out i))
                    apk = list.Where(item => item.Number == what).FirstOrDefault();
                if (apk == null)
                    apk = new APK() { Number = "-1" };
                jr = Json(apk.Number, JsonRequestBehavior.AllowGet);
            }
            else if (byWhom == 1) //by name
            {
                APK apk = list.Where(item => item.Name.ToUpper().Contains(what.ToUpper())).FirstOrDefault();
                if (apk == null)
                    apk = new APK() { Number = "-1" };
                jr = Json(apk.Number, JsonRequestBehavior.AllowGet);   
            }
            return jr;
        }
        
        public JsonResult GetAPK(bool all, int id)
        {
            List<APK> lst = APKContext.GetAPKList();
            JsonResult jr;
            if (all)
                jr = Json(lst, JsonRequestBehavior.AllowGet);
            else
            {
                APK tmp = lst.Where(apk => apk.ID == id).FirstOrDefault();
                if (tmp == null)
                    tmp = new APK(){ID = -1};
                jr = Json(tmp , JsonRequestBehavior.AllowGet);
            }
            return jr;
        }

        public JsonResult GenerateAPK(bool all, int id)
        {
            List<APK> lst = APKContext.GenerateAPKList();
            JsonResult jr;
            if (all)
                jr = Json(lst, JsonRequestBehavior.AllowGet);
            else
            {
                APK tmp = lst.Where(apk => apk.ID == id).FirstOrDefault();
                if (tmp == null)
                    tmp = new APK() { ID = -1 };
                jr = Json(tmp, JsonRequestBehavior.AllowGet);
            }
            return jr;
        }
        
        public JsonResult GetMarkers()
        {
            List<APK> lst = APKContext.GetAPKList();

            List<int> ID = new List<int>();
            List<double> Lat = new List<double>();
            List<double> Lan = new List<double>();
            List<string> Name = new List<string>();
            List<int> Status = new List<int>();
            List<int> Type = new List<int>();

            foreach (var item in lst)
            {
                ID.Add(item.ID);
                Lat.Add(item.Lat);
                Lan.Add(item.Lan);
                Name.Add(item.Name);
                Status.Add((int)(item.Status));
                Type.Add((int)(item.Type));
            }

            JsonResult jr = Json(new { id = ID, lat = Lat, lan = Lan, name = Name, status = Status, type = Type}, JsonRequestBehavior.AllowGet);

            return jr;
        }

        public JsonResult GetAPKTypes()
        {
            IEnumerable<Types> ie = Enum.GetValues(typeof(Types)).Cast<Types>();
            List<string> lst = ie.Select(item => item.ToString()).ToList();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAPKType(int number)
        {
            APK apk = APKContext.GetAPKList()[number];
            int type = (int)apk.Type;
            return Json(type, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult CreateReportXLS(string hiddenAPKIDSelector, string hiddenPIB, string hiddenDateDay, string hiddenDateMonth, string hiddenDateYear)
        {
            /*string file = Path.Combine(Server.MapPath("~/Files"), "report.xls");
            Workbook workbook = Workbook.Load(file);
            Worksheet worksheet = workbook.Worksheets[0];

            worksheet.Cells[1, 1].Value = "EHUUUU";
            worksheet.Cells[0, 1] = new Cell("1, 2");
            workbook.Save(file);
            return File(file, "application/vnd.ms-excel", "report.xls");*/

            int apkNumber = 0;
            Int32.TryParse(hiddenAPKIDSelector, out apkNumber);

            string file = Path.Combine(Server.MapPath("~/Files"), "report" + ((int)APKContext.GetAPKList()[apkNumber].Type).ToString() + ".xlsx");

            //видалимо попередній файл
            /*string tmp_path = (string)TempData["tmp_file"];
            if(tmp_path != null)
                System.IO.File.Delete(tmp_path);*/

            foreach (string files in System.IO.Directory.EnumerateFiles(Server.MapPath("~/Files/tmp_files")))
            {
                try
                {
                    string tmp = Path.Combine(Server.MapPath("~/Files/tmp_files"), files);
                    System.IO.File.Delete(tmp);
                }
                catch (IOException ex)
                {

                }
            }
            //створимо новий файл із шаблону
            string tmp_file = Path.Combine(Server.MapPath("~/Files/tmp_files"), "report" +  Guid.NewGuid() + ".xlsx");
            
            System.IO.File.Copy(file, tmp_file);
            
            var xlApp = new Excel.Application();
            try
            {
                xlApp.DisplayAlerts = false;
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(tmp_file);
                int cnt = xlWorkBook.Sheets.Count;
                Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[1];

                xlWorkSheet.Cells[1][1] = hiddenDateDay + "." + hiddenDateMonth + "." + hiddenDateYear;
                xlWorkSheet.Cells[17][4] = hiddenPIB;
                xlWorkSheet.Cells[17][4] = hiddenPIB;
                xlWorkSheet.Cells[1][9] = APKContext.GetAPKList()[apkNumber].Description;
                xlWorkSheet.Cells[1][10] = "за адресою: " + APKContext.GetAPKList()[apkNumber].Name;

                int id = APKContext.GetAPKList()[apkNumber].ID;
                APK apk = APKContext.GetAPKList()[apkNumber];
                List<List<string>> lst = APKContext.GetListOfParameterValuesReport(id, GetAPKParametersIdForReport(apk), hiddenDateDay, hiddenDateMonth, hiddenDateYear);
                for (int i = 0; i < lst.Count; i++)
                    for (int j = 0; j < 24; j++)
                    {
                        xlWorkSheet.Cells[6 + j][15 + i] = "";
                    }

                for (int i = 0; i < lst.Count; i++)
                    for (int j = 0; j < lst[i].Count; j++)
                    {
                        xlWorkSheet.Cells[6 + j][15 + i] = lst[i][j].ToString();
                    }

                xlWorkBook.SaveAs(tmp_file, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                xlWorkBook.Close(true);
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
            }
            catch (Exception ex)
            {
                Marshal.ReleaseComObject(xlApp);
            }

            return File(tmp_file, "application/vnd.ms-excel", "report.xlsx");
        }
        
        [HttpPost]
        public ActionResult UploadXLSImage(string imageData, int width, int height)
        {
            string fileNameWitPath = Path.Combine(Server.MapPath("~/Files"), "graph.png"); ;
            using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    TempData["width"] = width;
                    TempData["height"] = height;
                    bw.Write(data);
                    bw.Close();
                }
            }
            return Content("Success");
        }

        /*[HttpPost]
        public ActionResult UploadXLSTable(Graph<> imageData, int width, int height)
        {
            string fileNameWitPath = Path.Combine(Server.MapPath("~/Files"), "graph.png"); ;
            using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] data = Convert.FromBase64String(imageData);
                    TempData["width"] = width;
                    TempData["height"] = height;
                    bw.Write(data);
                    bw.Close();
                }
            }
            return Content("Success");
        }*/

        public FileResult CreateGraphXLS()
        {
            string file = Path.Combine(Server.MapPath("~/Files"), "graph.xls");
            string pic_file = Path.Combine(Server.MapPath("~/Files"), "graph.png");

            var xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[1];

            xlWorkSheet.Shapes.AddPicture(pic_file, MsoTriState.msoFalse, MsoTriState.msoCTrue, 10, 10, (int)TempData["width"], (int)TempData["height"]);

            xlWorkBook.SaveAs(file, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
            xlWorkBook.Close(true);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlApp);


            return File(file, "application/vnd.ms-excel", "graph.xls");
        }

        public FileResult GetTableXLS()
        {
            Graph<DateTime, Value> graph = (Graph<DateTime, Value>)TempData["graph"];
            List<string> Names = (List<string>)TempData["names"];
            string file = Path.Combine(Server.MapPath("~/Files"), "table.xls");

            var xlApp = new Excel.Application();
            xlApp.DisplayAlerts = false;
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[1];

            for (int column = 0; column <= graph.Curves.Count; column++)
            {
                if (column < graph.Curves.Count)
                    xlWorkSheet.Cells[1][column + 2] = Names[column];
                for (int row = 0; row < graph.X.Count; row++)
                {
                    if (column == 0)
                    {
                        xlWorkSheet.Cells[row + 2][column + 1] = graph.X[row].ToString();
                    }
                    else
                    {
                        if (xlWorkSheet.Cells[row + 2][column + 1] = graph.Curves[column - 1].Y[row].IsSet == true)
                            xlWorkSheet.Cells[row + 2][column + 1] = graph.Curves[column - 1].Y[row].Val;
                        else
                            xlWorkSheet.Cells[row + 2][column + 1] = "-";
                    }
                }
            }

            xlWorkBook.SaveAs(file, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
            xlWorkBook.Close(true);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlApp);
                        
            return File(file, "application/vnd.ms-excel", "table.xls");
        }

        public JsonResult GetAPKParametersForReport(int APKID, string day, string month, string year)
        {
            int id = APKContext.GetAPKList()[APKID].ID;
            APK apk = APKContext.GetAPKList()[APKID];
            TempData["REPORTAPKTYPE"] = (int)apk.Type;
            List<List<string>> lst = APKContext.GetListOfParameterValuesReport(id, GetAPKParametersIdForReport(apk), day, month, year);

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        private List<int> GetAPKParametersIdForReport(APK apk)
        {
            List<int> list = new List<int>();
            if (apk.Type == Types.chem)
            {
                list.Add(1); list.Add(2); list.Add(3);
            }
            else if (apk.Type == Types.gama)
            {
                list.Add(11);
            }
            else if (apk.Type == Types.gamaANDcontrol)
            {
                list.Add(9); list.Add(10); list.Add(17);
            }
            else if (apk.Type == Types.radio)
            {
                list.Add(7); list.Add(8);
            }
            else if (apk.Type == Types.meteo)
            {
                list.Add(4); list.Add(12); list.Add(13); list.Add(14); list.Add(16); list.Add(15);
            }
            else if (apk.Type == Types.gamaANDchem)
            {
                list.Add(1); list.Add(2); list.Add(3); list.Add(11);
            }
            return list;
        }

        /*public JsonResult GetAPKParametersValueForGraph(int APKID, string From, string To)
        {
            int id = APKContext.GetAPKList()[APKID].ID;
            var dateFrom = DateTime.Parse(From);
            var dateTo = DateTime.Parse(To);
            List<int> paramsID = new List<int>();
            Dictionary<List<DateTime>, List<double>> lst = APKContext.GetParametersValuesGraph(id, dateFrom, dateTo, ref paramsID);
            
            return Json(new { dates = lst.Keys.ToList(), values = lst.Values.ToList(), paramIDs = paramsID }, JsonRequestBehavior.AllowGet);
        }*/

        public JsonResult GetAPKParameterNamesForGraph(int APKNumber)
        {
            int id = APKContext.GetAPKList()[APKNumber].ID;
            Dictionary<int, string> lst = APKContext.GetListOfPostParametersGraph(id);

            return Json(new {IDs = lst.Keys, Names = lst.Values}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDOZORParameterNamesForGraph()
        {
            Dictionary<string, int> lst = APKContext.GetListOfSensorParametersGraph(1);

            return Json(new { Name = lst.Keys, ID = lst.Values }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTableValuesForGraph(List<int> PostId, string From, string To, List<int> ParameterId, List<string> ParameterName)
        {
            DateTime DateFrom = DateTime.Parse(From);
            DateTime DateTo = DateTime.Parse(To);
            List<int> paramsID = new List<int>();
            int postid = 0;
            if (PostId.Count == 1)
                postid = PostId[0];
            Dictionary<List<DateTime>, List<double>> lst = APKContext.GetParametersValuesGraph(postid, DateFrom, DateTo, ParameterId);

            return Json(GetGraphValues(lst, ParameterName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTableValuesForGraphNoModel(List<int> PostId, string From, string To, int ParameterId, List<string> APKName)
        {
            DateTime DateFrom = DateTime.Parse(From);
            DateTime DateTo = DateTime.Parse(To);
            List<int> paramsID = new List<int>();
            Dictionary<List<DateTime>, List<double>> lst = APKContext.GetParametersValuesGraph2(PostId, DateFrom, DateTo, ParameterId);

            return Json(GetGraphValues(lst, APKName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDBEConnection()
        {
            bool result = APKContext.CheckDBConnection();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private Graph<string, Value> GetGraphValues (Dictionary<List<DateTime>, List<double>> lst, List<string> Names)
        {
            List<List<DateTime>> dates = lst.Keys.ToList();
            List<List<double>> values = lst.Values.ToList();

            int curvesNumber = lst.Count;
            int pointsNumber = 0;
            int maxpointsindex = 0;
            for (int i = 0; i < dates.Count; i++)
            {
                if (dates[i].Count > pointsNumber)
                {
                    pointsNumber = dates[i].Count;
                    maxpointsindex = i;
                }
            }

            Graph<string, Value> graph = new Graph<string, Value>() { AbscissaName = "Дата", OrdinateName = "Значення" };

            for (int j = 0; j < pointsNumber; j++)
            {
                graph.X.Add(dates[maxpointsindex][j].ToString());
            }

            for (int i = 0; i < curvesNumber; i++)
            {
                Curve<Value> curve = new Curve<Value>();
                curve.Name = "Графік№" + i;

                int startIndexInData = 0;
                for (int j = 0; j < pointsNumber; j++)
                {
                    if (i == maxpointsindex)
                    {
                        curve.Y.Add(new Value() { IsSet = true, Val = values[i][j] });
                    }
                    else
                    {
                        DateTime xPoint = DateTime.Parse(graph.X[j]);

                        int index = -1;
                        for (int k = startIndexInData; k < values[i].Count; k++)
                        {
                            if (xPoint == dates[i][k])//100% співпадіння дати з вісі Х і дати в даних
                            {
                                index = k;
                                startIndexInData = k;
                                break;
                            }
                            if (k + 1 == values[i].Count)
                            {
                                /*if (dates[i][k] > xPoint) //дата вісі Х знаходиться між датами з даними
                                {
                                    index = k;
                                    startIndexInData = k;
                                    break;
                                }  */  
                            }
                            else
                            {
                                if (dates[i][k] > xPoint && dates[i][k + 1] < xPoint) //дата вісі Х знаходиться між датами з даними
                                {
                                    if ((dates[i][k] - xPoint) <= (xPoint - dates[i][k + 1]))
                                        index = k;
                                    else
                                        index = k + 1;
                                    startIndexInData = k;
                                    break;
                                }
                            }
                        }

                        if (index == -1)
                        {
                            curve.Y.Add(new Value() { IsSet = false, Val = 0 });
                        }
                        else
                            curve.Y.Add(new Value() { IsSet = true, Val = values[i][index] });
                    }
                }
                graph.Curves.Add(curve);
            }
            //створити файл ексель
            TempData["graph"] = graph;
            TempData["names"] = Names;
            return graph;
        }

        /*public JsonResult GetValues()
        {
            if (Request.Params["makeChart"] != null) // Если пришла команда создать данные для графика
            {
                try
                {
                    int curvesNumber = Convert.ToInt32(Request.Params["curvesNumber"].ToString());
                    int minValue = Convert.ToInt32(Request.Params["minValue"].ToString());
                    int maxValue = Convert.ToInt32(Request.Params["maxValue"].ToString());
                    int pointsNumber = Convert.ToInt32(Request.Params["pointsNumber"].ToString());

                    Curves curves = new Curves(curvesNumber, pointsNumber);

                    curves.vName = "Па(Подпись вертикальной оси)";
                    //заполняем названия колонок
                    for (int j = 0; j < curvesNumber; j++)
                    {
                        curves.columns[j] = string.Format("Curve №{0}", j);
                    }

                    Random r = new Random();
                    for (int i = 0; i < pointsNumber; i++)
                    {
                        curves.x[i] = i;
                        for (int j = 0; j < curvesNumber; j++)
                        {
                            curves.values[i * curvesNumber + j] = r.Next(minValue, maxValue);
                        }
                    }

                    return Json(new { curves });
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.ToString() });
                }
            }
            return null;
        }

        private DataTable createData(int points, int curves, int maxVal, int minVal)
        {
            DataTable result = new DataTable();
            object[] colNames = new object[curves];
            for (int column = 0; column < curves; column++)
            {
                result.Columns.Add();
                colNames[column] = "Curve " + column.ToString();
            }
            result.Rows.Add(colNames);

            Random r = new Random();
            for (int row = 1; row <= points; row++)
            {
                object[] celVals = new object[curves];
                for (int col = 0; col < curves; col++)
                {
                    celVals[col] = r.Next(minVal, maxVal);
                }
                result.Rows.Add(celVals);
            }

            return result;
        }

        public JsonResult GetExcel()
        {
            if (Request.Params["makeExcel"] != null) // Если пришла команда создать данные для графика
            {
                try
                {
                    int curvesNumber = Convert.ToInt32(Request.Params["curvesNumber"].ToString());
                    int minValue = Convert.ToInt32(Request.Params["minValue"].ToString());
                    int maxValue = Convert.ToInt32(Request.Params["maxValue"].ToString());
                    int pointsNumber = Convert.ToInt32(Request.Params["pointsNumber"].ToString());

                    DataTable data = createData(pointsNumber, curvesNumber, maxValue, minValue);
                    createFile(data);

                    return Json(new { file = "report.xls" });
                }
                catch (Exception ex)
                {
                    return Json(new { error = ex.ToString() });
                }
            }

            return null;
        }

        private void createFile(DataTable data)
        {
            string file = "report.xls";
            if (System.IO.File.Exists(file)) 
                System.IO.File.Delete(file); //можно добавлять к названию файла время или еще что нибудь, но пока что от простоты
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("Values");

            for (int column = 0; column < data.Columns.Count; column++)
            {
                for (int row = 0; row < data.Rows.Count; row++)
                {
                    worksheet.Cells[row, column] = new Cell(data.Rows[row][column].ToString());
                }
            }
            workbook.Worksheets.Add(worksheet);
            workbook.Save(file);
        }

        public FileContentResult GetFile(string id)
        {
            if (!System.IO.File.Exists("1.txt"))
            {
                System.IO.File.Create("1.txt");
                System.IO.StreamWriter sW = new System.IO.StreamWriter("1.txt");
                sW.WriteLine("test");
                sW.Close();
            }

            byte[] fileContent = System.IO.File.ReadAllBytes("1.txt");

            return File(fileContent, "text/plain", "1.txt");
        }*/
    }


    public class Graph<T, T1>
    {
        List<Curve<T1>> list = new List<Curve<T1>>();
        List<T> x = new List<T>();
        public string AbscissaName { get; set; }
        public string OrdinateName { get; set; }
        public List<T> X { get { return x; } } 
        public List<Curve<T1>> Curves
        {
            get
            {
                return list;  
            }
        }
        /*public void AddCurve(Curve<DateTime, double> curve)
        {
            list.Add(curve);
        }*/
    }

    public class Curve<T>
    {
        List<T> y = new List<T>();
        public List<T> Y { get{return y;} }
        public string Name { get; set; }
    }
    
    public class Value
    {
        public bool IsSet { get; set; }
        public double Val { get; set; }
    }
    /*public class Curves
    {
        /// <summary>
        /// Подпись вертикальной оси
        /// </summary>
        public string vName { get; set; }
        /// <summary>
        /// Названия колонок
        /// </summary>
        public string[] columns = null;
        /// <summary>
        /// Массив значений x
        /// </summary>
        public DateTime[] x = null;
        /// <summary>
        /// Массив значений f(x)
        /// </summary>
        public double[] values = null;

        public Curves(int curvesNumber, int points)
        {
            columns = new string[curvesNumber];
            x = new DateTime[points];
            values = new double[curvesNumber * points];
        }
    }*/
}
