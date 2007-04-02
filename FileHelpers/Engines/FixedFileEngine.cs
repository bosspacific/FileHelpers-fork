using System;

namespace FileHelpers
{

	/// <summary>
	/// Is a version of the <see cref="FileHelperEngine"/> exclusive for 
	/// fixed length records that allow you to change the delimiter an other options at runtime
	/// </summary>
	/// <remarks>
	/// Useful when you need to export or import the same info with little different options.
	/// </remarks>
	public sealed class FixedFileEngine : FileHelperEngine
	{

        #region "  Constructor  "

		/// <summary>
		/// Creates a version of the <see cref="FileHelperEngine"/> exclusive for 
		/// fixed length records that allow you to change the delimiter an other options at runtime
		/// </summary>
		/// <remarks>
		/// Useful when you need to export or import the same info with little different options.
		/// </remarks>
		/// <param name="recordType">The record mapping class.</param>
		public FixedFileEngine(Type recordType)
			: base(recordType)
		{
			if (mRecordInfo.mFields[0] is FixedLengthField  == false)
				throw new BadUsageException("The FixedFileEngine only accepts Record Types marked with FixedLengthRecord attribute");
		}

		#endregion
		
		/// <summary>Allow changes some fixed length options and others common settings.</summary>
		public new FixedRecordOptions DynamicOptions
		{
			get { return (FixedRecordOptions) mDynamicOptions; }
		}
	}


#if NET_2_0

	/// <summary>
	/// Is a version of the <see cref="FileHelperEngine"/> exclusive for 
	/// fixed length records that allow you to change the delimiter an other options at runtime
	/// </summary>
	/// <remarks>
	/// Useful when you need to export or import the same info with little different options.
	/// </remarks>
	public sealed class FixedFileEngine<T> : FileHelperEngine<T>
	{
	#region "  Constructor  "

		/// <summary>
		/// Creates a version of the <see cref="FileHelperEngine"/> exclusive for 
		/// fixed length records that allow you to change the delimiter an other options at runtime
		/// </summary>
		/// <remarks>
		/// Useful when you need to export or import the same info with little different options.
		/// </remarks>
		public FixedFileEngine()
			: base()
		{
			if (mRecordInfo.mFields[0] is FixedLengthField  == false)
				throw new BadUsageException("The FixedFileEngine only accepts Record Types marked with FixedLengthRecord attribute");
		}

	#endregion

		
		/// <summary>Allow changes some fixed length options and others common settings.</summary>
		public new FixedRecordOptions DynamicOptions
		{
            get { return (FixedRecordOptions)mDynamicOptions; }
		}
	}
#endif
}
