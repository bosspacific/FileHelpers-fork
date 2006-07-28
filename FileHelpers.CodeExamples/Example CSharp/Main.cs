using System;
using System.IO;
using FileHelpers;

namespace Examples
{
	class MainClass
	{
		[STAThread]
		static void Main()
		{
			Delimited();

			FixedLength();

			ErrorHandling();

			FormatTransformation();

			EventHandling();

		}

		#region "  Delimited  "

		static void Delimited()
		{

            Console.WriteLine("Reading Delimited File...");
			Console.WriteLine();

			// Estas dos lineas son el uso de la librer�a
			FileHelperEngine engine = new FileHelperEngine(typeof(Customer));
			Customer[] customers = (Customer[]) engine.ReadFile(@"..\Data\CustomersDelimited.txt");

			// A partir de la versi�n 1.4.0 se puede
			// inclusive escribir en una sola l�nea:

			// Cliente[] clientes = (Cliente[]) CommonEngine.ReadFile(typeof(Cliente), @"..\Data\ClientesDelimitados.txt");

			// Aqui es donde ustedes agregan su c�digo
			foreach (Customer cli in customers)
			{
				Console.WriteLine();
				Console.WriteLine("Customer: " + cli.CustId.ToString() + " - " + cli.Name);
				Console.WriteLine("Added Date: " + cli.AddedDate.ToString("d-M-yyyy"));
				Console.WriteLine("Balance: " + cli.Balance.ToString());
				Console.WriteLine();
				Console.WriteLine("-----------------------------");
			}

			Console.ReadLine();
			Console.WriteLine("Writing data to a delimited file...");
			Console.WriteLine();

			// write the data to a file
			engine.WriteFile("temp.txt", customers);

            Console.WriteLine("Data successful written !!!");
			Console.ReadLine();

			if (File.Exists("temp.txt")) File.Delete("temp.txt");
		}

		#endregion

		#region "  FixedLength  "

		static void FixedLength()
		{
			
			Console.WriteLine("Reading Fixed Length file...");
			Console.WriteLine();



			// Estas dos lineas son el uso de la librer�a
			FileHelperEngine engine = new FileHelperEngine(typeof(Customer2));
			Customer2[] clientes = (Customer2[]) engine.ReadFile(@"..\Data\CustomersFixedLength.txt");


			// A partir de la versi�n 1.4.0 se puede
			// inclusive escribir en una sola l�nea:

			// Cliente[] clientes = (Cliente[]) CommonEngine.ReadFile(typeof(Cliente), @"..\Data\ClientesDelimitados.txt");

			// Aqui es donde ustedes agregan su c�digo
			foreach (Customer2 cli in clientes)
			{
				Console.WriteLine();
				Console.WriteLine("Customer: " + cli.CustId.ToString() + " - " + cli.Name);
				Console.WriteLine("Added Date: " + cli.AddedDate.ToString("d-M-yyyy"));
				Console.WriteLine("Balance: " + cli.Balance.ToString());
				Console.WriteLine();
				Console.WriteLine("-----------------------------");
			}

			Console.ReadLine();
			Console.WriteLine("Writing data to a Fixed Length file...");
			Console.WriteLine();

			// Esta l�nea es para escribir
			engine.WriteFile("temp.txt", clientes);

			Console.WriteLine("Data successful written !!!");
			Console.ReadLine();

			if (File.Exists("temp.txt")) File.Delete("temp.txt");
		}

		#endregion

		#region "  FormatTransformation  "

		static void FormatTransformation()
		{
			Console.WriteLine("Testing Format Transformation");
			Console.WriteLine();

			FileTransformEngine engine = new FileTransformEngine(typeof(Customer), typeof(Customer2));
			engine.TransformFile(@"..\Data\CustomersDelimited.txt", "temp.txt");


			Console.WriteLine("Format Transformation Successful");

			if (File.Exists("temp.txt")) File.Delete("temp.txt");
			Console.ReadLine();

		}

		#endregion

		#region "  ErrorHandling  "

		static void ErrorHandling()
		{

			Console.WriteLine("Testing error handling...");
			Console.WriteLine();

			// Estas dos lineas son el uso de la librer�a
			FileHelperEngine engine = new FileHelperEngine(typeof(Customer));
			
			engine.ErrorManager.ErrorMode = ErrorMode.SaveAndContinue;
            
			Customer[] customers = (Customer[]) engine.ReadFile(@"..\Data\CustomersWithErrors.txt");

			if (engine.ErrorManager.ErrorCount > 0)
			{
				Console.Write("Records: ");
				Console.WriteLine(engine.TotalRecords);

				Console.Write("Successful: ");
				Console.WriteLine(customers.Length);
				
				Console.Write("With Error: ");
				Console.WriteLine(engine.ErrorManager.ErrorCount);

				Console.Write("Error: ");
				Console.WriteLine(engine.ErrorManager.Errors[0].ExceptionInfo.Message);

			}

			engine.ErrorManager.SaveErrors("errors.txt");
		
			Console.ReadLine();

			if (File.Exists("errors.txt")) File.Delete("errors.txt");


		}

		#endregion

		#region "  Delimited  "

		static void EventHandling()
		{

			Console.WriteLine("Reading Using EventHandlers ...");
			Console.WriteLine();

			// Estas dos lineas son el uso de la librer�a
			FileHelperEngine engine = new FileHelperEngine(typeof(Customer));

			engine.BeforeReadRecord += new BeforeReadRecordHandler(BeforeReadRecord);
			engine.AfterWriteRecord +=new AfterWriteRecordHandler(AfterWriteRecord);

			Customer[] customers = (Customer[]) engine.ReadFile(@"..\Data\CustomersDelimited.txt");

			// A partir de la versi�n 1.4.0 se puede
			// inclusive escribir en una sola l�nea:

			// Cliente[] clientes = (Cliente[]) CommonEngine.ReadFile(typeof(Cliente), @"..\Data\ClientesDelimitados.txt");

			// Aqui es donde ustedes agregan su c�digo
			foreach (Customer cli in customers)
			{
				Console.WriteLine("Customer: " + cli.CustId.ToString() + " - " + cli.Name);
			}

			Console.ReadLine();
			Console.WriteLine("Writing data to a delimited file...");
			Console.WriteLine();

			// write the data to a file
			engine.WriteFile("temp.txt", customers);

			Console.WriteLine("Data successful written !!!");
			Console.ReadLine();

			if (File.Exists("temp.txt")) File.Delete("temp.txt");
		}

		#endregion

		private static void BeforeReadRecord(EngineBase engine, BeforeReadRecordEventArgs e)
		{
			Console.WriteLine("--> Before read line: " + e.RecordLine); 

			if (e.LineNumber == 2)
			{
				e.SkipThisRecord = true;
				Console.WriteLine("-->   skiping line 2"); 
			}
		}

		private static void AfterWriteRecord(EngineBase engine, AfterWriteRecordEventArgs e)
		{
			Console.WriteLine("--> After write record: " + e.RecordLine); 
		}
	}
}
