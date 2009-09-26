#region "  � Copyright 2005-07 to Marcos Meli - http://www.devoo.net" 

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Reflection.Emit;

namespace FileHelpers
{
    
    /// <summary>An internal class used to store information about the Record Type.</summary>
    /// <remarks>Is public to provide extensibility of DataSorage from outside the library.</remarks>
    internal sealed class RecordInfo
    {
        // --------------------------------------
        // Constructor and Init Methods

        #region "  Internal Fields  "

        // Cache of all the fields that must be used for a Type
        // More info at:  http://www.filehelpers.com/forums/viewtopic.php?t=387
        // Thanks Brian for the report, research and fix
        private static readonly Dictionary<Type, List<FieldInfo>> mCachedRecordFields = new Dictionary<Type, List<FieldInfo>>();

        internal readonly Type mRecordType;
        internal FieldBase[] mFields;
        internal int mIgnoreFirst;
        internal int mIgnoreLast;
        internal bool mIgnoreEmptyLines;
        private bool mIgnoreEmptySpaces;

        internal string mCommentMarker;
        internal bool mCommentAnyPlace = true;

        internal RecordCondition mRecordCondition = RecordCondition.None;
        internal string mRecordConditionSelector = string.Empty;

        internal bool mNotifyRead;
        internal bool mNotifyWrite;
        private Regex mConditionRegEx;
        internal int mFieldCount;

        private ConstructorInfo mRecordConstructor;

        private static readonly Type[] mEmptyTypeArr = new Type[] { };

        #endregion

        internal bool IsDelimited
        {
            get { return mFields[0] is DelimitedField; }
        }

        #region "  Constructor &c "

        /// <summary>The unique constructor for this class. It needs the subyacent record class.</summary>
        /// <param name="recordType">The Type of the record class.</param>
        internal RecordInfo(Type recordType)
        {
            mRecordType = recordType;
            InitRecordFields();
        }

        private void InitRecordFields()
        {
            if (mRecordType.IsDefined(typeof(TypedRecordAttribute), true) == false)
                throw new BadUsageException("The class " + mRecordType.Name + " must be marked with the [DelimitedRecord] or [FixedLengthRecord] Attribute.");
            object[] attbs = mRecordType.GetCustomAttributes(typeof(TypedRecordAttribute), true);
            TypedRecordAttribute recordAttribute = (TypedRecordAttribute)attbs[0];

            mRecordConstructor =
                mRecordType.GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null,
                                           mEmptyTypeArr, new ParameterModifier[] {});
            if (mRecordConstructor == null)
                throw new BadUsageException("The record class " + mRecordType.Name + " need a constructor with no args (public or private)");

            if (mRecordType.IsDefined(typeof(IgnoreFirstAttribute), false))
                mIgnoreFirst = ((IgnoreFirstAttribute)mRecordType.GetCustomAttributes(typeof(IgnoreFirstAttribute), false)[0]).NumberOfLines;

            if (mRecordType.IsDefined(typeof(IgnoreLastAttribute), false))
                mIgnoreLast = ((IgnoreLastAttribute)mRecordType.GetCustomAttributes(typeof(IgnoreLastAttribute), false)[0]).NumberOfLines;

            if (mRecordType.IsDefined(typeof(IgnoreEmptyLinesAttribute), false))
            {
                mIgnoreEmptyLines = true;
                mIgnoreEmptySpaces = ((IgnoreEmptyLinesAttribute)mRecordType.GetCustomAttributes(typeof(IgnoreEmptyLinesAttribute), false)[0]).
                        mIgnoreSpaces;
            }

            if (mRecordType.IsDefined(typeof(IgnoreCommentedLinesAttribute), false))
            {
                IgnoreCommentedLinesAttribute ignoreComments =
                    (IgnoreCommentedLinesAttribute)mRecordType.GetCustomAttributes(typeof(IgnoreCommentedLinesAttribute), false)[0];
                mCommentMarker = ignoreComments.mCommentMarker;
                mCommentAnyPlace = ignoreComments.mAnyPlace;
            }

            if (mRecordType.IsDefined(typeof(ConditionalRecordAttribute), false))
            {
                ConditionalRecordAttribute conditional =
                    (ConditionalRecordAttribute)mRecordType.GetCustomAttributes(typeof(ConditionalRecordAttribute), false)[0];

                mRecordCondition = conditional.mCondition;
                mRecordConditionSelector = conditional.mConditionSelector;

                if (mRecordCondition == RecordCondition.ExcludeIfMatchRegex ||
                    mRecordCondition == RecordCondition.IncludeIfMatchRegex)
                {
                    mConditionRegEx = new Regex(mRecordConditionSelector, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                }
            }

            if (typeof(INotifyRead).IsAssignableFrom(mRecordType))
                mNotifyRead = true;

            if (typeof(INotifyWrite).IsAssignableFrom(mRecordType))
                mNotifyWrite = true;

            // Create fields
            // Search for cached fields
            List<FieldInfo> fields;

            lock (mCachedRecordFields)
            {
                if (!mCachedRecordFields.TryGetValue(mRecordType, out fields))
                {
                    fields = new List<FieldInfo>(RecursiveGetFields(mRecordType));
                    mCachedRecordFields.Add(mRecordType, fields);
                }
            }

            mFields = CreateCoreFields(fields, recordAttribute);
            mFieldCount = mFields.Length;

            if (recordAttribute is FixedLengthRecordAttribute)
            {
                // Defines the initial size of the StringBuilder
                mSizeHint = 0; 
                for (int i = 0; i < mFieldCount; i++)
                    mSizeHint += ((FixedLengthField)mFields[i]).mFieldLength;
            }

            if (mFieldCount == 0)
                throw new BadUsageException("The record class " + mRecordType.Name + " don't contains any field.");
        }

        private IEnumerable<FieldInfo> RecursiveGetFields(Type currentType)
        {
            if (currentType.BaseType != null && !currentType.IsDefined(typeof(IgnoreInheritedClassAttribute), false))
               foreach (var item in RecursiveGetFields(currentType.BaseType)) yield return item;

            if (currentType == typeof(object))
                yield break;

            lock (mTypeCacheLock)
            {
                ClearFieldInfoCache();

                foreach (FieldInfo fi in currentType.GetFields(BindingFlags.Public |
                                                                    BindingFlags.NonPublic | 
                                                                    BindingFlags.Instance |
                                                                    BindingFlags.DeclaredOnly))
                {
                    if (!(typeof(Delegate)).IsAssignableFrom(fi.FieldType))
                        yield return fi;
                }
            }
        }

        #endregion

        #region "  FieldCache Magick  "

        private static PropertyInfo mTypeCacheInfo;
        private static readonly object mTypeCacheLock = new object();

        private static FieldInfo mFieldCachePointer;
        private void ClearFieldInfoCache()
        {
                if (mTypeCacheInfo == null)
                    mTypeCacheInfo = mRecordType.GetType().GetProperty("Cache", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);

                if (mTypeCacheInfo != null)
                {
                object cache = mTypeCacheInfo.GetValue(mRecordType, null);

                if (mFieldCachePointer == null)
                    mFieldCachePointer = cache.GetType().GetField("m_fieldInfoCache", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic);

                mFieldCachePointer.SetValue(cache, null);
                }
        }

        #endregion

        #region "  CreateFields  "
        
        private static FieldBase[] CreateCoreFields(IList<FieldInfo> fields, TypedRecordAttribute recordAttribute)
        {
            List<FieldBase> resFields = new List<FieldBase>();

            for (int i = 0; i < fields.Count; i++)
            {
                FieldBase currentField = FieldBase.CreateField(fields[i], recordAttribute);

                if (currentField != null)
                {
                    // Add to the result
                    resFields.Add(currentField);

                    // Check some differences with the previous field
                    if (resFields.Count > 1)
                    {
                        FieldBase prevField = resFields[resFields.Count - 2];

                        prevField.mNextIsOptional = currentField.mIsOptional;

                        // Check for optional problems
                        if (prevField.mIsOptional && currentField.mIsOptional == false)
                            throw new BadUsageException("The field: " + prevField.mFieldInfo.Name + " must be marked as optional bacause after a field with FieldOptional, the next fields must be marked with the same attribute. ( Try adding [FieldOptional] to " + currentField.mFieldInfo.Name + " )");

                        // Check for array problems
                        if (prevField.mIsArray)
                        {
                            if (prevField.mArrayMinLength == int.MinValue)
                                throw new BadUsageException("The field: " + prevField.mFieldInfo.Name + " is an array and must contain a [FieldArrayLength] attribute because not is the last field.");

                            if (prevField.mArrayMinLength != prevField.mArrayMaxLength)
                                throw new BadUsageException("The array field: " + prevField.mFieldInfo.Name + " must contain a fixed length, i.e. the min and max length of the [FieldArrayLength] attribute must be the same because not is the last field.");
                        }
                    }

                }
            }

            if (resFields.Count > 0)
            {
                resFields[0].mIsFirst = true;
                resFields[resFields.Count - 1].mIsLast = true;
            }

            return resFields.ToArray();

        }
        #endregion

        // ----------------------------------------
        // String <--> Record <--> Values methods

        #region "  StringToRecord  "

        internal object StringToRecord(LineInfo line, object[] values)
        {
            if (MustIgnoreLine(line.mLineStr))
                return null;

            for (int i = 0; i < mFieldCount; i++)
            {
                values[i] = mFields[i].ExtractFieldValue(line);
            }

            try
            {
                // Asign all values via dinamic method that creates an object and assign values
                return CreateHandler(values);
            }
            catch (InvalidCastException)
            {
                // Occurrs when the a custom converter returns an invalid value for the field.
                for (int i = 0; i < mFieldCount; i++)
                {
                    if (values[i] != null && !mFields[i].mFieldTypeInternal.IsInstanceOfType(values[i]))
                        throw new ConvertException(null, mFields[i].mFieldTypeInternal, mFields[i].mFieldInfo.Name, line.mReader.LineNumber, -1, "The converter for the field: " + mFields[i].mFieldInfo.Name + " returns an object of Type: " + values[i].GetType().Name + " and the field is of type: " + mFields[i].mFieldTypeInternal.Name, null);
                }
                return null;
            }
        }

        private bool MustIgnoreLine(string line)
        {
            if (mIgnoreEmptyLines)
                if ((mIgnoreEmptySpaces && line.TrimStart().Length == 0) ||
                    line.Length == 0)
                    return true;

            if (!string.IsNullOrEmpty(mCommentMarker))
                if ((mCommentAnyPlace && line.TrimStart().StartsWith(mCommentMarker)) ||
                    line.StartsWith(mCommentMarker))
                    return true;

            if (mRecordCondition != RecordCondition.None)
            {
                switch (mRecordCondition)
                {
                    case RecordCondition.ExcludeIfBegins:
                        return ConditionHelper.BeginsWith(line, mRecordConditionSelector);
                    case RecordCondition.IncludeIfBegins:
                        return ! ConditionHelper.BeginsWith(line, mRecordConditionSelector);

                    case RecordCondition.ExcludeIfContains:
                        return ConditionHelper.Contains(line, mRecordConditionSelector);
                    case RecordCondition.IncludeIfContains:
                        return ! ConditionHelper.Contains(line, mRecordConditionSelector);


                    case RecordCondition.ExcludeIfEnclosed:
                        return ConditionHelper.Enclosed(line, mRecordConditionSelector);
                    case RecordCondition.IncludeIfEnclosed:
                        return ! ConditionHelper.Enclosed(line, mRecordConditionSelector);

                    case RecordCondition.ExcludeIfEnds:
                        return ConditionHelper.EndsWith(line, mRecordConditionSelector);
                    case RecordCondition.IncludeIfEnds:
                        return ! ConditionHelper.EndsWith(line, mRecordConditionSelector);

                    case RecordCondition.ExcludeIfMatchRegex:
                        return mConditionRegEx.IsMatch(line);
                    case RecordCondition.IncludeIfMatchRegex:
                        return ! mConditionRegEx.IsMatch(line);
                }
            }

            return false;
        }

        #endregion

        #region "  RecordToString  "

        private int mSizeHint = 32;

        internal string RecordToString(object record)
        {
            StringBuilder sb = new StringBuilder(mSizeHint);

            object[] mValues = GetAllValuesHandler(record);

            for (int f = 0; f < mFieldCount; f++)
            {
                mFields[f].AssignToString(sb, mValues[f]);
            }

            return sb.ToString();
        }

        internal string RecordValuesToString(object[] recordValues)
        {
            StringBuilder sb = new StringBuilder(mSizeHint);

            for (int f = 0; f < mFieldCount; f++)
            {
                mFields[f].AssignToString(sb, recordValues[f]);
            }

            return sb.ToString();
        }

        #endregion

        #region "  ValuesToRecord  "

        /// <summary>Returns a record formed with the passed values.</summary>
        /// <param name="values">The source Values.</param>
        /// <returns>A record formed with the passed values.</returns>
        public object ValuesToRecord(object[] values)
        {
            for (int i = 0; i < mFieldCount; i++)
            {
                if (mFields[i].mFieldTypeInternal == typeof(DateTime) && values[i] is double)
                    values[i] = DoubleToDate((int)(double)values[i]);

                values[i] = mFields[i].CreateValueForField(values[i]);
            }

            // Asign all values via dinamic method that creates an object and assign values
            return CreateHandler(values);
        }

        private static DateTime DoubleToDate(int serialNumber)
        {

            if (serialNumber < 59)
            {
                // Because of the 29-02-1900 bug, any serial date 
                // under 60 is one off... Compensate. 
                serialNumber++;
            }

            return new DateTime((serialNumber + 693593) * (10000000L * 24 * 3600));
        }

        #endregion

        #region "  RecordToValues  "
        /// <summary>Get an object[] of the values in the fields of the passed record.</summary>
        /// <param name="record">The source record.</param>
        /// <returns>An object[] of the values in the fields.</returns>
        public object[] RecordToValues(object record)
        {
            return GetAllValuesHandler(record);
        }

        #endregion

        #region "  RecordsToDataTable  "

        internal DataTable RecordsToDataTable(ICollection records)
        {
            return RecordsToDataTable(records, -1);
        }

        internal DataTable RecordsToDataTable(ICollection records, int maxRecords)
        {
            DataTable res = CreateEmptyDataTable();

            res.BeginLoadData();

            res.MinimumCapacity = records.Count;

            if (maxRecords == -1)
            {
                foreach (object r in records)
                    res.Rows.Add(RecordToValues(r));
            }
            else
            {
                int i = 0;
                foreach (object r in records)
                {
                    if (i == maxRecords)
                        break;

                    res.Rows.Add(RecordToValues(r));
                    i++;
                }

            }

            res.EndLoadData();
            return res;
        }

        internal DataTable CreateEmptyDataTable()
        {
            DataTable res = new DataTable();

            foreach (FieldBase f in mFields)
            {
                DataColumn column1 = res.Columns.Add(f.mFieldInfo.Name, f.mFieldInfo.FieldType);
                column1.ReadOnly = true;
            }
            return res;
        }

        #endregion

        #region "  Lightweight code generation (NET 2.0)  "

        private delegate object[] GetAllValuesCallback(object record);
        private GetAllValuesCallback mGetAllValuesHandler;

        private GetAllValuesCallback GetAllValuesHandler
        {
            get
            {
                if (mGetAllValuesHandler == null)
                    mGetAllValuesHandler = CreateGetAllMethod();
                return mGetAllValuesHandler;
            }
        }

        private delegate object CreateAndAssignCallback(object[] values);
        private CreateAndAssignCallback mCreateHandler;

        private CreateAndAssignCallback CreateHandler
        {
            get
            {
                if (mCreateHandler == null)
                    mCreateHandler = CreateAssignMethods();
                return mCreateHandler;
            }
        }

        private GetAllValuesCallback CreateGetAllMethod()
        {
            DynamicMethod dm = new DynamicMethod("_GetAllValues_FH_RT_",
                                                 MethodAttributes.Static | MethodAttributes.Public,
                                                 CallingConventions.Standard, typeof (object[]),
                                                 new[] {typeof (object)}, mRecordType, true);

            ILGenerator generator = dm.GetILGenerator();

            generator.DeclareLocal(typeof(object[]));
            generator.DeclareLocal(mRecordType);

            generator.Emit(OpCodes.Ldc_I4, mFieldCount);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc_0);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, mRecordType);
            generator.Emit(OpCodes.Stloc_1);


            for (int i = 0; i < mFieldCount; i++)
            {
                FieldBase field = mFields[i];

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldloc_1);

                generator.Emit(OpCodes.Ldfld, field.mFieldInfo);


                if (field.FieldType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, field.FieldType);
                }

                generator.Emit(OpCodes.Stelem_Ref);
            }

            // return the value
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            return (GetAllValuesCallback)dm.CreateDelegate(typeof(GetAllValuesCallback));
        }

        private CreateAndAssignCallback CreateAssignMethods()
        {
            DynamicMethod dm = new DynamicMethod("_CreateAndAssing_FH_RT_",
                                                 MethodAttributes.Static | MethodAttributes.Public,
                                                 CallingConventions.Standard, typeof (object), new[] {typeof (object[])},
                                                 mRecordType, true);

            ILGenerator generator = dm.GetILGenerator();

            generator.DeclareLocal(mRecordType);
            generator.Emit(OpCodes.Newobj, mRecordConstructor);
            generator.Emit(OpCodes.Stloc_0);

            for (int i = 0; i < mFieldCount; i++)
            {
                FieldBase field = mFields[i];

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Ldelem_Ref);


                if (field.FieldType.IsValueType)
                {
                    generator.Emit(OpCodes.Unbox_Any, field.FieldType);
                }
                else
                {
                    generator.Emit(OpCodes.Castclass, field.FieldType);
                }

                generator.Emit(OpCodes.Stfld, field.mFieldInfo);
            }

            // return the value
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            return (CreateAndAssignCallback)dm.CreateDelegate(typeof(CreateAndAssignCallback));
        }

        internal delegate object CreateNewObject();
        private CreateNewObject mFastConstructor;

        internal CreateNewObject CreateRecordObject
        {
            get
            {
                if (mFastConstructor == null)
                    mFastConstructor = CreateFastConstructor();
                return mFastConstructor;
            }
        }

        private CreateNewObject CreateFastConstructor()
        {
            DynamicMethod dm = new DynamicMethod("_CreateRecordFast_FH_RT_",
                                                 MethodAttributes.Static | MethodAttributes.Public,
                                                 CallingConventions.Standard, typeof (object),
                                                 new[] {typeof (object[])}, mRecordType, true);

            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Newobj, mRecordConstructor);
            generator.Emit(OpCodes.Ret);

            return (CreateNewObject)dm.CreateDelegate(typeof(CreateNewObject));
        }

        public static GetFieldValueCallback CreateGetFieldMethod(FieldInfo fi)
        {
            DynamicMethod dm = new DynamicMethod("_GetValue" + fi.Name + "_FH_RT_",
                                                 MethodAttributes.Static | MethodAttributes.Public,
                                                 CallingConventions.Standard, typeof (object), new[] {typeof (object)},
                                                 fi.DeclaringType, true);

            ILGenerator generator = dm.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, fi.DeclaringType);
            generator.Emit(OpCodes.Ldfld, fi);
            generator.Emit(OpCodes.Ret);

            return (GetFieldValueCallback)dm.CreateDelegate(typeof(GetFieldValueCallback));

        }

        #endregion

        #region " FieldIndexes  "

        private Dictionary<string, int> mMapFieldIndex;
        public int GetFieldIndex(string fieldName)
        {
            if (mMapFieldIndex == null)
            {
                mMapFieldIndex = new Dictionary<string, int>(mFieldCount);
                for (int i = 0; i < mFieldCount; i++)
                {
                    mMapFieldIndex.Add(mFields[i].mFieldInfo.Name, i);
                }
            }

            int res;
            if (!mMapFieldIndex.TryGetValue(fieldName, out res))
                throw new BadUsageException("The field: " + fieldName + " was not found in the class: " + mRecordType.Name + ". Remember that this option is case sensitive.");

            return res;
        }

        #endregion

        #region "  GetFieldInfo  "

        internal FieldInfo GetFieldInfo(string name)
        {
            foreach (FieldBase field in mFields)
            {
                if (field.mFieldInfo.Name.ToLower() == name.ToLower())
                    return field.mFieldInfo;
            }

            return null;
        }
        #endregion
    }
}
