using System.Collections;
using System.Text;
using FileHelpers;
using NUnit.Framework;

namespace FileHelpersTests.Common
{
	[TestFixture]
	public class FileEncoding
	{
		FileHelperEngine engine;
		FileHelperAsyncEngine asyncEngine;

		private const int ExpectedRecords = 7;

		private void RunTests(string fileName, Encoding enc)
		{
			engine = new FileHelperEngine(typeof (CustomersVerticalBar));
			engine.Encoding = enc;
			Assert.AreEqual(enc, engine.Encoding);

			CustomersVerticalBar[] res = (CustomersVerticalBar[]) TestCommon.ReadTest(engine, fileName);

			Assert.AreEqual(ExpectedRecords, res.Length);
			Assert.AreEqual(ExpectedRecords, engine.TotalRecords);

			Assert.AreEqual("Ana Tru�i�o Emparedados y helados", res[1].CompanyName);
			Assert.AreEqual("Blondesddsl p�re et fils", res[6].CompanyName);
			Assert.AreEqual("Fr�d�rique Citeaux", res[6].ContactName);

			Assert.AreEqual("24, place Kl�ber", res[6].Address);
			Assert.AreEqual("Berguvsv�gen  8", res[4].Address);
			Assert.AreEqual("Lule�", res[4].City);
		}

		private void RunAsyncTests(string fileName, Encoding enc)
		{
			asyncEngine = new FileHelperAsyncEngine(typeof (CustomersVerticalBar));
			asyncEngine.Encoding = enc;

			ArrayList arr = new ArrayList();

			TestCommon.BeginReadTest(asyncEngine, fileName);

			while (asyncEngine.ReadNext() != null)
			{
				arr.Add(asyncEngine.LastRecord);
			}

			CustomersVerticalBar[] res = (CustomersVerticalBar[]) arr.ToArray(typeof (CustomersVerticalBar));
			Assert.AreEqual(ExpectedRecords, res.Length);
			Assert.AreEqual(ExpectedRecords, engine.TotalRecords);

			Assert.AreEqual("Ana Tru�i�o Emparedados y helados", res[1].CompanyName);
			Assert.AreEqual("Blondesddsl p�re et fils", res[6].CompanyName);
			Assert.AreEqual("Fr�d�rique Citeaux", res[6].ContactName);

			Assert.AreEqual("24, place Kl�ber", res[6].Address);
			Assert.AreEqual("Berguvsv�gen  8", res[4].Address);
			Assert.AreEqual("Lule�", res[4].City);
		}

		[Test]
		public void EncodingANSI()
		{
			RunTests(@"Good\EncodingANSI.txt", Encoding.Default);
		}

		[Test]
		public void EncodingUTF8()
		{
			RunTests(@"Good\EncodingUTF8.txt", Encoding.UTF8);
		}

		[Test]
		public void EncodingUnicode()
		{
			RunTests(@"Good\EncodingUnicode.txt", Encoding.Unicode);
		}

		[Test]
		public void EncodingAsyncUnicodeBig()
		{
			RunAsyncTests(@"Good\EncodingUnicodeBig.txt", Encoding.BigEndianUnicode);
		}

		[Test]
		public void EncodingAsyncANSI()
		{
			RunAsyncTests(@"Good\EncodingANSI.txt", Encoding.Default);
		}

		[Test]
		public void EncodingAsyncUTF8()
		{
			RunAsyncTests(@"Good\EncodingUTF8.txt", Encoding.UTF8);
		}

		[Test]
		public void EncodingAsyncUnicode()
		{
			RunAsyncTests(@"Good\EncodingUnicode.txt", Encoding.Unicode);
		}

		[Test]
		public void EncodingUnicodeBig()
		{
			RunTests(@"Good\EncodingUnicodeBig.txt", Encoding.BigEndianUnicode);
		}


	}
}