[<AutoOpen>]
module System
    let toString f =
        f.ToString()

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
