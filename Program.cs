using System.Collections.Generic;
using System.Text.Json;
using System.Net;
using System.Threading;
using System;
using CNCNetLib;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;

namespace CNCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // .env
            DotNetEnv.Env.Load();
            string HOST_IP = DotNetEnv.Env.GetString("HOST_IP");
            Console.WriteLine("HOSTIP: " + HOST_IP);
            string CNC_IP = DotNetEnv.Env.GetString("CNC_IP");
            Console.WriteLine("CNC_IP: " + CNC_IP);
            string INFLUX_CONSTR = DotNetEnv.Env.GetString("INFLUXDB_IP") + ":" + DotNetEnv.Env.GetString("INFLUXDB_PORT");
            Console.WriteLine("INFLUXDB_CONNECTION: " + INFLUX_CONSTR);

            ReturnJson returnJson = new ReturnJson();

            CNCInfoClass CNCInfo = new CNCInfoClass();
            CNCInfo.SetConnectInfo(HOST_IP, CNC_IP, 5000);
            CNCInfo.Connect();

            // while (true) {
            //     Self(ref CNCInfo, ref returnJson);
            //     string returnString = JsonSerializer.Serialize( returnJson );
            //     Send( returnString );
            // }

            while (true)
            {
            	try
            	{
            		Self(ref CNCInfo, ref returnJson);
            		string returnString = JsonSerializer.Serialize( returnJson );
                    Console.Write( returnString );
            		Send(INFLUX_CONSTR, ref returnJson);
            	}
            	catch
            	{
            		
            	}
            	Thread.Sleep(5000);
            }

            CNCInfo.Disconnect();
        }

        // static void Send(string returnString)
        // {
        //     returnString = returnString.Replace("{","").Replace("}","")
        //         .Replace("\"FeedSpindleOvFeed\":","FeedSpindleOvFeed=")
        //         .Replace("\"FeedSpindleOvSpindle\":","FeedSpindleOvSpindle=")
        //         .Replace("\"FeedSpindleActFeed\":", "FeedSpindleActFeed=")
        //         .Replace("\"FeedSpindleActSpindle\":", "FeedSpindleActSpindle=")
        //         .Replace("\"ServoLoadAxisCount\":", "ServoLoadAxisCount=")
        //         .Replace("\"ServoLoadAxisNr\":", "ServoLoadAxisNr=")
        //         .Replace("\"ServoLoadAxisValue\":", "ServoLoadAxisValue=")
        //         .Replace("\"SpindleCurrentAxisCount\":", "SpindleCurrentAxisCount=")
        //         .Replace("\"SpindleCurrentAxisNr\":", "SpindleCurrentAxisNr=")
        //         .Replace("\"SpindleCurrentAxisValue\":", "SpindleCurrentAxisValue=")
        //         .Replace("\"WorkingFlag\":", "WorkingFlag=")
        //         .Replace("\"AlarmFlag\":", "AlarmFlag=")
        //         .Replace("\"NCodeLineNo\":", "NCodeLineNo=")
        //         .Replace("\"NCodeContent\":", "NCodeContent=")
        //         .Replace("\"NCPointerLineNum\":", "NCPointerLineNum=")
        //         .Replace("\"NCPointerMDILineNum\":", "NCPointerMDILineNum=")
        //         .Replace("\"PositionAxisName\":", "PositionAxisName=")
        //         .Replace("\"PositionCoorMach\":", "PositionCoorMach=")
        //         .Replace("\"PositionCoorAbs\":", "PositionCoorAbs=")
        //         .Replace("\"PositionCoorRel\":", "PositionCoorRel=")
        //         .Replace("\"PositionCoorRes\":", "PositionCoorRes=")
        //         .Replace("\"PositionCoorOffset\":", "PositionCoorOffset=")
        //         .Replace("\"TotalWorkTime\":", "TotalWorkTime=")
        //         .Replace("\"SingleWorkTime\":", "SingleWorkTime=");

        //     WebClient client = new WebClient();
        //     string url = "http://140.135.106.215:38086/write?db=factory";
        //     string data = "CNC," + returnString + " value=1.0";
        //     Console.WriteLine( returnString.Length );
        //     Console.WriteLine( url + " --data-binary " + data );
        //     var response = client.UploadString(url, data);
        // }

        static void Send(string influxConnection, ref ReturnJson returnJson)
        {
            var dbName = "factory";
            var influxDbClient = new InfluxDbClient(influxConnection, "", "", InfluxDbVersion.Latest);

            var pointToWrite = new Point()
            {
                Name = "CNC", // serie/measurement/table to write into
                Tags = new Dictionary<string, object>()
                {
                    { "SerialId", "delta" }
                },
                Fields = new Dictionary<string, object>()
                {
                    {"feedSpindleOvFeed", returnJson.FeedSpindleOvFeed},
                    {"feedSpindleOvSpindle", returnJson.FeedSpindleOvSpindle},
                    {"feedSpindleActFeed", returnJson.FeedSpindleActFeed},
                    {"feedSpindleActSpindle", returnJson.FeedSpindleActSpindle},
                    {"servoLoadAxisCount", returnJson.ServoLoadAxisCount},
                    {"servoLoadAxisNr", returnJson.ServoLoadAxisNr},
                    {"servoLoadAxisValue", returnJson.ServoLoadAxisValue},
                    {"spindleCurrentAxisCount", returnJson.SpindleCurrentAxisCount},
                    {"spindleCurrentAxisNr", returnJson.SpindleCurrentAxisNr},
                    {"spindleCurrentAxisValue", returnJson.SpindleCurrentAxisValue},
                    {"workingFlag", returnJson.WorkingFlag},
                    {"alarmFlag", returnJson.AlarmFlag},
                    {"nCodeLineNo", returnJson.NCodeLineNo},
                    {"nCodeContent", returnJson.NCodeContent},
                    {"nCPointerLineNum", returnJson.NCPointerLineNum},
                    {"nCPointerMDILineNum", returnJson.NCPointerMDILineNum},
                    {"positionAxisName", returnJson.PositionAxisName},
                    {"positionCoorMach", returnJson.PositionCoorMach},
                    {"positionCoorAbs", returnJson.PositionCoorAbs},
                    {"positionCoorRel", returnJson.PositionCoorRel},
                    {"positionCoorRes", returnJson.PositionCoorRes},
                    {"positionCoorOffset", returnJson.PositionCoorOffset},
                    {"totalWorkTime", returnJson.TotalWorkTime},
                   	{"singleWorkTime", returnJson.SingleWorkTime}
                }
            };

            var response = influxDbClient.Client.WriteAsync(pointToWrite, dbName);
        }

        private class ReturnJson
        {
            // Feed Spindle
            public string FeedSpindleOvFeed { get; set; }
            public string FeedSpindleOvSpindle { get; set; }
            public string FeedSpindleActFeed { get; set; }
            public string FeedSpindleActSpindle { get; set; }
            // Servo Load
            public int ServoLoadAxisCount { get; set; }
            public string ServoLoadAxisNr { get; set; }
            public string ServoLoadAxisValue { get; set; }
            // Spindle Current
            public int SpindleCurrentAxisCount { get; set; }
            public string SpindleCurrentAxisNr { get; set; }
            public string SpindleCurrentAxisValue { get; set; }
            // Flag
            public string WorkingFlag { get; set; }
            public string AlarmFlag { get; set; }
            // NC Code
            public int NCodeLineNo { get; set; }
            public string NCodeContent { get; set; }
            // NC Pointer
            public int NCPointerLineNum { get; set; }
            public int NCPointerMDILineNum { get; set; }
            // Position
            public string PositionAxisName { get; set; }
            public string PositionCoorMach { get; set; }
            public string PositionCoorAbs { get; set; }
            public string PositionCoorRel { get; set; }
            public string PositionCoorRes { get; set; }
            public string PositionCoorOffset { get; set; }
            // Process Time
            public int TotalWorkTime { get; set; }
            public int SingleWorkTime { get; set; }
        }

        static void Self(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            Console.Clear();
            Console.WriteLine("!========= START =========!");

            Console.Write("=> Work Time ......... ");
            GetWorkTime(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.Write("=> Feed Spindle ...... ");
            GetFeedSpindle(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");
            
            Console.Write("=> Spindle Current ... ");
            GetSpindleCurrent(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.Write("=> CNC Flag .......... ");
            GetCNCFlag(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.Write("=> NC Code ........... ");
            GetNCode(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.Write("=> NC Pointer ........ ");
            GetNCPointer(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.Write("=> Position .......... ");
            GetPosition(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.Write("=> Servo Load ........ ");
            GetServoLoad(ref CNCInfo, ref returnJson);
            Console.WriteLine("[ OK ]");

            Console.WriteLine("!========== END ==========!");
        }

        static void GetFeedSpindle(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_feed_spindle(0 ,0, out double OvFeed, out uint OvSpindle, out double ActFeed, out uint ActSpindle);
            returnJson.FeedSpindleOvFeed = OvFeed.ToString();
            returnJson.FeedSpindleOvSpindle = OvSpindle.ToString();
            returnJson.FeedSpindleActFeed = ActFeed.ToString();
            returnJson.FeedSpindleActSpindle = ActSpindle.ToString();
        }

        static void GetServoLoad(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_servo_load(0, out ushort AxisCount, out ushort[] AxisNr, out bool[] Result, out int[] AxisValue);
            returnJson.ServoLoadAxisCount = (int)AxisCount;
            try {
            	returnJson.ServoLoadAxisNr = String.Join("|", AxisNr);
        	}
        	catch {
        		returnJson.ServoLoadAxisNr = "";
        	}
        	try {
            	returnJson.ServoLoadAxisValue = String.Join("|", AxisValue);
        	}
        	catch {
        		returnJson.ServoLoadAxisValue = "";
        	}
        }

        static void GetSpindleCurrent(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_spindle_current(0, out ushort AxisCount, out ushort[] AxisNr, out bool[] Result, out int[] AxisValue);
            returnJson.SpindleCurrentAxisCount = (int)AxisCount;
            returnJson.SpindleCurrentAxisNr = "";
            try {
                returnJson.SpindleCurrentAxisNr = String.Join("|", AxisNr);
            }
            catch {
            	returnJson.SpindleCurrentAxisNr = "";
            }
            try {
            	returnJson.SpindleCurrentAxisValue = String.Join("|", AxisValue);
            }
            catch {
            	returnJson.SpindleCurrentAxisValue = "";
            }
        }

        static void GetCNCFlag(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_CNCFlag(out bool WorkingFlag, out bool AlarmFlag);
            returnJson.WorkingFlag = WorkingFlag.ToString();
            returnJson.AlarmFlag = AlarmFlag.ToString();
        }

        static void GetNCode(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_current_code(1, out uint[] LineNo, out string[] Content);
            if (LineNo == null || Content == null) {
                returnJson.NCodeLineNo = -1;
                returnJson.NCodeContent = "empty";
                return;
            }
            returnJson.NCodeLineNo = (int)LineNo[0];
            returnJson.NCodeContent = Content[0];
            // returnJson.NCodeLineNo = Array.ConvertAll(LineNo, item => (int)item);
            // returnJson.NCodeContent = Content;
        }

        static void GetNCPointer(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_nc_pointer(out int LineNum, out int MDILineNum);
            returnJson.NCPointerLineNum = LineNum;
            returnJson.NCPointerMDILineNum = MDILineNum;
        }

        static void GetPosition(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_POSITION(0, 0, out string[] AxisName, out double[] CoorMach, out double[] CoorAbs, out double[] CoorRel, out double[] CoorRes,out double[] CoorOffset);
            try {
            	returnJson.PositionAxisName = String.Join("|", AxisName);
            }
            catch {
            	returnJson.PositionAxisName = "";
            }
            try {
            	returnJson.PositionCoorMach = String.Join("|", CoorMach);
            }
            catch {
            	returnJson.PositionCoorMach = "";
            }
            try {
            	returnJson.PositionCoorAbs = String.Join("|", CoorAbs);
            }
            catch {
            	returnJson.PositionCoorAbs = "";
            }
            try {
            	returnJson.PositionCoorRel = String.Join("|", CoorRel);
            }
            catch {
            	returnJson.PositionCoorRel = "";
            }
            try {
            	returnJson.PositionCoorRes = String.Join("|", CoorRes);
            }
            catch {
            	returnJson.PositionCoorRes = "";
            }
            try {
            	returnJson.PositionCoorOffset = String.Join("|", CoorOffset);
            }
            catch {
            	returnJson.PositionCoorOffset = "";
            }
        }

        static void GetWorkTime(ref CNCInfoClass CNCInfo, ref ReturnJson returnJson)
        {
            CNCInfo.READ_processtime(out uint TotalWorkTime, out uint SingleWorkTime);
            returnJson.TotalWorkTime = (int)TotalWorkTime;
            returnJson.SingleWorkTime = (int)SingleWorkTime;
        }
    }
} 
