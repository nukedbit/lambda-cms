namespace Lambda.Cms
open System
open System.Text
open System.Text.RegularExpressions

module Domain =

    module EmailType = 
        type Email = Email of string

        let create s = 
            let r = Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
            if r.IsMatch(s) then
                Result.Ok (Email s)
            else 
                Result.Error "Invalid email address"

    module CategoryIdType = 
        type CategoryId = CategoryId of Guid
        let create g =
            CategoryId g

    module DocumentIdType =
        type DocumentId = DocumentId of Guid
        let create g =
            DocumentId g

    module UserIdType =
        type UserId = UserId of Guid
        let create g =
            UserId g

    module MediaIdType = 
        type MediaId = MediaId of Guid
        let create g = 
            MediaId g
    
    module SlugType = 
        type Slug = Slug of string

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
                       Result.Ok (Slug(sb.ToString().Substring(0, sb.Length - 1)))
                    else 
                        Result.Ok (Slug(sb.ToString()))
        let fromSlug s = 
            fromTitle s


    type Version = Version of int
   
    type Media = 
        {
            Id : MediaIdType.MediaId
            MimeType : string
            Name : string
            Slug : SlugType.Slug            
        }

    type DocumentStatus = 
        | Draft
        | Published
        | Deleted

    type User =
        {
            UserId : UserIdType.UserId   
            Email : EmailType.Email
            Slug : SlugType.Slug
        }

    type Category = 
        {
            Id: CategoryIdType.CategoryId
            Title : string
            Slug : SlugType.Slug
            ParentId : CategoryIdType.CategoryId option
        }

    type Document = 
        {
            Id: DocumentIdType.DocumentId
            Title : string
            Content : string
            Category : Category
            Version : Version
            Status : DocumentStatus
            Owner : User
            Medias : Media list
        }