using FileHelpers;
using System.Diagnostics;
using System; 

namespace FileHelpers
{
internal  partial class Messages
{
public  partial class Errors
{

private static TypesOfMessages.Errors.FieldOptionalClass mFieldOptional = new TypesOfMessages.Errors.FieldOptionalClass();
public static TypesOfMessages.Errors.FieldOptionalClass FieldOptional
{ get { return  mFieldOptional; } }
private static TypesOfMessages.Errors.InvalidIdentifierClass mInvalidIdentifier = new TypesOfMessages.Errors.InvalidIdentifierClass();
public static TypesOfMessages.Errors.InvalidIdentifierClass InvalidIdentifier
{ get { return  mInvalidIdentifier; } }
private static TypesOfMessages.Errors.EmptyClassNameClass mEmptyClassName = new TypesOfMessages.Errors.EmptyClassNameClass();
public static TypesOfMessages.Errors.EmptyClassNameClass EmptyClassName
{ get { return  mEmptyClassName; } }
private static TypesOfMessages.Errors.EmptyFieldNameClass mEmptyFieldName = new TypesOfMessages.Errors.EmptyFieldNameClass();
public static TypesOfMessages.Errors.EmptyFieldNameClass EmptyFieldName
{ get { return  mEmptyFieldName; } }
private static TypesOfMessages.Errors.EmptyFieldTypeClass mEmptyFieldType = new TypesOfMessages.Errors.EmptyFieldTypeClass();
public static TypesOfMessages.Errors.EmptyFieldTypeClass EmptyFieldType
{ get { return  mEmptyFieldType; } }
private static TypesOfMessages.Errors.ClassWithOutRecordAttributeClass mClassWithOutRecordAttribute = new TypesOfMessages.Errors.ClassWithOutRecordAttributeClass();
public static TypesOfMessages.Errors.ClassWithOutRecordAttributeClass ClassWithOutRecordAttribute
{ get { return  mClassWithOutRecordAttribute; } }
private static TypesOfMessages.Errors.ClassWithOutDefaultConstructorClass mClassWithOutDefaultConstructor = new TypesOfMessages.Errors.ClassWithOutDefaultConstructorClass();
public static TypesOfMessages.Errors.ClassWithOutDefaultConstructorClass ClassWithOutDefaultConstructor
{ get { return  mClassWithOutDefaultConstructor; } }
private static TypesOfMessages.Errors.ClassWithOutFieldsClass mClassWithOutFields = new TypesOfMessages.Errors.ClassWithOutFieldsClass();
public static TypesOfMessages.Errors.ClassWithOutFieldsClass ClassWithOutFields
{ get { return  mClassWithOutFields; } }
private static TypesOfMessages.Errors.ExpectingFieldOptionalClass mExpectingFieldOptional = new TypesOfMessages.Errors.ExpectingFieldOptionalClass();
public static TypesOfMessages.Errors.ExpectingFieldOptionalClass ExpectingFieldOptional
{ get { return  mExpectingFieldOptional; } }


}


}internal  partial class TypesOfMessages
{
public  partial class Errors
{
public  partial class ClassWithOutDefaultConstructorClass: MessageBase
{

public ClassWithOutDefaultConstructorClass(): base(@"The record class $ClassName$ needs a constructor with no args (public or private)") {}
 private string mClassName = null;
 public ClassWithOutDefaultConstructorClass ClassName(string value)
{
    mClassName = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$ClassName$", mClassName);
        return res;
    }


}public  partial class ClassWithOutFieldsClass: MessageBase
{

public ClassWithOutFieldsClass(): base(@"The record class $ClassName$ don't contains any field") {}
 private string mClassName = null;
 public ClassWithOutFieldsClass ClassName(string value)
{
    mClassName = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$ClassName$", mClassName);
        return res;
    }


}public  partial class ClassWithOutRecordAttributeClass: MessageBase
{

public ClassWithOutRecordAttributeClass(): base(@"The record class $ClassName$ must be marked with the [DelimitedRecord] or [FixedLengthRecord] Attribute") {}
 private string mClassName = null;
 public ClassWithOutRecordAttributeClass ClassName(string value)
{
    mClassName = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$ClassName$", mClassName);
        return res;
    }


}public  partial class EmptyClassNameClass: MessageBase
{

public EmptyClassNameClass(): base(@"The ClassName can't be empty") {}
    protected override string GenerateText() 
    {
        var res = SourceText;
        return res;
    }


}public  partial class EmptyFieldNameClass: MessageBase
{

public EmptyFieldNameClass(): base(@"The $Position$th field name can't be empty") {}
 private string mPosition = null;
 public EmptyFieldNameClass Position(string value)
{
    mPosition = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$Position$", mPosition);
        return res;
    }


}public  partial class EmptyFieldTypeClass: MessageBase
{

public EmptyFieldTypeClass(): base(@"The $Position$th field type can't be empty") {}
 private string mPosition = null;
 public EmptyFieldTypeClass Position(string value)
{
    mPosition = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$Position$", mPosition);
        return res;
    }


}public  partial class ExpectingFieldOptionalClass: MessageBase
{

public ExpectingFieldOptionalClass(): base(@"The field: $FieldName$ must be marked as optional because his previoud field is marked as optional. (Try adding [FieldOptional] to $FieldName$)") {}
 private string mFieldName = null;
 public ExpectingFieldOptionalClass FieldName(string value)
{
    mFieldName = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$FieldName$", mFieldName);
        return res;
    }


}public  partial class FieldOptionalClass: MessageBase
{

public FieldOptionalClass(): base(@"The field: $Field$ must be marked as optional because the previous field is marked with FieldOptional. (Try adding [FieldOptional] to $Field$)") {}
 private string mField = null;
 public FieldOptionalClass Field(string value)
{
    mField = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$Field$", mField);
        return res;
    }


}public  partial class InvalidIdentifierClass: MessageBase
{

public InvalidIdentifierClass(): base(@"The string '$Identifier$' not is a valid .NET identifier") {}
 private string mIdentifier = null;
 public InvalidIdentifierClass Identifier(string value)
{
    mIdentifier = value;
    return this;
}
    protected override string GenerateText() 
    {
        var res = SourceText;
        res = StringHelper.ReplaceIgnoringCase(res, "$Identifier$", mIdentifier);
        return res;
    }


}


}


}
}

