namespace FriendlyDotNetTests

module SystemTests =
    open Xunit

    [<Fact>]
    let ``toString test`` () =
        let i = 4
        <@ "4" = toString i @>

