#region "  � Copyright 2005-06 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcosdotnet[at]yahoo.com.ar.

#endregion

#if ! MINI
using System;
using System.Data;
using System.Data.SqlClient;

namespace FileHelpers.DataLink
{
	/// <summary>This is a base class that implements the <see cref="DataStorage"/> for Microsoft SqlServer.</summary>
	public sealed class SqlServerStorage : DatabaseStorage
	{

		/// <summary>Create a new instance of the SqlServerStorage based on the record type provided.</summary>
		/// <param name="recordType">The type of the record class.</param>
		public SqlServerStorage(Type recordType): base(recordType)
		{}

		/// <summary>Create a new instance of the SqlServerStorage based on the record type provided.</summary>
		/// <param name="recordType">The type of the record class.</param>
		/// <param name="server">The server name or IP of the sqlserver</param>
		/// <param name="database">The database name into the server.</param>
		public SqlServerStorage(Type recordType, string server, string database): base(recordType)
		{
			mServerName = server;
			mDatabaseName = database;
		}

		public SqlServerStorage(Type recordType, string server, string database, string user, string pass): this(recordType, server, database)
		{
			mUserName = user;
			mUserPass = pass;
		}

		#region "  Create Connection and Command  "

		/// <summary>Must create an abstract connection object.</summary>
		/// <returns>An Abstract Connection Object.</returns>
		protected sealed override IDbConnection CreateConnection()
		{
			string conString = DataBaseHelper.SqlConnectionString(ServerName, DatabaseName, UserName, UserPass);
			return new SqlConnection(conString);
		}

		/// <summary>Must create an abstract command object.</summary>
		/// <returns>An Abstract Command Object.</returns>
		protected sealed override IDbCommand CreateCommand()
		{
			return new SqlCommand();
		}

		#endregion

		internal override bool ExecuteInBatch
		{
			get { return true; }
		}


		#region "  Connection Properties  "

		private string mServerName = string.Empty;

		/// <summary> The server name or IP of the SqlServer </summary>
		public string ServerName
		{
			get { return mServerName; }
			set { mServerName = value; }
		}

		private string mDatabaseName = string.Empty;
		/// <summary> The name of the database. </summary> 
		public string DatabaseName
		{
			get { return mDatabaseName; }
			set { mDatabaseName = value; }
		}

		private string mUserName = string.Empty;
		/// <summary> The user name used to logon into the SqlServer. (leave empty for WindowsAuth)</summary>
		public string UserName
		{
			get { return mUserName; }
			set { mUserName = value; }
		}

		private string mUserPass = string.Empty;

		/// <summary> The user pass used to logon into the SqlServer. (leave empty for WindowsAuth)</summary>
		public string UserPass
		{
			get { return mUserPass; }
			set { mUserPass = value; }
		}
		#endregion
	}
}

#endif