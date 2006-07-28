using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using FileHelpers.RunTime;

namespace FileHelpers.WizardApp
{
    
    [XmlInclude(typeof(DelimitedFieldBuilder))]
    [XmlInclude(typeof(FixedFieldBuilder))]
    public class WizardInfo
    {

        //public void LoadFields(Control.ControlCollection col)
        //{
        //    mFields = new ArrayList(col.Count);

        //    foreach (FieldBaseControl ctrl in col)
        //    {
        //        mFields.Add(ctrl.FieldInfo);
        //    }
        //}

        private ClassBuilder mClassBuilder;

        public ClassBuilder ClassBuilder
        {
            get { return mClassBuilder; }
            set { mClassBuilder = value; }
        }


        private NetVisibility mFieldVisibility = NetVisibility.Public;

        public NetVisibility FieldVisibility
        {
            get { return mFieldVisibility; }
            set { mFieldVisibility = value; }
        }

        private NetVisibility mClassVisibility = NetVisibility.Public;

        public NetVisibility ClassVisibility
        {
            get { return mClassVisibility; }
            set { mClassVisibility = value; }
        }


        private bool mUseProperties = false;
        
        public bool UseProperties
        {
            get { return mUseProperties; }
            set { mUseProperties = value; }
        }


        public string WizardOutput(NetLanguage lang)
        {
            return mClassBuilder.GetClassSourceCode(lang);
        }

        private string mDefaultType;

        public string DefaultType
        {
            get { return mDefaultType; }
            set { mDefaultType = value; }
        }


    }
}
