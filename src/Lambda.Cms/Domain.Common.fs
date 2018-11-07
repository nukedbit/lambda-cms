namespace Lambda.Cms
open System
open System.Text.RegularExpressions
open System.Text


type Email = private Email of string

module internal Email = 
    open Chessie
    
    let create s = 
        let r = Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
        if r.IsMatch(s) then
            Result<Email,string>.Ok (Email s)
        else 
            Result<Email, string>.Error ("Invalid email address")

type CategoryId = CategoryId of Guid

type DocumentId = DocumentId of Guid

type UserId = UserId of Guid

type FileId = FileId of Guid

type Title = Title of string

module Title =
    
    let create s =
        let maxLength = 60
        if String.IsNullOrEmpty(s) then
            Result.Error "Title can't be null or empty"
        else if s.Length >= maxLength then
            Result.Error (sprintf "Length should be less than %d." maxLength)
        else
            Result<Title,string>.Ok (Title s)
            
    let toString (Title t) = 
        t

type Slug = Slug of string

module Slug = 
    
    let value (Slug s ) = s

    let private remapInternationalCharToAscii (c:Char) =
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
                            
    let fromString s =
        let isLowerCaseLetter c = c >= 'a' && c <= 'z'
        let isDigit c = (c >= '0' && c <= '9')
        let isInvalidSeparator c =
            c = ' ' || c = ',' || c = '.' || c = '/' 
                ||c = '\\' || c = '-' || c = '_' || c = '='
        let isNotAscii c = (int)c >= 128
        
        if String.IsNullOrEmpty(s) then
                Chessie.Result<Slug,string>.Bad ["Title can't be null or empty"]
            else
                let sb = new StringBuilder()
                let mutable prevdash = false;
                let mutable pos = 0
                while pos <= 80 && pos < s.Length do
                    let c = s.[pos]
                    if (isLowerCaseLetter c || isDigit c) then
                        sb.Append(c) |> ignore
                        prevdash <- false;
                    else if(c >= 'A' && c <= 'Z') then
                        sb.Append(Char.ToLower(c)) |> ignore
                        prevdash <- false;
                    else if (isInvalidSeparator c) then
                        if (not(prevdash) && sb.Length > 0) then
                            sb.Append('-') |> ignore
                            prevdash <- true
                    else if (isNotAscii c) then
                        match remapInternationalCharToAscii(c) with 
                        | Some s ->
                            let prevlen = sb.Length
                            sb.Append(s) |> ignore
                            if (prevlen <> sb.Length) then prevdash <- false
                        | _ -> ()
                    pos <- pos + 1
                if (prevdash) then 
                   Chessie.Result<Slug,string>.Ok (Slug(sb.ToString().Substring(0, sb.Length - 1)) , [])
                else 
                   Chessie.Result<Slug,string>.Ok (Slug(sb.ToString()) , [])
                    
    let fromTitle t =
        fromString (Title.toString t) 

