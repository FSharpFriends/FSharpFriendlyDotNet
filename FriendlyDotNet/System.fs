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
    let unNullable defaultValue (i:Nullable<_>) = 
        if i.HasValue then i.Value else defaultValue

    /// Turn 'null' into 'None'. Nice for wrapping the result of a .Net api call or deserialization of Json or Xml data
    let nullAsNone v = 
        if isNull v then None else Some v

    // Convert 'None' into null. 
    // Should only be used when interfacing with other .Net code or serializing to Json, Xml, etc.
    let noneAsNull v =
        match v with
        | None -> null
        | Some x -> x


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
        static member tryParse str =
            match System.Boolean.TryParse str with
            | true, x -> Some x
            | false, _ -> None

    type System.Double with
        static member tryParse str =
            match System.Double.TryParse str with
            | true, x -> Some x
            | false, _ -> None

    type System.Int32 with 
        static member tryParse str =
            match System.Int32.TryParse str with
            | true, x -> Some x
            | false, _ -> None

    type System.Int64 with 
        static member tryParse str =
            match System.Int64.TryParse str with
            | true, x -> Some x
            | false, _ -> None

    type System.Guid with 
        static member tryParse str =
            match System.Guid.TryParse str with
            | true, x -> Some x
            | false, _ -> None

    type System.DateTime with 
            static member tryParse str =
                match System.DateTime.TryParse str with
                | true, x -> Some x
                | false, _ -> None
    
    // for each of the above extension methods, define an active pattern below
    let (|Bool|_|) str = System.Boolean.tryParse str
    let (|Float|_|) str = System.Double.tryParse str
    let (|Int|_|) str = System.Int32.tryParse str
    let (|Int64|_|) str = System.Int64.tryParse str
    let (|Guid|_|) str = System.Guid.tryParse str
    let (|DateTime|_|) str = System.DateTime.tryParse str
