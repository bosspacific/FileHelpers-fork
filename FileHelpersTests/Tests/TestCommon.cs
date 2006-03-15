using FileHelpers;
using FileHelpers.MasterDetail;
using System.Collections;


namespace FileHelpersTests
{
	// this class only adds the relative path to the saple files.
	public sealed class TestCommon
	{
		public static string TestPath(string fileName)
		{
			return @"..\data\" + fileName;
		}

		public static object[] ReadTest(FileHelperEngine engine, string fileName)
		{
			return engine.ReadFile(@"..\data\" + fileName);
		}

		public static object[] ReadAllAsync(FileHelperAsyncEngine engine, string fileName)
		{
			ArrayList arr = new ArrayList();
			
			engine.BeginReadFile(@"..\data\" + fileName);
			while(engine.ReadNext() != null)
				arr.Add(engine.LastRecord);
			engine.EndsRead();

			return arr.ToArray();

		}

		public static MasterDetails[] ReadTest(MasterDetailEngine engine, string fileName)
        {
            return engine.ReadFile(@"..\data\" + fileName);
        }

        public static void BeginReadTest(FileHelperAsyncEngine engine, string fileName)
		{
			engine.BeginReadFile(@"..\data\" + fileName);
		}


		private TestCommon()
		{
		}
	}
}