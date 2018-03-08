# F# friendly .Net 

*Making interaction with the .Net Api feel much more natural and friendlier for the eye.* For an in-depth explanation see https://github.com/FSharpFriends/FSharpFriendlyDotNet/blob/master/Ramblings.md 


Typical problems when interacting with the .Net api are
  * Dealing with `null`'s and try-methods feels wrong when we have `Option` in F#. An example of a try-method is `int.Parse()`
  * `KeyValuePair<>` makes sense in C#, but is more naturally expressed at tuples in F#
  * C# has automatic type conversion, F# doesn't - this makes calling some code look ridiculous
  * Some actions are verbose done purely in F# (such as using `sprintf`)

The goals of this project are

  * By defining a set of helper functions, we can make the interaction much smoother and generally making the coding experince better.
  * When defining the helper methods, we can rectify or make clearer the existing .Net api.


### Examples 

For example turning an int `i` into a string `s` one can do

```F#
let i = 4
let s1 = i.ToString()    // I dont like the parenthesis and it feels non-functional
let s2 = sprintf "%d" i  // involves lots of shift-key pressing and little autocompletion
let s3 = toString i      // functional style and terse
```
clearly the `s3` version is the less verbose and most functional.. and a part of this library.

Or how about turning try-method matching like 

```F#
match int.TryParse(httpParameter) with
| true, x -> x
| false, _ -> ...
```

into something that returns `Option` like

```F#
match int.tryParse httpParameter with // using this library
| Some x -> x
| None -> ...
```

The missing type conversions in F# make for some really odd looking code. Just look at the `TimeSpan` api. Most of the `TimeSpan` methods take a `double` as an argument. 
I have yet to find a situation where you want to use a double as an argument. 
Pop quiz: What is the result of `TimeSpan.FromMinutes(0.12345)` ? Answer `00:00:07.4070000` ([Microsoft](https://msdn.microsoft.com/en-us/library/system.timespan.fromminutes%28v=vs.110%29.asp)). 
Are you kidding me Microsoft? Seriously? Please allow me to only use integers.

```F#
TimeSpan.FromMinutes(1.0)   // original
TimeSpan.fromMinutes 1      // using this library
```


# Documentation

Here is the full documentation of the package. See also the code, it is pretty-self documenting anyways :-)


## `System`

Implementations: [System.fs](https://github.com/FSharpFriends/FSharpFriendlyDotNet/blob/master/src/FriendlyDotNet/System.fs)

How to access this below code 


### String parsing

nicer `tryParse` methods that return `Option<'T>` rather than `(bool * 'T)`

* `Boolean.tryParse s:string -> bool option` 
* `Dobule.tryParse s:string -> float option` 
* `Int32.tryParse s:string -> int option` 
* `Int64.tryParse s:string -> int64 option` 
* `Guid.tryParse s:string -> Guid option` 
* `DateTime.tryParse s:string -> DateTime option` 

Also there are active patterns declared for conversion

```F#
let str = "8284784593947573344"
      let conv = match str with
                 | Int i -> "is int"
                 | Int64 i -> "is int64"
                 | Float f -> "is float"
                 | Bool b -> "is bool"
                 | _ -> invalidArg str "should be something"
 // conv -> "is int64"
```

# Getting the package
The package is found on nuget. Use any of the following

    Install-Package FSharpFriendlyDotNet  
    
or

    dotnet add package FSharpFriendlyDotNet 

or

    paket add FSharpFriendlyDotNet  
