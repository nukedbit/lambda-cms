open System.Text
#load "Domain.fs"
open System

let remapInternationalCharToAscii (c:Char) =
    let s = c.ToString().ToLowerInvariant()
    let table = [
        ("àåáâäãåą", "a")
        ("èéêëę", "e")
        ("ìíîïı", "i")
        ("òóôõöøőð", "o")
        ("ùúûüŭů", "u")
        ("çćčĉ", "c")
        ("żźž", "z")
        ("śşšŝ", "s")
        ("ñń", "n")
        ("ýÿ", "y")
        ("ğĝ", "g")
        ("ř", "r")
        ("ł", "l")
        ("đ", "d")
        ("ß", "ss")
        ("Þ", "th")
        ("ĥ", "h")
        ("ĵ", "j")
        ] 

    table|> Seq.tryPick(fun (t, v) -> 
             match t.Contains(s) with 
             | true -> Some v
             | _ -> None
            )
    

let fromTitle s =
    if String.IsNullOrEmpty(s) then
        Result.Error "Title can't be null or empty"
    else
        let sb = new StringBuilder()
        let mutable prevdash = false;
        let mutable pos = 0
        while pos <= 80 && pos < s.Length do
            let c = s.[pos]
            if (c >= 'a' && c <= 'z' || (c >= '0' && c <= '9')) then
                sb.Append(c) |> ignore
                prevdash <- false;
            else if(c >= 'A' && c <= 'Z') then
                sb.Append(Char.ToLower(c)) |> ignore
                prevdash <- false;
            else if (c = ' ' || c = ',' || c = '.' || c = '/' ||
                         c = '\\' || c = '-' || c = '_' || c = '=') then
                if (not(prevdash) && sb.Length > 0) then
                    sb.Append('-') |> ignore
                    prevdash <- true
            else if ((int)c >= 128) then
                let prevlen = sb.Length
                sb.Append(remapInternationalCharToAscii(c)) |> ignore
                if (prevlen <> sb.Length) then prevdash <- false
            pos <- pos + 1
        if (prevdash) then 
           Result.Ok(sb.ToString().Substring(0, sb.Length - 1))
        else 
            Result.Ok (sb.ToString())

fromTitle("F# is an amazing programming language.")

