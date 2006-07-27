using System;
using System.ComponentModel;
using System.Text;

namespace FileHelpers.RunTime
{
	/// <summary>Base class for the field converters</summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class FieldBuilder
	{
		private string mFieldName;
		private string mFieldType;

		internal FieldBuilder(string fieldName, Type fieldType)
		{
			mFieldName = fieldName;
			mFieldType = fieldType.FullName;
		}

		internal FieldBuilder(string fieldName, string fieldType)
		{
			mFieldName = fieldName;
			mFieldType = fieldType;
		}

		#region TrimMode

		private TrimMode mTrimMode = TrimMode.None;

		/// <summary>Indicates the TrimMode for the field.</summary>
		public TrimMode TrimMode
		{
			get { return mTrimMode; }
			set { mTrimMode = value; }
		}
		
		private string mTrimChars = " \t";
		
		/// <summary>Indicates the trim chars used if TrimMode is set.</summary>
		public string TrimChars
		{
			get { return mTrimChars; }
			set { mTrimChars = value; }
		}

		#endregion

		

		internal int mFieldIndex = -1;

		/// <summary>The position index inside the class.</summary>
		public int FieldIndex
		{
			get { return mFieldIndex; }
		}

		private bool mFieldInNewLine = false;

		/// <summary>Indicates that this field is at the begging of a new line.</summary>
		public bool FieldInNewLine
		{
			get { return mFieldInNewLine; }
			set { mFieldInNewLine = value; }
		}

		private bool mFieldIgnored = false;

		/// <summary>Indicates that this field must be ignored by the engine.</summary>
		public bool FieldIgnored
		{
			get { return mFieldIgnored; }
			set { mFieldIgnored = value; }
		}

		private bool mFieldOptional = false;

		/// <summary>Indicates that this field is optional.</summary>
		public bool FieldOptional
		{
			get { return mFieldOptional; }
			set { mFieldOptional = value; }
		}

		/// <summary>Uset to create the converter for the current field.</summary>
		public ConverterBuilder Converter
		{
			get { return mConverter; }
		}

		/// <summary>The name of the field.</summary>
		public string FieldName
		{
			get { return mFieldName; }
			set { mFieldName = value; }
		}

		/// <summary>The Type of the field</summary>
		public string FieldType
		{
			get { return mFieldType; }
			set { mFieldType = value; }
		}

		private object mFieldNullValue = null;

		/// <summary>The null value of the field when their value not is in the file.</summary>
		public object FieldNullValue
		{
			get { return mFieldNullValue; }
			set { mFieldNullValue = value; }
		}


		private ConverterBuilder mConverter = new ConverterBuilder();
		
		internal string GetFieldCode(NetLanguage leng)
		{
			StringBuilder sb = new StringBuilder(100);
			
			AttributesBuilder attbs = new AttributesBuilder(leng);
			
			AddAttributesInternal(attbs, leng);
			AddAttributesCode(attbs, leng);
			
			sb.Append(attbs.GetAttributesCode());
			
			switch (leng)
			{
				case NetLanguage.VbNet:
					sb.Append("Public " + mFieldName + " As " + mFieldType);
					break;
				case NetLanguage.CSharp:
					sb.Append("public " + mFieldType + " " + mFieldName+ ";");
					break;
				default:
					break;
			}

			sb.Append(StringHelper.NewLine);
			
			return sb.ToString();
		}
		
		
		internal abstract void AddAttributesCode(AttributesBuilder attbs, NetLanguage leng);

		private void AddAttributesInternal(AttributesBuilder attbs, NetLanguage leng)
		{

			if (mFieldOptional == true)
				attbs.AddAttribute("FieldOptional()");

			if (mFieldIgnored == true)
				attbs.AddAttribute("FieldIgnored()");

			if (mFieldInNewLine == true)
				attbs.AddAttribute("FieldInNewLine()");


			if (mFieldNullValue != null)
			{
				string t = mFieldNullValue.GetType().FullName;
				string gt = string.Empty;
				if (leng == NetLanguage.CSharp)
					gt = "typeof(" + t + ")";
				else if (leng == NetLanguage.VbNet)
					gt = "GetType(" + t + ")";

				attbs.AddAttribute("FieldNullValue("+ gt +", \""+ mFieldNullValue.ToString() +"\")");
			}

			
		
			attbs.AddAttribute(mConverter.GetConverterCode(leng));
			
			if (mTrimMode != TrimMode.None)
			{
				attbs.AddAttribute("FieldTrim(TrimMode."+ mTrimMode.ToString()+", \""+ mTrimChars.ToString() +"\")");
			}
		}

		private NetVisibility mVisibility = NetVisibility.Public;

		public NetVisibility Visibility
		{
			get { return mVisibility; }
			set { mVisibility = value; }
		}
		
	}
}
