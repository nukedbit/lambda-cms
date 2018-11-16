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
                
    let (|LowerCase|Digit|InvalidSeparator|NotAscii|Skip|) (c:char) =
        let isLowerCaseLetter c = c >= 'a' && c <= 'z'
        let isDigit c = (c >= '0' && c <= '9')
        let isInvalidSeparator c =
            c = ' ' || c = ',' || c = '.' || c = '/' 
                ||c = '\\' || c = '-' || c = '_' || c = '='
        let isNotAscii c = (int)c >= 128
                
        if isLowerCaseLetter c then
           LowerCase(c)
        elif c >= 'A' && c <= 'Z' then
           LowerCase(Char.ToLower(c)) 
        elif isDigit c then
           Digit(c)
        elif isInvalidSeparator c then
           InvalidSeparator(c)
        elif (isNotAscii c) then
           NotAscii (c)           
        else
            Skip
            
                
    let fromString s =
        let isLowerCaseLetter c = c >= 'a' && c <= 'z'
        let isDigit c = (c >= '0' && c <= '9')
        let isInvalidSeparator c =
            c = ' ' || c = ',' || c = '.' || c = '/' 
                ||c = '\\' || c = '-' || c = '_' || c = '='
        let isNotAscii c = (int)c >= 128
        
        let rec eval (sourceStr:string) (prevdash:bool) (slug:string) =
            if slug.Length > 80 then
               slug
            elif sourceStr = "" then
               if prevdash then
                   slug.Substring(0, slug.Length - 1)
               else
                   slug                
            else
                let c = sourceStr.[0]
                let remaining = sourceStr.Substring(1)
                match c with
                | LowerCase chr ->
                    eval remaining false (slug + chr.ToString())
                | Digit chr -> 
                    eval remaining false (slug + chr.ToString())
                | InvalidSeparator chr ->
                    if (not(prevdash) && slug.Length > 0) then
                        eval remaining true (slug + '-'.ToString())
                    else
                        eval remaining prevdash slug
                | NotAscii chr ->
                    match remapInternationalCharToAscii(c) with 
                    | Some s ->
                        let prevlen = slug.Length
                        let sl = slug + s
                        let pvd = prevlen = sl.Length
                        eval remaining pvd sl
                    | _ ->                 
                        eval remaining prevdash slug
                | Skip ->
                    eval remaining prevdash slug
                                                                                                     
        if String.IsNullOrEmpty(s) then
            Chessie.Result<Slug,string>.Bad ["Title can't be null or empty"]
        else
            let result = eval s false ""
            Chessie.Result<Slug,string>.Ok (Slug(result) , [])
                 
                    
    let fromTitle t =
        fromString (Title.toString t) 

