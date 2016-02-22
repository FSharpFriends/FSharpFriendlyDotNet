namespace FriendlyDotNetTests

module SystemTests =
    open Xunit
    open System
    open Swensen.Unquote

    [<Fact>]
    let ``toString test`` () =
        let i = 4
        test <@ "4" = toString i @>

    [<Fact>]
    let ``Boolean.tryparse nonnumber = None`` () =
        test <@ Boolean.tryParse "baby" = None @>

    [<Fact>]
    let ``Boolean.tryparse number = Some number`` () =
        test <@ Boolean.tryParse "true" = (Some true) @>

    [<Fact>]
    let ``Int32.tryparse nonnumber = None`` () =
        test <@ Int32.tryParse "baby" = None @>

    [<Fact>]
    let ``Int32.tryparse number = Some number`` () =
        test <@ Int32.tryParse "44" = (Some 44) @>

    [<Fact>]
    let ``Int64.tryparse nonnumber = None`` () =
        test <@ Int64.tryParse "baby" = None @>

    [<Fact>]
    let ``Int64.tryparse number = Some number`` () =
        test <@ Int64.tryParse "44" = (Some 44L) @>

    [<Fact>]
    let ``Active patterns for conversion int`` () =
        let str = "44"
        let conv = match str with
                    | Int i -> i
                    | _ -> invalidArg str "should be an int"
        test <@ conv = 44 @>

    [<Fact>]
    let ``Active patterns for conversion bool`` () =
        let str = "false"
        let conv = match str with
                    | Bool b -> b
                    | _ -> invalidArg str "should be a bool"
        test <@ conv = false @>


    [<Fact>]
    let ``Active patterns for conversion double`` () =
        let str = "3,14159265"
        let conv = match str with
                    | Float f -> f
                    | _ -> invalidArg str "should be a double"
        test <@ conv = 3.14159265 @>


    [<Fact>]
    let ``Active patterns for conversion int64`` () =
        let str = "8284784593947573344"
        let conv = match str with
                    | Int64 i -> i
                    | _ -> invalidArg str "should be a bool"
        test <@ conv = 8284784593947573344L @>

    [<Fact>]
    let ``Active patterns for conversion mix`` () =
        let str = "8284784593947573344"
        let conv = match str with
                    | Int i -> "is int"
                    | Int64 i -> "is int64"
                    | Float f -> "is float"
                    | Bool b -> "is bool"
                    | _ -> invalidArg str "should be something"
        test <@ conv = "is int64" @>
        let d = new DateTime()
        d.Add (TimeSpan.FromDays(1.0))
        d.Add <| TimeSpan.FromDays 1.0
        d.Add TimeSpan.FromDays(1.0)
        d.Add (TimeSpan.FromDays 1.0)

