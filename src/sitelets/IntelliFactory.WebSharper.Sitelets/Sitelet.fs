// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2014 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}

namespace IntelliFactory.WebSharper.Sitelets

open System
open System.Collections.Generic
open System.Web.UI

type Sitelet<'T when 'T : equality> =
    {
        Router : Router<'T>
        Controller : Controller<'T>
    }

    static member ( <|> ) (s1: Sitelet<'Action>, s2: Sitelet<'Action>) =
        {
            Router = s1.Router <|> s2.Router
            Controller =
                {
                    Handle = fun action ->
                        match s1.Router.Link action with
                        | Some _ -> s1.Controller.Handle action
                        | None -> s2.Controller.Handle action
                }
        }

/// Provides combinators over sitelets.
module Sitelet =
    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Reflection
    module C = Content
    type C<'T> = Content<'T>

    /// Creates an empty sitelet.
    let Empty<'Action when 'Action : equality> : Sitelet<'Action> =
        {
            Router = Router.New (fun _ -> None) (fun _ -> None)
            Controller =
                {
                    Handle = fun action ->
                        Content.CustomContent <| fun _ ->
                            {
                                Status = Http.Status.NotFound
                                Headers = []
                                WriteBody = ignore
                            }
                }
        }

    /// Represents filters for protecting sitelets.
    type Filter<'Action> =
        {
            VerifyUser : string -> bool;
            LoginRedirect : 'Action -> 'Action
        }

    /// Constructs a protected sitelet given the filter specification.
    let Protect (filter: Filter<'Action>) (site: Sitelet<'Action>)
        : Sitelet<'Action> =
        {
            Router = site.Router
            Controller =
                {
                    Handle = fun action ->
                        let prot = filter

                        let failure = Content.Redirect (prot.LoginRedirect action)

                        try
                          match UserSession.GetLoggedInUser () with
                          | Some user ->
                              if prot.VerifyUser user then
                                  site.Controller.Handle action
                              else
                                  failure
                          | None ->
                               failure
                        with :? NullReferenceException ->
                          // If server crashes or is restarted and doesn't have a hardcoded machine
                          // key then GetLoggedInUser() throws an exception. Log out in this case.
                          UserSession.Logout()
                          failure
                }
        }

    /// Constructs a singleton sitelet that contains exactly one action
    /// and serves a single content value at a given location.
    let Content (location: string) (action: 'Action) (cnt: Content<'Action>) =
        {
            Router = Router.Table [action, location]
            Controller = { Handle = fun _ -> cnt}
        }

    /// Maps over the sitelet action type. Requires a bijection.
    let Map (f: 'T1 -> 'T2) (g: 'T2 -> 'T1) (s: Sitelet<'T1>) : Sitelet<'T2> =
        {
            Router = Router.Map f g s.Router
            Controller =
                {
                    Handle = fun action ->
                        match s.Controller.Handle <| g action with
                        | Content.CustomContent genResp ->
                            CustomContent (genResp << Context.Map f)
                        | Content.CustomContentAsync genResp ->
                            CustomContentAsync (genResp << Context.Map f)
                        | Content.PageContent genPage ->
                            PageContent (genPage << Context.Map f)
                        | Content.PageContentAsync genPage ->
                            PageContentAsync (genPage << Context.Map f)
                }
        }

    /// Maps over the sitelet action type with only an injection.
    let Embed embed unembed sitelet =
        {
            Router = Router.TryMap (Some << embed) unembed sitelet.Router
            Controller =
                { Handle = fun a ->
                    match unembed a with
                    | Some ea -> C.CustomContent <| fun ctx ->
                        C.ToResponse (sitelet.Controller.Handle ea) (Context.Map embed ctx)
                    | None -> failwith "Invalid action in Sitelet.Embed" }
        }

    let tryGetEmbedFunctionsFromExpr (expr: Expr<'T1 -> 'T2>) =
        match expr with
        | ExprShape.ShapeLambda(_, Patterns.NewUnionCase (uci, _)) ->
            let embed (y: 'T1) = FSharpValue.MakeUnion(uci, [|box y|]) :?> 'T2
            let unembed (x: 'T2) =
                let uci', args' = FSharpValue.GetUnionFields(box x, uci.DeclaringType)
                if uci.Tag = uci'.Tag then
                    Some (args'.[0] :?> 'T1)
                else None
            Some (embed, unembed)
        | _ -> None
 
    /// Maps over the sitelet action type, where the destination type
    /// is a discriminated union with a case containing the source type.
    let EmbedInUnion (case: Expr<'T1 -> 'T2>) sitelet =
        match tryGetEmbedFunctionsFromExpr case with
        | Some (embed, unembed) -> Embed embed unembed sitelet
        | None -> failwith "Invalid union case in Sitelet.EmbedInUnion"

    /// Shifts all sitelet locations by a given prefix.
    let Shift (prefix: string) (sitelet: Sitelet<'T>) =
        {
            Router = Router.Shift prefix sitelet.Router
            Controller = sitelet.Controller
        }

    /// Combines several sitelets, leftmost taking precedence.
    /// Is equivalent to folding with the choice operator.
    let Sum (sitelets: seq<Sitelet<'T>>) : Sitelet<'T> =
        if Seq.isEmpty sitelets then Empty else
            Seq.reduce (<|>) sitelets

    /// Serves the sum of the given sitelets under a given prefix.
    /// This function is convenient for folder-like structures.
    let Folder<'T when 'T : equality> (prefix: string)
                                      (sitelets: seq<Sitelet<'T>>) =
        Shift prefix (Sum sitelets)

    /// Boxes the sitelet action type to Object type.
    let Upcast (sitelet: Sitelet<'T>) : Sitelet<obj> =
        Map box unbox sitelet

    /// Reverses the Upcast operation on the sitelet.
    let UnsafeDowncast<'T when 'T : equality> (sitelet: Sitelet<obj>) : Sitelet<'T> =
        Map unbox box sitelet

    /// Constructs a sitelet with an inferred router and a given controller
    /// function.
    let Infer<'T when 'T : equality> (handle : 'T -> Content<'T>) =
        {
            Router = Router.Infer()
            Controller = { Handle = handle }
        }

    let InferPartial (embed: 'T1 -> 'T2) (unembed: 'T2 -> 'T1 option) (mkContent: 'T1 -> Content<'T2>) : Sitelet<'T2> =
        {
            Router = Router.Infer() |> Router.TryMap (Some << embed) unembed
            Controller =
                { Handle = fun p ->
                    match unembed p with
                    | Some e -> mkContent e
                    | None -> failwith "Invalid action in Sitelet.InferPartial" }
        }

    let InferPartialInUnion (case: Expr<'T1 -> 'T2>) mkContent =
        match tryGetEmbedFunctionsFromExpr case with
        | Some (embed, unembed) -> InferPartial embed unembed mkContent
        | None -> failwith "Invalid union case in Sitelet.InferPartialInUnion"
