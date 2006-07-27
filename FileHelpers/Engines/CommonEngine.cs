#region "  � Copyright 2005-06 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcosdotnet[at]yahoo.com.ar.

#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;


namespace FileHelpers
{
	/// <summary>This class only have <b>static methods</b> to work with files and strings (the most common of them)</summary>
	public sealed class CommonEngine
	{
		// No instanciate
		private CommonEngine()
		{}

		#region "  FileHelperEngine  "

		/// <summary>
		/// Used to read a file without instanciate the engine.<br />
		/// <b>This is feature limited method try to use the non static methods.</b>
		/// </summary>
		/// <param name="recordClass">The record class.</param>
		/// <param name="fileName">The file name</param>
		/// <returns>The read records.</returns>
		public static object[] ReadFile(Type recordClass, string fileName)
		{
			FileHelperEngine engine = new FileHelperEngine(recordClass);
			return engine.ReadFile(fileName);
		}

		/// <summary>
		/// Used to read a string without instanciate the engine.<br />
		/// <b>This is feature limited method try to use the non static methods.</b>
		/// </summary>
		/// <param name="recordClass">The record class.</param>
		/// <param name="input">The input string.</param>
		/// <returns>The read records.</returns>
		public static object[] ReadString(Type recordClass, string input)
		{
			FileHelperEngine engine = new FileHelperEngine(recordClass);
			return engine.ReadString(input);
		}

		/// <summary>
		/// Used to write a file without instanciate the engine.<br />
		/// <b>This is feature limited method try to use the non static methods.</b>
		/// </summary>
		/// <param name="recordClass">The record class.</param>
		/// <param name="fileName">The file name</param>
		/// <param name="records">The records to write</param>
		public static void WriteFile(Type recordClass, string fileName, object[] records)
		{
			FileHelperEngine engine = new FileHelperEngine(recordClass);
			engine.WriteFile(fileName, records);
		}

		/// <summary>
		/// Used to write a string without instanciate the engine.<br />
		/// <b>This is feature limited method try to use the non static methods.</b>
		/// </summary>
		/// <param name="recordClass">The record class.</param>
		/// <param name="records">The records to write</param>
		/// <returns>The string with the writen records.</returns>
		public static string WriteString(Type recordClass, object[] records)
		{
			FileHelperEngine engine = new FileHelperEngine(recordClass);
			return engine.WriteString(records);
		}

		#endregion

		/// <summary><b>Faster way</b> to Transform the records of type sourceType in the sourceFile in records of type destType and write them to the destFile.</summary>
		/// <param name="sourceType">The Type of the records in the source File.</param>
		/// <param name="destType">The Type of the records in the dest File.</param>
		/// <param name="sourceFile">The file with records to be transformed</param>
		/// <param name="destFile">The destination file with the transformed records</param>
		/// <returns>The number of transformed records</returns>
		public static int TransformFileAsync(string sourceFile, Type sourceType, string destFile, Type destType)
		{
			FileTransformEngine engine = new FileTransformEngine(sourceType, destType);
			return engine.TransformFileAsync(sourceFile, destFile);
		}

		/// <summary>Transform the records of type sourceType in the sourceFile in records of type destType and write them to the destFile. (but returns the transformed records) WARNING: this is a slower method that the TransformFileAssync.</summary>
		/// <param name="sourceType">The Type of the records in the source File.</param>
		/// <param name="destType">The Type of the records in the dest File.</param>
		/// <param name="sourceFile">The file with records to be transformed</param>
		/// <param name="destFile">The destination file with the transformed records</param>
		/// <returns>The transformed records.</returns>
		public static object[] TransformFile(string sourceFile, Type sourceType, string destFile, Type destType)
		{
			FileTransformEngine engine = new FileTransformEngine(sourceType, destType);
			return engine.TransformFile(sourceFile, destFile);
		}


		/// <summary>
		/// Read the contents of a file and sort the records.
		/// </summary>
		/// <param name="recordClass">Record Class (remember that need to implement the IComparer interface, or you can use SortFileByfield)</param>
		/// <param name="fileName">The file to read.</param>
		public static object[] ReadSortedFile(Type recordClass, string fileName)
		{
			if (typeof(IComparable).IsAssignableFrom(recordClass) == false)
				throw new BadUsageException("The record class must implement the interface IComparable to use the Sort feature.");

			FileHelperEngine engine = new FileHelperEngine(recordClass);
			object[] res = engine.ReadFile(fileName);

			if (res.Length == 0)
				return res;

			Array.Sort(res);
			return res;
		}

		/// <summary>
		/// Sort the contents of the source file and write them to the destination file. 
		/// </summary>
		/// <param name="recordClass">Record Class (remember that need to implement the IComparable interface or use the SortFileByfield instead)</param>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="sortedFile">The destination File.</param>
		public static void SortFile(Type recordClass, string sourceFile, string sortedFile)
		{
			if (typeof(IComparable).IsAssignableFrom(recordClass) == false)
				throw new BadUsageException("The record class must implement the interface IComparable to use the Sort feature.");

			FileHelperEngine engine = new FileHelperEngine(recordClass);
			object[] res = engine.ReadFile(sourceFile);

			if (res.Length == 0)
				engine.WriteFile(sortedFile, res);

			Array.Sort(res);
			engine.WriteFile(sortedFile, res);
		}

		/// <summary>
		/// Sort the content of a File using the field name provided
		/// </summary>
		/// <param name="recordClass">The class for each record of the file.</param>
		/// <param name="fieldName">The name of the field used to sort the file.</param>
		/// <param name="asc">The sort direction.</param>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="sortedFile">The destination File.</param>
		public static void SortFileByField(Type recordClass, string fieldName, bool asc, string sourceFile, string sortedFile)
		{

			FileHelperEngine engine = new FileHelperEngine(recordClass);
			FieldInfo fi = engine.mRecordInfo.GetFieldInfo(fieldName);

			if (fi == null)
				throw new BadUsageException("The record class not contains the field " + fieldName);

			object[] res = engine.ReadFile(sourceFile);

			IComparer comparer = new FieldComparer(fi, asc);
			Array.Sort(res, comparer);

			engine.WriteFile(sortedFile, res);
		}

		/// <summary>
		/// Sort the Record Array based in the field name provided. (for advanced sorting use SortRecords)
		/// </summary>
		/// <param name="fieldName">The field name.</param>
		/// <param name="records">The records Array.</param>
		public static void SortRecordsByField(object[] records, string fieldName)
		{
			SortRecordsByField(records, fieldName, true);
		}

		/// <summary>
		/// Sort the Record Array based in the field name provided. (for advanced sorting use SortRecords)
		/// </summary>
		/// <param name="fieldName">The field name.</param>
		/// <param name="records">The records Array.</param>
		/// <param name="ascending">The direction of the sort. True means Ascending.</param>
		public static void SortRecordsByField(object[] records, string fieldName, bool ascending)
		{
			if (records.Length > 0 && records[0] != null)
			{
				FileHelperEngine engine = new FileHelperEngine(records[0].GetType());
				FieldInfo fi = engine.mRecordInfo.GetFieldInfo(fieldName);

				if (fi == null)
					throw new BadUsageException("The record class not contains the field " + fieldName);

				IComparer comparer = new FieldComparer(fi, ascending);
				Array.Sort(records, comparer);
			}
		}

		/// <summary>
		/// Sort the Record Array. The records must be of a Type that implements the IComparable interface.
		/// </summary>
		/// <param name="records">The records Array.</param>
		public static void SortRecords(object[] records)
		{
			if (records.Length > 0 && records[0] != null)
			{
				Type recordClass = records[0].GetType();

				if (typeof(IComparable).IsAssignableFrom(recordClass) == false)
					throw new BadUsageException("The record class must implement the interface IComparable to use the Sort feature.");

				Array.Sort(records);
			}
		}

		#region "  FieldComparer  "

		internal class FieldComparer : IComparer
		{
			FieldInfo mFieldInfo;
			int mAscending;
			
			public FieldComparer(FieldInfo fi, bool asc)
			{
				mFieldInfo = fi;
				mAscending = asc ? 1 : -1;
				if (typeof(IComparable).IsAssignableFrom(mFieldInfo.FieldType) == false)
					throw new BadUsageException("The field " + mFieldInfo.Name + " need to implement the interface IComparable");

			}

			public int Compare(object x, object y)
			{
				IComparable xv = mFieldInfo.GetValue(x) as IComparable;
				return xv.CompareTo(mFieldInfo.GetValue(y)) * mAscending;
			}
		}

		#endregion

		
#if ! MINI
		/// <summary>Converts any collection of records to a DataTebla using reflection. WARNING: this methods returns null if the number of records is 0, pass the Type of the records to get an empty DataTable.</summary>
		/// <param name="records">The records to be converted to a DataTable</param>
		/// <returns>The datatable containing the records as DataRows</returns>
		public DataTable RecordsToDataTable(ICollection records)
		{

			RecordInfo ri = null;
			foreach (object obj in records)
			{
				if (obj != null)
				{
                    ri = new RecordInfo(obj.GetType());
				}
			}

			if (ri == null)
				return new DataTable();

			return ri.RecordsToDataTable(records);
		}

		/// <summary>Converts any collection of records to a DataTebla using reflection. If the number of records is 0 this methods returns an empty DataTable with the columns based on the fields of the Type.</summary>
		/// <param name="records">The records to be converted to a DataTable</param>
		/// <returns>The datatable containing the records as DataRows</returns>
		/// <param name="recordType">The type of the inner records.</param>
		public DataTable RecordsToDataTable(ICollection records, Type recordType)
		{
			RecordInfo ri = new RecordInfo(recordType);
			return ri.RecordsToDataTable(records);
		}

#endif

		/// <summary>
		/// Reads the file1 and file2 using the recordType and write it to destinationFile
		/// </summary>
		public void MergeFiles(Type recordType, string file1, string file2, string destinationFile)
		{
			FileHelperAsyncEngine engineRead= new FileHelperAsyncEngine(recordType);
			FileHelperAsyncEngine engineWrite = new FileHelperAsyncEngine(recordType);
			
			engineWrite.BeginWriteFile(destinationFile);

			object[] readRecords;

			// Read FILE 1
			engineRead.BeginReadFile(file1);
			
			readRecords = engineRead.ReadNexts(50);
			while(readRecords.Length > 0)
			{
				engineWrite.WriteNexts(readRecords);
				readRecords = engineRead.ReadNexts(50);
			}
			engineRead.EndsRead();

			// Read FILE 2
			engineRead.BeginReadFile(file2);

			readRecords = engineRead.ReadNexts(50);
			while(readRecords.Length > 0)
			{
				engineWrite.WriteNexts(readRecords);
				readRecords = engineRead.ReadNexts(50);
			}
			engineRead.EndsRead();
			
			engineWrite.EndsWrite();
		} 

		/// <summary>
		/// Reads the file1 and file2 using the recordType and write it to destFile sorting it using the specified field.
		/// </summary>
		/// <param name="recordType"></param>
		/// <param name="file1"></param>
		/// <param name="file2"></param>
		/// <param name="destFile"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public object[] MergeAndSortFile(Type recordType, string file1, string file2, string destFile, string field)
		{
			return MergeAndSortFile(recordType, file1, file2, destFile, field, true);
		}

		public object[] MergeAndSortFile(Type recordType, string file1, string file2, string destinationFile, string field, bool ascending)
		{
			FileHelperEngine engine = new FileHelperEngine(recordType);

			ArrayList arr = new ArrayList();

			arr.AddRange(engine.ReadFile(file1));
			arr.AddRange(engine.ReadFile(file2));
            
			object[] res = (object[]) arr.ToArray(recordType);
			arr = null; // <- better performance (memory)

			CommonEngine.SortRecordsByField(res, field, ascending);

			return res;
		}	

		public object[] MergeAndSortFile(Type recordType,string file1, string file2, string destinationFile)
		{
			FileHelperEngine engine = new FileHelperEngine(recordType);

			ArrayList arr = new ArrayList();

			arr.AddRange(engine.ReadFile(file1));
			arr.AddRange(engine.ReadFile(file2));
            
			object[] res = (object[]) arr.ToArray(recordType);
			arr = null; // <- better performance (memory)
			
			CommonEngine.SortRecords(res);

			return res;
		}	
		
		/// <summary>Simply dumps the DataTable contents to a delimited file using a ',' as delimiter.</summary>
		/// <param name="dt">The source Data Table</param>
		/// <param name="filename">The destination file.</param>
		public static void DataTableToCSV(DataTable dt, string filename)
		{
			DataTableToCSV(dt, filename, ",");
		}


		/// <summary>Simply dumps the DataTable contents to a delimited file. Only allows to set the delimiter.</summary>
		/// <param name="dt">The source Data Table</param>
		/// <param name="filename">The destination file.</param>
		/// <param name="delimiter">The delimiter used to write the file</param>
		public static void DataTableToCSV(DataTable dt, string filename, string delimiter)
		{
			using (StreamWriter fs = new StreamWriter(filename, false))
			{
				foreach (DataRow dr in dt.Rows)
				{
					object[] fields = dr.ItemArray;
					
					for(int i = 0; i < fields.Length; i++)
					{
						if (i > 0)
							fs.Write(delimiter);
						
						fs.Write(ObjectString(fields[i]));
					}
					fs.Write(StringHelper.NewLine);
				}
				fs.Close();
			}
		}

		private static string ObjectString(object o)
		{
			if (o == null)
				return string.Empty;
			else
				return o.ToString();
		}

	
		#region "  RemoveDuplicateRecords  "

		public static IComparableRecord[] RemoveDuplicateRecords(IComparableRecord[] arr)
		{
			if (arr == null || arr.Length == 0)
				return arr;

			ArrayList nodup = new ArrayList();

			for(int i = 0; i < arr.Length; i++)
			{
				bool isUnique = true; 
				
				for(int j = i+1; j < arr.Length; j++)
				{
					if (arr[i].IsEqualRecord(arr[j]))
					{
						isUnique = false;
						break;
					}
				}

				if (isUnique) nodup.Add(arr[i]); 
			}

			return (IComparableRecord[]) nodup.ToArray(arr[1].GetType());
    
		}

		#endregion


	}
}