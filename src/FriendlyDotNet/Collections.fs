/// This code is licensed as Apache 2.0 
/// Access this below functionality by issuing an "open System.Collections.Generic"
namespace System.Collections.Generic

[<AutoOpen>]
module Collections =
    open System.Core

    let kvpToTuple (kvp : System.Collections.Generic.KeyValuePair<_,_>) = 
        (kvp.Key, kvp.Value) 

    type System.Collections.Generic.IDictionary<'k,'v> with 
        member this.tryGetValue (key : 'k) =
            this.TryGetValue key |> tryresultAsOption

    type System.Collections.Generic.Dictionary<'k,'v> with 
        member this.tryGetValue (key : 'k) =
            this.TryGetValue key |> tryresultAsOption



