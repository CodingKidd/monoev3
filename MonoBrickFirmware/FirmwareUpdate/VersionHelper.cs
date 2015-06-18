﻿using System;
using System.Net;
using System.Reflection;


namespace MonoBrickFirmware.FirmwareUpdate
{
	public class VersionInfo
	{
		public VersionInfo(string firmware, string image, string addIn)
		{
			Firmware = firmware;
			Image = image;
			AddIn = addIn;
		}

		public string Firmware{ get; private set;}
		public string Image{ get; private set;}
		public string AddIn{ get; private set;}
	}

	public static class VersionHelper
	{
		private static string imageVersionPath = @"/usr/local/bin/version.txt";
		private static string addInVersionPath = @"/usr/local/bin/add-inVersion.txt";
		private static string repositoryFile = @"/usr/local/bin/repository.txt";
		private static string repository = null;
		private static bool urlRead = false;

		public static VersionInfo AvailableVersions()
		{
			VersionInfo info = null;

			try{
				string[] downloadInfo = new WebClient ().DownloadString (versionURL).Split (new char[] {'\n' });
				foreach(string s in downloadInfo)
				{
					Console.WriteLine("Download info: " + s);
				}

				info = new VersionInfo(downloadInfo[0].Split(new char[] {':'})[1].Trim(),downloadInfo[1].Split(new char[] {':'})[1].Trim(), downloadInfo[2].Split(new char[] {':'})[1].Trim());
			}
			catch{}
			return info;
		}


		public static VersionInfo InstalledVersion()
		{
			string firmware = CurrentFirmwareVersion (); 
			string image = CurrentImageVersion();
			string addIn = CurrentAddInVersion ();
			return new VersionInfo(firmware, image, addIn);
		}

		private static string GetRepositoryVersionFile()
		{
			if (repository == null)
			{
				repository = System.IO.File.ReadAllLines (repositoryFile)[0] + "version.txt";		
			}
		
		}

		private static string CurrentFirmwareVersion()
		{
			return Assembly.LoadFrom("MonoBrickFirmware.dll").GetName().Version.ToString();
		}

		private static string CurrentImageVersion()
		{
			return System.IO.File.ReadAllLines(imageVersionPath)[1].Split(new char[] {':'})[1].Trim();
		}

		private static string CurrentAddInVersion ()
		{
			string val = null;
			try 
			{
				val = System.IO.File.ReadAllLines (addInVersionPath) [0].Split (new char[] { ':' }) [1].Trim ();
			} 
			catch 
			{
			}
			return val;
		}
	}
}

