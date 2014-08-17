using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using MonoBrickFirmware.Native;
using System.Net.Sockets;
namespace MonoBrickFirmware.Services
{
	public static class WebServer
	{
		private const string logDir = "/var/log/lighttpd/";
		private const string wwwDir = "/www";
		
		private const string fastCGIName = @"fastcgi-mono-server4.exe";
		private const string fastCGIPath = @"/usr/local/bin/";
		private const string fastCGIArgs = @"/socket=tcp:9000 /address=127.0.0.1 /applications=/:.,/peanuts:"+ wwwDir + @"/" + @" /logfile=" +logDir + @"fastcgi.log /verbose";
		private const int fastCGICheckIntercal = 1000;//ms
		private const int fastCGITimeOut = 60000; //ms
		private const int fastCGIStartUpTime = 30000;//ms
		
		private const string lighttpdName = "lighttpd";
		private const string lighttpdPath = "/usr/local/sbin/";
		private const string lighttpdConf = @"/etc/lighttpd/lighttpd.conf";
		
		
		
		public static bool StartFastCGI ()
		{
			ProcessHelper.StartProcess ("mono", fastCGIPath + fastCGIName + @" " + fastCGIArgs);
			System.Threading.Thread.Sleep (fastCGIStartUpTime); //Wait for CGI to start
			bool running = false;
			int numberOfChecks = 0;
			int remainingTime = fastCGITimeOut - fastCGIStartUpTime;
			int fastCGICheckCount = 1;
			if (remainingTime > 0) {
				fastCGICheckCount = remainingTime/fastCGICheckIntercal;
			} 
			while (!running && numberOfChecks < fastCGICheckCount) {
				running = ProcessHelper.IsProcessRunning(fastCGIName);
				System.Threading.Thread.Sleep(fastCGICheckIntercal);
				numberOfChecks++;
			}
			return running;
		}
		
		public static bool StopFastCGI ()
		{
			ProcessHelper.KillProcess(fastCGIName);
			return true;
		}
		
		public static bool StartLighttpd ()
		{
			//ProcessHelper.RunAndWaitForProcess("adduser", "lighttpd");
			ProcessHelper.RunAndWaitForProcess("mkdir", logDir);
			//ProcessHelper.RunAndWaitForProcess("chown", "_R lighttpd:lighttpd " +  logDir);
			ProcessHelper.RunAndWaitForProcess("chown", "_R root:root " +  logDir);
			ProcessHelper.StartProcess(lighttpdPath+lighttpdName, "-f " + lighttpdConf);
			return true;
		}
		
		public static bool StopLighttpd ()
		{
			ProcessHelper.KillProcess(lighttpdName);
			return true;
		}
		
		public static bool LoadPage()
		{
			return true;
			/*int attemps = 0;
			bool loaded = false;
			while(attemps < 2 && !loaded){
				//try {
					/*Console.WriteLine("Load webpage");
					WebClient client = new WebClient();
					client.DownloadString("http://127.0.0.1"+":"+port);
					loaded = true;*/
					/*HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(@"http://127.0.0.1:"+ port);
					myRequest.Method = "GET";
					WebResponse myResponse = myRequest.GetResponse();
					StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
					string result = sr.ReadToEnd();
					sr.Close();
					myResponse.Close();
					loaded = true;*/
				/*} 
				catch(Exception e) 
				{
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
				}
				attemps++;
			}
			return true;*/
			//return loaded;
		}
		
		public static bool StartAll()
		{
			bool ok = StartFastCGI() && StartLighttpd() && LoadPage();
			if(!ok)
				StopAll();
			return ok;
		}
		
		public static bool RestartAll()
		{
			StopAll();
			Thread.Sleep(1000);
			return StartAll();
		}
		
		public static void StopAll()
		{
			StopFastCGI();
			StopLighttpd();
		}
		
		public static bool IsRunning ()
		{
			return ProcessHelper.IsProcessRunning(lighttpdName) && ProcessHelper.IsProcessRunning(fastCGIName);
		}
	}
}

