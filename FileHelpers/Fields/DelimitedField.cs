#region "  � Copyright 2005-07 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace FileHelpers
{
	internal sealed class DelimitedField : FieldBase
	{

		#region "  Constructor  "

        private static CompareInfo mCompare = StringHelper.CreateComparer();
        
        internal DelimitedField(FieldInfo fi, string sep)
            : base(fi)
		{
            Separator = sep; // string.Intern(sep);
		}

		#endregion

		#region "  Properties  "

		private string mSeparator;

		internal string Separator
		{
			get { return mSeparator; }
			set
			{
				mSeparator = value;

				if (mIsLast)
					mCharsToDiscard = 0;
				else
					mCharsToDiscard = mSeparator.Length;
			}
		}

		internal char mQuoteChar = '\0';
		internal QuoteMode mQuoteMode;
		internal MultilineMode  mQuoteMultiline = MultilineMode.AllowForBoth;

		#endregion

		#region "  Overrides String Handling  "


		internal override ExtractedInfo ExtractFieldString(LineInfo line)
		{
			if (mIsOptional && line.IsEOL() )
				return ExtractedInfo.Empty;


			if (mQuoteChar == '\0')
				return BasicExtractString(line);
			else
			{
				//TODO: UnComment and Fix

				if (mTrimMode == TrimMode.Both || mTrimMode == TrimMode.Left)
				{

					//int pos = line.mCurrentPos;
					line.TrimStart(mTrimChars);
//					from2 = from.TrimStart(mTrimChars);
					//res.CharsRemoved = line.mCurrentPos - pos;
				}

				string quotedStr = mQuoteChar.ToString();
				if (line.StartsWith(quotedStr))
				{

//					ExtractedInfo res = null;
//					res = new ExtractedInfo(line, line.mCurrentPos);

					return StringHelper.ExtractQuotedString(line, mQuoteChar, mQuoteMultiline == MultilineMode.AllowForBoth || mQuoteMultiline == MultilineMode.AllowForRead);
//					if (mQuoteMultiline == MultilineMode.AllowForBoth || mQuoteMultiline == MultilineMode.AllowForRead)
//					{
//
//						//res.ExtractedString = ei.ExtractedString;
//						//res.CharsRemoved += ei.CharsRemoved;
//						//res.ExtraLines = ei.ExtraLines;
//						//res.NewRestOfLine = ei.NewRestOfLine;
//					}
//					else
//					{
//						return StringHelper.ExtractQuotedString(from2, mQuoteChar, out index);
//						//res.CharsRemoved += index;
//					}

				//	return res;

				}
				else
				{
					if (mQuoteMode == QuoteMode.OptionalForBoth || mQuoteMode == QuoteMode.OptionalForRead)
						return BasicExtractString(line);
					else if (line.StartsWithTrim(quotedStr))
						throw new BadUsageException("The field '" + this.mFieldInfo.Name + "' has spaces before the QuotedChar at line "+ line.mReader.LineNumber.ToString() + ". Use the TrimAttribute to by pass this error. Field String: " + line.CurrentString);
					else
						throw new BadUsageException("The field '" + this.mFieldInfo.Name + "' not begin with the QuotedChar at line "+ line.mReader.LineNumber.ToString() + ". You can use FieldQuoted(QuoteMode.OptionalForRead) to allow optional quoted field.. Field String: " + line.CurrentString);
				}

			}


		}

		private ExtractedInfo BasicExtractString(LineInfo line)
		{
		
			if (mIsLast && ! mIsArray)
				return new ExtractedInfo(line);
			else
			{
				int sepPos;

				sepPos = line.IndexOf(mSeparator);

				if (sepPos == -1)
				{
                    if (mIsLast && mIsArray)
                        return new ExtractedInfo(line);

					if (this.mNextIsOptional == false)
					{
						string msg = null;

						if (mIsFirst && line.EmptyFromPos())
							msg = "The line " + line.mReader.LineNumber.ToString() + " is empty. Maybe you need to use the attribute [IgnoreEmptyLines] in your record class.";
						else
							msg = "The delimiter '" + this.mSeparator + "' can�t be found after the field '" + this.mFieldInfo.Name + "' at line " + line.mReader.LineNumber.ToString() + " (the record has less fields, the delimiter is wrong or the next field must be marked as optional).";

						throw new FileHelpersException(line.mReader.LineNumber, line.mCurrentPos, msg);

					}
					else
						sepPos = line.mLine.Length;
				}

				return new ExtractedInfo(line, sepPos);
			}
		}

        internal override void CreateFieldString(StringBuilder sb, object fieldValue)
		{
			string field = base.CreateFieldString(fieldValue);

            bool hasNewLine = mCompare.IndexOf(field, StringHelper.NewLine, CompareOptions.Ordinal) >= 0;

			// If have a new line and this is not allowed throw an exception
			if (hasNewLine &&
				(mQuoteMultiline == MultilineMode.AllowForRead || 
				 mQuoteMultiline == MultilineMode.NotAllow))
				throw new BadUsageException("One value for the field " + this.mFieldInfo.Name + " has a new line inside. To allow write this value you must add a FieldQuoted attribute with the multiline option in true.");

			// Add Quotes If:
			//     -  optional == false
			//     -  is optional and contains the separator 
			//     -  is optional and contains a new line

			if ((mQuoteChar != '\0') && 
				(mQuoteMode == QuoteMode.AlwaysQuoted || 
					mQuoteMode == QuoteMode.OptionalForRead || 
					( (mQuoteMode == QuoteMode.OptionalForWrite || mQuoteMode == QuoteMode.OptionalForBoth)  
					&& mCompare.IndexOf(field, mSeparator, CompareOptions.Ordinal) >= 0) || hasNewLine))
				StringHelper.CreateQuotedString(sb, field, mQuoteChar);
			else
				sb.Append(field);

			if (mIsLast == false)
				sb.Append(mSeparator);
		}

		#endregion
	}
}