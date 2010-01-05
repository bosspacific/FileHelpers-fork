

using System;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;
using FileHelpers.Events;
using Container=FileHelpers.IoC.Container;

namespace FileHelpers
{
	/// <summary>Base class for the two engines of the library: <see cref="FileHelperEngine"/> and <see cref="FileHelperAsyncEngine"/></summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class EngineBase 
        //#if ! MINI
        //:Component
        //#endif
	{
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        
        internal readonly IRecordInfo mRecordInfo;

		#region "  Constructor  "

		internal EngineBase(Type recordType):this(recordType, Encoding.Default)
		{}

		internal EngineBase(Type recordType, Encoding encoding)
		{
			if (recordType == null)
				throw new BadUsageException("The record type can't be null");
			if (recordType.IsValueType)
				throw new BadUsageException("The record type must be a class not a struct.");

			mRecordType = recordType;
		    mRecordInfo = Container.Resolve<IRecordInfo>(recordType);
		    mEncoding = encoding;
		}

		internal EngineBase(RecordInfo ri)
		{
			mRecordType = ri.RecordType;
			mRecordInfo = ri;
		}

		
		#endregion

		#region "  LineNumber  "


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal int mLineNumber;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal int mTotalRecords;

		/// <include file='FileHelperEngine.docs.xml' path='doc/LineNum/*'/>
		public int LineNumber
		{
			get { return mLineNumber; }
		}

		#endregion

		#region "  TotalRecords  "

		/// <include file='FileHelperEngine.docs.xml' path='doc/TotalRecords/*'/>
		public int TotalRecords
		{
			get { return mTotalRecords; }
		}

		#endregion

		#region "  RecordType  "

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type mRecordType;

		/// <include file='FileHelperEngine.docs.xml' path='doc/RecordType/*'/>
		public Type RecordType
		{
			get { return mRecordType; }
		}

		#endregion

		#region "  HeaderText  "

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string mHeaderText = String.Empty;

		/// <summary>The read header in the last read operation. If any.</summary>
		public string HeaderText
		{
			get { return mHeaderText; }
			set { mHeaderText = value; }
		}

		#endregion

		#region "  FooterText"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string mFooterText = String.Empty;

		/// <summary>The read footer in the last read operation. If any.</summary>
		public string FooterText
		{
			get { return mFooterText; }
			set { mFooterText = value; }
		}

		#endregion

		#region "  Encoding  "

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal Encoding mEncoding = Encoding.Default;

        /// <summary>The encoding to Read and Write the streams. Default is the system's current ANSI code page.</summary>
		/// <value>Default is the system's current ANSI code page.</value>
		public Encoding Encoding
		{
			get { return mEncoding; }
			set { mEncoding = value; }
		}

		#endregion

		#region "  ErrorManager"

        /// <summary>This is a common class that manage the errors of the library.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected ErrorManager mErrorManager = new ErrorManager();

		/// <summary>This is a common class that manage the errors of the library.</summary>
		/// <remarks>You can, for example, get the errors, their number, Save them to a file, etc.</remarks>
		public ErrorManager ErrorManager
		{
			get { return mErrorManager; }
		}


	    /// <summary>Indicates the behavior of the engine when it found an error. (shortcut for ErrorManager.ErrorMode)</summary>
	    public ErrorMode ErrorMode
	    {
	        get { return mErrorManager.ErrorMode; }
	        set { mErrorManager.ErrorMode = value; }
	    }

	    #endregion

		#region "  ResetFields  "

		internal void ResetFields()
		{
			mLineNumber = 0;
			mErrorManager.ClearErrors();
			mTotalRecords = 0;
		}

		#endregion

		#if ! MINI

        /// <summary>Called to notify progress.</summary>
        public event EventHandler<ProgressEventArgs> Progress;

        /// <summary>
        /// Raises the Progress Event
        /// </summary>
        /// <param name="e">The Event Args</param>
        protected void OnProgress(ProgressEventArgs e)
        {
            if (Progress == null)
                return;

            Progress(this, e);
        }
#endif

	}
}

