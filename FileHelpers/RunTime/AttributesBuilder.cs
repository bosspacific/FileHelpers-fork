using System;
using System.Text;

namespace FileHelpers
{

	internal sealed class AttributesBuilder
	{
		StringBuilder mSb = new StringBuilder(250);
		NetLanguage mLeng;
		
		public AttributesBuilder(NetLanguage leng)
		{
			mLeng = leng;
		}
		
		private bool mFirst = true;
		
		public void AddAttribute(string attribute)
		{
			if (attribute == null || attribute == string.Empty)
				return;
			
			if (mFirst)
			{
				switch(mLeng)
				{
					case NetLanguage.CSharp:
						mSb.Append("[");
						break;
					case NetLanguage.VbNet:
						mSb.Append("<");
						break;
				}
				mFirst = false;
			}
			else
			{
				switch(mLeng)
				{
					case NetLanguage.VbNet:
						mSb.Append(", ");
						break;
					case NetLanguage.CSharp:
						mSb.Append("[");
						break;
				}
			}
		
			mSb.Append(attribute);
						
			switch(mLeng)
			{
				case NetLanguage.CSharp:
					mSb.Append("]");
					break;
				case NetLanguage.VbNet:
					mSb.Append(" _");
					break;
			}

			mSb.Append(StringHelper.NewLine);

		}
		
		public string GetAttributesCode()
		{
			if (mFirst == true)
				return string.Empty;
			
			switch(mLeng)
			{
				case NetLanguage.VbNet:
					mSb.Append(" > _");
					mSb.Append(StringHelper.NewLine);
					break;
			}
			
			return mSb.ToString();
		}
	}
}
