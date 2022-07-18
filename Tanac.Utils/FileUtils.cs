using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tanac.Utils
{
    public class FileUtils
    {
		public static DataTable OpenCSV(string filePath)
		{
			Encoding encoding = Encoding.GetEncoding(0);
			DataTable dt = new DataTable();
			FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			StreamReader sr = new StreamReader(fs, encoding);
			string strLine = "";
			string[] aryLine = null;
			string[] tableHead = null;
			int columnCount = 0;
			bool IsFirst = true;
			while ((strLine = sr.ReadLine()) != null && strLine != "")
			{
				if (IsFirst)
				{
					tableHead = strLine.Split(',');
					IsFirst = false;
					columnCount = tableHead.Length;
					for (int i = 0; i < columnCount; i++)
					{
						DataColumn dc = new DataColumn(tableHead[i]);
						dt.Columns.Add(dc);
					}
					continue;
				}
				aryLine = strLine.Split(',');
				DataRow dr = dt.NewRow();
				try
				{
					for (int j = 0; j < columnCount; j++)
					{
						dr[j] = aryLine[j];
					}
				}
				catch
				{
				}
				dt.Rows.Add(dr);
			}
			if (aryLine != null && aryLine.Length != 0)
			{
				dt.DefaultView.Sort = tableHead[0] + " asc";
			}
			sr.Close();
			fs.Close();
			return dt;
		}

		public static void SaveCSV(DataTable dt, string fileName)
		{
			File.Exists(fileName);
			FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(fs, Encoding.Default);
			string data = "";
			for (int j = 0; j < dt.Columns.Count; j++)
			{
				data += dt.Columns[j].ColumnName.ToString();
				if (j < dt.Columns.Count - 1)
				{
					data += ",";
				}
			}
			sw.WriteLine(data);
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				data = "";
				for (int k = 0; k < dt.Columns.Count; k++)
				{
					data += dt.Rows[i][k].ToString();
					if (k < dt.Columns.Count - 1)
					{
						data += ",";
					}
				}
				sw.WriteLine(data);
			}
			sw.Close();
			fs.Close();
		}

		public static void WriteText(string filePath, string text, Encoding encoding)
		{
			if (!File.Exists(filePath))
			{
				File.Create(filePath).Close();
			}
			File.WriteAllText(filePath, text, encoding);
		}

		public static bool IsExistDirectory(string directoryPath)
		{
			return Directory.Exists(directoryPath);
		}

		public static bool IsExistFile(string filePath)
		{
			return File.Exists(filePath);
		}

		public static string[] GetFileNames(string directoryPath)
		{
			if (!IsExistDirectory(directoryPath))
			{
				throw new FileNotFoundException();
			}
			return Directory.GetFiles(directoryPath);
		}

		public static string[] GetDirectories(string directoryPath)
		{
			try
			{
				return Directory.GetDirectories(directoryPath);
			}
			catch (IOException ex)
			{
				throw ex;
			}
		}

		public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
		{
			if (!IsExistDirectory(directoryPath))
			{
				throw new FileNotFoundException();
			}
			try
			{
				if (isSearchChild)
				{
					return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
				}
				return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
			}
			catch (IOException ex)
			{
				throw ex;
			}
		}

		public static bool IsEmptyDirectory(string directoryPath)
		{
			try
			{
				string[] fileNames = GetFileNames(directoryPath);
				if (fileNames.Length != 0)
				{
					return false;
				}
				string[] directoryNames = GetDirectories(directoryPath);
				if (directoryNames.Length != 0)
				{
					return false;
				}
				return true;
			}
			catch
			{
				return true;
			}
		}

		public static bool Contains(string directoryPath, string searchPattern)
		{
			try
			{
				string[] fileNames = GetFileNames(directoryPath, searchPattern, isSearchChild: false);
				if (fileNames.Length == 0)
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.StackTrace);
			}
		}

		public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild)
		{
			try
			{
				string[] fileNames = GetFileNames(directoryPath, searchPattern, isSearchChild: true);
				if (fileNames.Length == 0)
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.StackTrace);
			}
		}

		public static string GetAppDirectory()
		{
			string str = Process.GetCurrentProcess().MainModule.FileName;
			return str.Substring(0, str.LastIndexOf("\\"));
		}

		public static string GetDateDir()
		{
			return DateTime.Now.ToString("yyyyMMdd");
		}

		public static string GetDateFile()
		{
			return DateTime.Now.ToString("HHmmssff");
		}

		public static void CopyFolder(string varFromDirectory, string varToDirectory)
		{
			Directory.CreateDirectory(varToDirectory);
			if (!Directory.Exists(varFromDirectory))
			{
				return;
			}
			string[] directories = Directory.GetDirectories(varFromDirectory);
			if (directories.Length != 0)
			{
				string[] array = directories;
				foreach (string d in array)
				{
					CopyFolder(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
				}
			}
			string[] files = Directory.GetFiles(varFromDirectory);
			if (files.Length != 0)
			{
				string[] array2 = files;
				foreach (string s in array2)
				{
					File.Copy(s, varToDirectory + s.Substring(s.LastIndexOf("\\")), overwrite: true);
				}
			}
		}

		public static void ExistsFile(string FilePath)
		{
			if (!File.Exists(FilePath))
			{
				FileStream fs = File.Create(FilePath);
				fs.Close();
			}
		}

		public static void DeleteFolderFiles(string varFromDirectory, string varToDirectory)
		{
			Directory.CreateDirectory(varToDirectory);
			if (!Directory.Exists(varFromDirectory))
			{
				return;
			}
			string[] directories = Directory.GetDirectories(varFromDirectory);
			if (directories.Length != 0)
			{
				string[] array = directories;
				foreach (string d in array)
				{
					DeleteFolderFiles(d, varToDirectory + d.Substring(d.LastIndexOf("\\")));
				}
			}
			string[] files = Directory.GetFiles(varFromDirectory);
			if (files.Length != 0)
			{
				string[] array2 = files;
				foreach (string s in array2)
				{
					File.Delete(varToDirectory + s.Substring(s.LastIndexOf("\\")));
				}
			}
		}

		public static string GetFileName(string filePath)
		{
			FileInfo fi = new FileInfo(filePath);
			return fi.Name;
		}

		public static void CreateDirectory(string directoryPath)
		{
			if (!IsExistDirectory(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		public static void CreateFile(string filePath)
		{
			try
			{
				if (!IsExistFile(filePath))
				{
					FileInfo file = new FileInfo(filePath);
					FileStream fs = file.Create();
					fs.Close();
				}
			}
			catch
			{
			}
		}

		public static void CreateFile(string filePath, byte[] buffer)
		{
			try
			{
				if (!IsExistFile(filePath))
				{
					FileInfo file = new FileInfo(filePath);
					FileStream fs = file.Create();
					fs.Write(buffer, 0, buffer.Length);
					fs.Close();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void DeleteFile(string file)
		{
			if (File.Exists(file))
			{
				File.Delete(file);
			}
		}

		public static int GetLineCount(string filePath)
		{
			string[] rows = File.ReadAllLines(filePath);
			return rows.Length;
		}

		public static int GetFileSize(string filePath)
		{
			FileInfo fi = new FileInfo(filePath);
			return (int)fi.Length;
		}

		public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
		{
			try
			{
				if (isSearchChild)
				{
					return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
				}
				return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
			}
			catch (IOException ex)
			{
				throw ex;
			}
		}

		public static void WriteText(string filePath, string content)
		{
			try
			{
				File.WriteAllText(filePath, content);
			}
			catch (Exception ex)
			{
				MessageBox.Show("写文件失败！\r\n" + ex.StackTrace);
			}
		}

		public static void AppendText(string filePath, string content)
		{
			try
			{
				File.AppendAllText(filePath, content);
			}
			catch (Exception ex)
			{
				MessageBox.Show("写文件失败！\r\n" + ex.StackTrace);
			}
		}

		public static void Copy(string sourceFilePath, string destFilePath)
		{
			File.Copy(sourceFilePath, destFilePath, overwrite: true);
		}

		public static void Move(string sourceFilePath, string descDirectoryPath)
		{
			string sourceFileName = GetFileName(sourceFilePath);
			if (IsExistDirectory(descDirectoryPath))
			{
				if (IsExistFile(descDirectoryPath + "\\" + sourceFileName))
				{
					DeleteFile(descDirectoryPath + "\\" + sourceFileName);
				}
				File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceFileName);
			}
		}

		public static string GetFileNameNoExtension(string filePath)
		{
			FileInfo fi = new FileInfo(filePath);
			return fi.Name.Split('.')[0];
		}

		public static string GetExtension(string filePath)
		{
			FileInfo fi = new FileInfo(filePath);
			return fi.Extension;
		}

		public static void ClearDirectory(string directoryPath)
		{
			if (IsExistDirectory(directoryPath))
			{
				string[] fileNames = GetFileNames(directoryPath);
				for (int j = 0; j < fileNames.Length; j++)
				{
					DeleteFile(fileNames[j]);
				}
				string[] directoryNames = GetDirectories(directoryPath);
				for (int i = 0; i < directoryNames.Length; i++)
				{
					DeleteDirectory(directoryNames[i]);
				}
			}
		}

		public static void ClearFile(string filePath)
		{
			File.Delete(filePath);
			CreateFile(filePath);
		}

		public static void DeleteDirectory(string directoryPath)
		{
			if (IsExistDirectory(directoryPath))
			{
				Directory.Delete(directoryPath, recursive: true);
			}
		}

		public static string ReadText(string filePath, int index)
		{
			string strTemp = string.Empty;
			try
			{
				string[] strAll = File.ReadAllLines(filePath);
				if (index < strAll.Length)
				{
					strTemp = strAll[index];
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("读文件失败！\r\n" + ex.StackTrace);
			}
			return strTemp;
		}

		public static string ReadAllText(string filePath)
		{
			string strTemp = string.Empty;
			try
			{
				string[] strAll = File.ReadAllLines(filePath);
				strTemp = string.Join(",", strAll);
			}
			catch (Exception ex)
			{
				MessageBox.Show("读文件失败！\r\n" + ex.StackTrace);
			}
			return strTemp;
		}


	}
}
