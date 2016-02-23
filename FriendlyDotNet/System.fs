/// This code is licensed as Apache 2.0 
/// Access this below functionality by issuing an "open System"
namespace System

[<AutoOpen>]
module Core =
    /// Call ToString() on the argument
    let toString x = 
        x.ToString ()

    /// records may be null when created from C# or other .Net languages 
    /// Use when F# complains: "The Type 'foo' does not have 'null' as a proper value"
    let nullRecordAsNone x = 
        if(box x = null) then None else Some x
    
    /// Unwrap a Nullable<_> by providing a defaultValue if null
    let unNullable defaultValue (x :Nullable<_>) = 
        if x.HasValue then x.Value else defaultValue
    
    /// Nicer implementation than the 'core.defualtArg' since the default value is the first parameter - making it easier to use with piping
    let unOption defaultValue x =
        match x with 
        | Some y -> y
        | None -> defaultValue

    let tryresultAsOption x =
        match x with
        | true, y -> Some y
        | false, _ -> None


[<AutoOpen>]
module DateAndTime =
    type System.TimeSpan with
        static member fromDays x =         TimeSpan.FromDays (float x) 
        static member fromHours x =        TimeSpan.FromHours(float x) 
        static member fromMilliseconds x = TimeSpan.FromMilliseconds(float x) 
        static member fromMinutes x =      TimeSpan.FromMinutes (float x) 
        static member fromSeconds x =      TimeSpan.FromSeconds(float x) 


[<AutoOpen>]
module Parsing =
    type System.Boolean with
        static member tryParse str = System.Boolean.TryParse str |> tryresultAsOption   

    type System.Double with
        static member tryParse str = System.Double.TryParse str |> tryresultAsOption   

    type System.Int32 with 
        static member tryParse str = System.Int32.TryParse str |> tryresultAsOption   

    type System.Int64 with 
        static member tryParse str = System.Int64.TryParse str |> tryresultAsOption   

    type System.Guid with 
        static member tryParse str = System.Guid.TryParse str |> tryresultAsOption   

    type System.DateTime with 
        static member tryParse str = System.DateTime.TryParse str |> tryresultAsOption   
    
    // for each of the above extension methods, define an active pattern below
    let (|Bool|_|) str = System.Boolean.tryParse str
    let (|Float|_|) str = System.Double.tryParse str
    let (|Int|_|) str = System.Int32.tryParse str
    let (|Int64|_|) str = System.Int64.tryParse str
    let (|Guid|_|) str = System.Guid.tryParse str
    let (|DateTime|_|) str = System.DateTime.tryParse str
