# F# friendly .Net 

This project aims to make the interactions with .Net types from F# friendlier.

As a recent convert from C# to F#, I can't help but feel a bit dirty whenever I have to do things that are clearly non-functional. 
For example, turning an `int` into a `string` I can either call the `.ToString()` method, or use the more verbose `sprintf "%i" i`.  

Or how about deleting a folder of the hard drive? Then I have to issue a `Directory.Delete(name, false)`. Why do I need to type those parentheseses? And by the way, what is the meaning of that `false`? 
Why can't I call the method like `Directory.Delete name NonRecursive`

Another nagging problem is the lot of .Net methods return `null` signaling that there is nothing to return. `null` is sort of the family's black sheep that no one wants to hang around. Yet when you use the .Net api you are constantly smacked in the head with them. 
We need wrappers to turn `null` into `Option`. 
Finally, a wide range of Api methods make use of the Try-pattern, returning a `bool` and is equipped with an `out` parameter. Dealing with say a `int.TryParse()` is certainly nicer in F# than in C# but it still feels a bit wrong

```F#
match int.TryParse(httpParameter) with
| true, x -> x
| false, _ -> ...
```

this is not very F#'ish, why not have

```F#
match int.tryParse httpParameter with
| Some x -> x
| None -> ...
```

Another solution is active patterns, more on those later.

To summarize. 
  * It feels unnatural to be forced into using `()` and commas when invoking methods
  * Some actions are verbose done purely in F# (such as using `sprintf`)
  * Some Api can be made clearer by replacing the use of bool and other "undescriptive types" with say discriminating unions.
  * Dealing with `null`'s and try-patterns feels wrong when we have `Option`.


## Goals

The goals of this project are

  * By defining a set of helper functions, we can make the interaction much smoother and generally making the coding experince better.
  * When defining the helper methods, we can rectify or make clearer the existing .Net api.


## Challenges
There are several challenges. One of them being that F# does not have function overloading like C# has method overloading. A lot of methods in the .Net framework are overloaded. 
And a lot of methods are instance methods, because that's natural for any OOP language. But many instance methods are pure, that is, they have no side-effects on the object on which it is invoked.
Take the `DateTime.Add(TimeSpan)` method which returns a **new** `DateTime` objet. From F# I'm forced to call this method with the ugly parenthesis again like `let date2 = date1.Add(TimeSpan.FromSeconds(3)`. 
It sure would look nice if we could just do `let date 2 = TimeSpan.FromSeconds 3 |> date1.add`

F# offer a range of language constructs not found in C#, active patterns for one. So if we provide a nice


# Inspiration for methods to wrap

This list was created by executing the `Analysis.fsx` script. 

The list shows all static methods for a couple of central assemblies, with methods enclosed in `*` to denote they are not overloaded (ie. easy to wrap)


```
####################
mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
####################

Fusion
----------------
*Fusion.ReadCache(ArrayList,String,UInt32)*

Registry
----------------
*Registry.GetValue(String,String,Object)*
Registry.SetValue(String,String,Object)
Registry.SetValue(String,String,Object,RegistryValueKind)

RegistryKey
----------------
*RegistryKey.OpenBaseKey(RegistryHive,RegistryView)*
RegistryKey.OpenRemoteBaseKey(RegistryHive,String)
RegistryKey.OpenRemoteBaseKey(RegistryHive,String,RegistryView)
RegistryKey.FromHandle(SafeRegistryHandle)
RegistryKey.FromHandle(SafeRegistryHandle,RegistryView)

ReflectionExtensions
----------------
*ReflectionExtensions.IsEnum(Type)*
*ReflectionExtensions.IsAbstract(Type)*
*ReflectionExtensions.IsSealed(Type)*
*ReflectionExtensions.BaseType(Type)*
*ReflectionExtensions.Assembly(Type)*
*ReflectionExtensions.GetTypeCode(Type)*
*ReflectionExtensions.ReflectionOnly(Assembly)*

AppContext
----------------
*AppContext.TryGetSwitch(String,Boolean&)*
*AppContext.SetSwitch(String,Boolean)*

AppContextDefaultValues
----------------
*AppContextDefaultValues.PopulateDefaultValues()*
*AppContextDefaultValues.TryGetSwitchOverride(String,Boolean&)*

String
----------------
String.Join(String,String[])
String.Join(String,Object[])
String.Join(String,IEnumerable`1)
String.Join(String,IEnumerable`1)
String.Join(String,String[],Int32,Int32)
String.Equals(String,String)
String.Equals(String,String,StringComparison)
*String.IsNullOrEmpty(String)*
*String.IsNullOrWhiteSpace(String)*
String.Compare(String,String)
String.Compare(String,String,Boolean)
String.Compare(String,String,StringComparison)
String.Compare(String,String,CultureInfo,CompareOptions)
String.Compare(String,String,Boolean,CultureInfo)
String.Compare(String,Int32,String,Int32,Int32)
String.Compare(String,Int32,String,Int32,Int32,Boolean)
String.Compare(String,Int32,String,Int32,Int32,Boolean,CultureInfo)
String.Compare(String,Int32,String,Int32,Int32,CultureInfo,CompareOptions)
String.Compare(String,Int32,String,Int32,Int32,StringComparison)
String.CompareOrdinal(String,String)
String.CompareOrdinal(String,Int32,String,Int32,Int32)
String.Format(String,Object)
String.Format(String,Object,Object)
String.Format(String,Object,Object,Object)
String.Format(String,Object[])
String.Format(IFormatProvider,String,Object)
String.Format(IFormatProvider,String,Object,Object)
String.Format(IFormatProvider,String,Object,Object,Object)
String.Format(IFormatProvider,String,Object[])
*String.Copy(String)*
String.Concat(Object)
String.Concat(Object,Object)
String.Concat(Object,Object,Object)
String.Concat(Object,Object,Object,Object)
String.Concat(Object[])
String.Concat(IEnumerable`1)
String.Concat(IEnumerable`1)
String.Concat(String,String)
String.Concat(String,String,String)
String.Concat(String,String,String,String)
String.Concat(String[])
*String.Intern(String)*
*String.IsInterned(String)*

StringComparer
----------------
*StringComparer.Create(CultureInfo,Boolean)*

DateTime
----------------
*DateTime.Compare(DateTime,DateTime)*
*DateTime.DaysInMonth(Int32,Int32)*
*DateTime.Equals(DateTime,DateTime)*
*DateTime.FromBinary(Int64)*
*DateTime.FromFileTime(Int64)*
*DateTime.FromFileTimeUtc(Int64)*
*DateTime.FromOADate(Double)*
*DateTime.SpecifyKind(DateTime,DateTimeKind)*
*DateTime.IsLeapYear(Int32)*
DateTime.Parse(String)
DateTime.Parse(String,IFormatProvider)
DateTime.Parse(String,IFormatProvider,DateTimeStyles)
DateTime.ParseExact(String,String,IFormatProvider)
DateTime.ParseExact(String,String,IFormatProvider,DateTimeStyles)
DateTime.ParseExact(String,String[],IFormatProvider,DateTimeStyles)
DateTime.TryParse(String,DateTime&)
DateTime.TryParse(String,IFormatProvider,DateTimeStyles,DateTime&)
DateTime.TryParseExact(String,String,IFormatProvider,DateTimeStyles,DateTime&)
DateTime.TryParseExact(String,String[],IFormatProvider,DateTimeStyles,DateTime&)

DateTimeOffset
----------------
*DateTimeOffset.Compare(DateTimeOffset,DateTimeOffset)*
*DateTimeOffset.Equals(DateTimeOffset,DateTimeOffset)*
*DateTimeOffset.FromFileTime(Int64)*
*DateTimeOffset.FromUnixTimeSeconds(Int64)*
*DateTimeOffset.FromUnixTimeMilliseconds(Int64)*
DateTimeOffset.Parse(String)
DateTimeOffset.Parse(String,IFormatProvider)
DateTimeOffset.Parse(String,IFormatProvider,DateTimeStyles)
DateTimeOffset.ParseExact(String,String,IFormatProvider)
DateTimeOffset.ParseExact(String,String,IFormatProvider,DateTimeStyles)
DateTimeOffset.ParseExact(String,String[],IFormatProvider,DateTimeStyles)
DateTimeOffset.TryParse(String,DateTimeOffset&)
DateTimeOffset.TryParse(String,IFormatProvider,DateTimeStyles,DateTimeOffset&)
DateTimeOffset.TryParseExact(String,String,IFormatProvider,DateTimeStyles,DateTimeOffset&)
DateTimeOffset.TryParseExact(String,String[],IFormatProvider,DateTimeStyles,DateTimeOffset&)

Delegate
----------------
Delegate.Combine(Delegate,Delegate)
Delegate.Combine(Delegate[])
*Delegate.Remove(Delegate,Delegate)*
*Delegate.RemoveAll(Delegate,Delegate)*
Delegate.CreateDelegate(Type,Object,String)
Delegate.CreateDelegate(Type,Object,String,Boolean)
Delegate.CreateDelegate(Type,Object,String,Boolean,Boolean)
Delegate.CreateDelegate(Type,Type,String)
Delegate.CreateDelegate(Type,Type,String,Boolean)
Delegate.CreateDelegate(Type,Type,String,Boolean,Boolean)
Delegate.CreateDelegate(Type,MethodInfo,Boolean)
Delegate.CreateDelegate(Type,Object,MethodInfo)
Delegate.CreateDelegate(Type,Object,MethodInfo,Boolean)
Delegate.CreateDelegate(Type,MethodInfo)

BCLDebug
----------------
*BCLDebug.Assert(Boolean,String)*
BCLDebug.Log(String)
BCLDebug.Log(String,String)
BCLDebug.Log(String,LogLevel,Object[])
BCLDebug.Trace(String,Object[])
BCLDebug.Trace(String,String,Object[])
*BCLDebug.DumpStack(String)*

Activator
----------------
Activator.CreateInstance(Type,BindingFlags,Binder,Object[],CultureInfo)
Activator.CreateInstance(Type,BindingFlags,Binder,Object[],CultureInfo,Object[])
Activator.CreateInstance(Type,Object[])
Activator.CreateInstance(Type,Object[],Object[])
Activator.CreateInstance(Type)
Activator.CreateInstance(String,String)
Activator.CreateInstance(String,String,Object[])
Activator.CreateInstance(Type,Boolean)
Activator.CreateInstance()
Activator.CreateInstanceFrom(String,String)
Activator.CreateInstanceFrom(String,String,Object[])
Activator.CreateInstance(String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[],Evidence)
Activator.CreateInstance(String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[])
Activator.CreateInstanceFrom(String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[],Evidence)
Activator.CreateInstanceFrom(String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[])
Activator.CreateInstance(AppDomain,String,String)
Activator.CreateInstance(AppDomain,String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[],Evidence)
Activator.CreateInstance(AppDomain,String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[])
Activator.CreateInstanceFrom(AppDomain,String,String)
Activator.CreateInstanceFrom(AppDomain,String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[],Evidence)
Activator.CreateInstanceFrom(AppDomain,String,String,Boolean,BindingFlags,Binder,Object[],CultureInfo,Object[])
Activator.CreateInstance(ActivationContext)
Activator.CreateInstance(ActivationContext,String[])
Activator.CreateComInstanceFrom(String,String)
Activator.CreateComInstanceFrom(String,String,Byte[],AssemblyHashAlgorithm)
Activator.GetObject(Type,String)
Activator.GetObject(Type,String,Object)

AppDomain
----------------
*AppDomain.GetCurrentThreadId()*
*AppDomain.Unload(AppDomain)*
AppDomain.CreateDomain(String,Evidence)
AppDomain.CreateDomain(String,Evidence,String,String,Boolean)
AppDomain.CreateDomain(String)
AppDomain.CreateDomain(String,Evidence,AppDomainSetup)
AppDomain.CreateDomain(String,Evidence,AppDomainSetup,PermissionSet,StrongName[])
AppDomain.CreateDomain(String,Evidence,String,String,Boolean,AppDomainInitializer,String[])

ActivationContext
----------------
ActivationContext.CreatePartialActivationContext(ApplicationIdentity)
ActivationContext.CreatePartialActivationContext(ApplicationIdentity,String[])

Attribute
----------------
Attribute.GetCustomAttributes(MemberInfo,Type)
Attribute.GetCustomAttributes(MemberInfo,Type,Boolean)
Attribute.GetCustomAttributes(MemberInfo)
Attribute.GetCustomAttributes(MemberInfo,Boolean)
Attribute.IsDefined(MemberInfo,Type)
Attribute.IsDefined(MemberInfo,Type,Boolean)
Attribute.GetCustomAttribute(MemberInfo,Type)
Attribute.GetCustomAttribute(MemberInfo,Type,Boolean)
Attribute.GetCustomAttributes(ParameterInfo)
Attribute.GetCustomAttributes(ParameterInfo,Type)
Attribute.GetCustomAttributes(ParameterInfo,Type,Boolean)
Attribute.GetCustomAttributes(ParameterInfo,Boolean)
Attribute.IsDefined(ParameterInfo,Type)
Attribute.IsDefined(ParameterInfo,Type,Boolean)
Attribute.GetCustomAttribute(ParameterInfo,Type)
Attribute.GetCustomAttribute(ParameterInfo,Type,Boolean)
Attribute.GetCustomAttributes(Module,Type)
Attribute.GetCustomAttributes(Module)
Attribute.GetCustomAttributes(Module,Boolean)
Attribute.GetCustomAttributes(Module,Type,Boolean)
Attribute.IsDefined(Module,Type)
Attribute.IsDefined(Module,Type,Boolean)
Attribute.GetCustomAttribute(Module,Type)
Attribute.GetCustomAttribute(Module,Type,Boolean)
Attribute.GetCustomAttributes(Assembly,Type)
Attribute.GetCustomAttributes(Assembly,Type,Boolean)
Attribute.GetCustomAttributes(Assembly)
Attribute.GetCustomAttributes(Assembly,Boolean)
Attribute.IsDefined(Assembly,Type)
Attribute.IsDefined(Assembly,Type,Boolean)
Attribute.GetCustomAttribute(Assembly,Type)
Attribute.GetCustomAttribute(Assembly,Type,Boolean)

BitConverter
----------------
BitConverter.GetBytes(Boolean)
BitConverter.GetBytes(Char)
BitConverter.GetBytes(Int16)
BitConverter.GetBytes(Int32)
BitConverter.GetBytes(Int64)
BitConverter.GetBytes(UInt16)
BitConverter.GetBytes(UInt32)
BitConverter.GetBytes(UInt64)
BitConverter.GetBytes(Single)
BitConverter.GetBytes(Double)
*BitConverter.ToChar(Byte[],Int32)*
*BitConverter.ToInt16(Byte[],Int32)*
*BitConverter.ToInt32(Byte[],Int32)*
*BitConverter.ToInt64(Byte[],Int32)*
*BitConverter.ToUInt16(Byte[],Int32)*
*BitConverter.ToUInt32(Byte[],Int32)*
*BitConverter.ToUInt64(Byte[],Int32)*
*BitConverter.ToSingle(Byte[],Int32)*
*BitConverter.ToDouble(Byte[],Int32)*
*BitConverter.ToBoolean(Byte[],Int32)*
*BitConverter.DoubleToInt64Bits(Double)*
*BitConverter.Int64BitsToDouble(Int64)*

Boolean
----------------
*Boolean.Parse(String)*
*Boolean.TryParse(String,Boolean&)*

Buffer
----------------
*Buffer.BlockCopy(Array,Int32,Array,Int32,Int32)*
*Buffer.GetByte(Array,Int32)*
*Buffer.SetByte(Array,Int32,Byte)*
*Buffer.ByteLength(Array)*
Buffer.MemoryCopy(Void*,Void*,Int64,Int64)
Buffer.MemoryCopy(Void*,Void*,UInt64,UInt64)

Byte
----------------
Byte.Parse(String)
Byte.Parse(String,NumberStyles)
Byte.Parse(String,IFormatProvider)
Byte.Parse(String,NumberStyles,IFormatProvider)
Byte.TryParse(String,Byte&)
Byte.TryParse(String,NumberStyles,IFormatProvider,Byte&)

Char
----------------
*Char.Parse(String)*
*Char.TryParse(String,Char&)*
Char.IsDigit(Char)
Char.IsLetter(Char)
Char.IsWhiteSpace(Char)
Char.IsUpper(Char)
Char.IsLower(Char)
Char.IsPunctuation(Char)
Char.IsLetterOrDigit(Char)
Char.ToUpper(Char,CultureInfo)
Char.ToUpper(Char)
*Char.ToUpperInvariant(Char)*
Char.ToLower(Char,CultureInfo)
Char.ToLower(Char)
*Char.ToLowerInvariant(Char)*
Char.IsControl(Char)
Char.IsControl(String,Int32)
Char.IsDigit(String,Int32)
Char.IsLetter(String,Int32)
Char.IsLetterOrDigit(String,Int32)
Char.IsLower(String,Int32)
Char.IsNumber(Char)
Char.IsNumber(String,Int32)
Char.IsPunctuation(String,Int32)
Char.IsSeparator(Char)
Char.IsSeparator(String,Int32)
Char.IsSurrogate(Char)
Char.IsSurrogate(String,Int32)
Char.IsSymbol(Char)
Char.IsSymbol(String,Int32)
Char.IsUpper(String,Int32)
Char.IsWhiteSpace(String,Int32)
Char.GetUnicodeCategory(Char)
Char.GetUnicodeCategory(String,Int32)
Char.GetNumericValue(Char)
Char.GetNumericValue(String,Int32)
Char.IsHighSurrogate(Char)
Char.IsHighSurrogate(String,Int32)
Char.IsLowSurrogate(Char)
Char.IsLowSurrogate(String,Int32)
Char.IsSurrogatePair(String,Int32)
Char.IsSurrogatePair(Char,Char)
*Char.ConvertFromUtf32(Int32)*
Char.ConvertToUtf32(Char,Char)
Char.ConvertToUtf32(String,Int32)

Console
----------------
Console.Beep()
Console.Beep(Int32,Int32)
*Console.Clear()*
*Console.ResetColor()*
Console.MoveBufferArea(Int32,Int32,Int32,Int32,Int32,Int32)
Console.MoveBufferArea(Int32,Int32,Int32,Int32,Int32,Int32,Char,ConsoleColor,ConsoleColor)
*Console.SetBufferSize(Int32,Int32)*
*Console.SetWindowSize(Int32,Int32)*
*Console.SetWindowPosition(Int32,Int32)*
*Console.SetCursorPosition(Int32,Int32)*
Console.ReadKey()
Console.ReadKey(Boolean)
*Console.add_CancelKeyPress(ConsoleCancelEventHandler)*
*Console.remove_CancelKeyPress(ConsoleCancelEventHandler)*
Console.OpenStandardError()
Console.OpenStandardError(Int32)
Console.OpenStandardInput()
Console.OpenStandardInput(Int32)
Console.OpenStandardOutput()
Console.OpenStandardOutput(Int32)
*Console.SetIn(TextReader)*
*Console.SetOut(TextWriter)*
*Console.SetError(TextWriter)*
*Console.Read()*
*Console.ReadLine()*
Console.WriteLine()
Console.WriteLine(Boolean)
Console.WriteLine(Char)
Console.WriteLine(Char[])
Console.WriteLine(Char[],Int32,Int32)
Console.WriteLine(Decimal)
Console.WriteLine(Double)
Console.WriteLine(Single)
Console.WriteLine(Int32)
Console.WriteLine(UInt32)
Console.WriteLine(Int64)
Console.WriteLine(UInt64)
Console.WriteLine(Object)
Console.WriteLine(String)
Console.WriteLine(String,Object)
Console.WriteLine(String,Object,Object)
Console.WriteLine(String,Object,Object,Object)
Console.WriteLine(String,Object,Object,Object,Object)
Console.WriteLine(String,Object[])
Console.Write(String,Object)
Console.Write(String,Object,Object)
Console.Write(String,Object,Object,Object)
Console.Write(String,Object,Object,Object,Object)
Console.Write(String,Object[])
Console.Write(Boolean)
Console.Write(Char)
Console.Write(Char[])
Console.Write(Char[],Int32,Int32)
Console.Write(Double)
Console.Write(Decimal)
Console.Write(Single)
Console.Write(Int32)
Console.Write(UInt32)
Console.Write(Int64)
Console.Write(UInt64)
Console.Write(Object)
Console.Write(String)

Convert
----------------
*Convert.GetTypeCode(Object)*
*Convert.IsDBNull(Object)*
Convert.ChangeType(Object,TypeCode)
Convert.ChangeType(Object,TypeCode,IFormatProvider)
Convert.ChangeType(Object,Type)
Convert.ChangeType(Object,Type,IFormatProvider)
Convert.ToBoolean(Object)
Convert.ToBoolean(Object,IFormatProvider)
Convert.ToBoolean(Boolean)
Convert.ToBoolean(SByte)
Convert.ToBoolean(Char)
Convert.ToBoolean(Byte)
Convert.ToBoolean(Int16)
Convert.ToBoolean(UInt16)
Convert.ToBoolean(Int32)
Convert.ToBoolean(UInt32)
Convert.ToBoolean(Int64)
Convert.ToBoolean(UInt64)
Convert.ToBoolean(String)
Convert.ToBoolean(String,IFormatProvider)
Convert.ToBoolean(Single)
Convert.ToBoolean(Double)
Convert.ToBoolean(Decimal)
Convert.ToBoolean(DateTime)
Convert.ToChar(Object)
Convert.ToChar(Object,IFormatProvider)
Convert.ToChar(Boolean)
Convert.ToChar(Char)
Convert.ToChar(SByte)
Convert.ToChar(Byte)
Convert.ToChar(Int16)
Convert.ToChar(UInt16)
Convert.ToChar(Int32)
Convert.ToChar(UInt32)
Convert.ToChar(Int64)
Convert.ToChar(UInt64)
Convert.ToChar(String)
Convert.ToChar(String,IFormatProvider)
Convert.ToChar(Single)
Convert.ToChar(Double)
Convert.ToChar(Decimal)
Convert.ToChar(DateTime)
Convert.ToSByte(Object)
Convert.ToSByte(Object,IFormatProvider)
Convert.ToSByte(Boolean)
Convert.ToSByte(SByte)
Convert.ToSByte(Char)
Convert.ToSByte(Byte)
Convert.ToSByte(Int16)
Convert.ToSByte(UInt16)
Convert.ToSByte(Int32)
Convert.ToSByte(UInt32)
Convert.ToSByte(Int64)
Convert.ToSByte(UInt64)
Convert.ToSByte(Single)
Convert.ToSByte(Double)
Convert.ToSByte(Decimal)
Convert.ToSByte(String)
Convert.ToSByte(String,IFormatProvider)
Convert.ToSByte(DateTime)
Convert.ToByte(Object)
Convert.ToByte(Object,IFormatProvider)
Convert.ToByte(Boolean)
Convert.ToByte(Byte)
Convert.ToByte(Char)
Convert.ToByte(SByte)
Convert.ToByte(Int16)
Convert.ToByte(UInt16)
Convert.ToByte(Int32)
Convert.ToByte(UInt32)
Convert.ToByte(Int64)
Convert.ToByte(UInt64)
Convert.ToByte(Single)
Convert.ToByte(Double)
Convert.ToByte(Decimal)
Convert.ToByte(String)
Convert.ToByte(String,IFormatProvider)
Convert.ToByte(DateTime)
Convert.ToInt16(Object)
Convert.ToInt16(Object,IFormatProvider)
Convert.ToInt16(Boolean)
Convert.ToInt16(Char)
Convert.ToInt16(SByte)
Convert.ToInt16(Byte)
Convert.ToInt16(UInt16)
Convert.ToInt16(Int32)
Convert.ToInt16(UInt32)
Convert.ToInt16(Int16)
Convert.ToInt16(Int64)
Convert.ToInt16(UInt64)
Convert.ToInt16(Single)
Convert.ToInt16(Double)
Convert.ToInt16(Decimal)
Convert.ToInt16(String)
Convert.ToInt16(String,IFormatProvider)
Convert.ToInt16(DateTime)
Convert.ToUInt16(Object)
Convert.ToUInt16(Object,IFormatProvider)
Convert.ToUInt16(Boolean)
Convert.ToUInt16(Char)
Convert.ToUInt16(SByte)
Convert.ToUInt16(Byte)
Convert.ToUInt16(Int16)
Convert.ToUInt16(Int32)
Convert.ToUInt16(UInt16)
Convert.ToUInt16(UInt32)
Convert.ToUInt16(Int64)
Convert.ToUInt16(UInt64)
Convert.ToUInt16(Single)
Convert.ToUInt16(Double)
Convert.ToUInt16(Decimal)
Convert.ToUInt16(String)
Convert.ToUInt16(String,IFormatProvider)
Convert.ToUInt16(DateTime)
Convert.ToInt32(Object)
Convert.ToInt32(Object,IFormatProvider)
Convert.ToInt32(Boolean)
Convert.ToInt32(Char)
Convert.ToInt32(SByte)
Convert.ToInt32(Byte)
Convert.ToInt32(Int16)
Convert.ToInt32(UInt16)
Convert.ToInt32(UInt32)
Convert.ToInt32(Int32)
Convert.ToInt32(Int64)
Convert.ToInt32(UInt64)
Convert.ToInt32(Single)
Convert.ToInt32(Double)
Convert.ToInt32(Decimal)
Convert.ToInt32(String)
Convert.ToInt32(String,IFormatProvider)
Convert.ToInt32(DateTime)
Convert.ToUInt32(Object)
Convert.ToUInt32(Object,IFormatProvider)
Convert.ToUInt32(Boolean)
Convert.ToUInt32(Char)
Convert.ToUInt32(SByte)
Convert.ToUInt32(Byte)
Convert.ToUInt32(Int16)
Convert.ToUInt32(UInt16)
Convert.ToUInt32(Int32)
Convert.ToUInt32(UInt32)
Convert.ToUInt32(Int64)
Convert.ToUInt32(UInt64)
Convert.ToUInt32(Single)
Convert.ToUInt32(Double)
Convert.ToUInt32(Decimal)
Convert.ToUInt32(String)
Convert.ToUInt32(String,IFormatProvider)
Convert.ToUInt32(DateTime)
Convert.ToInt64(Object)
Convert.ToInt64(Object,IFormatProvider)
Convert.ToInt64(Boolean)
Convert.ToInt64(Char)
Convert.ToInt64(SByte)
Convert.ToInt64(Byte)
Convert.ToInt64(Int16)
Convert.ToInt64(UInt16)
Convert.ToInt64(Int32)
Convert.ToInt64(UInt32)
Convert.ToInt64(UInt64)
Convert.ToInt64(Int64)
Convert.ToInt64(Single)
Convert.ToInt64(Double)
Convert.ToInt64(Decimal)
Convert.ToInt64(String)
Convert.ToInt64(String,IFormatProvider)
Convert.ToInt64(DateTime)
Convert.ToUInt64(Object)
Convert.ToUInt64(Object,IFormatProvider)
Convert.ToUInt64(Boolean)
Convert.ToUInt64(Char)
Convert.ToUInt64(SByte)
Convert.ToUInt64(Byte)
Convert.ToUInt64(Int16)
Convert.ToUInt64(UInt16)
Convert.ToUInt64(Int32)
Convert.ToUInt64(UInt32)
Convert.ToUInt64(Int64)
Convert.ToUInt64(UInt64)
Convert.ToUInt64(Single)
Convert.ToUInt64(Double)
Convert.ToUInt64(Decimal)
Convert.ToUInt64(String)
Convert.ToUInt64(String,IFormatProvider)
Convert.ToUInt64(DateTime)
Convert.ToSingle(Object)
Convert.ToSingle(Object,IFormatProvider)
Convert.ToSingle(SByte)
Convert.ToSingle(Byte)
Convert.ToSingle(Char)
Convert.ToSingle(Int16)
Convert.ToSingle(UInt16)
Convert.ToSingle(Int32)
Convert.ToSingle(UInt32)
Convert.ToSingle(Int64)
Convert.ToSingle(UInt64)
Convert.ToSingle(Single)
Convert.ToSingle(Double)
Convert.ToSingle(Decimal)
Convert.ToSingle(String)
Convert.ToSingle(String,IFormatProvider)
Convert.ToSingle(Boolean)
Convert.ToSingle(DateTime)
Convert.ToDouble(Object)
Convert.ToDouble(Object,IFormatProvider)
Convert.ToDouble(SByte)
Convert.ToDouble(Byte)
Convert.ToDouble(Int16)
Convert.ToDouble(Char)
Convert.ToDouble(UInt16)
Convert.ToDouble(Int32)
Convert.ToDouble(UInt32)
Convert.ToDouble(Int64)
Convert.ToDouble(UInt64)
Convert.ToDouble(Single)
Convert.ToDouble(Double)
Convert.ToDouble(Decimal)
Convert.ToDouble(String)
Convert.ToDouble(String,IFormatProvider)
Convert.ToDouble(Boolean)
Convert.ToDouble(DateTime)
Convert.ToDecimal(Object)
Convert.ToDecimal(Object,IFormatProvider)
Convert.ToDecimal(SByte)
Convert.ToDecimal(Byte)
Convert.ToDecimal(Char)
Convert.ToDecimal(Int16)
Convert.ToDecimal(UInt16)
Convert.ToDecimal(Int32)
Convert.ToDecimal(UInt32)
Convert.ToDecimal(Int64)
Convert.ToDecimal(UInt64)
Convert.ToDecimal(Single)
Convert.ToDecimal(Double)
Convert.ToDecimal(String)
Convert.ToDecimal(String,IFormatProvider)
Convert.ToDecimal(Decimal)
Convert.ToDecimal(Boolean)
Convert.ToDecimal(DateTime)
Convert.ToDateTime(DateTime)
Convert.ToDateTime(Object)
Convert.ToDateTime(Object,IFormatProvider)
Convert.ToDateTime(String)
Convert.ToDateTime(String,IFormatProvider)
Convert.ToDateTime(SByte)
Convert.ToDateTime(Byte)
Convert.ToDateTime(Int16)
Convert.ToDateTime(UInt16)
Convert.ToDateTime(Int32)
Convert.ToDateTime(UInt32)
Convert.ToDateTime(Int64)
Convert.ToDateTime(UInt64)
Convert.ToDateTime(Boolean)
Convert.ToDateTime(Char)
Convert.ToDateTime(Single)
Convert.ToDateTime(Double)
Convert.ToDateTime(Decimal)
Convert.ToByte(String,Int32)
Convert.ToSByte(String,Int32)
Convert.ToInt16(String,Int32)
Convert.ToUInt16(String,Int32)
Convert.ToInt32(String,Int32)
Convert.ToUInt32(String,Int32)
Convert.ToInt64(String,Int32)
Convert.ToUInt64(String,Int32)
Convert.ToBase64String(Byte[])
Convert.ToBase64String(Byte[],Base64FormattingOptions)
Convert.ToBase64String(Byte[],Int32,Int32)
Convert.ToBase64String(Byte[],Int32,Int32,Base64FormattingOptions)
Convert.ToBase64CharArray(Byte[],Int32,Int32,Char[],Int32)
Convert.ToBase64CharArray(Byte[],Int32,Int32,Char[],Int32,Base64FormattingOptions)
*Convert.FromBase64String(String)*
*Convert.FromBase64CharArray(Char[],Int32,Int32)*

Currency
----------------
*Currency.FromOACurrency(Int64)*
*Currency.ToDecimal(Currency)*

Decimal
----------------
*Decimal.ToOACurrency(Decimal)*
*Decimal.FromOACurrency(Int64)*
*Decimal.Add(Decimal,Decimal)*
*Decimal.Ceiling(Decimal)*
*Decimal.Compare(Decimal,Decimal)*
*Decimal.Divide(Decimal,Decimal)*
*Decimal.Equals(Decimal,Decimal)*
*Decimal.Floor(Decimal)*
Decimal.Parse(String)
Decimal.Parse(String,NumberStyles)
Decimal.Parse(String,IFormatProvider)
Decimal.Parse(String,NumberStyles,IFormatProvider)
Decimal.TryParse(String,Decimal&)
Decimal.TryParse(String,NumberStyles,IFormatProvider,Decimal&)
*Decimal.GetBits(Decimal)*
*Decimal.Remainder(Decimal,Decimal)*
*Decimal.Multiply(Decimal,Decimal)*
*Decimal.Negate(Decimal)*
Decimal.Round(Decimal)
Decimal.Round(Decimal,Int32)
Decimal.Round(Decimal,MidpointRounding)
Decimal.Round(Decimal,Int32,MidpointRounding)
*Decimal.Subtract(Decimal,Decimal)*
*Decimal.ToByte(Decimal)*
*Decimal.ToSByte(Decimal)*
*Decimal.ToInt16(Decimal)*
*Decimal.ToDouble(Decimal)*
*Decimal.ToInt32(Decimal)*
*Decimal.ToInt64(Decimal)*
*Decimal.ToUInt16(Decimal)*
*Decimal.ToUInt32(Decimal)*
*Decimal.ToUInt64(Decimal)*
*Decimal.ToSingle(Decimal)*
*Decimal.Truncate(Decimal)*

DefaultBinder
----------------
*DefaultBinder.ExactBinding(MethodBase[],Type[],ParameterModifier[])*
*DefaultBinder.ExactPropertyBinding(PropertyInfo[],Type,Type[],ParameterModifier[])*

Double
----------------
*Double.IsInfinity(Double)*
*Double.IsPositiveInfinity(Double)*
*Double.IsNegativeInfinity(Double)*
*Double.IsNaN(Double)*
Double.Parse(String)
Double.Parse(String,NumberStyles)
Double.Parse(String,IFormatProvider)
Double.Parse(String,NumberStyles,IFormatProvider)
Double.TryParse(String,Double&)
Double.TryParse(String,NumberStyles,IFormatProvider,Double&)

Enum
----------------
Enum.TryParse(String,TEnum&)
Enum.TryParse(String,Boolean,TEnum&)
Enum.Parse(Type,String)
Enum.Parse(Type,String,Boolean)
*Enum.GetUnderlyingType(Type)*
*Enum.GetValues(Type)*
*Enum.GetName(Type,Object)*
*Enum.GetNames(Type)*
Enum.ToObject(Type,Object)
*Enum.IsDefined(Type,Object)*
*Enum.Format(Type,Object,String)*
Enum.ToObject(Type,SByte)
Enum.ToObject(Type,Int16)
Enum.ToObject(Type,Int32)
Enum.ToObject(Type,Byte)
Enum.ToObject(Type,UInt16)
Enum.ToObject(Type,UInt32)
Enum.ToObject(Type,Int64)
Enum.ToObject(Type,UInt64)

Environment
----------------
*Environment.Exit(Int32)*
Environment.FailFast(String)
Environment.FailFast(String,Exception)
*Environment.ExpandEnvironmentVariables(String)*
*Environment.GetCommandLineArgs()*
Environment.GetEnvironmentVariable(String)
Environment.GetEnvironmentVariable(String,EnvironmentVariableTarget)
Environment.GetEnvironmentVariables()
Environment.GetEnvironmentVariables(EnvironmentVariableTarget)
Environment.SetEnvironmentVariable(String,String)
Environment.SetEnvironmentVariable(String,String,EnvironmentVariableTarget)
*Environment.GetLogicalDrives()*
Environment.GetFolderPath(SpecialFolder)
Environment.GetFolderPath(SpecialFolder,SpecialFolderOption)

FormattableString
----------------
*FormattableString.Invariant(FormattableString)*

Guid
----------------
*Guid.Parse(String)*
*Guid.TryParse(String,Guid&)*
*Guid.ParseExact(String,String)*
*Guid.TryParseExact(String,String,Guid&)*
*Guid.NewGuid()*

Int16
----------------
Int16.Parse(String)
Int16.Parse(String,NumberStyles)
Int16.Parse(String,IFormatProvider)
Int16.Parse(String,NumberStyles,IFormatProvider)
Int16.TryParse(String,Int16&)
Int16.TryParse(String,NumberStyles,IFormatProvider,Int16&)

Int32
----------------
Int32.Parse(String)
Int32.Parse(String,NumberStyles)
Int32.Parse(String,IFormatProvider)
Int32.Parse(String,NumberStyles,IFormatProvider)
Int32.TryParse(String,Int32&)
Int32.TryParse(String,NumberStyles,IFormatProvider,Int32&)

Int64
----------------
Int64.Parse(String)
Int64.Parse(String,NumberStyles)
Int64.Parse(String,IFormatProvider)
Int64.Parse(String,NumberStyles,IFormatProvider)
Int64.TryParse(String,Int64&)
Int64.TryParse(String,NumberStyles,IFormatProvider,Int64&)

IntPtr
----------------
*IntPtr.Add(IntPtr,Int32)*
*IntPtr.Subtract(IntPtr,Int32)*

Math
----------------
*Math.Acos(Double)*
*Math.Asin(Double)*
*Math.Atan(Double)*
*Math.Atan2(Double,Double)*
Math.Ceiling(Decimal)
Math.Ceiling(Double)
*Math.Cos(Double)*
*Math.Cosh(Double)*
Math.Floor(Decimal)
Math.Floor(Double)
*Math.Sin(Double)*
*Math.Tan(Double)*
*Math.Sinh(Double)*
*Math.Tanh(Double)*
Math.Round(Double)
Math.Round(Double,Int32)
Math.Round(Double,MidpointRounding)
Math.Round(Double,Int32,MidpointRounding)
Math.Round(Decimal)
Math.Round(Decimal,Int32)
Math.Round(Decimal,MidpointRounding)
Math.Round(Decimal,Int32,MidpointRounding)
Math.Truncate(Decimal)
Math.Truncate(Double)
*Math.Sqrt(Double)*
Math.Log(Double)
*Math.Log10(Double)*
*Math.Exp(Double)*
*Math.Pow(Double,Double)*
*Math.IEEERemainder(Double,Double)*
Math.Abs(SByte)
Math.Abs(Int16)
Math.Abs(Int32)
Math.Abs(Int64)
Math.Abs(Single)
Math.Abs(Double)
Math.Abs(Decimal)
Math.Max(SByte,SByte)
Math.Max(Byte,Byte)
Math.Max(Int16,Int16)
Math.Max(UInt16,UInt16)
Math.Max(Int32,Int32)
Math.Max(UInt32,UInt32)
Math.Max(Int64,Int64)
Math.Max(UInt64,UInt64)
Math.Max(Single,Single)
Math.Max(Double,Double)
Math.Max(Decimal,Decimal)
Math.Min(SByte,SByte)
Math.Min(Byte,Byte)
Math.Min(Int16,Int16)
Math.Min(UInt16,UInt16)
Math.Min(Int32,Int32)
Math.Min(UInt32,UInt32)
Math.Min(Int64,Int64)
Math.Min(UInt64,UInt64)
Math.Min(Single,Single)
Math.Min(Double,Double)
Math.Min(Decimal,Decimal)
Math.Log(Double,Double)
Math.Sign(SByte)
Math.Sign(Int16)
Math.Sign(Int32)
Math.Sign(Int64)
Math.Sign(Single)
Math.Sign(Double)
Math.Sign(Decimal)
*Math.BigMul(Int32,Int32)*
Math.DivRem(Int32,Int32,Int32&)
Math.DivRem(Int64,Int64,Int64&)

Number
----------------
*Number.FormatDecimal(Decimal,String,NumberFormatInfo)*
*Number.FormatDouble(Double,String,NumberFormatInfo)*
*Number.FormatInt32(Int32,String,NumberFormatInfo)*
*Number.FormatUInt32(UInt32,String,NumberFormatInfo)*
*Number.FormatInt64(Int64,String,NumberFormatInfo)*
*Number.FormatUInt64(UInt64,String,NumberFormatInfo)*
*Number.FormatSingle(Single,String,NumberFormatInfo)*
*Number.NumberBufferToDecimal(Byte*,Decimal&)*

ParseNumbers
----------------
ParseNumbers.StringToLong(String,Int32,Int32,Int32*)
ParseNumbers.StringToInt(String,Int32,Int32,Int32*)
*ParseNumbers.IntToString(Int32,Int32,Int32,Char,Int32)*
ParseNumbers.StringToLong(String,Int32,Int32)
ParseNumbers.StringToLong(String,Int32,Int32,Int32&)
ParseNumbers.StringToInt(String,Int32,Int32)
ParseNumbers.StringToInt(String,Int32,Int32,Int32&)
*ParseNumbers.LongToString(Int64,Int32,Int32,Char,Int32)*

SByte
----------------
SByte.Parse(String)
SByte.Parse(String,NumberStyles)
SByte.Parse(String,IFormatProvider)
SByte.Parse(String,NumberStyles,IFormatProvider)
SByte.TryParse(String,SByte&)
SByte.TryParse(String,NumberStyles,IFormatProvider,SByte&)

SharedStatics
----------------
*SharedStatics.GetSharedStringMaker()*
*SharedStatics.ReleaseSharedStringMaker(StringMaker&)*

Single
----------------
*Single.IsInfinity(Single)*
*Single.IsPositiveInfinity(Single)*
*Single.IsNegativeInfinity(Single)*
*Single.IsNaN(Single)*
Single.Parse(String)
Single.Parse(String,NumberStyles)
Single.Parse(String,IFormatProvider)
Single.Parse(String,NumberStyles,IFormatProvider)
Single.TryParse(String,Single&)
Single.TryParse(String,NumberStyles,IFormatProvider,Single&)

TimeSpan
----------------
*TimeSpan.Compare(TimeSpan,TimeSpan)*
*TimeSpan.FromDays(Double)*
*TimeSpan.Equals(TimeSpan,TimeSpan)*
*TimeSpan.FromHours(Double)*
*TimeSpan.FromMilliseconds(Double)*
*TimeSpan.FromMinutes(Double)*
*TimeSpan.FromSeconds(Double)*
*TimeSpan.FromTicks(Int64)*
TimeSpan.Parse(String)
TimeSpan.Parse(String,IFormatProvider)
TimeSpan.ParseExact(String,String,IFormatProvider)
TimeSpan.ParseExact(String,String[],IFormatProvider)
TimeSpan.ParseExact(String,String,IFormatProvider,TimeSpanStyles)
TimeSpan.ParseExact(String,String[],IFormatProvider,TimeSpanStyles)
TimeSpan.TryParse(String,TimeSpan&)
TimeSpan.TryParse(String,IFormatProvider,TimeSpan&)
TimeSpan.TryParseExact(String,String,IFormatProvider,TimeSpan&)
TimeSpan.TryParseExact(String,String[],IFormatProvider,TimeSpan&)
TimeSpan.TryParseExact(String,String,IFormatProvider,TimeSpanStyles,TimeSpan&)
TimeSpan.TryParseExact(String,String[],IFormatProvider,TimeSpanStyles,TimeSpan&)

TimeZone
----------------
*TimeZone.IsDaylightSavingTime(DateTime,DaylightTime)*

TimeZoneInfo
----------------
*TimeZoneInfo.ClearCachedData()*
TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset,String)
TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime,String)
TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime,String,String)
TimeZoneInfo.ConvertTime(DateTimeOffset,TimeZoneInfo)
TimeZoneInfo.ConvertTime(DateTime,TimeZoneInfo)
TimeZoneInfo.ConvertTime(DateTime,TimeZoneInfo,TimeZoneInfo)
*TimeZoneInfo.ConvertTimeFromUtc(DateTime,TimeZoneInfo)*
TimeZoneInfo.ConvertTimeToUtc(DateTime)
TimeZoneInfo.ConvertTimeToUtc(DateTime,TimeZoneInfo)
*TimeZoneInfo.FromSerializedString(String)*
*TimeZoneInfo.GetSystemTimeZones()*
TimeZoneInfo.CreateCustomTimeZone(String,TimeSpan,String,String)
TimeZoneInfo.CreateCustomTimeZone(String,TimeSpan,String,String,String,AdjustmentRule[])
TimeZoneInfo.CreateCustomTimeZone(String,TimeSpan,String,String,String,AdjustmentRule[],Boolean)
*TimeZoneInfo.FindSystemTimeZoneById(String)*

Type
----------------
Type.GetType(String,Boolean,Boolean)
Type.GetType(String,Boolean)
Type.GetType(String)
Type.GetType(String,Func`2,Func`4)
Type.GetType(String,Func`2,Func`4,Boolean)
Type.GetType(String,Func`2,Func`4,Boolean,Boolean)
*Type.ReflectionOnlyGetType(String,Boolean,Boolean)*
Type.GetTypeFromProgID(String)
Type.GetTypeFromProgID(String,Boolean)
Type.GetTypeFromProgID(String,String)
Type.GetTypeFromProgID(String,String,Boolean)
Type.GetTypeFromCLSID(Guid)
Type.GetTypeFromCLSID(Guid,Boolean)
Type.GetTypeFromCLSID(Guid,String)
Type.GetTypeFromCLSID(Guid,String,Boolean)
*Type.GetTypeCode(Type)*
*Type.GetTypeHandle(Object)*
*Type.GetTypeFromHandle(RuntimeTypeHandle)*
*Type.GetTypeArray(Object[])*

TypedReference
----------------
*TypedReference.MakeTypedReference(Object,FieldInfo[])*
*TypedReference.ToObject(TypedReference)*
*TypedReference.GetTargetType(TypedReference)*
*TypedReference.TargetTypeToken(TypedReference)*
*TypedReference.SetTypedReference(TypedReference,Object)*

UInt16
----------------
UInt16.Parse(String)
UInt16.Parse(String,NumberStyles)
UInt16.Parse(String,IFormatProvider)
UInt16.Parse(String,NumberStyles,IFormatProvider)
UInt16.TryParse(String,UInt16&)
UInt16.TryParse(String,NumberStyles,IFormatProvider,UInt16&)

UInt32
----------------
UInt32.Parse(String)
UInt32.Parse(String,NumberStyles)
UInt32.Parse(String,IFormatProvider)
UInt32.Parse(String,NumberStyles,IFormatProvider)
UInt32.TryParse(String,UInt32&)
UInt32.TryParse(String,NumberStyles,IFormatProvider,UInt32&)

UInt64
----------------
UInt64.Parse(String)
UInt64.Parse(String,NumberStyles)
UInt64.Parse(String,IFormatProvider)
UInt64.Parse(String,NumberStyles,IFormatProvider)
UInt64.TryParse(String,UInt64&)
UInt64.TryParse(String,NumberStyles,IFormatProvider,UInt64&)

Version
----------------
*Version.Parse(String)*
*Version.TryParse(String,Version&)*

Nullable
----------------
*Nullable.Compare(Nullable`1,Nullable`1)*
*Nullable.Equals(Nullable`1,Nullable`1)*
*Nullable.GetUnderlyingType(Type)*

Directory
----------------
*Directory.GetParent(String)*
Directory.CreateDirectory(String)
Directory.CreateDirectory(String,DirectorySecurity)
*Directory.Exists(String)*
*Directory.SetCreationTime(String,DateTime)*
*Directory.SetCreationTimeUtc(String,DateTime)*
*Directory.GetCreationTime(String)*
*Directory.GetCreationTimeUtc(String)*
*Directory.SetLastWriteTime(String,DateTime)*
*Directory.SetLastWriteTimeUtc(String,DateTime)*
*Directory.GetLastWriteTime(String)*
*Directory.GetLastWriteTimeUtc(String)*
*Directory.SetLastAccessTime(String,DateTime)*
*Directory.SetLastAccessTimeUtc(String,DateTime)*
*Directory.GetLastAccessTime(String)*
*Directory.GetLastAccessTimeUtc(String)*
Directory.GetAccessControl(String)
Directory.GetAccessControl(String,AccessControlSections)
*Directory.SetAccessControl(String,DirectorySecurity)*
Directory.GetFiles(String)
Directory.GetFiles(String,String)
Directory.GetFiles(String,String,SearchOption)
Directory.GetDirectories(String)
Directory.GetDirectories(String,String)
Directory.GetDirectories(String,String,SearchOption)
Directory.GetFileSystemEntries(String)
Directory.GetFileSystemEntries(String,String)
Directory.GetFileSystemEntries(String,String,SearchOption)
Directory.EnumerateDirectories(String)
Directory.EnumerateDirectories(String,String)
Directory.EnumerateDirectories(String,String,SearchOption)
Directory.EnumerateFiles(String)
Directory.EnumerateFiles(String,String)
Directory.EnumerateFiles(String,String,SearchOption)
Directory.EnumerateFileSystemEntries(String)
Directory.EnumerateFileSystemEntries(String,String)
Directory.EnumerateFileSystemEntries(String,String,SearchOption)
*Directory.GetLogicalDrives()*
*Directory.GetDirectoryRoot(String)*
*Directory.GetCurrentDirectory()*
*Directory.SetCurrentDirectory(String)*
*Directory.Move(String,String)*
Directory.Delete(String)
Directory.Delete(String,Boolean)

DriveInfo
----------------
*DriveInfo.GetDrives()*

File
----------------
*File.OpenText(String)*
*File.CreateText(String)*
*File.AppendText(String)*
File.Copy(String,String)
File.Copy(String,String,Boolean)
File.Create(String)
File.Create(String,Int32)
File.Create(String,Int32,FileOptions)
File.Create(String,Int32,FileOptions,FileSecurity)
*File.Delete(String)*
*File.Decrypt(String)*
*File.Encrypt(String)*
*File.Exists(String)*
File.Open(String,FileMode)
File.Open(String,FileMode,FileAccess)
File.Open(String,FileMode,FileAccess,FileShare)
*File.SetCreationTime(String,DateTime)*
*File.SetCreationTimeUtc(String,DateTime)*
*File.GetCreationTime(String)*
*File.GetCreationTimeUtc(String)*
*File.SetLastAccessTime(String,DateTime)*
*File.SetLastAccessTimeUtc(String,DateTime)*
*File.GetLastAccessTime(String)*
*File.GetLastAccessTimeUtc(String)*
*File.SetLastWriteTime(String,DateTime)*
*File.SetLastWriteTimeUtc(String,DateTime)*
*File.GetLastWriteTime(String)*
*File.GetLastWriteTimeUtc(String)*
*File.GetAttributes(String)*
*File.SetAttributes(String,FileAttributes)*
File.GetAccessControl(String)
File.GetAccessControl(String,AccessControlSections)
*File.SetAccessControl(String,FileSecurity)*
*File.OpenRead(String)*
*File.OpenWrite(String)*
File.ReadAllText(String)
File.ReadAllText(String,Encoding)
File.WriteAllText(String,String)
File.WriteAllText(String,String,Encoding)
*File.ReadAllBytes(String)*
*File.WriteAllBytes(String,Byte[])*
File.ReadAllLines(String)
File.ReadAllLines(String,Encoding)
File.ReadLines(String)
File.ReadLines(String,Encoding)
File.WriteAllLines(String,String[])
File.WriteAllLines(String,String[],Encoding)
File.WriteAllLines(String,IEnumerable`1)
File.WriteAllLines(String,IEnumerable`1,Encoding)
File.AppendAllText(String,String)
File.AppendAllText(String,String,Encoding)
File.AppendAllLines(String,IEnumerable`1)
File.AppendAllLines(String,IEnumerable`1,Encoding)
*File.Move(String,String)*
File.Replace(String,String,String)
File.Replace(String,String,String,Boolean)

Path
----------------
*Path.ChangeExtension(String,String)*
*Path.GetDirectoryName(String)*
*Path.GetInvalidPathChars()*
*Path.GetInvalidFileNameChars()*
*Path.GetExtension(String)*
*Path.GetFullPath(String)*
*Path.GetFileName(String)*
*Path.GetFileNameWithoutExtension(String)*
*Path.GetPathRoot(String)*
*Path.GetTempPath()*
*Path.GetRandomFileName()*
*Path.GetTempFileName()*
*Path.HasExtension(String)*
*Path.IsPathRooted(String)*
Path.Combine(String,String)
Path.Combine(String,String,String)
Path.Combine(String,String,String,String)
Path.Combine(String[])

Stream
----------------
*Stream.Synchronized(Stream)*

TextReader
----------------
*TextReader.Synchronized(TextReader)*

TextWriter
----------------
*TextWriter.Synchronized(TextWriter)*

IsolatedStorageFile
----------------
*IsolatedStorageFile.GetUserStoreForDomain()*
*IsolatedStorageFile.GetUserStoreForAssembly()*
*IsolatedStorageFile.GetUserStoreForApplication()*
*IsolatedStorageFile.GetUserStoreForSite()*
*IsolatedStorageFile.GetMachineStoreForDomain()*
*IsolatedStorageFile.GetMachineStoreForAssembly()*
*IsolatedStorageFile.GetMachineStoreForApplication()*
IsolatedStorageFile.GetStore(IsolatedStorageScope,Type,Type)
IsolatedStorageFile.GetStore(IsolatedStorageScope,Object,Object)
IsolatedStorageFile.GetStore(IsolatedStorageScope,Evidence,Type,Evidence,Type)
IsolatedStorageFile.GetStore(IsolatedStorageScope,Type)
IsolatedStorageFile.GetStore(IsolatedStorageScope,Object)
*IsolatedStorageFile.Remove(IsolatedStorageScope)*
*IsolatedStorageFile.GetEnumerator(IsolatedStorageScope)*

SecurityElement
----------------
*SecurityElement.FromString(String)*
*SecurityElement.IsValidTag(String)*
*SecurityElement.IsValidText(String)*
*SecurityElement.IsValidAttributeName(String)*
*SecurityElement.IsValidAttributeValue(String)*
*SecurityElement.Escape(String)*

SecurityDocument
----------------
*SecurityDocument.EncodedStringSize(String)*

CodeAccessPermission
----------------
*CodeAccessPermission.RevertAssert()*
*CodeAccessPermission.RevertDeny()*
*CodeAccessPermission.RevertPermitOnly()*
*CodeAccessPermission.RevertAll()*

PermissionSet
----------------
*PermissionSet.ConvertPermissionSet(String,Byte[],String)*
*PermissionSet.RevertAssert()*

PermissionToken
----------------
PermissionToken.GetToken(Type)
*PermissionToken.FindToken(Type)*
PermissionToken.GetToken(IPermission)
PermissionToken.GetToken(String)
PermissionToken.GetToken(String,Boolean)
*PermissionToken.FindTokenByIndex(Int32)*
*PermissionToken.IsTokenProperlyAssigned(IPermission,PermissionToken)*

SecurityContext
----------------
*SecurityContext.Run(SecurityContext,ContextCallback,Object)*
*SecurityContext.Capture()*
*SecurityContext.SuppressFlow()*
*SecurityContext.SuppressFlowWindowsIdentity()*
*SecurityContext.RestoreFlow()*
*SecurityContext.IsFlowSuppressed()*
*SecurityContext.IsWindowsIdentityFlowSuppressed()*

SecurityManager
----------------
*SecurityManager.GetStandardSandbox(Evidence)*
*SecurityManager.LoadPolicyLevelFromString(String,PolicyLevelType)*
*SecurityManager.IsGranted(IPermission)*
*SecurityManager.GetZoneAndOrigin(ArrayList&,ArrayList&)*
*SecurityManager.LoadPolicyLevelFromFile(String,PolicyLevelType)*
*SecurityManager.SavePolicyLevel(PolicyLevel)*
SecurityManager.ResolvePolicy(Evidence,PermissionSet,PermissionSet,PermissionSet,PermissionSet&)
SecurityManager.ResolvePolicy(Evidence)
SecurityManager.ResolvePolicy(Evidence[])
*SecurityManager.CurrentThreadRequiresSecurityContextCapture()*
*SecurityManager.ResolveSystemPolicy(Evidence)*
*SecurityManager.ResolvePolicyGroups(Evidence)*
*SecurityManager.PolicyHierarchy()*
*SecurityManager.SavePolicy()*

GenericAce
----------------
*GenericAce.CreateFromBinaryForm(Byte[],Int32)*

CommonAce
----------------
*CommonAce.MaxOpaqueLength(Boolean)*

ObjectAce
----------------
*ObjectAce.MaxOpaqueLength(Boolean)*

ObjectSecurity
----------------
*ObjectSecurity.IsSddlConversionSupported()*

GenericSecurityDescriptor
----------------
*GenericSecurityDescriptor.IsSddlConversionSupported()*

RandomNumberGenerator
----------------
RandomNumberGenerator.Create()
RandomNumberGenerator.Create(String)

Aes
----------------
Aes.Create()
Aes.Create(String)

AsymmetricAlgorithm
----------------
AsymmetricAlgorithm.Create()
AsymmetricAlgorithm.Create(String)

CryptoConfig
----------------
CryptoConfig.CreateFromName(String,Object[])
*CryptoConfig.AddAlgorithm(Type,String[])*
CryptoConfig.CreateFromName(String)
*CryptoConfig.AddOID(String,String[])*
*CryptoConfig.MapNameToOID(String)*
*CryptoConfig.EncodeOID(String)*

DES
----------------
DES.Create()
DES.Create(String)
*DES.IsWeakKey(Byte[])*
*DES.IsSemiWeakKey(Byte[])*

DSA
----------------
DSA.Create()
DSA.Create(String)

HMAC
----------------
HMAC.Create()
HMAC.Create(String)

HashAlgorithm
----------------
HashAlgorithm.Create()
HashAlgorithm.Create(String)

KeyedHashAlgorithm
----------------
KeyedHashAlgorithm.Create()
KeyedHashAlgorithm.Create(String)

MD5
----------------
MD5.Create()
MD5.Create(String)

RC2
----------------
RC2.Create()
RC2.Create(String)

RIPEMD160
----------------
RIPEMD160.Create()
RIPEMD160.Create(String)

RSA
----------------
RSA.Create()
RSA.Create(String)

RSAEncryptionPadding
----------------
*RSAEncryptionPadding.CreateOaep(HashAlgorithmName)*

Rijndael
----------------
Rijndael.Create()
Rijndael.Create(String)

SHA1
----------------
SHA1.Create()
SHA1.Create(String)

SHA256
----------------
SHA256.Create()
SHA256.Create(String)

SHA384
----------------
SHA384.Create()
SHA384.Create(String)

SHA512
----------------
SHA512.Create()
SHA512.Create(String)

SymmetricAlgorithm
----------------
SymmetricAlgorithm.Create()
SymmetricAlgorithm.Create(String)

TripleDES
----------------
TripleDES.Create()
TripleDES.Create(String)
*TripleDES.IsWeakKey(Byte[])*

X509Certificate
----------------
*X509Certificate.CreateFromCertFile(String)*
*X509Certificate.CreateFromSignedFile(String)*

WindowsIdentity
----------------
WindowsIdentity.GetCurrent()
WindowsIdentity.GetCurrent(Boolean)
WindowsIdentity.GetCurrent(TokenAccessLevels)
*WindowsIdentity.GetAnonymous()*
WindowsIdentity.RunImpersonated(SafeAccessTokenHandle,Action)
WindowsIdentity.RunImpersonated(SafeAccessTokenHandle,Func`1)
*WindowsIdentity.Impersonate(IntPtr)*

ApplicationSecurityManager
----------------
*ApplicationSecurityManager.DetermineApplicationTrust(ActivationContext,TrustManagerContext)*

CodeConnectAccess
----------------
*CodeConnectAccess.CreateOriginSchemeAccess(Int32)*
*CodeConnectAccess.CreateAnySchemeAccess(Int32)*

PolicyLevel
----------------
*PolicyLevel.CreateAppDomainLevel()*

Site
----------------
*Site.CreateFromUrl(String)*

Zone
----------------
*Zone.CreateFromUrl(String)*

Hash
----------------
*Hash.CreateSHA1(Byte[])*
*Hash.CreateSHA256(Byte[])*
*Hash.CreateMD5(Byte[])*

Hex
----------------
*Hex.EncodeHexString(Byte[])*
*Hex.ConvertHexDigit(Char)*
*Hex.DecodeHexString(String)*

URLString
----------------
*URLString.CompareUrls(URLString,URLString)*

XMLUtil
----------------
*XMLUtil.AddClassAttribute(SecurityElement,Type,String)*
*XMLUtil.CreatePermission(SecurityElement,PermissionState,Boolean)*
*XMLUtil.CreateCodeGroup(SecurityElement)*
*XMLUtil.IsPermissionElement(IPermission,SecurityElement)*
*XMLUtil.IsUnrestricted(SecurityElement)*
*XMLUtil.BitFieldEnumToString(Type,Object)*
XMLUtil.NewPermissionElement(IPermission)
XMLUtil.NewPermissionElement(String)
*XMLUtil.SecurityObjectToXmlString(Object)*
*XMLUtil.XmlStringToSecurityObject(String)*

FastResourceComparer
----------------
FastResourceComparer.CompareOrdinal(String,Byte[],Int32)
FastResourceComparer.CompareOrdinal(Byte[],Int32,String)

ResourceManager
----------------
*ResourceManager.CreateFileBasedResourceManager(String,String,Type)*

Calendar
----------------
*Calendar.ReadOnly(Calendar)*

CharUnicodeInfo
----------------
CharUnicodeInfo.GetNumericValue(Char)
CharUnicodeInfo.GetNumericValue(String,Int32)
CharUnicodeInfo.GetDecimalDigitValue(Char)
CharUnicodeInfo.GetDecimalDigitValue(String,Int32)
CharUnicodeInfo.GetDigitValue(Char)
CharUnicodeInfo.GetDigitValue(String,Int32)
CharUnicodeInfo.GetUnicodeCategory(Char)
CharUnicodeInfo.GetUnicodeCategory(String,Int32)

CompareInfo
----------------
CompareInfo.GetCompareInfo(Int32,Assembly)
CompareInfo.GetCompareInfo(String,Assembly)
CompareInfo.GetCompareInfo(Int32)
CompareInfo.GetCompareInfo(String)
CompareInfo.IsSortable(Char)
CompareInfo.IsSortable(String)

CultureInfo
----------------
*CultureInfo.CreateSpecificCulture(String)*
*CultureInfo.GetCultures(CultureTypes)*
*CultureInfo.ReadOnly(CultureInfo)*
CultureInfo.GetCultureInfo(Int32)
CultureInfo.GetCultureInfo(String)
CultureInfo.GetCultureInfo(String,String)
*CultureInfo.GetCultureInfoByIetfLanguageTag(String)*

DateTimeFormatInfo
----------------
*DateTimeFormatInfo.GetInstance(IFormatProvider)*
*DateTimeFormatInfo.ReadOnly(DateTimeFormatInfo)*

CalendricalCalculationsHelper
----------------
*CalendricalCalculationsHelper.Angle(Int32,Int32,Double)*
*CalendricalCalculationsHelper.AsDayFraction(Double)*
*CalendricalCalculationsHelper.JulianCenturies(Double)*
*CalendricalCalculationsHelper.Midday(Double,Double)*
*CalendricalCalculationsHelper.MiddayAtPersianObservationSite(Double)*
*CalendricalCalculationsHelper.Compute(Double)*
*CalendricalCalculationsHelper.AsSeason(Double)*

SortKey
----------------
*SortKey.Compare(SortKey,SortKey)*

StringInfo
----------------
StringInfo.GetNextTextElement(String,Int32)
StringInfo.GetNextTextElement(String)
StringInfo.GetTextElementEnumerator(String)
StringInfo.GetTextElementEnumerator(String,Int32)
*StringInfo.ParseCombiningCharacters(String)*

TextInfo
----------------
*TextInfo.ReadOnly(TextInfo)*

NumberFormatInfo
----------------
*NumberFormatInfo.GetInstance(IFormatProvider)*
*NumberFormatInfo.ReadOnly(NumberFormatInfo)*

Debugger
----------------
*Debugger.Break()*
*Debugger.Launch()*
*Debugger.NotifyOfCrossThreadDependency()*
*Debugger.Log(Int32,String,String)*
*Debugger.IsLogging()*

Log
----------------
*Log.AddOnLogMessage(LogMessageEventHandler)*
*Log.RemoveOnLogMessage(LogMessageEventHandler)*
*Log.AddOnLogSwitchLevel(LogSwitchLevelHandler)*
*Log.RemoveOnLogSwitchLevel(LogSwitchLevelHandler)*
Log.LogMessage(LoggingLevels,String)
Log.LogMessage(LoggingLevels,LogSwitch,String)
Log.Trace(LogSwitch,String)
Log.Trace(String,String)
Log.Trace(String)
Log.Status(LogSwitch,String)
Log.Status(String,String)
Log.Status(String)
Log.Warning(LogSwitch,String)
Log.Warning(String,String)
Log.Warning(String)
Log.Error(LogSwitch,String)
Log.Error(String,String)
Log.Error(String)
*Log.Panic(String)*

LogSwitch
----------------
*LogSwitch.GetSwitch(String)*

Contract
----------------
Contract.Assume(Boolean)
Contract.Assume(Boolean,String)
Contract.Assert(Boolean)
Contract.Assert(Boolean,String)
Contract.Requires(Boolean)
Contract.Requires(Boolean,String)
Contract.Requires(Boolean)
Contract.Requires(Boolean,String)
Contract.Ensures(Boolean)
Contract.Ensures(Boolean,String)
Contract.EnsuresOnThrow(Boolean)
Contract.EnsuresOnThrow(Boolean,String)
*Contract.Result()*
*Contract.ValueAtReturn(T&)*
*Contract.OldValue(T)*
Contract.Invariant(Boolean)
Contract.Invariant(Boolean,String)
Contract.ForAll(Int32,Int32,Predicate`1)
Contract.ForAll(IEnumerable`1,Predicate`1)
Contract.Exists(Int32,Int32,Predicate`1)
Contract.Exists(IEnumerable`1,Predicate`1)
*Contract.EndContractBlock()*
*Contract.add_ContractFailed(EventHandler`1)*
*Contract.remove_ContractFailed(EventHandler`1)*

ContractHelper
----------------
*ContractHelper.RaiseContractFailedEvent(ContractFailureKind,String,String,Exception)*
*ContractHelper.TriggerFailure(ContractFailureKind,String,String,String,Exception)*

EventProvider
----------------
*EventProvider.GetLastWriteEventError()*

EventSource
----------------
*EventSource.GetGuid(Type)*
*EventSource.GetName(Type)*
EventSource.GenerateManifest(Type,String)
EventSource.GenerateManifest(Type,String,EventManifestOptions)
*EventSource.GetSources()*
*EventSource.SendCommand(EventSource,EventCommand,IDictionary`2)*
EventSource.SetCurrentThreadActivityId(Guid)
EventSource.SetCurrentThreadActivityId(Guid,Guid&)

ActivityFilter
----------------
*ActivityFilter.DisableFilter(ActivityFilter&,EventSource)*
*ActivityFilter.UpdateFilter(ActivityFilter&,EventSource,Int32,String)*
*ActivityFilter.GetFilter(ActivityFilter,EventSource)*
*ActivityFilter.PassesActivityFilter(ActivityFilter,Guid*,Boolean,EventSource,Int32)*
*ActivityFilter.IsCurrentActivityActive(ActivityFilter)*
*ActivityFilter.FlowActivityIfNeeded(ActivityFilter,Guid*,Guid*)*
*ActivityFilter.UpdateKwdTriggers(ActivityFilter,Guid,EventSource,EventKeywords)*

EtwSession
----------------
*EtwSession.GetEtwSession(Int32,Boolean)*
*EtwSession.RemoveEtwSession(EtwSession)*

SessionMask
----------------
*SessionMask.FromId(Int32)*
*SessionMask.FromEventKeywords(UInt64)*

EnumHelper`1
----------------
*EnumHelper`1.Cast(ValueType)*

PropertyAccessor`1
----------------
*PropertyAccessor`1.Create(PropertyAnalysis)*

Environment
----------------
*Environment.GetResourceString(String,Object[])*
*Environment.GetRuntimeResourceString(String,Object[])*

Queue
----------------
*Queue.Synchronized(Queue)*

Stack
----------------
*Stack.Synchronized(Stack)*

Hashtable
----------------
*Hashtable.Synchronized(Hashtable)*

HashHelpers
----------------
*HashHelpers.IsPrime(Int32)*
*HashHelpers.GetPrime(Int32)*
*HashHelpers.GetMinPrime()*
*HashHelpers.ExpandPrime(Int32)*
*HashHelpers.IsWellKnownEqualityComparer(Object)*
*HashHelpers.GetRandomizedEqualityComparer(Object)*
*HashHelpers.GetEqualityComparerForSerialization(Object)*

SortedList
----------------
*SortedList.Synchronized(SortedList)*

Partitioner
----------------
Partitioner.Create(IList`1,Boolean)
Partitioner.Create(TSource[],Boolean)
Partitioner.Create(IEnumerable`1)
Partitioner.Create(IEnumerable`1,EnumerablePartitionerOptions)
Partitioner.Create(Int64,Int64)
Partitioner.Create(Int64,Int64,Int64)
Partitioner.Create(Int32,Int32)
Partitioner.Create(Int32,Int32,Int32)

Comparer`1
----------------
*Comparer`1.Create(Comparison`1)*

SynchronizationContext
----------------
*SynchronizationContext.SetSynchronizationContext(SynchronizationContext)*

CompressedStack
----------------
*CompressedStack.Capture()*
*CompressedStack.Run(CompressedStack,ContextCallback,Object)*
*CompressedStack.GetCompressedStack()*

EventWaitHandle
----------------
EventWaitHandle.OpenExisting(String)
EventWaitHandle.OpenExisting(String,EventWaitHandleRights)
EventWaitHandle.TryOpenExisting(String,EventWaitHandle&)
EventWaitHandle.TryOpenExisting(String,EventWaitHandleRights,EventWaitHandle&)

ExecutionContext
----------------
*ExecutionContext.Run(ExecutionContext,ContextCallback,Object)*
*ExecutionContext.SuppressFlow()*
*ExecutionContext.RestoreFlow()*
*ExecutionContext.IsFlowSuppressed()*
*ExecutionContext.Capture()*

HostExecutionContextSwitcher
----------------
*HostExecutionContextSwitcher.Undo(Object)*

Mutex
----------------
Mutex.OpenExisting(String)
Mutex.OpenExisting(String,MutexRights)
Mutex.TryOpenExisting(String,Mutex&)
Mutex.TryOpenExisting(String,MutexRights,Mutex&)

Overlapped
----------------
*Overlapped.Unpack(NativeOverlapped*)*
*Overlapped.Free(NativeOverlapped*)*

Gen2GcCallback
----------------
*Gen2GcCallback.Register(Func`2,Object)*

WaitHandleExtensions
----------------
*WaitHandleExtensions.GetSafeWaitHandle(WaitHandle)*
*WaitHandleExtensions.SetSafeWaitHandle(WaitHandle,SafeWaitHandle)*

TimeoutHelper
----------------
*TimeoutHelper.GetTime()*
*TimeoutHelper.UpdateTimeOut(UInt32,Int32)*

LazyInitializer
----------------
LazyInitializer.EnsureInitialized(T&)
LazyInitializer.EnsureInitialized(T&,Func`1)
LazyInitializer.EnsureInitialized(T&,Boolean&,Object&)
LazyInitializer.EnsureInitialized(T&,Boolean&,Object&,Func`1)

CancellationTokenSource
----------------
CancellationTokenSource.CreateLinkedTokenSource(CancellationToken,CancellationToken)
CancellationTokenSource.CreateLinkedTokenSource(CancellationToken[])

TaskToApm
----------------
*TaskToApm.Begin(Task,AsyncCallback,Object)*
TaskToApm.End(IAsyncResult)
TaskToApm.End(IAsyncResult)

Assembly
----------------
*Assembly.CreateQualifiedName(String,String)*
*Assembly.GetAssembly(Type)*
Assembly.LoadFrom(String)
*Assembly.ReflectionOnlyLoadFrom(String)*
Assembly.LoadFrom(String,Evidence)
Assembly.LoadFrom(String,Evidence,Byte[],AssemblyHashAlgorithm)
Assembly.LoadFrom(String,Byte[],AssemblyHashAlgorithm)
*Assembly.UnsafeLoadFrom(String)*
Assembly.Load(String)
Assembly.ReflectionOnlyLoad(String)
Assembly.Load(String,Evidence)
Assembly.Load(AssemblyName)
Assembly.Load(AssemblyName,Evidence)
Assembly.LoadWithPartialName(String)
Assembly.LoadWithPartialName(String,Evidence)
Assembly.Load(Byte[])
Assembly.ReflectionOnlyLoad(Byte[])
Assembly.Load(Byte[],Byte[])
Assembly.Load(Byte[],Byte[],SecurityContextSource)
Assembly.Load(Byte[],Byte[],Evidence)
Assembly.LoadFile(String)
Assembly.LoadFile(String,Evidence)
*Assembly.GetExecutingAssembly()*
*Assembly.GetCallingAssembly()*
*Assembly.GetEntryAssembly()*

AssemblyName
----------------
*AssemblyName.GetAssemblyName(String)*
*AssemblyName.ReferenceMatchesDefinition(AssemblyName,AssemblyName)*

CustomAttributeExtensions
----------------
CustomAttributeExtensions.GetCustomAttribute(Assembly,Type)
CustomAttributeExtensions.GetCustomAttribute(Module,Type)
CustomAttributeExtensions.GetCustomAttribute(MemberInfo,Type)
CustomAttributeExtensions.GetCustomAttribute(ParameterInfo,Type)
CustomAttributeExtensions.GetCustomAttribute(Assembly)
CustomAttributeExtensions.GetCustomAttribute(Module)
CustomAttributeExtensions.GetCustomAttribute(MemberInfo)
CustomAttributeExtensions.GetCustomAttribute(ParameterInfo)
CustomAttributeExtensions.GetCustomAttribute(MemberInfo,Type,Boolean)
CustomAttributeExtensions.GetCustomAttribute(ParameterInfo,Type,Boolean)
CustomAttributeExtensions.GetCustomAttribute(MemberInfo,Boolean)
CustomAttributeExtensions.GetCustomAttribute(ParameterInfo,Boolean)
CustomAttributeExtensions.GetCustomAttributes(Assembly)
CustomAttributeExtensions.GetCustomAttributes(Module)
CustomAttributeExtensions.GetCustomAttributes(MemberInfo)
CustomAttributeExtensions.GetCustomAttributes(ParameterInfo)
CustomAttributeExtensions.GetCustomAttributes(MemberInfo,Boolean)
CustomAttributeExtensions.GetCustomAttributes(ParameterInfo,Boolean)
CustomAttributeExtensions.GetCustomAttributes(Assembly,Type)
CustomAttributeExtensions.GetCustomAttributes(Module,Type)
CustomAttributeExtensions.GetCustomAttributes(MemberInfo,Type)
CustomAttributeExtensions.GetCustomAttributes(ParameterInfo,Type)
CustomAttributeExtensions.GetCustomAttributes(Assembly)
CustomAttributeExtensions.GetCustomAttributes(Module)
CustomAttributeExtensions.GetCustomAttributes(MemberInfo)
CustomAttributeExtensions.GetCustomAttributes(ParameterInfo)
CustomAttributeExtensions.GetCustomAttributes(MemberInfo,Type,Boolean)
CustomAttributeExtensions.GetCustomAttributes(ParameterInfo,Type,Boolean)
CustomAttributeExtensions.GetCustomAttributes(MemberInfo,Boolean)
CustomAttributeExtensions.GetCustomAttributes(ParameterInfo,Boolean)
CustomAttributeExtensions.IsDefined(Assembly,Type)
CustomAttributeExtensions.IsDefined(Module,Type)
CustomAttributeExtensions.IsDefined(MemberInfo,Type)
CustomAttributeExtensions.IsDefined(ParameterInfo,Type)
CustomAttributeExtensions.IsDefined(MemberInfo,Type,Boolean)
CustomAttributeExtensions.IsDefined(ParameterInfo,Type,Boolean)

CustomAttributeData
----------------
CustomAttributeData.GetCustomAttributes(MemberInfo)
CustomAttributeData.GetCustomAttributes(Module)
CustomAttributeData.GetCustomAttributes(Assembly)
CustomAttributeData.GetCustomAttributes(ParameterInfo)

FieldInfo
----------------
FieldInfo.GetFieldFromHandle(RuntimeFieldHandle)
FieldInfo.GetFieldFromHandle(RuntimeFieldHandle,RuntimeTypeHandle)

IntrospectionExtensions
----------------
*IntrospectionExtensions.GetTypeInfo(Type)*

RuntimeReflectionExtensions
----------------
*RuntimeReflectionExtensions.GetRuntimeProperties(Type)*
*RuntimeReflectionExtensions.GetRuntimeEvents(Type)*
*RuntimeReflectionExtensions.GetRuntimeMethods(Type)*
*RuntimeReflectionExtensions.GetRuntimeFields(Type)*
*RuntimeReflectionExtensions.GetRuntimeProperty(Type,String)*
*RuntimeReflectionExtensions.GetRuntimeEvent(Type,String)*
*RuntimeReflectionExtensions.GetRuntimeMethod(Type,String,Type[])*
*RuntimeReflectionExtensions.GetRuntimeField(Type,String)*
*RuntimeReflectionExtensions.GetRuntimeBaseDefinition(MethodInfo)*
*RuntimeReflectionExtensions.GetRuntimeInterfaceMap(TypeInfo,Type)*
*RuntimeReflectionExtensions.GetMethodInfo(Delegate)*

MdConstant
----------------
*MdConstant.GetValue(MetadataImport,Int32,RuntimeTypeHandle,Boolean)*

MetadataToken
----------------
*MetadataToken.IsTokenOfType(Int32,MetadataTokenType[])*
*MetadataToken.IsNullToken(Int32)*

MemberInfoSerializationHolder
----------------
MemberInfoSerializationHolder.GetSerializationInfo(SerializationInfo,String,RuntimeType,String,MemberTypes)
MemberInfoSerializationHolder.GetSerializationInfo(SerializationInfo,String,RuntimeType,String,String,MemberTypes,Type[])

MethodBase
----------------
MethodBase.GetMethodFromHandle(RuntimeMethodHandle)
MethodBase.GetMethodFromHandle(RuntimeMethodHandle,RuntimeTypeHandle)
*MethodBase.GetCurrentMethod()*

AssemblyBuilder
----------------
AssemblyBuilder.DefineDynamicAssembly(AssemblyName,AssemblyBuilderAccess)
AssemblyBuilder.DefineDynamicAssembly(AssemblyName,AssemblyBuilderAccess,IEnumerable`1)

MethodRental
----------------
*MethodRental.SwapMethodBody(Type,Int32,IntPtr,Int32,Int32)*

OpCodes
----------------
*OpCodes.TakesSingleByteArgument(OpCode)*

SignatureHelper
----------------
SignatureHelper.GetMethodSigHelper(Module,Type,Type[])
SignatureHelper.GetMethodSigHelper(Module,CallingConventions,Type)
SignatureHelper.GetMethodSigHelper(Module,CallingConvention,Type)
SignatureHelper.GetLocalVarSigHelper()
SignatureHelper.GetMethodSigHelper(CallingConventions,Type)
SignatureHelper.GetMethodSigHelper(CallingConvention,Type)
SignatureHelper.GetLocalVarSigHelper(Module)
*SignatureHelper.GetFieldSigHelper(Module)*
SignatureHelper.GetPropertySigHelper(Module,Type,Type[])
SignatureHelper.GetPropertySigHelper(Module,Type,Type[],Type[],Type[],Type[][],Type[][])
SignatureHelper.GetPropertySigHelper(Module,CallingConventions,Type,Type[],Type[],Type[],Type[][],Type[][])

TypeBuilder
----------------
*TypeBuilder.GetMethod(Type,MethodInfo)*
*TypeBuilder.GetConstructor(Type,ConstructorInfo)*
*TypeBuilder.GetField(Type,FieldInfo)*

StoreApplicationReference
----------------
*StoreApplicationReference.Destroy(IntPtr)*

ProfileOptimization
----------------
*ProfileOptimization.SetProfileRoot(String)*
*ProfileOptimization.StartProfile(String)*

VersioningHelper
----------------
VersioningHelper.MakeVersionSafeName(String,ResourceScope,ResourceScope,Type)
VersioningHelper.MakeVersionSafeName(String,ResourceScope,ResourceScope)

CompatibilitySwitch
----------------
*CompatibilitySwitch.IsEnabled(String)*
*CompatibilitySwitch.GetValue(String)*

FormatterServices
----------------
FormatterServices.GetSerializableMembers(Type)
FormatterServices.GetSerializableMembers(Type,StreamingContext)
*FormatterServices.CheckTypeSecurity(Type,TypeFilterLevel)*
*FormatterServices.GetUninitializedObject(Type)*
*FormatterServices.GetSafeUninitializedObject(Type)*
*FormatterServices.PopulateObjectMembers(Object,MemberInfo[],Object[])*
*FormatterServices.GetObjectData(Object,MemberInfo[])*
*FormatterServices.GetSurrogateForCyclicalReference(ISerializationSurrogate)*
*FormatterServices.GetTypeFromAssembly(Assembly,String)*

InternalRM
----------------
*InternalRM.InfoSoap(Object[])*
*InternalRM.SoapCheckEnabled()*

InternalST
----------------
*InternalST.InfoSoap(Object[])*
*InternalST.SoapCheckEnabled()*
*InternalST.Soap(Object[])*
*InternalST.SoapAssert(Boolean,String)*
*InternalST.SerializationSetValue(FieldInfo,Object,Object)*
*InternalST.LoadAssemblyFromString(String)*

BinaryUtil
----------------
BinaryUtil.NVTraceI(String,String)
BinaryUtil.NVTraceI(String,Object)

ExceptionDispatchInfo
----------------
*ExceptionDispatchInfo.Capture(Exception)*

RemotingConfiguration
----------------
RemotingConfiguration.RegisterWellKnownServiceType(Type,String,WellKnownObjectMode)
RemotingConfiguration.RegisterWellKnownServiceType(WellKnownServiceTypeEntry)
RemotingConfiguration.Configure(String)
RemotingConfiguration.Configure(String,Boolean)
*RemotingConfiguration.CustomErrorsEnabled(Boolean)*
RemotingConfiguration.RegisterActivatedServiceType(Type)
RemotingConfiguration.RegisterActivatedServiceType(ActivatedServiceTypeEntry)
RemotingConfiguration.RegisterActivatedClientType(Type,String)
RemotingConfiguration.RegisterActivatedClientType(ActivatedClientTypeEntry)
RemotingConfiguration.RegisterWellKnownClientType(Type,String)
RemotingConfiguration.RegisterWellKnownClientType(WellKnownClientTypeEntry)
*RemotingConfiguration.GetRegisteredActivatedServiceTypes()*
*RemotingConfiguration.GetRegisteredWellKnownServiceTypes()*
*RemotingConfiguration.GetRegisteredActivatedClientTypes()*
*RemotingConfiguration.GetRegisteredWellKnownClientTypes()*
RemotingConfiguration.IsRemotelyActivatedClientType(Type)
RemotingConfiguration.IsRemotelyActivatedClientType(String,String)
RemotingConfiguration.IsWellKnownClientType(Type)
RemotingConfiguration.IsWellKnownClientType(String,String)
*RemotingConfiguration.IsActivationAllowed(Type)*

RemotingServices
----------------
*RemotingServices.IsTransparentProxy(Object)*
*RemotingServices.IsObjectOutOfContext(Object)*
*RemotingServices.IsObjectOutOfAppDomain(Object)*
*RemotingServices.GetRealProxy(Object)*
*RemotingServices.GetSessionIdForMethodMessage(IMethodMessage)*
*RemotingServices.GetLifetimeService(MarshalByRefObject)*
*RemotingServices.GetObjectUri(MarshalByRefObject)*
*RemotingServices.SetObjectUriForMarshal(MarshalByRefObject,String)*
RemotingServices.Marshal(MarshalByRefObject)
RemotingServices.Marshal(MarshalByRefObject,String)
RemotingServices.Marshal(MarshalByRefObject,String,Type)
*RemotingServices.GetObjectData(Object,SerializationInfo,StreamingContext)*
RemotingServices.Unmarshal(ObjRef)
RemotingServices.Unmarshal(ObjRef,Boolean)
RemotingServices.Connect(Type,String)
RemotingServices.Connect(Type,String,Object)
*RemotingServices.Disconnect(MarshalByRefObject)*
*RemotingServices.GetEnvoyChainForProxy(MarshalByRefObject)*
*RemotingServices.GetObjRefForProxy(MarshalByRefObject)*
*RemotingServices.GetMethodBaseFromMethodMessage(IMethodMessage)*
*RemotingServices.IsMethodOverloaded(IMethodMessage)*
*RemotingServices.IsOneWay(MethodBase)*
*RemotingServices.GetServerTypeForUri(String)*
*RemotingServices.ExecuteMessage(MarshalByRefObject,IMethodCallMessage)*
*RemotingServices.LogRemotingStage(Int32)*

InternalRemotingServices
----------------
*InternalRemotingServices.DebugOutChnl(String)*
*InternalRemotingServices.RemotingTrace(Object[])*
*InternalRemotingServices.RemotingAssert(Boolean,String)*
*InternalRemotingServices.SetServerIdentity(MethodCall,Object)*
*InternalRemotingServices.GetCachedSoapAttribute(Object)*

SoapServices
----------------
*SoapServices.RegisterInteropXmlElement(String,String,Type)*
*SoapServices.RegisterInteropXmlType(String,String,Type)*
SoapServices.PreLoad(Type)
SoapServices.PreLoad(Assembly)
*SoapServices.GetInteropTypeFromXmlElement(String,String)*
*SoapServices.GetInteropTypeFromXmlType(String,String)*
*SoapServices.GetInteropFieldTypeAndNameFromXmlElement(Type,String,String,Type&,String&)*
*SoapServices.GetInteropFieldTypeAndNameFromXmlAttribute(Type,String,String,Type&,String&)*
*SoapServices.GetXmlElementForInteropType(Type,String&,String&)*
*SoapServices.GetXmlTypeForInteropType(Type,String&,String&)*
*SoapServices.GetXmlNamespaceForMethodCall(MethodBase)*
*SoapServices.GetXmlNamespaceForMethodResponse(MethodBase)*
SoapServices.RegisterSoapActionForMethodBase(MethodBase)
SoapServices.RegisterSoapActionForMethodBase(MethodBase,String)
*SoapServices.GetSoapActionFromMethodBase(MethodBase)*
*SoapServices.IsSoapActionValidForMethodBase(String,MethodBase)*
*SoapServices.GetTypeAndMethodNameFromSoapAction(String,String&,String&)*
*SoapServices.IsClrTypeNamespace(String)*
*SoapServices.CodeXmlNamespaceForClrTypeNamespace(String,String)*
*SoapServices.DecodeXmlNamespaceForClrTypeNamespace(String,String&,String&)*

SoapDateTime
----------------
*SoapDateTime.Parse(String)*

SoapDuration
----------------
*SoapDuration.Parse(String)*

SoapTime
----------------
*SoapTime.Parse(String)*

SoapDate
----------------
*SoapDate.Parse(String)*

SoapYearMonth
----------------
*SoapYearMonth.Parse(String)*

SoapYear
----------------
*SoapYear.Parse(String)*

SoapMonthDay
----------------
*SoapMonthDay.Parse(String)*

SoapDay
----------------
*SoapDay.Parse(String)*

SoapMonth
----------------
*SoapMonth.Parse(String)*

SoapHexBinary
----------------
*SoapHexBinary.Parse(String)*

SoapBase64Binary
----------------
*SoapBase64Binary.Parse(String)*

SoapInteger
----------------
*SoapInteger.Parse(String)*

SoapPositiveInteger
----------------
*SoapPositiveInteger.Parse(String)*

SoapNonPositiveInteger
----------------
*SoapNonPositiveInteger.Parse(String)*

SoapNonNegativeInteger
----------------
*SoapNonNegativeInteger.Parse(String)*

SoapNegativeInteger
----------------
*SoapNegativeInteger.Parse(String)*

SoapAnyUri
----------------
*SoapAnyUri.Parse(String)*

SoapQName
----------------
*SoapQName.Parse(String)*

SoapNotation
----------------
*SoapNotation.Parse(String)*

SoapNormalizedString
----------------
*SoapNormalizedString.Parse(String)*

SoapToken
----------------
*SoapToken.Parse(String)*

SoapLanguage
----------------
*SoapLanguage.Parse(String)*

SoapName
----------------
*SoapName.Parse(String)*

SoapIdrefs
----------------
*SoapIdrefs.Parse(String)*

SoapEntities
----------------
*SoapEntities.Parse(String)*

SoapNmtoken
----------------
*SoapNmtoken.Parse(String)*

SoapNmtokens
----------------
*SoapNmtokens.Parse(String)*

SoapNcName
----------------
*SoapNcName.Parse(String)*

SoapId
----------------
*SoapId.Parse(String)*

SoapIdref
----------------
*SoapIdref.Parse(String)*

SoapEntity
----------------
*SoapEntity.Parse(String)*

RealProxy
----------------
*RealProxy.SetStubData(RealProxy,Object)*
*RealProxy.GetStubData(RealProxy)*

AgileAsyncWorkerItem
----------------
*AgileAsyncWorkerItem.ThreadPoolCallBack(Object)*

EnterpriseServicesHelper
----------------
*EnterpriseServicesHelper.WrapIUnknownWithComObject(IntPtr)*
*EnterpriseServicesHelper.CreateConstructionReturnMessage(IConstructionCallMessage,MarshalByRefObject)*
*EnterpriseServicesHelper.SwitchWrappers(RealProxy,RealProxy)*

TrackingServices
----------------
*TrackingServices.RegisterTrackingHandler(ITrackingHandler)*
*TrackingServices.UnregisterTrackingHandler(ITrackingHandler)*

Context
----------------
*Context.AllocateDataSlot()*
*Context.AllocateNamedDataSlot(String)*
*Context.GetNamedDataSlot(String)*
*Context.FreeNamedDataSlot(String)*
*Context.SetData(LocalDataStoreSlot,Object)*
*Context.GetData(LocalDataStoreSlot)*
*Context.RegisterDynamicProperty(IDynamicProperty,ContextBoundObject,Context)*
*Context.UnregisterDynamicProperty(String,ContextBoundObject,Context)*

ChannelServices
----------------
ChannelServices.RegisterChannel(IChannel,Boolean)
ChannelServices.RegisterChannel(IChannel)
*ChannelServices.UnregisterChannel(IChannel)*
*ChannelServices.GetChannel(String)*
*ChannelServices.GetUrlsForObject(MarshalByRefObject)*
*ChannelServices.GetChannelSinkProperties(Object)*
*ChannelServices.DispatchMessage(IServerChannelSinkStack,IMessage,IMessage&)*
*ChannelServices.SyncDispatchMessage(IMessage)*
*ChannelServices.AsyncDispatchMessage(IMessage,IMessageSink)*
*ChannelServices.CreateServerChannelSinkChain(IServerChannelSinkProvider,IChannelReceiver)*

Message
----------------
*Message.DebugOut(String)*

CallContext
----------------
*CallContext.FreeNamedDataSlot(String)*
*CallContext.LogicalGetData(String)*
*CallContext.GetData(String)*
*CallContext.SetData(String,Object)*
*CallContext.LogicalSetData(String,Object)*
*CallContext.GetHeaders()*
*CallContext.SetHeaders(Header[])*

RemotingXmlConfigFileParser
----------------
*RemotingXmlConfigFileParser.ParseDefaultConfiguration()*
*RemotingXmlConfigFileParser.ParseConfigFile(String)*

ContractHelper
----------------
*ContractHelper.RaiseContractFailedEvent(ContractFailureKind,String,String,Exception)*
*ContractHelper.TriggerFailure(ContractFailureKind,String,String,String,Exception)*

RuntimeHelpers
----------------
*RuntimeHelpers.InitializeArray(Array,RuntimeFieldHandle)*
*RuntimeHelpers.GetObjectValue(Object)*
*RuntimeHelpers.RunClassConstructor(RuntimeTypeHandle)*
*RuntimeHelpers.RunModuleConstructor(ModuleHandle)*
RuntimeHelpers.PrepareMethod(RuntimeMethodHandle)
RuntimeHelpers.PrepareMethod(RuntimeMethodHandle,RuntimeTypeHandle[])
*RuntimeHelpers.PrepareDelegate(Delegate)*
*RuntimeHelpers.PrepareContractedDelegate(Delegate)*
*RuntimeHelpers.GetHashCode(Object)*
*RuntimeHelpers.Equals(Object,Object)*
*RuntimeHelpers.EnsureSufficientExecutionStack()*
*RuntimeHelpers.ProbeForSufficientStack()*
*RuntimeHelpers.PrepareConstrainedRegions()*
*RuntimeHelpers.PrepareConstrainedRegionsNoOP()*
*RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(TryCode,CleanupCode,Object)*

AsyncVoidMethodBuilder
----------------
*AsyncVoidMethodBuilder.Create()*

AsyncTaskMethodBuilder`1
----------------
*AsyncTaskMethodBuilder`1.Create()*

FormattableStringFactory
----------------
*FormattableStringFactory.Create(String,Object[])*

HandleRef
----------------
*HandleRef.ToIntPtr(HandleRef)*

Marshal
----------------
Marshal.PtrToStringAnsi(IntPtr)
Marshal.PtrToStringAnsi(IntPtr,Int32)
Marshal.PtrToStringUni(IntPtr,Int32)
Marshal.PtrToStringAuto(IntPtr,Int32)
Marshal.PtrToStringUni(IntPtr)
Marshal.PtrToStringAuto(IntPtr)
Marshal.SizeOf(Object)
Marshal.SizeOf(T)
Marshal.SizeOf(Type)
Marshal.SizeOf()
Marshal.OffsetOf(Type,String)
Marshal.OffsetOf(String)
Marshal.UnsafeAddrOfPinnedArrayElement(Array,Int32)
Marshal.UnsafeAddrOfPinnedArrayElement(T[],Int32)
Marshal.Copy(Int32[],Int32,IntPtr,Int32)
Marshal.Copy(Char[],Int32,IntPtr,Int32)
Marshal.Copy(Int16[],Int32,IntPtr,Int32)
Marshal.Copy(Int64[],Int32,IntPtr,Int32)
Marshal.Copy(Single[],Int32,IntPtr,Int32)
Marshal.Copy(Double[],Int32,IntPtr,Int32)
Marshal.Copy(Byte[],Int32,IntPtr,Int32)
Marshal.Copy(IntPtr[],Int32,IntPtr,Int32)
Marshal.Copy(IntPtr,Int32[],Int32,Int32)
Marshal.Copy(IntPtr,Char[],Int32,Int32)
Marshal.Copy(IntPtr,Int16[],Int32,Int32)
Marshal.Copy(IntPtr,Int64[],Int32,Int32)
Marshal.Copy(IntPtr,Single[],Int32,Int32)
Marshal.Copy(IntPtr,Double[],Int32,Int32)
Marshal.Copy(IntPtr,Byte[],Int32,Int32)
Marshal.Copy(IntPtr,IntPtr[],Int32,Int32)
Marshal.ReadByte(Object,Int32)
Marshal.ReadByte(IntPtr,Int32)
Marshal.ReadByte(IntPtr)
Marshal.ReadInt16(Object,Int32)
Marshal.ReadInt16(IntPtr,Int32)
Marshal.ReadInt16(IntPtr)
Marshal.ReadInt32(Object,Int32)
Marshal.ReadInt32(IntPtr,Int32)
Marshal.ReadInt32(IntPtr)
Marshal.ReadIntPtr(Object,Int32)
Marshal.ReadIntPtr(IntPtr,Int32)
Marshal.ReadIntPtr(IntPtr)
Marshal.ReadInt64(Object,Int32)
Marshal.ReadInt64(IntPtr,Int32)
Marshal.ReadInt64(IntPtr)
Marshal.WriteByte(IntPtr,Int32,Byte)
Marshal.WriteByte(Object,Int32,Byte)
Marshal.WriteByte(IntPtr,Byte)
Marshal.WriteInt16(IntPtr,Int32,Int16)
Marshal.WriteInt16(Object,Int32,Int16)
Marshal.WriteInt16(IntPtr,Int16)
Marshal.WriteInt16(IntPtr,Int32,Char)
Marshal.WriteInt16(Object,Int32,Char)
Marshal.WriteInt16(IntPtr,Char)
Marshal.WriteInt32(IntPtr,Int32,Int32)
Marshal.WriteInt32(Object,Int32,Int32)
Marshal.WriteInt32(IntPtr,Int32)
Marshal.WriteIntPtr(IntPtr,Int32,IntPtr)
Marshal.WriteIntPtr(Object,Int32,IntPtr)
Marshal.WriteIntPtr(IntPtr,IntPtr)
Marshal.WriteInt64(IntPtr,Int32,Int64)
Marshal.WriteInt64(Object,Int32,Int64)
Marshal.WriteInt64(IntPtr,Int64)
*Marshal.GetLastWin32Error()*
*Marshal.GetHRForLastWin32Error()*
*Marshal.Prelink(MethodInfo)*
*Marshal.PrelinkAll(Type)*
*Marshal.NumParamBytes(MethodInfo)*
*Marshal.GetExceptionPointers()*
*Marshal.GetExceptionCode()*
Marshal.StructureToPtr(Object,IntPtr,Boolean)
Marshal.StructureToPtr(T,IntPtr,Boolean)
Marshal.PtrToStructure(IntPtr,Object)
Marshal.PtrToStructure(IntPtr,T)
Marshal.PtrToStructure(IntPtr,Type)
Marshal.PtrToStructure(IntPtr)
Marshal.DestroyStructure(IntPtr,Type)
Marshal.DestroyStructure(IntPtr)
*Marshal.GetHINSTANCE(Module)*
Marshal.ThrowExceptionForHR(Int32)
Marshal.ThrowExceptionForHR(Int32,IntPtr)
Marshal.GetExceptionForHR(Int32)
Marshal.GetExceptionForHR(Int32,IntPtr)
*Marshal.GetHRForException(Exception)*
*Marshal.GetUnmanagedThunkForManagedMethodPtr(IntPtr,IntPtr,Int32)*
*Marshal.GetManagedThunkForUnmanagedMethodPtr(IntPtr,IntPtr,Int32)*
*Marshal.GetThreadFromFiberCookie(Int32)*
Marshal.AllocHGlobal(IntPtr)
Marshal.AllocHGlobal(Int32)
*Marshal.FreeHGlobal(IntPtr)*
*Marshal.ReAllocHGlobal(IntPtr,IntPtr)*
*Marshal.StringToHGlobalAnsi(String)*
*Marshal.StringToHGlobalUni(String)*
*Marshal.StringToHGlobalAuto(String)*
Marshal.GetTypeLibName(UCOMITypeLib)
Marshal.GetTypeLibName(ITypeLib)
Marshal.GetTypeLibGuid(UCOMITypeLib)
Marshal.GetTypeLibGuid(ITypeLib)
Marshal.GetTypeLibLcid(UCOMITypeLib)
Marshal.GetTypeLibLcid(ITypeLib)
*Marshal.GetTypeLibGuidForAssembly(Assembly)*
*Marshal.GetTypeLibVersionForAssembly(Assembly,Int32&,Int32&)*
Marshal.GetTypeInfoName(UCOMITypeInfo)
Marshal.GetTypeInfoName(ITypeInfo)
*Marshal.GetTypeForITypeInfo(IntPtr)*
*Marshal.GetTypeFromCLSID(Guid)*
*Marshal.GetITypeInfoForType(Type)*
*Marshal.GetIUnknownForObject(Object)*
*Marshal.GetIUnknownForObjectInContext(Object)*
*Marshal.GetIDispatchForObject(Object)*
*Marshal.GetIDispatchForObjectInContext(Object)*
Marshal.GetComInterfaceForObject(Object,Type)
Marshal.GetComInterfaceForObject(T)
Marshal.GetComInterfaceForObject(Object,Type,CustomQueryInterfaceMode)
*Marshal.GetComInterfaceForObjectInContext(Object,Type)*
*Marshal.GetObjectForIUnknown(IntPtr)*
*Marshal.GetUniqueObjectForIUnknown(IntPtr)*
*Marshal.GetTypedObjectForIUnknown(IntPtr,Type)*
Marshal.CreateAggregatedObject(IntPtr,Object)
Marshal.CreateAggregatedObject(IntPtr,T)
*Marshal.CleanupUnusedObjectsInCurrentContext()*
*Marshal.AreComObjectsAvailableForCleanup()*
*Marshal.IsComObject(Object)*
*Marshal.AllocCoTaskMem(Int32)*
*Marshal.StringToCoTaskMemUni(String)*
*Marshal.StringToCoTaskMemAuto(String)*
*Marshal.StringToCoTaskMemAnsi(String)*
*Marshal.FreeCoTaskMem(IntPtr)*
*Marshal.ReAllocCoTaskMem(IntPtr,Int32)*
*Marshal.ReleaseComObject(Object)*
*Marshal.FinalReleaseComObject(Object)*
*Marshal.GetComObjectData(Object,Object)*
*Marshal.SetComObjectData(Object,Object,Object)*
Marshal.CreateWrapperOfType(Object,Type)
Marshal.CreateWrapperOfType(T)
*Marshal.ReleaseThreadCache()*
*Marshal.IsTypeVisibleFromCom(Type)*
*Marshal.QueryInterface(IntPtr,Guid&,IntPtr&)*
*Marshal.AddRef(IntPtr)*
*Marshal.Release(IntPtr)*
*Marshal.FreeBSTR(IntPtr)*
*Marshal.StringToBSTR(String)*
*Marshal.PtrToStringBSTR(IntPtr)*
Marshal.GetNativeVariantForObject(Object,IntPtr)
Marshal.GetNativeVariantForObject(T,IntPtr)
Marshal.GetObjectForNativeVariant(IntPtr)
Marshal.GetObjectForNativeVariant(IntPtr)
Marshal.GetObjectsForNativeVariants(IntPtr,Int32)
Marshal.GetObjectsForNativeVariants(IntPtr,Int32)
*Marshal.GetStartComSlot(Type)*
*Marshal.GetEndComSlot(Type)*
*Marshal.GetMethodInfoForComSlot(Type,Int32,ComMemberType&)*
*Marshal.GetComSlotForMethodInfo(MemberInfo)*
*Marshal.GenerateGuidForType(Type)*
*Marshal.GenerateProgIdForType(Type)*
*Marshal.BindToMoniker(String)*
*Marshal.GetActiveObject(String)*
*Marshal.ChangeWrapperHandleStrength(Object,Boolean)*
Marshal.GetDelegateForFunctionPointer(IntPtr,Type)
Marshal.GetDelegateForFunctionPointer(IntPtr)
Marshal.GetFunctionPointerForDelegate(Delegate)
Marshal.GetFunctionPointerForDelegate(TDelegate)
*Marshal.SecureStringToBSTR(SecureString)*
*Marshal.SecureStringToCoTaskMemAnsi(SecureString)*
*Marshal.SecureStringToCoTaskMemUnicode(SecureString)*
*Marshal.ZeroFreeBSTR(IntPtr)*
*Marshal.ZeroFreeCoTaskMemAnsi(IntPtr)*
*Marshal.ZeroFreeCoTaskMemUnicode(IntPtr)*
*Marshal.SecureStringToGlobalAllocAnsi(SecureString)*
*Marshal.SecureStringToGlobalAllocUnicode(SecureString)*
*Marshal.ZeroFreeGlobalAllocAnsi(IntPtr)*
*Marshal.ZeroFreeGlobalAllocUnicode(IntPtr)*

RuntimeEnvironment
----------------
*RuntimeEnvironment.FromGlobalAccessCache(Assembly)*
*RuntimeEnvironment.GetSystemVersion()*
*RuntimeEnvironment.GetRuntimeDirectory()*
*RuntimeEnvironment.GetRuntimeInterfaceAsIntPtr(Guid,Guid)*
*RuntimeEnvironment.GetRuntimeInterfaceAsObject(Guid,Guid)*

ExtensibleClassFactory
----------------
*ExtensibleClassFactory.RegisterObjectCreationCallback(ObjectCreationDelegate)*

ComEventsHelper
----------------
*ComEventsHelper.Combine(Object,Guid,Int32,Delegate)*
*ComEventsHelper.Remove(Object,Guid,Int32,Delegate)*

NameSpaceExtractor
----------------
*NameSpaceExtractor.ExtractNameSpace(String)*

EventRegistrationTokenTable`1
----------------
*EventRegistrationTokenTable`1.GetOrCreateEventRegistrationTokenTable(EventRegistrationTokenTable`1&)*

WindowsRuntimeMarshal
----------------
*WindowsRuntimeMarshal.AddEventHandler(Func`2,Action`1,T)*
*WindowsRuntimeMarshal.RemoveEventHandler(Action`1,T)*
*WindowsRuntimeMarshal.RemoveAllEventHandlers(Action`1)*
*WindowsRuntimeMarshal.GetActivationFactory(Type)*
*WindowsRuntimeMarshal.StringToHString(String)*
*WindowsRuntimeMarshal.PtrToStringHString(IntPtr)*
*WindowsRuntimeMarshal.FreeHString(IntPtr)*

WindowsRuntimeMetadata
----------------
WindowsRuntimeMetadata.ResolveNamespace(String,IEnumerable`1)
WindowsRuntimeMetadata.ResolveNamespace(String,String,IEnumerable`1)
*WindowsRuntimeMetadata.add_ReflectionOnlyNamespaceResolve(EventHandler`1)*
*WindowsRuntimeMetadata.remove_ReflectionOnlyNamespaceResolve(EventHandler`1)*
*WindowsRuntimeMetadata.add_DesignerNamespaceResolve(EventHandler`1)*
*WindowsRuntimeMetadata.remove_DesignerNamespaceResolve(EventHandler`1)*

StringBuilderCache
----------------
*StringBuilderCache.Acquire(Int32)*
*StringBuilderCache.Release(StringBuilder)*
*StringBuilderCache.GetStringAndRelease(StringBuilder)*

Encoding
----------------
Encoding.Convert(Encoding,Encoding,Byte[])
Encoding.Convert(Encoding,Encoding,Byte[],Int32,Int32)
*Encoding.RegisterProvider(EncodingProvider)*
Encoding.GetEncoding(Int32)
Encoding.GetEncoding(Int32,EncoderFallback,DecoderFallback)
Encoding.GetEncoding(String)
Encoding.GetEncoding(String,EncoderFallback,DecoderFallback)
*Encoding.GetEncodings()*

AdjustmentRule
----------------
*AdjustmentRule.CreateAdjustmentRule(DateTime,DateTime,TimeSpan,TransitionTime,TransitionTime)*

TransitionTime
----------------
*TransitionTime.CreateFixedDateRule(DateTime,Int32,Int32)*
*TransitionTime.CreateFloatingDateRule(DateTime,Int32,Int32,DayOfWeek)*

StringSerializer
----------------
*StringSerializer.GetSerializedString(TimeZoneInfo)*
*StringSerializer.GetDeserializedTimeZoneInfo(String)*

ActivityInfo
----------------
*ActivityInfo.Path(ActivityInfo)*
*ActivityInfo.LiveActivities(ActivityInfo)*

CreateActContextParametersSource
----------------
*CreateActContextParametersSource.Destroy(IntPtr)*

CreateActContextParametersSourceDefinitionAppid
----------------
*CreateActContextParametersSourceDefinitionAppid.Destroy(IntPtr)*




####################
System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
####################

VBCodeGenerator
----------------
*VBCodeGenerator.IsKeyword(String)*

SystemEvents
----------------
*SystemEvents.add_DisplaySettingsChanging(EventHandler)*
*SystemEvents.remove_DisplaySettingsChanging(EventHandler)*
*SystemEvents.add_DisplaySettingsChanged(EventHandler)*
*SystemEvents.remove_DisplaySettingsChanged(EventHandler)*
*SystemEvents.add_EventsThreadShutdown(EventHandler)*
*SystemEvents.remove_EventsThreadShutdown(EventHandler)*
*SystemEvents.add_InstalledFontsChanged(EventHandler)*
*SystemEvents.remove_InstalledFontsChanged(EventHandler)*
*SystemEvents.add_LowMemory(EventHandler)*
*SystemEvents.remove_LowMemory(EventHandler)*
*SystemEvents.add_PaletteChanged(EventHandler)*
*SystemEvents.remove_PaletteChanged(EventHandler)*
*SystemEvents.add_PowerModeChanged(PowerModeChangedEventHandler)*
*SystemEvents.remove_PowerModeChanged(PowerModeChangedEventHandler)*
*SystemEvents.add_SessionEnded(SessionEndedEventHandler)*
*SystemEvents.remove_SessionEnded(SessionEndedEventHandler)*
*SystemEvents.add_SessionEnding(SessionEndingEventHandler)*
*SystemEvents.remove_SessionEnding(SessionEndingEventHandler)*
*SystemEvents.add_SessionSwitch(SessionSwitchEventHandler)*
*SystemEvents.remove_SessionSwitch(SessionSwitchEventHandler)*
*SystemEvents.add_TimeChanged(EventHandler)*
*SystemEvents.remove_TimeChanged(EventHandler)*
*SystemEvents.add_TimerElapsed(TimerElapsedEventHandler)*
*SystemEvents.remove_TimerElapsed(TimerElapsedEventHandler)*
*SystemEvents.add_UserPreferenceChanged(UserPreferenceChangedEventHandler)*
*SystemEvents.remove_UserPreferenceChanged(UserPreferenceChangedEventHandler)*
*SystemEvents.add_UserPreferenceChanging(UserPreferenceChangingEventHandler)*
*SystemEvents.remove_UserPreferenceChanging(UserPreferenceChangingEventHandler)*
*SystemEvents.CreateTimer(Int32)*
*SystemEvents.InvokeOnEventsThread(Delegate)*
*SystemEvents.KillTimer(IntPtr)*

AppContextDefaultValues
----------------
*AppContextDefaultValues.PopulateDefaultValues()*

UriParser
----------------
*UriParser.Register(UriParser,String,Int32)*
*UriParser.IsKnownScheme(String)*

Uri
----------------
*Uri.CheckHostName(String)*
*Uri.HexEscape(Char)*
*Uri.HexUnescape(String,Int32&)*
*Uri.IsHexEncoding(String,Int32)*
*Uri.CheckSchemeName(String)*
*Uri.IsHexDigit(Char)*
*Uri.FromHex(Char)*
Uri.TryCreate(String,UriKind,Uri&)
Uri.TryCreate(Uri,String,Uri&)
Uri.TryCreate(Uri,Uri,Uri&)
*Uri.Compare(Uri,Uri,UriComponents,UriFormat,StringComparison)*
*Uri.IsWellFormedUriString(String,UriKind)*
*Uri.UnescapeDataString(String)*
*Uri.EscapeUriString(String)*
*Uri.EscapeDataString(String)*

ClientUtils
----------------
*ClientUtils.IsCriticalException(Exception)*
*ClientUtils.IsSecurityOrCriticalException(Exception)*
*ClientUtils.GetBitCount(UInt32)*
ClientUtils.IsEnumValid(Enum,Int32,Int32,Int32)
ClientUtils.IsEnumValid(Enum,Int32,Int32,Int32,Int32)
*ClientUtils.IsEnumValid_Masked(Enum,Int32,UInt32)*
*ClientUtils.IsEnumValid_NotSequential(Enum,Int32,Int32[])*

Gen2GcCallback
----------------
*Gen2GcCallback.Register(Func`2,Object)*

LocalAppContext
----------------
*LocalAppContext.IsSwitchEnabled(String)*

SR
----------------
SR.GetString(String,Object[])
SR.GetString(String)
SR.GetString(String,Boolean&)
*SR.GetObject(String)*

UriSectionReader
----------------
UriSectionReader.Read(String)
UriSectionReader.Read(String,UriSectionData)

ConfigurationException
----------------
*ConfigurationException.GetXmlNodeFilename(XmlNode)*
*ConfigurationException.GetXmlNodeLineNumber(XmlNode)*

ConfigurationSettings
----------------
*ConfigurationSettings.GetConfig(String)*

SettingsBase
----------------
*SettingsBase.Synchronized(SettingsBase)*

AuthenticationManager
----------------
*AuthenticationManager.Authenticate(String,WebRequest,ICredentials)*
*AuthenticationManager.PreAuthenticate(WebRequest,ICredentials)*
*AuthenticationManager.Register(IAuthenticationModule)*
AuthenticationManager.Unregister(IAuthenticationModule)
AuthenticationManager.Unregister(String)

Dns
----------------
*Dns.GetHostByName(String)*
Dns.GetHostByAddress(String)
Dns.GetHostByAddress(IPAddress)
*Dns.GetHostName()*
*Dns.Resolve(String)*
*Dns.BeginGetHostByName(String,AsyncCallback,Object)*
*Dns.EndGetHostByName(IAsyncResult)*
Dns.GetHostEntry(String)
Dns.GetHostEntry(IPAddress)
*Dns.GetHostAddresses(String)*
Dns.BeginGetHostEntry(String,AsyncCallback,Object)
Dns.BeginGetHostEntry(IPAddress,AsyncCallback,Object)
*Dns.EndGetHostEntry(IAsyncResult)*
*Dns.BeginGetHostAddresses(String,AsyncCallback,Object)*
*Dns.EndGetHostAddresses(IAsyncResult)*
*Dns.BeginResolve(String,AsyncCallback,Object)*
*Dns.EndResolve(IAsyncResult)*
*Dns.GetHostAddressesAsync(String)*
Dns.GetHostEntryAsync(IPAddress)
Dns.GetHostEntryAsync(String)

GlobalProxySelection
----------------
*GlobalProxySelection.GetEmptyWebProxy()*

HttpListenerRequestUriBuilder
----------------
*HttpListenerRequestUriBuilder.GetRequestUri(String,String,String,String,String)*

ValidationHelper
----------------
*ValidationHelper.MakeEmptyArrayNull(String[])*
*ValidationHelper.MakeStringNull(String)*
*ValidationHelper.ExceptionMessage(Exception)*
*ValidationHelper.HashString(Object)*
*ValidationHelper.IsInvalidHttpString(String)*
*ValidationHelper.IsBlankString(String)*
*ValidationHelper.ValidateTcpPort(Int32)*
*ValidationHelper.ValidateRange(Int32,Int32,Int32)*

KnownHttpVerb
----------------
*KnownHttpVerb.Parse(String)*

IPAddress
----------------
*IPAddress.TryParse(String,IPAddress&)*
*IPAddress.Parse(String)*
IPAddress.HostToNetworkOrder(Int64)
IPAddress.HostToNetworkOrder(Int32)
IPAddress.HostToNetworkOrder(Int16)
IPAddress.NetworkToHostOrder(Int64)
IPAddress.NetworkToHostOrder(Int32)
IPAddress.NetworkToHostOrder(Int16)
*IPAddress.IsLoopback(IPAddress)*

RegistryConfiguration
----------------
*RegistryConfiguration.GlobalConfigReadInt(String,Int32)*
*RegistryConfiguration.AppConfigReadInt(String,Int32)*
*RegistryConfiguration.GlobalConfigReadString(String,String)*
*RegistryConfiguration.AppConfigReadString(String,String)*

ServicePointManager
----------------
ServicePointManager.FindServicePoint(Uri)
ServicePointManager.FindServicePoint(String,IWebProxy)
ServicePointManager.FindServicePoint(Uri,IWebProxy)
*ServicePointManager.SetTcpKeepAlive(Boolean,Int32,Int32)*

UnsafeNclNativeMethods
----------------
*UnsafeNclNativeMethods.CoCreateInstance(Guid&,IntPtr,Int32,Guid&,Object&)*

WebHeaderCollection
----------------
WebHeaderCollection.IsRestricted(String)
WebHeaderCollection.IsRestricted(String,Boolean)

WebProxy
----------------
*WebProxy.GetDefaultProxy()*

WebRequest
----------------
*WebRequest.RegisterPortableWebRequestCreator(IWebRequestCreate)*
WebRequest.Create(String)
WebRequest.Create(Uri)
*WebRequest.CreateDefault(Uri)*
WebRequest.CreateHttp(String)
WebRequest.CreateHttp(Uri)
*WebRequest.RegisterPrefix(String,IWebRequestCreate)*
*WebRequest.GetSystemWebProxy()*

WebUtility
----------------
WebUtility.HtmlEncode(String)
WebUtility.HtmlEncode(String,TextWriter)
WebUtility.HtmlDecode(String)
WebUtility.HtmlDecode(String,TextWriter)
*WebUtility.UrlEncode(String)*
*WebUtility.UrlEncodeToBytes(Byte[],Int32,Int32)*
*WebUtility.UrlDecode(String)*
*WebUtility.UrlDecodeToBytes(Byte[],Int32,Int32)*

HttpDateParse
----------------
*HttpDateParse.ParseHttpDate(String,DateTime&)*

GlobalLog
----------------
*GlobalLog.AddToArray(String)*
*GlobalLog.Ignore(Object)*
*GlobalLog.Print(String)*
*GlobalLog.PrintHex(String,Object)*
GlobalLog.Enter(String)
GlobalLog.Enter(String,String)
GlobalLog.Assert(Boolean,String,Object[])
GlobalLog.Assert(String)
GlobalLog.Assert(String,String)
*GlobalLog.LeaveException(String,Exception)*
GlobalLog.Leave(String)
GlobalLog.Leave(String,String)
GlobalLog.Leave(String,Int32)
GlobalLog.Leave(String,Boolean)
*GlobalLog.DumpArray()*
GlobalLog.Dump(Byte[])
GlobalLog.Dump(Byte[],Int32)
GlobalLog.Dump(Byte[],Int32,Int32)
GlobalLog.Dump(IntPtr,Int32,Int32)

NetRes
----------------
NetRes.GetWebStatusString(String,WebExceptionStatus)
NetRes.GetWebStatusCodeString(HttpStatusCode,String)
NetRes.GetWebStatusString(WebExceptionStatus)
NetRes.GetWebStatusCodeString(FtpStatusCode,String)

NetworkingPerfCounters
----------------
*NetworkingPerfCounters.GetTimestamp()*

SafeFreeContextBuffer
----------------
*SafeFreeContextBuffer.QueryContextAttributes(SecurDll,SafeDeleteContext,ContextAttribute,Byte*,SafeHandle)*
*SafeFreeContextBuffer.SetContextAttributes(SecurDll,SafeDeleteContext,ContextAttribute,Byte[])*

SafeLocalFree
----------------
*SafeLocalFree.LocalAlloc(Int32)*

SafeOverlappedFree
----------------
SafeOverlappedFree.Alloc()
SafeOverlappedFree.Alloc(SafeCloseSocket)

SafeLoadLibrary
----------------
*SafeLoadLibrary.LoadLibraryEx(String)*

SafeFreeCredentials
----------------
*SafeFreeCredentials.AcquireDefaultCredential(SecurDll,String,CredentialUse,SafeFreeCredentials&)*
SafeFreeCredentials.AcquireCredentialsHandle(SecurDll,String,CredentialUse,SecureCredential&,SafeFreeCredentials&)
SafeFreeCredentials.AcquireCredentialsHandle(SecurDll,String,CredentialUse,AuthIdentity&,SafeFreeCredentials&)
SafeFreeCredentials.AcquireCredentialsHandle(String,CredentialUse,SafeSspiAuthDataHandle&,SafeFreeCredentials&)

SafeLocalFreeChannelBinding
----------------
*SafeLocalFreeChannelBinding.LocalAlloc(Int32)*

SafeFreeContextBufferChannelBinding
----------------
*SafeFreeContextBufferChannelBinding.QueryContextChannelBinding(SecurDll,SafeDeleteContext,ContextAttribute,Bindings*,SafeFreeContextBufferChannelBinding)*

SSPIWrapper
----------------
*SSPIWrapper.AcquireDefaultCredential(SSPIInterface,String,CredentialUse)*
SSPIWrapper.AcquireCredentialsHandle(SSPIInterface,String,CredentialUse,SecureCredential)
SSPIWrapper.QueryContextAttributes(SSPIInterface,SafeDeleteContext,ContextAttribute,Int32&)
*SSPIWrapper.SetContextAttributes(SSPIInterface,SafeDeleteContext,ContextAttribute,Object)*
SSPIWrapper.AcquireCredentialsHandle(SSPIInterface,String,CredentialUse,AuthIdentity&)
SSPIWrapper.AcquireCredentialsHandle(SSPIInterface,String,CredentialUse,SafeSspiAuthDataHandle&)
*SSPIWrapper.QuerySecurityContextToken(SSPIInterface,SafeDeleteContext,SafeCloseHandle&)*
*SSPIWrapper.EncryptMessage(SSPIInterface,SafeDeleteContext,SecurityBuffer[],UInt32)*
*SSPIWrapper.DecryptMessage(SSPIInterface,SafeDeleteContext,SecurityBuffer[],UInt32)*
*SSPIWrapper.VerifySignature(SSPIInterface,SafeDeleteContext,SecurityBuffer[],UInt32)*
*SSPIWrapper.QueryContextChannelBinding(SSPIInterface,SafeDeleteContext,ContextAttribute)*
SSPIWrapper.QueryContextAttributes(SSPIInterface,SafeDeleteContext,ContextAttribute)
*SSPIWrapper.ErrorDescription(Int32)*

WebSocket
----------------
*WebSocket.CreateClientBuffer(Int32,Int32)*
*WebSocket.CreateServerBuffer(Int32)*
*WebSocket.CreateClientWebSocket(Stream,String,Int32,Int32,TimeSpan,Boolean,ArraySegment`1)*
*WebSocket.RegisterPrefixes()*
*WebSocket.IsApplicationTargeting45()*

WebSocketProtocolComponent
----------------
*WebSocketProtocolComponent.Succeeded(Int32)*

AlternateView
----------------
AlternateView.CreateAlternateViewFromString(String)
AlternateView.CreateAlternateViewFromString(String,Encoding,String)
AlternateView.CreateAlternateViewFromString(String,ContentType)

Attachment
----------------
Attachment.CreateAttachmentFromString(String,String)
Attachment.CreateAttachmentFromString(String,String,Encoding,String)
Attachment.CreateAttachmentFromString(String,ContentType)

LinkedResource
----------------
LinkedResource.CreateLinkedResourceFromString(String)
LinkedResource.CreateLinkedResourceFromString(String,Encoding,String)
LinkedResource.CreateLinkedResourceFromString(String,ContentType)

IPGlobalProperties
----------------
*IPGlobalProperties.GetIPGlobalProperties()*

NetworkChange
----------------
*NetworkChange.RegisterNetworkChange(NetworkChange)*
*NetworkChange.add_NetworkAvailabilityChanged(NetworkAvailabilityChangedEventHandler)*
*NetworkChange.remove_NetworkAvailabilityChanged(NetworkAvailabilityChangedEventHandler)*
*NetworkChange.add_NetworkAddressChanged(NetworkAddressChangedEventHandler)*
*NetworkChange.remove_NetworkAddressChanged(NetworkAddressChangedEventHandler)*

NetworkInterface
----------------
*NetworkInterface.GetAllNetworkInterfaces()*
*NetworkInterface.GetIsNetworkAvailable()*

PhysicalAddress
----------------
*PhysicalAddress.Parse(String)*

TeredoHelper
----------------
*TeredoHelper.UnsafeNotifyStableUnicastIpAddressTable(Action`1,Object)*

Rfc2616
----------------
*Rfc2616.OnValidateRequest(HttpRequestCacheValidator)*
*Rfc2616.OnValidateFreshness(HttpRequestCacheValidator)*
*Rfc2616.OnValidateCache(HttpRequestCacheValidator)*
*Rfc2616.OnValidateResponse(HttpRequestCacheValidator)*
*Rfc2616.OnUpdateCache(HttpRequestCacheValidator)*

NetSectionGroup
----------------
*NetSectionGroup.GetSectionGroup(Configuration)*

Socket
----------------
*Socket.Select(IList,IList,IList,Int32)*
*Socket.ConnectAsync(SocketType,ProtocolType,SocketAsyncEventArgs)*
*Socket.CancelConnectAsync(SocketAsyncEventArgs)*

TcpListener
----------------
*TcpListener.Create(Int32)*

DynamicWinsockMethods
----------------
*DynamicWinsockMethods.GetMethods(AddressFamily,SocketType,ProtocolType)*

DynamicRoleClaimProvider
----------------
*DynamicRoleClaimProvider.AddDynamicRoleClaims(ClaimsIdentity,IEnumerable`1)*

CAPISafe
----------------
*CAPISafe.FreeLibrary(IntPtr)*

Oid
----------------
*Oid.FromFriendlyName(String,OidGroup)*
*Oid.FromOidValue(String,OidGroup)*

X509Certificate2
----------------
X509Certificate2.GetCertContentType(Byte[])
X509Certificate2.GetCertContentType(String)

X509Chain
----------------
*X509Chain.Create()*

BitVector32
----------------
BitVector32.CreateMask()
BitVector32.CreateMask(Int32)
BitVector32.CreateSection(Int16)
BitVector32.CreateSection(Int16,Section)

CollectionsUtil
----------------
CollectionsUtil.CreateCaseInsensitiveHashtable(Int32)
CollectionsUtil.CreateCaseInsensitiveHashtable()
CollectionsUtil.CreateCaseInsensitiveHashtable(IDictionary)
*CollectionsUtil.CreateCaseInsensitiveSortedList()*

BackCompatibleStringComparer
----------------
*BackCompatibleStringComparer.GetHashCode(String)*

SortedSet`1
----------------
SortedSet`1.CreateSetComparer()
SortedSet`1.CreateSetComparer(IEqualityComparer`1)

BlockingCollection`1
----------------
BlockingCollection`1.AddToAny(BlockingCollection`1[],T)
BlockingCollection`1.AddToAny(BlockingCollection`1[],T,CancellationToken)
BlockingCollection`1.TryAddToAny(BlockingCollection`1[],T)
BlockingCollection`1.TryAddToAny(BlockingCollection`1[],T,TimeSpan)
BlockingCollection`1.TryAddToAny(BlockingCollection`1[],T,Int32)
BlockingCollection`1.TryAddToAny(BlockingCollection`1[],T,Int32,CancellationToken)
BlockingCollection`1.TakeFromAny(BlockingCollection`1[],T&)
BlockingCollection`1.TakeFromAny(BlockingCollection`1[],T&,CancellationToken)
BlockingCollection`1.TryTakeFromAny(BlockingCollection`1[],T&)
BlockingCollection`1.TryTakeFromAny(BlockingCollection`1[],T&,TimeSpan)
BlockingCollection`1.TryTakeFromAny(BlockingCollection`1[],T&,Int32)
BlockingCollection`1.TryTakeFromAny(BlockingCollection`1[],T&,Int32,CancellationToken)

Semaphore
----------------
Semaphore.OpenExisting(String)
Semaphore.OpenExisting(String,SemaphoreRights)
Semaphore.TryOpenExisting(String,Semaphore&)
Semaphore.TryOpenExisting(String,SemaphoreRights,Semaphore&)

PatternMatcher
----------------
*PatternMatcher.StrictMatchPattern(String,String)*

SerialPort
----------------
*SerialPort.GetPortNames()*

Crc32Helper
----------------
*Crc32Helper.UpdateCrc32(UInt32,Byte[],Int32,Int32)*

ZLibNative
----------------
ZLibNative.CreateZLibStreamForDeflate(ZLibStreamHandle&)
ZLibNative.CreateZLibStreamForDeflate(ZLibStreamHandle&,CompressionLevel,Int32,Int32,CompressionStrategy)
ZLibNative.CreateZLibStreamForInflate(ZLibStreamHandle&)
ZLibNative.CreateZLibStreamForInflate(ZLibStreamHandle&,Int32)
*ZLibNative.ZLibCompileFlags()*

FastEncoderStatics
----------------
*FastEncoderStatics.BitReverse(UInt32,Int32)*

AssertWrapper
----------------
*AssertWrapper.ShowAssert(String,StackFrame,String,String)*

Debug
----------------
*Debug.Flush()*
*Debug.Close()*
Debug.Assert(Boolean)
Debug.Assert(Boolean,String)
Debug.Assert(Boolean,String,String)
Debug.Assert(Boolean,String,String,Object[])
Debug.Fail(String)
Debug.Fail(String,String)
Debug.Print(String)
Debug.Print(String,Object[])
Debug.Write(String)
Debug.Write(Object)
Debug.Write(String,String)
Debug.Write(Object,String)
Debug.WriteLine(String)
Debug.WriteLine(Object)
Debug.WriteLine(String,String)
Debug.WriteLine(Object,String)
Debug.WriteLine(String,Object[])
Debug.WriteIf(Boolean,String)
Debug.WriteIf(Boolean,Object)
Debug.WriteIf(Boolean,String,String)
Debug.WriteIf(Boolean,Object,String)
Debug.WriteLineIf(Boolean,String)
Debug.WriteLineIf(Boolean,Object)
Debug.WriteLineIf(Boolean,String,String)
Debug.WriteLineIf(Boolean,Object,String)
*Debug.Indent()*
*Debug.Unindent()*

SwitchAttribute
----------------
*SwitchAttribute.GetAll(Assembly)*

Trace
----------------
*Trace.Flush()*
*Trace.Close()*
Trace.Assert(Boolean)
Trace.Assert(Boolean,String)
Trace.Assert(Boolean,String,String)
Trace.Fail(String)
Trace.Fail(String,String)
*Trace.Refresh()*
Trace.TraceInformation(String)
Trace.TraceInformation(String,Object[])
Trace.TraceWarning(String)
Trace.TraceWarning(String,Object[])
Trace.TraceError(String)
Trace.TraceError(String,Object[])
Trace.Write(String)
Trace.Write(Object)
Trace.Write(String,String)
Trace.Write(Object,String)
Trace.WriteLine(String)
Trace.WriteLine(Object)
Trace.WriteLine(String,String)
Trace.WriteLine(Object,String)
Trace.WriteIf(Boolean,String)
Trace.WriteIf(Boolean,Object)
Trace.WriteIf(Boolean,String,String)
Trace.WriteIf(Boolean,Object,String)
Trace.WriteLineIf(Boolean,String)
Trace.WriteLineIf(Boolean,Object)
Trace.WriteLineIf(Boolean,String,String)
Trace.WriteLineIf(Boolean,Object,String)
*Trace.Indent()*
*Trace.Unindent()*

TraceInternal
----------------
*TraceInternal.Flush()*
TraceInternal.Assert(Boolean,String)
TraceInternal.Fail(String)
*TraceInternal.TraceEvent(TraceEventType,Int32,String,Object[])*
TraceInternal.WriteLine(String)
TraceInternal.WriteLine(String,String)
TraceInternal.WriteLineIf(Boolean,String)
*TraceInternal.Indent()*
*TraceInternal.Unindent()*
*TraceInternal.Close()*
TraceInternal.Assert(Boolean)
TraceInternal.Assert(Boolean,String,String)
TraceInternal.Fail(String,String)
TraceInternal.Write(String)
TraceInternal.Write(Object)
TraceInternal.Write(String,String)
TraceInternal.Write(Object,String)
TraceInternal.WriteLine(Object)
TraceInternal.WriteLine(Object,String)
TraceInternal.WriteIf(Boolean,String)
TraceInternal.WriteIf(Boolean,Object)
TraceInternal.WriteIf(Boolean,String,String)
TraceInternal.WriteIf(Boolean,Object,String)
TraceInternal.WriteLineIf(Boolean,Object)
TraceInternal.WriteLineIf(Boolean,String,String)
TraceInternal.WriteLineIf(Boolean,Object,String)

CounterSample
----------------
CounterSample.Calculate(CounterSample)
CounterSample.Calculate(CounterSample,CounterSample)

CounterSampleCalculator
----------------
CounterSampleCalculator.ComputeCounterValue(CounterSample)
CounterSampleCalculator.ComputeCounterValue(CounterSample,CounterSample)

EventLog
----------------
EventLog.CreateEventSource(String,String)
EventLog.CreateEventSource(String,String,String)
EventLog.CreateEventSource(EventSourceCreationData)
EventLog.Delete(String)
EventLog.Delete(String,String)
EventLog.DeleteEventSource(String)
EventLog.DeleteEventSource(String,String)
EventLog.Exists(String)
EventLog.Exists(String,String)
EventLog.GetEventLogs()
EventLog.GetEventLogs(String)
EventLog.SourceExists(String)
EventLog.SourceExists(String,String)
*EventLog.LogNameFromSourceName(String,String)*
EventLog.WriteEntry(String,String)
EventLog.WriteEntry(String,String,EventLogEntryType)
EventLog.WriteEntry(String,String,EventLogEntryType,Int32)
EventLog.WriteEntry(String,String,EventLogEntryType,Int32,Int16)
EventLog.WriteEntry(String,String,EventLogEntryType,Int32,Int16,Byte[])
EventLog.WriteEvent(String,EventInstance,Object[])
EventLog.WriteEvent(String,EventInstance,Byte[],Object[])

FileVersionInfo
----------------
*FileVersionInfo.GetVersionInfo(String)*

PerformanceCounter
----------------
*PerformanceCounter.CloseSharedResources()*

PerformanceCounterCategory
----------------
PerformanceCounterCategory.CounterExists(String,String)
PerformanceCounterCategory.CounterExists(String,String,String)
PerformanceCounterCategory.Create(String,String,String,String)
PerformanceCounterCategory.Create(String,String,PerformanceCounterCategoryType,String,String)
PerformanceCounterCategory.Create(String,String,CounterCreationDataCollection)
PerformanceCounterCategory.Create(String,String,PerformanceCounterCategoryType,CounterCreationDataCollection)
*PerformanceCounterCategory.Delete(String)*
PerformanceCounterCategory.Exists(String)
PerformanceCounterCategory.Exists(String,String)
PerformanceCounterCategory.GetCategories()
PerformanceCounterCategory.GetCategories(String)
PerformanceCounterCategory.InstanceExists(String,String)
PerformanceCounterCategory.InstanceExists(String,String,String)

Process
----------------
*Process.EnterDebugMode()*
*Process.LeaveDebugMode()*
Process.GetProcessById(Int32,String)
Process.GetProcessById(Int32)
Process.GetProcessesByName(String)
Process.GetProcessesByName(String,String)
Process.GetProcesses()
Process.GetProcesses(String)
*Process.GetCurrentProcess()*
Process.Start(String,String,SecureString,String)
Process.Start(String,String,String,SecureString,String)
Process.Start(String)
Process.Start(String,String)
Process.Start(ProcessStartInfo)

EnvironmentBlock
----------------
*EnvironmentBlock.ToByteArray(StringDictionary,Boolean)*

ProcessManager
----------------
*ProcessManager.OpenProcess(Int32,Int32,Boolean)*
*ProcessManager.IsRemoteMachine(String)*
*ProcessManager.GetProcessInfos(String)*
ProcessManager.GetProcessIds()
ProcessManager.GetProcessIds(String)
ProcessManager.IsProcessRunning(Int32,String)
ProcessManager.IsProcessRunning(Int32)
*ProcessManager.GetProcessIdFromHandle(SafeProcessHandle)*
*ProcessManager.GetMainWindowHandle(Int32)*
*ProcessManager.GetModuleInfos(Int32)*
*ProcessManager.OpenThread(Int32,Int32)*

WinProcessManager
----------------
*WinProcessManager.GetProcessIds()*
*WinProcessManager.GetProcessInfos()*
*WinProcessManager.GetModuleInfos(Int32)*

NtProcessManager
----------------
NtProcessManager.GetProcessIds()
NtProcessManager.GetProcessIds(String,Boolean)
*NtProcessManager.GetModuleInfos(Int32)*
*NtProcessManager.GetFirstModuleInfo(Int32)*
*NtProcessManager.GetProcessIdFromHandle(SafeProcessHandle)*
*NtProcessManager.GetProcessInfos(String,Boolean)*

NtProcessInfoHelper
----------------
*NtProcessInfoHelper.GetProcessInfos()*

Stopwatch
----------------
*Stopwatch.StartNew()*
*Stopwatch.GetTimestamp()*

AsyncOperationManager
----------------
*AsyncOperationManager.CreateOperation(Object)*

AttributeCollection
----------------
*AttributeCollection.FromExisting(AttributeCollection,Attribute[])*

IntSecurity
----------------
*IntSecurity.UnsafeGetFullPath(String)*

LicenseManager
----------------
*LicenseManager.LockContext(Object)*
LicenseManager.CreateWithContext(Type,LicenseContext)
LicenseManager.CreateWithContext(Type,LicenseContext,Object[])
*LicenseManager.IsLicensed(Type)*
LicenseManager.IsValid(Type)
LicenseManager.IsValid(Type,Object,License&)
*LicenseManager.UnlockContext(Object)*
LicenseManager.Validate(Type)
LicenseManager.Validate(Type,Object)

MaskedTextProvider
----------------
*MaskedTextProvider.GetOperationResultFromHint(MaskedTextResultHint)*
*MaskedTextProvider.IsValidInputChar(Char)*
*MaskedTextProvider.IsValidMaskChar(Char)*
*MaskedTextProvider.IsValidPasswordChar(Char)*

SyntaxCheck
----------------
*SyntaxCheck.CheckMachineName(String)*
*SyntaxCheck.CheckPath(String)*
*SyntaxCheck.CheckRootedPath(String)*

TypeDescriptor
----------------
*TypeDescriptor.add_Refreshed(RefreshEventHandler)*
*TypeDescriptor.remove_Refreshed(RefreshEventHandler)*
TypeDescriptor.AddAttributes(Type,Attribute[])
TypeDescriptor.AddAttributes(Object,Attribute[])
*TypeDescriptor.AddEditorTable(Type,Hashtable)*
TypeDescriptor.AddProvider(TypeDescriptionProvider,Type)
TypeDescriptor.AddProvider(TypeDescriptionProvider,Object)
TypeDescriptor.AddProviderTransparent(TypeDescriptionProvider,Type)
TypeDescriptor.AddProviderTransparent(TypeDescriptionProvider,Object)
*TypeDescriptor.CreateAssociation(Object,Object)*
*TypeDescriptor.CreateDesigner(IComponent,Type)*
TypeDescriptor.CreateEvent(Type,String,Type,Attribute[])
TypeDescriptor.CreateEvent(Type,EventDescriptor,Attribute[])
*TypeDescriptor.CreateInstance(IServiceProvider,Type,Type[],Object[])*
TypeDescriptor.CreateProperty(Type,String,Type,Attribute[])
TypeDescriptor.CreateProperty(Type,PropertyDescriptor,Attribute[])
*TypeDescriptor.GetAssociation(Type,Object)*
TypeDescriptor.GetAttributes(Type)
TypeDescriptor.GetAttributes(Object)
TypeDescriptor.GetAttributes(Object,Boolean)
TypeDescriptor.GetClassName(Object)
TypeDescriptor.GetClassName(Object,Boolean)
TypeDescriptor.GetClassName(Type)
TypeDescriptor.GetComponentName(Object)
TypeDescriptor.GetComponentName(Object,Boolean)
TypeDescriptor.GetConverter(Object)
TypeDescriptor.GetConverter(Object,Boolean)
TypeDescriptor.GetConverter(Type)
TypeDescriptor.GetDefaultEvent(Type)
TypeDescriptor.GetDefaultEvent(Object)
TypeDescriptor.GetDefaultEvent(Object,Boolean)
TypeDescriptor.GetDefaultProperty(Type)
TypeDescriptor.GetDefaultProperty(Object)
TypeDescriptor.GetDefaultProperty(Object,Boolean)
TypeDescriptor.GetEditor(Object,Type)
TypeDescriptor.GetEditor(Object,Type,Boolean)
TypeDescriptor.GetEditor(Type,Type)
TypeDescriptor.GetEvents(Type)
TypeDescriptor.GetEvents(Type,Attribute[])
TypeDescriptor.GetEvents(Object)
TypeDescriptor.GetEvents(Object,Boolean)
TypeDescriptor.GetEvents(Object,Attribute[])
TypeDescriptor.GetEvents(Object,Attribute[],Boolean)
*TypeDescriptor.GetFullComponentName(Object)*
TypeDescriptor.GetProperties(Type)
TypeDescriptor.GetProperties(Type,Attribute[])
TypeDescriptor.GetProperties(Object)
TypeDescriptor.GetProperties(Object,Boolean)
TypeDescriptor.GetProperties(Object,Attribute[])
TypeDescriptor.GetProperties(Object,Attribute[],Boolean)
TypeDescriptor.GetProvider(Type)
TypeDescriptor.GetProvider(Object)
TypeDescriptor.GetReflectionType(Type)
TypeDescriptor.GetReflectionType(Object)
TypeDescriptor.Refresh(Object)
TypeDescriptor.Refresh(Type)
TypeDescriptor.Refresh(Module)
TypeDescriptor.Refresh(Assembly)
*TypeDescriptor.RemoveAssociation(Object,Object)*
*TypeDescriptor.RemoveAssociations(Object)*
TypeDescriptor.RemoveProvider(TypeDescriptionProvider,Type)
TypeDescriptor.RemoveProvider(TypeDescriptionProvider,Object)
TypeDescriptor.RemoveProviderTransparent(TypeDescriptionProvider,Type)
TypeDescriptor.RemoveProviderTransparent(TypeDescriptionProvider,Object)
*TypeDescriptor.SortDescriptorArray(IList)*

DesigntimeLicenseContextSerializer
----------------
*DesigntimeLicenseContextSerializer.Serialize(Stream,String,DesigntimeLicenseContext)*

CodeDomProvider
----------------
CodeDomProvider.CreateProvider(String,IDictionary`2)
*CodeDomProvider.GetLanguageFromExtension(String)*
*CodeDomProvider.IsDefinedExtension(String)*
*CodeDomProvider.GetCompilerInfo(String)*
*CodeDomProvider.GetAllCompilerInfo()*
CodeDomProvider.CreateProvider(String)
*CodeDomProvider.IsDefinedLanguage(String)*

CodeGenerator
----------------
*CodeGenerator.IsValidLanguageIndependentIdentifier(String)*
*CodeGenerator.ValidateIdentifiers(CodeObject)*

Executor
----------------
*Executor.ExecWait(String,TempFileCollection)*
Executor.ExecWaitWithCapture(String,TempFileCollection,String&,String&)
Executor.ExecWaitWithCapture(String,String,TempFileCollection,String&,String&)
Executor.ExecWaitWithCapture(IntPtr,String,TempFileCollection,String&,String&)
Executor.ExecWaitWithCapture(IntPtr,String,String,TempFileCollection,String&,String&)

RedistVersionInfo
----------------
*RedistVersionInfo.GetCompilerPath(IDictionary`2,String)*

Regex
----------------
*Regex.Escape(String)*
*Regex.Unescape(String)*
Regex.IsMatch(String,String)
Regex.IsMatch(String,String,RegexOptions)
Regex.IsMatch(String,String,RegexOptions,TimeSpan)
Regex.Match(String,String)
Regex.Match(String,String,RegexOptions)
Regex.Match(String,String,RegexOptions,TimeSpan)
Regex.Matches(String,String)
Regex.Matches(String,String,RegexOptions)
Regex.Matches(String,String,RegexOptions,TimeSpan)
Regex.Replace(String,String,String)
Regex.Replace(String,String,String,RegexOptions)
Regex.Replace(String,String,String,RegexOptions,TimeSpan)
Regex.Replace(String,String,MatchEvaluator)
Regex.Replace(String,String,MatchEvaluator,RegexOptions)
Regex.Replace(String,String,MatchEvaluator,RegexOptions,TimeSpan)
Regex.Split(String,String)
Regex.Split(String,String,RegexOptions)
Regex.Split(String,String,RegexOptions,TimeSpan)
Regex.CompileToAssembly(RegexCompilationInfo[],AssemblyName)
Regex.CompileToAssembly(RegexCompilationInfo[],AssemblyName,CustomAttributeBuilder[])
Regex.CompileToAssembly(RegexCompilationInfo[],AssemblyName,CustomAttributeBuilder[],String)

Group
----------------
*Group.Synchronized(Group)*

Match
----------------
*Match.Synchronized(Match)*

Util
----------------
*Util.HIWORD(Int32)*
*Util.LOWORD(Int32)*

HttpApi
----------------
*HttpApi.TokenBindingVerifyMessage(Byte*,UInt32,IntPtr,Byte*,UInt32,HeapAllocHandle&)*

HtmlEntities
----------------
*HtmlEntities.Lookup(String)*

KeepAliveTracker
----------------
*KeepAliveTracker.Create(TimeSpan)*

Common
----------------
*Common.OnValidateRequest(HttpRequestCacheValidator)*
*Common.ComputeFreshness(HttpRequestCacheValidator)*
*Common.ValidateCacheByClientPolicy(HttpRequestCacheValidator)*
*Common.ValidateCacheAfterResponse(HttpRequestCacheValidator,HttpWebResponse)*
*Common.ValidateCacheOn5XXResponse(HttpRequestCacheValidator)*
*Common.TryConditionalRequest(HttpRequestCacheValidator)*
*Common.TryResponseFromCache(HttpRequestCacheValidator)*
*Common.ConstructConditionalRequest(HttpRequestCacheValidator)*
*Common.Construct206PartialContent(HttpRequestCacheValidator,Int32)*
*Common.Construct200ok(HttpRequestCacheValidator)*
*Common.ConstructUnconditionalRefreshRequest(HttpRequestCacheValidator)*
*Common.ReplaceOrUpdateCacheHeaders(HttpRequestCacheValidator,HttpWebResponse)*
*Common.GetBytesRange(String,Int64&,Int64&,Int64&,Boolean)*

FrozenCacheEntry
----------------
*FrozenCacheEntry.Create(FrozenCacheEntry)*

CultureInfoMapper
----------------
*CultureInfoMapper.GetCultureInfoName(String)*
```
