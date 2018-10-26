namespace Lambda.Cms

open System.Collections.Generic
open System

module Ioc =
    let dic = Dictionary<Type,obj>()

    let register<'T>(o: obj) =
        dic.Add(typedefof<'T>, o)

    let resolve<'T>() =
        dic.[typedefof<'T>] :?> 'T