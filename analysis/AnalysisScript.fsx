open System.Reflection
open System.Runtime
open System.IO
open System.Linq
open System.Text.RegularExpressions
     
let simpleClassFilter name =
    match name with
    | "Parallel" | "Task" | "TaskScheduler" | "WaitHandle" | "ThreadPool" | "Thread" | "Monitor" | "Interlocked" | "IsolationInterop" | "Volatile" | "SpinWait"
    | "AsyncTaskMethodBuilder"  | "InternalActivationContextHelper" | "InternalApplicationIdentityHelper" 
    | "NativeMethods" | "SafeNativeMethods" | "UnsafeNativeMethods"
    | "UnmanagedMarshal"
    | "GC" | "GCHandle" | "GCSettings" 
    | "Array" | "Tuple" | "ArrayList"
    | "WindowsRuntimeDesignerContext"
    | "UIntPtr" | "Pointer"
    | "Statics" | "StrongNameHelpers" | "Object"  -> false
    | _ -> true

let simpleMethodfilter (mth:MethodInfo) =
    let name = mth.Name
    match name with
    | "OpFlags" | "Disposition" | "RefFlags" | "ToString" | "get_XsdType"-> false
    | _ -> (name.StartsWith("op_") || name.StartsWith("get_") || name.StartsWith("set_") || name.StartsWith("_") || name.StartsWith("<>")) |> not


let processs (assembly : Assembly) : unit =
    printfn "####################"
    printfn "%s" assembly.FullName
    printfn "####################"
    for typee in assembly.GetTypes() do
        if simpleClassFilter typee.Name then
            let methods = typee .GetMethods(BindingFlags.Public ||| BindingFlags.Static) |> Seq.where simpleMethodfilter
            if methods.Any() then
                let overloads = 
                    methods                            
                    |> Seq.distinctBy(fun f->f.Name)
                    |> Seq.map(fun f -> (f.Name, methods |> Seq.where(fun x -> x.Name = f.Name) |> List.ofSeq))
                    |> Map.ofSeq
                printfn "\n%s" typee.Name
                printfn "----------------"        
                for meth in methods do
                    let hasoverload = overloads.Item(meth.Name).Length = 1 
                    let hasMultipleParameters = meth.GetParameters().Count() > 1 
                    let highlight s = if hasoverload && hasMultipleParameters then "*"+s+"*" else s
                    let signature = meth.GetParameters() |> Seq.map(fun typee -> typee.ParameterType.Name) |> String.concat ","
                    sprintf "%s.%s(%s)" typee.Name meth.Name signature |> highlight |> printfn "%s"

processs <| Assembly.GetAssembly("".GetType())

printfn "\n\n\n"
processs <| Assembly.GetAssembly(typedefof<Regex>)
        
0
