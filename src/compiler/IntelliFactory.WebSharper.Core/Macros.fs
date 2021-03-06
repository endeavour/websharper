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

module IntelliFactory.WebSharper.Core.Macros

module C = IntelliFactory.JavaScript.Core
module Q = IntelliFactory.WebSharper.Core.Quotations
module R = IntelliFactory.WebSharper.Core.Reflection
module S = IntelliFactory.JavaScript.Syntax

type Translator = Q.Expression -> C.Expression

type Body =
    | CoreBody of C.Expression
    | SyntaxBody of S.Expression

type Macro =
    {
        Body            : option<Body>
        Expand          : Translator -> Translator
        Requirements    : list<Metadata.Node>
    }

type IMacroDefinition =
    abstract member Macro : Macro
