#region "  � Copyright 2005-06 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcosdotnet[at]yahoo.com.ar.

#endregion

using System.Reflection;

namespace FileHelpers
{
	internal sealed class FixedLengthField : FieldBase
	{
		#region "  Properties  "

		internal int mFieldLength;
		internal FieldAlignAttribute mAlign = new FieldAlignAttribute(AlignMode.Left, ' ');

		#endregion

		#region "  Constructor  "

		internal FixedLengthField(FieldInfo fi, int length, FieldAlignAttribute align) : base(fi)
		{
			this.mFieldLength = length;

			if (align != null)
				this.mAlign = align;
		}

		#endregion

		#region "  Overrides String Handling  "

		protected override ExtractedInfo ExtractFieldString(LineInfo line)
		{
			if (line.CurrentLength == 0)
			{
				if (mIsOptional)
					return ExtractedInfo.Empty;
				else
					throw new BadUsageException("End Of Line found processing the field: " + mFieldInfo.Name + " at line "+ line.mReader.LineNumber.ToString() + ". (You need to mark it as [FieldOptional] if you want to avoid this exception)");
			}
			
			ExtractedInfo res;

			if (line.CurrentLength < this.mFieldLength)
				if (mFixedMode == FixedMode.AllowLessChars || mFixedMode == FixedMode.AllowVariableLength)
					res = new ExtractedInfo(line);
				else
					throw new BadUsageException("The string '" + line.CurrentString + "' (length " + line.CurrentLength.ToString() + ") at line "+ line.mReader.LineNumber.ToString() + " has less chars than the defined for " + mFieldInfo.Name + " (" + mFieldLength.ToString() + "). You can use the [FixedLengthRecord(FixedMode.AllowLessChars)] to avoid this problem.");
			else if (mIsLast && line.CurrentLength > mFieldLength && mFixedMode != FixedMode.AllowMoreChars && mFixedMode != FixedMode.AllowVariableLength)
				throw new BadUsageException("The string '" + line.CurrentString + "' (length " + line.CurrentLength.ToString() + ") at line "+ line.mReader.LineNumber.ToString() + " has more chars than the defined for the last field " + mFieldInfo.Name + " (" + mFieldLength.ToString() + ").You can use the [FixedLengthRecord(FixedMode.AllowMoreChars)] to avoid this problem.");
			else
				res = new ExtractedInfo(line, line.mCurrentPos + mFieldLength);

			return res;
		}

		protected override string CreateFieldString(object record)
		{
			string res;
			res = base.CreateFieldString(record);

			if (res.Length > this.mFieldLength)
			{
				res = res.Substring(0, this.mFieldLength);
			}

			if (mAlign.Align == AlignMode.Left)
				res = res.PadRight(mFieldLength, mAlign.AlignChar);
			else if (mAlign.Align == AlignMode.Right)
				res = res.PadLeft(mFieldLength, mAlign.AlignChar);
			else
			{
				int middle = (mFieldLength - res.Length)/2;
				if (middle > 0)
					res = res.PadLeft(mFieldLength - middle, mAlign.AlignChar).PadRight(mFieldLength, mAlign.AlignChar);
			}

			return res;
		}

		#endregion
	}
}