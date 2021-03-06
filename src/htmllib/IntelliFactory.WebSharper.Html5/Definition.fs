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

namespace IntelliFactory.WebSharper.Html5

open IntelliFactory.WebSharper.InterfaceGenerator
open IntelliFactory.WebSharper.Dom
open IntelliFactory.WebSharper.EcmaScript

module Utils =
    let RenamedEnumStrings n f l =
        List.map (fun x -> (f x, sprintf "'%s'" x)) l
        |> List.toSeq
        |> Pattern.EnumInlines n

    let EnumInts n l = 
        List.map (fun (x, i) -> (x, sprintf "%d" i)) l
        |> List.toSeq
        |> Pattern.EnumInlines n

module Canvas =
    open Utils

    let GlobalCompositeOperation =
        let renamer n =
            match n with
            | "source-atop" -> "SourceATop"
            | "source-in" -> "SourceIn"
            | "source-out" -> "SourceOut"
            | "source-over" -> "SourceOver"
            | "destination-atop" -> "DestinationATop"
            | "destination-in" -> "DestinationIn"
            | "destination-out" -> "DestinationOut"
            | "destination-over" -> "DestinationOver"
            | x -> x
        
        RenamedEnumStrings "GlobalCompositeOperation" renamer [
            // A atop B. Display the source image wherever both images are opaque. Display the destination image wherever the destination image is opaque but the source image is transparent. Display transparency elsewhere.
            "source-atop"
            // A in B. Display the source image wherever both the source image and destination image are opaque. Display transparency elsewhere.
            "source-in"
            // A out B. Display the source image wherever the source image is opaque and the destination image is transparent. Display transparency elsewhere.
            "source-out"
            // A over B. Display the source image wherever the source image is opaque. Display the destination image elsewhere.
            "source-over"
            // B atop A. Same as source-atop but using the destination image instead of the source image and vice versa.
            "destination-atop"
            // B in A. Same as source-in but using the destination image instead of the source image and vice versa.
            "destination-in"
            // B out A. Same as source-out but using the destination image instead of the source image and vice versa.
            "destination-out"
            // B over A. Same as source-over but using the destination image instead of the source image and vice versa.
            "destination-over"
            // A plus B. Display the sum of the source image and destination image, with color values approaching 1 as a limit.
            "lighter"
            // A (B is ignored). Display the source image instead of the destination image.
            "copy"
            // A xor B. Exclusive OR of the source image and destination image.
            "xor"
        ]

    let CanvasGradient =
        Class "CanvasGradient"
        |+> Protocol [
            "addColorStop" => (T<float> * T<string>) ^-> T<unit>
        ]
    
    let Repetition = 
        let renamer n = 
            match n with
            | "repeat-x" -> "RepeatX"
            | "repeat-y" -> "RepeatY"
            | "no-repeat" -> "NoRepeat"
            | x -> x
        RenamedEnumStrings "Repetition" renamer [
            "repeat"
            "repeat-x"
            "repeat-y"
            "no-repeat"
        ]

    let CanvasPattern =
        Class "CanvasPattern"
    
    let LineCap = 
        Pattern.EnumStrings "LineCap" [
            "butt"
            "round"
            "square"
        ]
    
    let LineJoin = 
        Pattern.EnumStrings "LineJoin" [
            "round"
            "bevel"
            "miter"
        ]

    let TextAlign = 
        Pattern.EnumStrings "TextAlign" [
            "start"
            "end"
            "left"
            "right"
            "center"
        ]
    
    let TextBaseline = 
        Pattern.EnumStrings "TextBaseLine" [
            "top"
            "hanging"
            "middle"
            "alphabetic"
            "ideographic"
            "bottom"
        ]
    
    let TextMetrics = 
        Class "TextMetrics"
        |+> Protocol ["width" =@ T<float> ]
    
    let CanvasPixelArray = 
        Class "CanvasPixelArray"
        |+> Protocol [
            "length" =? T<int>
            "Get" => T<int> ^-> T<int>
            |> WithInline "$this[$0]"
            "Set" => T<int> * T<int> ^-> T<int>
            |> WithInline "$this[$0] = $1"
        ]

    let ImageData =
        Class "ImageData"
        |+> Protocol [
            "height" =? T<int> 
            "width" =? T<int> 
            "data" =? CanvasPixelArray
        ]
    
    let CanvasRenderingContext2D = 
        Class "CanvasRenderingContext2D"
        |+> Protocol [
            "canvas" =? T<Element> // FIXME
            // push state on state stack
            "save" => T<unit> ^-> T<unit>
            // pop state stack and restore state
            "restore" => T<unit> ^-> T<unit>
            "scale" => (T<float> * T<float>) ^-> T<unit>
            "rotate" => (T<float>) ^-> T<unit>
            "translate" => (T<float> * T<float>) ^-> T<unit>
            "transform" => (T<float> * T<float> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "setTransform" => (T<float> * T<float> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "globalAlpha" =@ T<float>
            "globalCompositeOperation" =@ GlobalCompositeOperation
            // (default black)
            "strokeStyle" =@ T<obj>
            // (default black)
            "fillStyle" =@ T<obj>
            "createLinearGradient" => (T<float> * T<float> * T<float> * T<float>) ^-> CanvasGradient
            "createRadialGradient" => (T<float> * T<float> * T<float> * T<float> * T<float> * T<float>) ^-> CanvasGradient
            "createPattern" => (T<Element> * Repetition) ^-> CanvasPattern
            // (default 1)
            "lineWidth" =@ T<float> 
            // "butt", "round", "square" (default "butt")
            "lineCap" =@ LineCap  
            // "round", "bevel", "miter" (default "miter")
            "lineJoin" =@ LineJoin
            // (default 10)            
            "miterLimit" =@ T<float>
            // (default 0)
            "shadowOffsetX" =@ T<float>
            // (default 0)
            "shadowOffsetY" =@ T<float>
            // (default 0)
            "shadowBlur" =@ T<float>
            // (default transparent black)
            "shadowColor" =@ T<string>

            // rects
            "clearRect" => (T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "fillRect" => (T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "strokeRect" => (T<float> * T<float> * T<float> * T<float>) ^-> T<unit>

            // path API
            "beginPath" => T<unit> ^-> T<unit>
            "closePath" => T<unit> ^-> T<unit>
            "moveTo" => (T<float> * T<float>) ^-> T<unit>
            "lineTo" => (T<float> * T<float>) ^-> T<unit>
            "quadraticCurveTo" => (T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "bezierCurveTo" => (T<float> * T<float> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "arcTo" => (T<float> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "rect" => (T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "arc" => (T<float> * T<float> * T<float> * T<float> * T<float> * T<bool>) ^-> T<unit>
            "fill" => T<unit> ^-> T<unit>
            "stroke" => T<unit> ^-> T<unit>
            "clip" => T<unit> ^-> T<unit>
            "isPointInPath" => (T<float> * T<float>) ^-> T<bool>

            // focus management
            "drawFocusRing" => (T<Element>?el * T<float>?x * T<float>?y * !? T<bool>) ^-> T<bool>

            // text
            // (default 10px sans-serif)
            "font" =@ T<string>
            "textAlign" =@ TextAlign
            "textBaseline" =@ TextBaseline

            "fillText" => T<string> * T<float> * T<float> ^-> T<unit>
            "fillText" => T<string> * T<float> * T<float> * T<float> ^-> T<unit>
            "strokeText" => T<string> * T<float> * T<float> ^-> T<unit>
            "strokeText" => T<string> * T<float> * T<float> * T<float> ^-> T<unit>
            "measureText" => T<string> ^-> TextMetrics

            // drawing images
            "drawImage" => (T<Element> * T<float> * T<float>) ^-> T<unit>
            "drawImage" => (T<Element> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
            "drawImage" => (T<Element> * T<float> * T<float> * T<float> * T<float> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>

            // pixel manipulation
            "createImageData" => (T<float> * T<float>) ^-> ImageData
            "createImageData" => (ImageData) ^-> ImageData
            "getImageData"    => (T<float> * T<float> * T<float> * T<float>) ^-> ImageData

            "putImageData" => (ImageData * T<float> * T<float> ) ^-> T<unit>
            "putImageData" => (ImageData * T<float> * T<float> * T<float> * T<float> * T<float> * T<float>) ^-> T<unit>
        ]

module AudioVideoCommon =
    
    let TimeRanges = 
        Class "TimeRanges"
        |+> Protocol [
            "length" =? T<int>
            "start" => (T<int>) ^-> T<int>
            "end" => T<int> ^-> T<int>
        ]

    let MediaError = 
        Utils.EnumInts "MediaError" [
            "MEDIA_ERR_ABORTED", 1
            "MEDIA_ERR_NETWORK", 2
            "MEDIA_ERR_DECODE", 3
            "MEDIA_ERR_SRC_NOT_SUPPORTED", 4
        ]
    
    let TrackType =
        Pattern.EnumStrings "TrackType" [
            "subtitles"
            "captions"
            "descriptions"
            "chapters"
            "metadata"
        ]
    
    let TimedTrack = Class "TimedTrack"

    let TimedTrackCue =
        Class "TimedTrackCue"
        |+> Protocol [
            "track" =? TimedTrack
            "id" => T<string>

            "startTime" =? T<float>
            "endTime" =? T<float>
            "pauseOnExit" =? T<bool>

            "direction" =? T<string>
            "snapToLines" =? T<bool>
            "linePosition" =? T<int>
            "textPosition" =? T<int>
            "size" =? T<int>
            "alignment" =? T<string>
            
            "voice" =? T<string>
            "getCueAsSource" => (T<unit>) ^-> T<string>
            "getCueAsHTML" => (T<unit>) ^-> T<obj> // FIXME DocumentFragment

            "onenter" => T<unit> ^-> T<unit>
            "onexit" => T<unit> ^-> T<unit>

        ]

    let TimedTrackCueList = 
        Class "TimedTrackCueList"
        |+> Protocol [
            "length" =? T<int>
            "getter" => (T<int>) ^-> TimedTrackCue
            "getCueById" => (T<string>) ^-> TimedTrackCue
        ]

    do  TimedTrack
        |+> Protocol [
            "kind" =? TrackType
            "label" =? T<string> 
            "language" =? T<string> 

            "NONE" =? T<int>
            "LOADING" =? T<int>
            "LOADED" =? T<int>
            "ERROR" =? T<int>

            "readyState" =? T<int>

            "onload" => T<unit> ^-> T<unit>
            "onerror" => T<unit> ^-> T<unit>

            "OFF" =? T<int>
            "HIDDEN" =? T<int>
            "SHOWING" =? T<int>

            "mode" =? T<int>

            "cues" =? TimedTrackCueList
            "activeCues" =? TimedTrackCueList

            "oncuechange" => T<int> ^-> T<int>
        ] |> ignore

    let MutableTimedTrack = 
        Class "MutableTimedTrack"
        |=> Inherits TimedTrack
        |+> Protocol [
            "addCue" => TimedTrackCue ^-> T<unit>
            "removeCue" => TimedTrackCue ^-> T<unit>
        ]

module Elements =
    open Canvas
    open AudioVideoCommon

    let CanvasElement =
        Class "CanvasElement"
        // |=> Inherits Type.Node
        |+> Protocol [
            "width" =@ T<int>
            "height" =@ T<int>
            "toDataURL" => !? T<string>?a * !? T<float>?b ^-> T<string>
            "getContext" => T<string> ^-> Canvas.CanvasRenderingContext2D
        ]

    let HTMLMediaElement =
        Class "HTMLMediaElement"
        // |=> Inherits Type.Node
        |+> Protocol [
            // error state
            "error" =?  MediaError

            // network state
            "src" =@ T<string>
            "currentSrc" =? T<string>

            "NETWORK_EMPTY" =? T<int>
            "NETWORK_IDLE" =? T<int>
            "NETWORK_LOADING" =? T<int>
            "NETWORK_NO_SOURCE" =? T<int>

            "networkState" =? T<int>

            "preload" =@ T<string>
            "buffered" =? TimeRanges

            "load" => T<unit> ^-> T<unit>
            "canPlayType" => T<string> ^-> T<unit>

            // ready state
            "HAVE_NOTHING" =? T<int>
            "HAVE_METADATA" =? T<int>
            "HAVE_CURRENT_DATA" =? T<int>
            "HAVE_FUTURE_DATA" =? T<int>
            "HAVE_ENOUGH_DATA" =? T<int>

            "readyState" =? T<int>
            "seeking" =? T<bool>

            // playback state
            "autoplay" =@ T<bool>
            "loop" =@ T<bool>
            "currentTime" =@ T<float>
            "defaultPlaybackRate" =@ T<float>
            "playbackRate" =@ T<float>

            "startOffsetTime" =? T<Date>
            "played" =? TimeRanges
            "seekable" =? TimeRanges
            "ended" =? T<bool>
            "paused" =? T<bool>
            "duration" =? T<float>
            "initialTime" =? T<float>

            "play" => T<unit> ^-> T<unit>
            "pause" => T<unit> ^-> T<unit>

            // controls
            "controls" =@ T<bool>
            "volume" =@ T<float>
            "muted" =@ T<bool>
            
            // timed tracks
            "tracks" =? Type.ArrayOf TimedTrack
            "addTrack" => (T<string> * TrackType * T<string>) ^-> MutableTimedTrack
        ]

    let HTMLVideoElement = 
        Class "HTMLVideoElement"
        |=> Inherits HTMLMediaElement
        |+> Protocol [
            "width" =@ T<string>
            "height" =@ T<string>
            "videoWidth" =? T<int>
            "videoHeight" =? T<int>
            "poster" =@ T<string>
        ]

    let HTMLAudioElement =
        Class "HTMLAudioElement"
        |=> Inherits HTMLMediaElement
        |+> Protocol [
            Constructor T<unit> |> WithInline "new Audio()"
            Constructor T<string> |> WithInline "new Audio($0)"
        ]

module Geolocation =
    
    let PositionOptions = 
        Pattern.Config "PositionOptions" {
            Required = []
            Optional = 
                ["enableHighAccuracy", T<bool>
                 "timeout", T<int>
                 "maximumAge", T<int>
                ]
        }

    let Coordinates =
        Class "Coordinates"
        |+> Protocol [
            "latitude" =? T<float>
            "longitude" =? T<float>
            "altitude" =? T<float>
            "accuracy" =? T<float>
            "altitudeAccuracy" =? T<float>
            "heading" =? T<float>
            "speed" =? T<float>        
        ]

    let Position = 
        Class "Position"
        |+> Protocol [
            "coords" =? Coordinates
            "timestamp" =? T<Date>
        ]

    let PositionError = 
        Class "PositionError"
        |+> Protocol [
            "UNKNOWN_ERROR" =? T<int>
            "PERMISSION_DENIED" =? T<int>
            "POSITION_UNAVAILABLE" =? T<int>
            "TIMEOUT" =? T<int>
            "code" =? T<int>
            "message" =? T<string>
        ]
         
    let Geolocation =
        let positionCallback = Position ^-> T<unit>
        let errorCallback = PositionError ^-> T<unit>
        
        Class "Geolocation"
        |+> Protocol [
            "getCurrentPosition" => (positionCallback?p * !? errorCallback?e * !? PositionOptions?o) ^-> T<unit>
            "watchPosition" => (positionCallback?p * !? errorCallback?e * !? PositionOptions?o) ^-> T<int>
            "clearWatch" => T<int> ^-> T<unit>
        ]

    let NavigatorGeolocation =
        Class "NavigatorGeolocation" 
        |+> Protocol ["geolocation" =? Geolocation]

module WebStorage =

    let Storage =
        Class "Storage"
        |+> Protocol [
                "length" =? T<int>
                "key" => T<int -> string>
                "getItem" => T<string->string>
                "setItem" => T<string>?key * T<string>?value ^-> T<unit>
                "removeItem" => T<string -> unit>
                "clear" => T<unit->unit>
            ]

    let StorageEvent =
        Class "StorageEvent"
        |+> Protocol [
                "key" =? T<string>
                "newValue" =? T<string>
                "oldValue" =? T<string>
                "storageArea" =? Storage
                "url" =? T<string>
            ]

module AppCache =
    let ApplicationCache =
        Class "ApplicationCache"
        // |=> Implements [T<EventTarget>]
        |+> Protocol [
            // update status
            "UNCACHED" =? T<int>
            "IDLE" =? T<int>
            "CHECKING" =? T<int>
            "DOWNLOADING" =? T<int>
            "UPDATEREADY" =? T<int>
            "OBSOLETE" =? T<int>
            "status" =? T<int>

              // updates
            "update" => T<unit> ^-> T<unit>
            "swapCache" => T<unit> ^-> T<unit>

            // events
            ///  Function onchecking;
            ///  Function onerror;
            ///  Function onnoupdate;
            ///  Function ondownloading;
            ///  Function onprogress;
            ///  Function onupdateready;
            ///  Function oncached;
            ///  Function onobsolete;
        ]

module WebWorkers =

    let WorkerNavigator = Class "WorkerNavigator"
    let MessagePortArray = Class "MessagePortArray"

    let WorkerUtils =
        Class "WorkerUtils"
        |+> [
            "importScripts" => (!+ T<string>) ^-> T<unit>
            "navigator" =? WorkerNavigator
        ]

    let WorkerLocation =
        Class "WorkerLocation"

    let WorkerGlobalScope = 
        let WorkerGlobalScope = Class "WorkerGlobalScope"
        WorkerGlobalScope
        // |=> Implements [T<EventTarget>; WorkerUtils]
        |+> Protocol [
            "self" =? WorkerGlobalScope
            "location" =? WorkerLocation
            "close" => T<unit> ^-> T<unit>
            // attribute Function onerror;
        ]
    
    let SharedWorkerScope =   
        Class "SharedWorkerGlobalScope"
            |=> Inherits WorkerGlobalScope
            |+> Protocol [
                "name" =? T<string>
                "applicationCache" =? AppCache.ApplicationCache
                //           attribute Function onconnect;
            ]

    let DedicatedWorkerGlobalScope =
        Class "DedicatedWorkerGlobalScope"
        |=> Inherits WorkerGlobalScope
        |+> Protocol [
            "postMessage" => (T<obj> * !? MessagePortArray) ^-> T<unit>
            /// attribute Function onmessage;
            
        ]
        
    let AbstractWorker =
        Class "AbstractWorker"
        // |=> Implements [T<EventTarget>]
        |+> Protocol [
            // attribute Function onerror;
            ]

    let Worker =           
        Class "Worker"
        |=> Inherits AbstractWorker
        |+> Protocol [
            "terminate" => T<unit> ^-> T<unit>
            "postMessage" => (T<obj> * !? MessagePortArray) ^-> T<unit>
            // attribute Function onmessage;
        ]

module General = 
    let BarProp =
        let BarProp = Class "BarProp"
        BarProp
        |+> Protocol [
            "visible" =@ T<bool>
        ]
    
    let History =
        let History = Class "History"
        History
        |+> Protocol [
            "length" =? T<int>
            "go" => T<unit> ^-> T<unit>
            "go" => T<int> ^-> T<unit>
            "back" => T<unit> ^-> T<unit>
            "forward" => T<unit> ^-> T<unit>
            "pushState" => T<obj> * T<string> ^-> T<unit>
            "pushState" => T<obj> * T<string> * T<string> ^-> T<unit>
            "replaceState" => T<obj> * T<string> ^-> T<unit>
            "replaceState" => T<obj> * T<string> * T<string> ^-> T<unit>
        ]

    let Location =
        Class "Location" 
        |+> Protocol [
            "href" =@ T<string>
            "assign" => T<string> ^-> T<unit> 
            "replace" => T<string> ^-> T<unit> 
            "reload" => T<unit> ^-> T<unit> 

                // URL decomposition IDL attributes 
            "protocol" =@ T<string>
            "host" =@ T<string>
            "hostname" =@ T<string>
            "port" =@ T<string>
            "pathname" =@ T<string>
            "search" =@ T<string>
            "hash" =@ T<string>

            "resolveURL" => T<string> ^-> T<unit>
        ]

    let UndoManager =
        Class "UndoManager"
        |+> Protocol [
            "length" =? T<int>
            "item" => T<int> ^-> T<obj>
            "position" =? T<int>
            "add" => T<obj> * T<string> ^-> T<unit>
            "remove" => T<int> ^-> T<unit>
            "clearUndo" => T<unit> ^-> T<unit> 
            "clearRedo" => T<unit> ^-> T<unit> 
        ]

    let WindowProxyType = Class "Window"
    let MessagePortType = Class "MessagePort"

    let MessageEvent =
        Class "MessageEvent"
        // |=> Implements [T<Event>]
        |+> Protocol [
            "data" =? T<obj>
            "origin" =? T<string>
            "lastEventId" =? T<string>
            "source" =? WindowProxyType
            "ports" =? Type.ArrayOf(MessagePortType)
            "initMessageEvent" => T<string> * T<bool> * T<bool> * T<obj> * T<string> * T<string> * WindowProxyType * Type.ArrayOf(MessagePortType) ^-> T<unit>
        ]

    let MessagePort =
        MessagePortType
        |+> Protocol [
            "postMessage" => T<obj> * Type.ArrayOf(MessagePortType) ^-> T<unit>
            "start" => T<unit> ^-> T<unit>
            "close" => T<unit> ^-> T<unit>
            "onmessage" =@ MessageEvent ^-> T<unit>
        ]

    let Window = 
        let f = T<Event> ^-> T<unit>
        WindowProxyType
        |+> [ "self" =? WindowProxyType |> WithGetterInline "window" ]  // Because it conflicts with the class name
        |+> Protocol [
            "history" =? History
            "document" =? T<Document>
            "name" =@ T<string>
            "location" =? Location
            "undoManager" =? UndoManager

            "locationbar" =? BarProp
            "menubar" =? BarProp
            "personalbar" =? BarProp
            "scrollbars" =? BarProp
            "statusbar" =? BarProp
            "toolbar" =? BarProp

            "frames" =? WindowProxyType
            "length" =? T<int>
            "top" =? WindowProxyType
            "opener" =? WindowProxyType
            "parent" =? WindowProxyType
            "frameElement" =? T<Element>
            "open" => (T<string> * T<string> * T<string> * T<string>) ^->  WindowProxyType
            "open" => (T<string> * T<string> * T<string>) ^->  WindowProxyType
            "open" => (T<string> * T<string>) ^->  WindowProxyType
            "open" => (T<string>) ^->  WindowProxyType
            "open" => (T<unit>) ^->  WindowProxyType

            "navigator" =? Geolocation.NavigatorGeolocation
            "applicationCache" =? AppCache.ApplicationCache
            "localStorage" =? WebStorage.Storage
            "sessionStorage" =? WebStorage.Storage
            "alert" => T<string> ^-> T<unit>
            "confirm" => T<string> ^-> T<bool>
            "prompt" => T<string> ^-> T<string>
            "prompt" => T<string> * T<string> ^-> T<string>
            "print" => T<unit> ^-> T<unit>
            "showModalDialog" => T<string> * T<obj> ^-> T<bool>
            "showModalDialog" => T<string> ^-> T<bool>

            "postMessage" => T<string> * T<string> * Type.ArrayOf(MessagePort) ^-> T<unit> 
            "postMessage" => T<string> * T<string> ^-> T<unit> 

            "onabort" =@ f
            "onafterprint" =@ f
            "onbeforeprint" =@ f
            "onbeforeunload" =@ f
            "onblur" =@ f
            "oncanplay" =@ f
            "oncanplaythrough" =@ f
            "onchange" =@ f
            "onclick" =@ f
            "oncontextmenu" =@ f

            "oncuechange" =@ f

            "ondblclick" =@ f
            "ondrag" =@ f
            "ondragend" =@ f
            "ondragenter" =@ f
            "ondragleave" =@ f
            "ondragover" =@ f
            "ondragstart" =@ f
            "ondrop" =@ f
            "ondurationchange" =@ f
            "onemptied" =@ f
            "onended" =@ f
            "onerror" =@ f
            "onfocus" =@ f
            "onformchange" =@ f
            "onforminput" =@ f
            "onhashchange" =@ f
            "oninput" =@ f
            "oninvalid" =@ f
            "onkeydown" =@ f
            "onkeypress" =@ f
            "onkeyup" =@ f
            "onload" =@ f
            "onloadeddata" =@ f
            "onloadedmetadata" =@ f
            "onloadstart" =@ f
            "onmessage" =@ f
            "onmousedown" =@ f
            "onmousemove" =@ f
            "onmouseout" =@ f
            "onmouseover" =@ f
            "onmouseup" =@ f
            "onmousewheel" =@ f
            "onoffline" =@ f
            "ononline" =@ f
            "onpause" =@ f
            "onplay" =@ f
            "onplaying" =@ f
            "onpagehide" =@ f
            "onpageshow" =@ f
            "onpopstate" =@ f
            "onprogress" =@ f
            "onratechange" =@ f
            "onreadystatechange" =@ f
            "onredo" =@ f
            "onreset" =@ f
            "onresize" =@ f
            "onscroll" =@ f
            "onseeked" =@ f
            "onseeking" =@ f
            "onselect" =@ f
            "onshow" =@ f
            "onstalled" =@ f
            "onstorage" =@ WebStorage.StorageEvent ^-> T<unit>
            "onsubmit" =@ f
            "onsuspend" =@ f
            "ontimeupdate" =@ f
            "onundo" =@ f
            "onunload" =@ f
            "onvolumechange" =@ f
            "onwaiting" =@ f
        ]

module TypedArrays =


    let ArrayBuffer =
        let ArrayBuffer = Type.New()
        Class "ArrayBuffer"
        |=> ArrayBuffer
        |+> Protocol [
                "byteLength" =? T<int>
                /// Warning: although part of the spec, may not work in IE10 as of 6/6/2013.
                "slice" => T<int> * T<int> ^-> ArrayBuffer
            ]
        |+> [ Constructor T<int> ]

    module DataView =

        let private Getter<'T> name =
            name => T<int>?byteOffset * !? T<bool>?littleEndian ^-> T<'T>

        let private Setter<'T> name =
            name => T<int>?byteOffset * T<'T>?value * !? T<bool>?littleEndian ^-> T<unit>

        let Class =
            Class "DataView"
            |+> [
                    Constructor ArrayBuffer
                    Constructor (ArrayBuffer * T<int>?byteOffset)
                    Constructor (ArrayBuffer * T<int>?byteOffset * T<int>?byteLength)
                ]
            |+> Protocol [
                    "getInt8" => T<int>?byteOffset ^-> T<sbyte>
                    "getUint8" => T<int>?byteOffset ^-> T<byte>
                    Getter<int16> "getInt16"
                    Getter<uint16> "getUint16"
                    Getter<int32> "getInt32"
                    Getter<uint32> "getUint32"
                    Getter<float32> "getFloat32"
                    Getter<double> "getFloat64"
                    "setInt8" => T<int>?byteOffset * T<sbyte>?value ^-> T<unit>
                    "setUint8" => T<int>?byteOffset * T<byte>?value ^-> T<unit>
                    Setter<int16> "setInt16"
                    Setter<uint16> "setUint16"
                    Setter<int32> "setInt32"
                    Setter<uint32> "setUint32"
                    Setter<float32> "setFloat32"
                    Setter<double> "setFloat64"
                ]

    let ArrayBufferView =
        Class "ArrayBufferView"
        |+> Protocol
            [
                "buffer" =? ArrayBuffer
                "byteOffset" =? T<int>
                "byteLength" =? T<int>
            ]

    let private MakeTypedArray typedArray (elementType: Type.Type) =
        let self = Type.New()
        Class typedArray
        |=> self
        |=> Inherits ArrayBufferView
        |+> [
                Constructor T<unit>
                Constructor T<int>
                Constructor self
                Constructor (Type.ArrayOf elementType)
                Constructor (ArrayBuffer?buffer * !? T<int>?byteOffset * !? T<int>?length)
                "BYTES_PER_ELEMENT" =? T<int>
            ]
        |+> Protocol [
                "length" =? T<int>
                "get" =>
                    T<int>?offset ^-> elementType
                    |> WithInline "$this[$offset]"
                "set" =>
                    T<int>?offset * elementType?value ^-> T<unit>
                    |> WithInline "void($this[$offset]=$value)"
                "set" => self?array * !? T<int>?offset ^-> T<unit>
                "set" => (Type.ArrayOf elementType)?array * !? T<int>?offset ^-> T<unit>
                "subarray" => T<int64>?``begin`` * T<int>?``end`` ^-> self
            ]

    let Int8Array = MakeTypedArray "Int8Array" T<sbyte>
    let Uint8Array = MakeTypedArray "Uint8Array" T<byte>
    let Uint8ClampedArray = MakeTypedArray "Uint8ClampedArray" T<byte>
    let Int16Array = MakeTypedArray "Int16Array" T<int16>
    let Uint16Array = MakeTypedArray "Uint16Array" T<uint16>
    let Int32Array = MakeTypedArray "Int32Array" T<int32>
    let Uint32Array = MakeTypedArray "Uint32Array" T<uint32>
    let Float32Array = MakeTypedArray "Float32Array" T<float32>
    let Float64Array = MakeTypedArray "Float64Array" T<double>

module File =

    let BlobPropertyBag =
        Pattern.Config "BlobPropertyBag" {
            Required = []
            Optional =
                [
                    "type", T<string>
                ]
        }

    let Blob =
        let Blob = Type.New()
        Class "Blob"
        |=> Blob
        |+> [
                Constructor T<unit>
                Constructor ((Type.ArrayOf TypedArrays.ArrayBuffer + Type.ArrayOf TypedArrays.ArrayBufferView + Blob + T<string>) * !?BlobPropertyBag)
            ]
        |+> Protocol [
                "size" =? T<int>
                "type" =? T<string>
                "slice" => T<int>?start * T<int>?``end`` * T<string>?contentType ^-> Blob
                "close" => T<unit> ^-> T<unit>
            ]

    let File =
        Class "File"
        |=> Inherits Blob
        |+> Protocol [
                "name" =? T<string>
                "lastModifiedDate" =? T<Date>
            ]

    let ProgressEvent =
        Class "ProgressEvent"
        |=> Inherits T<Event>
        |+> Protocol [
                "lengthComputable" =? T<bool>
                "loaded" =? T<int>
                "total" =? T<int>
            ]

    let FileList =
        let FileList = Type.New()
        Class "FileList"
        |=> FileList
        |+> Protocol [
                "item" => T<int> ^-> File
                "length" =? T<int>
            ]
        |+> [
                "ofElement" => T<Element>?input ^-> FileList
                |> WithInline "$input.files"
                "ofEvent" => ProgressEvent?event ^-> FileList
                |> WithInline "$event.target.files"
            ]

    let FileReaderReadyState =
        Pattern.EnumInlines "FileReaderReadyState" [
            "Empty", "0"
            "Loading", "1"
            "Done", "2"
        ]

    let FileReader =
        let EventListener = (ProgressEvent + T<unit>) ^-> T<unit>
        Generic / fun t ->
        Class "FileReader"
        |=> Inherits T<EventTarget>
        |+> Protocol [
                "abort" => T<unit> ^-> T<unit>
                "readyState" =? FileReaderReadyState
                "result" =? t
                "error" =? T<Error>
                "onloadstart" =@ ProgressEvent ^-> T<unit>
                "onprogress" =@ ProgressEvent ^-> T<unit>
                "onload" =@ ProgressEvent ^-> T<unit>
                "onabort" =@ ProgressEvent ^-> T<unit>
                "onerror" =@ ProgressEvent ^-> T<unit>
                "onloadend" =@ ProgressEvent ^-> T<unit>
        ]

    let TextFileReader =
        Class "TextFileReader"
        |=> Inherits (FileReader T<string>)
        |+> [
                Constructor T<unit>
                |> WithInline "new FileReader()"
            ]
        |+> Protocol [
                "readAsText" => Blob * !?T<string>?encoding ^-> T<unit>
                "readAsDataURL" => Blob ^-> T<unit>
            ]

    let BinaryFileReader =
        Class "BinaryFileReader"
        |=> Inherits (FileReader TypedArrays.ArrayBuffer)
        |+> [
                Constructor T<unit>
                |> WithInline "new FileReader()"
            ]
        |+> Protocol [
                "readAsArrayBuffer" => Blob ^-> T<unit>
            ]

module WebGL =

    let RenderingContext = Type.New()
    let ContextAttributes = Type.New()
    let Object = Type.New()
    let Buffer = Type.New()
    let Framebuffer = Type.New()
    let Program = Type.New()
    let Renderbuffer = Type.New()
    let Shader = Type.New()
    let Texture = Type.New()
    let UniformLocation = Type.New()
    let ActiveInfo = Type.New()
    let Enum = Type.New()
    let DataView = Type.New()

    let EnumClass =
        Class "Enum"
        |=> Enum

    let DataViewClass =
        Class "DataView"
        |=> DataView
        |=> Inherits TypedArrays.ArrayBufferView
        |+> [
                Constructor (TypedArrays.ArrayBuffer * T<int> * T<int>)
                Constructor (TypedArrays.ArrayBuffer * T<int>)
                Constructor TypedArrays.ArrayBuffer
            ]
        |+> Protocol
            [
                "buffer" =? TypedArrays.ArrayBuffer
                "byteOffset" =? T<uint64>
                "byteLength" =? T<uint64>
                "getInt8" => T<int> ^-> T<int>
                "getUint8" => T<int> ^-> T<int>
                "getInt16" => T<int> * T<bool> ^-> T<int>
                "getInt16" => T<int> ^-> T<int>
                "getUint16" => T<int> * T<bool> ^-> T<int>
                "getUint16" => T<int> ^-> T<int>
                "getInt32" => T<int> * T<bool> ^-> T<int>
                "getInt32" => T<int> ^-> T<int>
                "getUint32" => T<int> * T<bool> ^-> T<int>
                "getUint32" => T<int> ^-> T<int>
                "getFloat32" => T<int> * T<bool> ^-> T<int>
                "getFloat32" => T<int> ^-> T<int>
                "getFloat64" => T<int> * T<bool> ^-> T<int>
                "getFloat64" => T<int> ^-> T<int>
                "setInt8" => T<int> * T<int> ^-> T<int>
                "setUint8" => T<int> * T<int> ^-> T<int>
                "setInt16" => T<int> * T<int> * T<bool> ^-> T<int>
                "setInt16" => T<int> * T<int> ^-> T<int>
                "setUint16" => T<int> * T<int> * T<bool> ^-> T<int>
                "setUint16" => T<int> * T<int> ^-> T<int>
                "setInt32" => T<int> * T<int> * T<bool> ^-> T<int>
                "setInt32" => T<int> * T<int> ^-> T<int>
                "setUint32" => T<int> * T<int> * T<bool> ^-> T<int>
                "setUint32" => T<int> * T<int> ^-> T<int>
                "setFloat32" => T<int> * T<int> * T<bool> ^-> T<int>
                "setFloat32" => T<int> * T<int> ^-> T<int>
                "setFloat64" => T<int> * T<float> * T<bool> ^-> T<int>
                "setFloat64" => T<int> * T<float> ^-> T<int>
            ]

    let RenderingContextClass =
        Class "WebGLRenderingContext"
        |=> RenderingContext
        |+> Protocol
            [
                // GLEnum constants
                "DEPTH_BUFFER_BIT" =? T<int>
                "STENCIL_BUFFER_BIT" =? T<int>
                "COLOR_BUFFER_BIT" =? T<int>
                "POINTS" =? Enum
                "LINES" =? Enum
                "LINE_LOOP" =? Enum
                "LINE_STRIP" =? Enum
                "TRIANGLES" =? Enum
                "TRIANGLE_STRIP" =? Enum
                "TRIANGLE_FAN" =? Enum
                "ZERO" =? Enum
                "ONE" =? Enum
                "SRC_COLOR" =? Enum
                "ONE_MINUS_SRC_COLOR" =? Enum
                "SRC_ALPHA" =? Enum
                "ONE_MINUS_SRC_ALPHA" =? Enum
                "DST_ALPHA" =? Enum
                "ONE_MINUS_DST_ALPHA" =? Enum
                "DST_COLOR" =? Enum
                "ONE_MINUS_DST_COLOR" =? Enum
                "SRC_ALPHA_SATURATE" =? Enum
                "FUNC_ADD" =? Enum
                "BLEND_EQUATION" =? Enum
                "BLEND_EQUATION_RGB" =? Enum
                "BLEND_EQUATION_ALPHA" =? Enum
                "FUNC_SUBSTRACT" =? Enum
                "FUNC_REVERSE_SUBSTRACT" =? Enum
                "BLEND_DST_RGB" =? Enum
                "BLEND_SRC_RGB" =? Enum
                "BLEND_DST_ALPHA" =? Enum
                "BLEND_SRC_ALPHA" =? Enum
                "CONSTANT_COLOR" =? Enum
                "ONE_MINUS_CONSTANT_COLOR" =? Enum
                "CONSTANT_ALPHA" =? Enum
                "ONE_MINUS_CONSTANT_ALPHA" =? Enum
                "BLEND_COLOR" =? Enum
                "ARRAY_BUFFER" =? Enum
                "ELEMENT_ARRAY_BUFFER" =? Enum
                "ARRAY_BUFFER_BINDING" =? Enum
                "ELEMENT_ARRAY_BUFFER_BINDING" =? Enum
                "STREAM_DRAW" =? Enum
                "STATIC_DRAW" =? Enum
                "DYNAMIC_DRAW" =? Enum
                "BUFFER_SIZE" =? Enum
                "BUFFER_USAGE" =? Enum
                "CURRENT_VERTEX_ATTRIB" =? Enum
                "FRONT" =? Enum
                "BACK" =? Enum
                "FRONT_AND_BACK" =? Enum
                "CULL_FACE" =? Enum
                "BLEND" =? Enum
                "DITHER" =? Enum
                "STENCIL_TEST" =? Enum
                "DEPTH_TEST" =? Enum
                "SCISSOR_TEST" =? Enum
                "POLYGON_OFFSET_FILL" =? Enum
                "SAMPLE_ALPHA_TO_COVERAGE" =? Enum
                "SAMPLE_COVERAGE" =? Enum
                "NO_ERROR" =? Enum
                "INVALID_ENUM" =? Enum
                "INVALID_VALUE" =? Enum
                "INVALID_OPERATION" =? Enum
                "OUT_OF_MEMORY" =? Enum
                "CW" =? Enum
                "CCW" =? Enum
                "LINE_WIDTH" =? Enum
                "ALIASED_POINT_SIZE_RANGE" =? Enum
                "ALIASED_LINE_WIDTH_RANGE" =? Enum
                "CULL_FACE_MODE" =? Enum
                "FRONT_FACE" =? Enum
                "DEPTH_RANGE" =? Enum
                "DEPTH_WRITEMASK" =? Enum
                "DEPTH_CLEAR_VALUE" =? Enum
                "DEPTH_FUNC" =? Enum
                "STENCIL_CLEAR_VALUE" =? Enum
                "STENCIL_FUNC" =? Enum
                "STENCIL_FAIL" =? Enum
                "STENCIL_PASS_DEPTH_FAIL" =? Enum
                "STENCIL_PASS_DEPTH_PASS" =? Enum
                "STENCIL_REF" =? Enum
                "STENCIL_VALUE_MASK" =? Enum
                "STENCIL_WRITEMASK" =? Enum
                "STENCIL_BACK_FUNC" =? Enum
                "STENCIL_BACK_FAIL" =? Enum
                "STENCIL_BACK_PASS_DEPTH_FAIL" =? Enum
                "STENCIL_BACK_PASS_DEPTH_PASS" =? Enum
                "STENCIL_BACK_REF" =? Enum
                "STENCIL_BACK_VALUE_MASK" =? Enum
                "STENCIL_BACK_WRITEMASK" =? Enum
                "VIEWPORT" =? Enum
                "SCISSOR_BOX" =? Enum
                "COLOR_CLEAR_VALUE" =? Enum
                "COLOR_WRITEMASK" =? Enum
                "UNPACK_ALIGNMENT" =? Enum
                "PACK_ALIGNMENT" =? Enum
                "MAX_TEXTURE_SIZE" =? Enum
                "MAX_VIEWPORT_DIMS" =? Enum
                "SUBPIXEL_BITS" =? Enum
                "RED_BITS" =? Enum
                "GREEN_BITS" =? Enum
                "BLUE_BITS" =? Enum
                "ALPHA_BITS" =? Enum
                "DEPTH_BITS" =? Enum
                "STENCIL_BITS" =? Enum
                "POLYGON_OFFSET_UNITS" =? Enum
                "POLYGON_OFFSET_FACTOR" =? Enum
                "TEXTURE_BINDING_2D" =? Enum
                "SAMPLE_BUFFERS" =? Enum
                "SAMPLES" =? Enum
                "SAMPLE_COVERAGE_VALUE" =? Enum
                "SAMPLE_COVERAGE_INVERT" =? Enum
                "NUM_COMPRESSED_TEXTURE_FORMATS" =? Enum
                "COMPRESSED_TEXTURE_FORMATS" =? Enum
                "DONT_CARE" =? Enum
                "FASTEST" =? Enum
                "NICEST" =? Enum
                "GENERATE_MIPMAP_HINT" =? Enum
                "BYTE" =? Enum
                "UNSIGNED_BYTE" =? Enum
                "SHORT" =? Enum
                "UNSIGNED_SHORT" =? Enum
                "INT" =? Enum
                "UNSIGNED_INT" =? Enum
                "FLOAT" =? Enum
                "DEPTH_COMPONENT" =? Enum
                "ALPHA" =? Enum
                "RGB" =? Enum
                "RGBA" =? Enum
                "LUMINANCE" =? Enum
                "LUMINANCE_ALPHA" =? Enum
                "UNSIGNED_SHORT_4_4_4_4" =? Enum
                "UNSIGNED_SHORT_5_5_5_1" =? Enum
                "UNSIGNED_SHORT_5_6_5" =? Enum
                "FRAGMENT_SHADER" =? Enum
                "VERTEX_SHADER" =? Enum
                "MAX_VERTEX_ATTRIBS" =? Enum
                "MAX_VERTEX_UNIFORM_VECTORS" =? Enum
                "MAX_VARYING_VECTORS" =? Enum
                "MAX_COMBINED_TEXTURE_IMAGE_UNITS" =? Enum
                "MAX_VERTEX_TEXTURE_IMAGE_UNITS" =? Enum
                "MAX_TEXTURE_IMAGE_UNITS" =? Enum
                "MAX_FRAGMENT_UNIFORM_VECTORS" =? Enum
                "SHADER_TYPE" =? Enum
                "DELETE_STATUS" =? Enum
                "LINK_STATUS" =? Enum
                "VALIDATE_STATUS" =? Enum
                "ATTACHED_SHADERS" =? Enum
                "ACTIVE_UNIFORMS" =? Enum
                "ACTIVE_ATTRIBUTES" =? Enum
                "SHADING_LANGUAGE_VERSION" =? Enum
                "CURRENT_PROGRAM" =? Enum
                "NEVER" =? Enum
                "LESS" =? Enum
                "EQUAL" =? Enum
                "LEQUAL" =? Enum
                "GREATER" =? Enum
                "NOTEQUAL" =? Enum
                "GEQUAL" =? Enum
                "ALWAYS" =? Enum
                "KEEP" =? Enum
                "REPLACE" =? Enum
                "INCR" =? Enum
                "DECR" =? Enum
                "INVERT" =? Enum
                "INCR_WRAP" =? Enum
                "DECR_WRAP" =? Enum
                "VENDOR" =? Enum
                "RENDERER" =? Enum
                "VERSION" =? Enum
                "NEAREST" =? Enum
                "LINEAR" =? Enum
                "NEAREST_MIPMAP_NEAREST" =? Enum
                "LINEAR_MIPMAP_NEAREST" =? Enum
                "NEAREST_MIPMAP_LINEAR" =? Enum
                "LINEAR_MIPMAP_LINEAR" =? Enum
                "TEXTURE_MAG_FILTER" =? Enum
                "TEXTURE_MIN_FILTER" =? Enum
                "TEXTURE_WRAP_S" =? Enum
                "TEXTURE_WRAP_T" =? Enum
                "TEXTURE_2D" =? Enum
                "TEXTURE" =? Enum
                "TEXTURE_CUBE_MAP" =? Enum
                "TEXTURE_BINDING_CUBE_MAP" =? Enum
                "TEXTURE_CUBE_MAP_POSITIVE_X" =? Enum
                "TEXTURE_CUBE_MAP_NEGATIVE_X" =? Enum
                "TEXTURE_CUBE_MAP_POSITIVE_Y" =? Enum
                "TEXTURE_CUBE_MAP_NEGATIVE_Y" =? Enum
                "TEXTURE_CUBE_MAP_POSITIVE_Z" =? Enum
                "TEXTURE_CUBE_MAP_NEGATIVE_Z" =? Enum
                "MAX_CUBE_MAP_TEXTURE_SIZE" =? Enum
                "TEXTURE0" =? Enum
                "TEXTURE1" =? Enum
                "TEXTURE2" =? Enum
                "TEXTURE3" =? Enum
                "TEXTURE4" =? Enum
                "TEXTURE5" =? Enum
                "TEXTURE6" =? Enum
                "TEXTURE7" =? Enum
                "TEXTURE8" =? Enum
                "TEXTURE9" =? Enum
                "TEXTURE10" =? Enum
                "TEXTURE11" =? Enum
                "TEXTURE12" =? Enum
                "TEXTURE13" =? Enum
                "TEXTURE14" =? Enum
                "TEXTURE15" =? Enum
                "TEXTURE16" =? Enum
                "TEXTURE17" =? Enum
                "TEXTURE18" =? Enum
                "TEXTURE19" =? Enum
                "TEXTURE20" =? Enum
                "TEXTURE21" =? Enum
                "TEXTURE22" =? Enum
                "TEXTURE23" =? Enum
                "TEXTURE24" =? Enum
                "TEXTURE25" =? Enum
                "TEXTURE26" =? Enum
                "TEXTURE27" =? Enum
                "TEXTURE28" =? Enum
                "TEXTURE29" =? Enum
                "TEXTURE30" =? Enum
                "TEXTURE31" =? Enum
                "ACTIVE_TEXTURE" =? Enum
                "REPEAT" =? Enum
                "CLAMP_TO_EDGE" =? Enum
                "MIRRORED_REPEAT" =? Enum
                "FLOAT_VEC2" =? Enum
                "FLOAT_VEC3" =? Enum
                "FLOAT_VEC4" =? Enum
                "INT_VEC2" =? Enum
                "INT_VEC3" =? Enum
                "INT_VEC4" =? Enum
                "BOOL" =? Enum
                "BOOL_VEC2" =? Enum
                "BOOL_VEC3" =? Enum
                "BOOL_VEC4" =? Enum
                "FLOAT_MAT2" =? Enum
                "FLOAT_MAT3" =? Enum
                "FLOAT_MAT4" =? Enum
                "SAMPLER_2D" =? Enum
                "SAMPLER_CUBE" =? Enum
                "VERTEX_ATTRIB_ARRAY_ENABLED" =? Enum
                "VERTEX_ATTRIB_ARRAY_SIZE" =? Enum
                "VERTEX_ATTRIB_ARRAY_STRIDE" =? Enum
                "VERTEX_ATTRIB_ARRAY_TYPE" =? Enum
                "VERTEX_ATTRIB_ARRAY_NORMALIZED" =? Enum
                "VERTEX_ATTRIB_ARRAY_POINTER" =? Enum
                "VERTEX_ATTRIB_ARRAY_BUFFER_BINDING" =? Enum
                "COMPILE_STATUS" =? Enum
                "LOW_FLOAT" =? Enum
                "MEDIUM_FLOAT" =? Enum
                "HIGH_FLOAT" =? Enum
                "LOW_INT" =? Enum
                "MEDIUM_INT" =? Enum
                "HIGH_INT" =? Enum
                "FRAMEBUFFER" =? Enum
                "RENDERBUFFER" =? Enum
                "RGBA4" =? Enum
                "RGB5_A1" =? Enum
                "RGB565" =? Enum
                "DEPTH_COMPONENT16" =? Enum
                "STENCIL_INDEX" =? Enum
                "STENCIL_INDEX8" =? Enum
                "DEPTH_STENCIL" =? Enum
                "RENDERBUFFER_WIDTH" =? Enum
                "RENDERBUFFER_HEIGHT" =? Enum
                "RENDERBUFFER_INTERNAL_FORMAT" =? Enum
                "RENDERBUFFER_RED_SIZE" =? Enum
                "RENDERBUFFER_GREEN_SIZE" =? Enum
                "RENDERBUFFER_BLUE_SIZE" =? Enum
                "RENDERBUFFER_ALPHA_SIZE" =? Enum
                "RENDERBUFFER_DEPTH_SIZE" =? Enum
                "RENDERBUFFER_STENCIL_SIZE" =? Enum
                "FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE" =? Enum
                "FRAMEBUFFER_ATTACHMENT_OBJECT_NAME" =? Enum
                "FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL" =? Enum
                "FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE" =? Enum
                "COLOR_ATTACHMENT0" =? Enum
                "DEPTH_ATTACHMENT" =? Enum
                "STENCIL_ATTACHMENT" =? Enum
                "DEPTH_STENCIL_ATTACHMENT" =? Enum
                "NONE" =? Enum
                "FRAMEBUFFER_COMPLETE" =? Enum
                "FRAMEBUFFER_INCOMPLETE_ATTACHMENT" =? Enum
                "FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT" =? Enum
                "FRAMEBUFFER_INCOMPLETE_DIMENSIONS" =? Enum
                "FRAMEBUFFER_UNSUPPORTED" =? Enum
                "FRAMEBUFFER_BINDING" =? Enum
                "RENDERBUFFER_BINDING" =? Enum
                "MAX_RENDERBUFFER_SIZE" =? Enum
                "INVALID_FRAMEBUFFER_OPERATION" =? Enum

                // WebGL-specific GLEnums
                "UNPACK_FLIP_Y_WEBGL" =? Enum
                "UNPACK_PREMATURELY_ALPHA_WEBGL" =? Enum
                "CONTEXT_LOST_WEBGL" =? Enum
                "UNPACK_COLORSPACE_CONVERSION_WEBGL" =? Enum
                "BROWSER_DEFAULT_WEBGL" =? Enum

                "canvas" =? T<Element>
                "drawingBufferWidth" =? T<int>
                "drawingBufferHeight" =? T<int>

                "getContextAttributes" => T<unit> ^-> ContextAttributes
                "isContextLost" => T<unit> ^-> T<bool>
                "getSupportedExtensions" => T<unit> ^-> (Type.ArrayOf T<string>)
                "getExtension" => T<string>?name ^-> T<obj>
                "activeTexture" => Enum?texture ^-> T<unit>
                "attachShader" => Program?program * Shader?shader ^-> T<unit>
                "bindAttribLocation" => Program?program * T<int>?index * T<string>?name ^-> T<unit>
                "bindBuffer" => Enum?target * Buffer?buffer ^-> T<unit>
                "bindFramebuffer" => Enum?target * Framebuffer?framebuffer ^-> T<unit>
                "bindRenderbuffer" => Enum?target * Renderbuffer?renderbuffer ^-> T<unit>
                "bindTexture" => Enum?target * Texture?texture ^-> T<unit>
                "blendColor" => T<float>?red * T<float>?green * T<float>?blue * T<float>?alpha ^-> T<unit>
                "blendEquation" => Enum?mode ^-> T<unit>
                "blendEquationSeparate" => Enum?modeRGB * Enum?modeAlpha ^-> T<unit>
                "blendFunc" => Enum?sfactor * Enum?dfactor ^-> T<unit>
                "blendFuncSeparate" => Enum?srcRGB * Enum?dstRGB * Enum?srcAlpha * Enum?dstAlpha ^-> T<unit>
                "bufferData" => Enum?target * T<int>?size * Enum?usage ^-> T<unit>
                "bufferData" => Enum?target * TypedArrays.ArrayBufferView?data * Enum?usage ^-> T<unit>
                "bufferData" => Enum?target * TypedArrays.ArrayBuffer?data * Enum?usage ^-> T<unit>
                "bufferSubData" => Enum?target * T<int>?offset * TypedArrays.ArrayBufferView?data ^-> T<unit>
                "bufferSubData" => Enum?target * T<int>?offset * TypedArrays.ArrayBuffer?data ^-> T<unit>
                "checkFramebufferStatus" => Enum?target ^-> Enum
                "clear" => T<int>?mask ^-> T<unit>
                "clearColor" => T<float>?red * T<float>?green * T<float>?blue * T<float>?alpha ^-> T<unit>
                "clearDepth" => T<float>?depth ^-> T<unit>
                "clearStencil" => T<int>?s ^-> T<unit>
                "colorMask" => T<bool>?red * T<bool>?green * T<bool>?blue * T<bool>?alpha ^-> T<unit>
                "compileShader" => Shader?shader ^-> T<unit>
                "copyTexImage2D" => Enum?target * T<int>?level * Enum?internalformat * T<int>?x * T<int>?y * T<int>?width * T<int>?height * T<int>?border ^-> T<unit>
                "copyTexSubImage2D" => Enum?target * T<int>?level * T<int>?xoffset * T<int>?yoffset * T<int>?x * T<int>?y * T<int>?width * T<int>?height ^-> T<unit>
                "createBuffer" => T<unit> ^-> Buffer
                "createFramebuffer" => T<unit> ^-> Framebuffer
                "createProgram" => T<unit> ^-> Program
                "createRenderbuffer" => T<unit> ^-> Renderbuffer
                "createShader" => Enum?typ ^-> Shader
                "createTexture" => T<unit> ^-> Texture
                "cullFace" => Enum?mode ^-> T<unit>
                "deleteBuffer" => Buffer?buffer ^-> T<unit>
                "deleteFramebuffer" => Framebuffer?framebuffer ^-> T<unit>
                "deleteProgram" => Program?program ^-> T<unit>
                "deleteRenderbuffer" => Renderbuffer?renderbuffer ^-> T<unit>
                "deleteShader" => Shader?shader ^-> T<unit>
                "deleteTexture" => Texture?texture ^-> T<unit>
                "depthFunc" => Enum?func ^-> T<unit>
                "depthMask" => T<bool>?flag ^-> T<unit>
                "depthRange" => T<float>?zNear * T<float>?zFar ^-> T<unit>
                "detachShader" => Program?program * Shader?shader ^-> T<unit>
                "disable" => Enum?cap ^-> T<unit>
                "disableVertexAttribArray" => T<int>?index ^-> T<unit>
                "drawArrays" => Enum?mode * T<int>?first * T<int>?count ^-> T<unit>
                "drawElements" => Enum?mode * T<int>?count * Enum?typ * T<int>?offset ^-> T<unit>
                "enable" => Enum?cap ^-> T<unit>
                "enableVertexAttribArray" => T<int>?index ^-> T<unit>
                "finish" => T<unit> ^-> T<unit>
                "flush" => T<unit> ^-> T<unit>
                "framebufferRenderbuffer" => Enum?target * Enum?attachment * Enum?renderbuffertarget * Renderbuffer?renderbuffer ^-> T<unit>
                "framebufferTexture2D" => Enum?target * Enum?attachment * Enum?textarget * Texture?texture * T<int>?level ^-> T<unit>
                "frontFace" => Enum?mode ^-> T<unit>
                "generateMipmap" => Enum?target ^-> T<unit>
                "getActiveAttrib" => Program?program * T<int>?index ^-> ActiveInfo
                "getActiveUniform" => Program?program * T<int>?index ^-> ActiveInfo
                "getAttachedShaders" => Program?program ^-> (Type.ArrayOf Shader)
                "getAttribLocation" => Program?program * T<string>?name ^-> T<int>
                "getParameter" => Enum?pname ^-> T<obj>
                "getBufferParameter" => Enum?target * Enum?pname ^-> T<obj>
                "getError" => T<unit> ^-> Enum
                "getFramebufferAttachmentParameter" => Enum?target * Enum?attachment * Enum?pname ^-> T<obj>
                "getProgramParameter" => Program?program * Enum?pname ^-> T<obj>
                "getProgramInfoLog" => Program?program ^-> T<string>
                "getRenderbufferParameter" => Enum?target * Enum?pname ^-> T<obj>
                "getShaderParameter" => Shader?shader * Enum?pname ^-> T<obj>
                "getShaderInfoLog" => Shader?shader ^-> T<string>
                "getShaderSource" => Shader?shader ^-> T<string>
                "getTexParameter" => Enum?target * Enum?pname ^-> T<obj>
                "getUniform" => Program?program * UniformLocation?location ^-> T<obj>
                "getUniformLocation" => Program?program * T<string>?name ^-> UniformLocation
                "getVertexAttrib" => T<int>?index * Enum?pname ^-> T<obj>
                "getVertexAttribOffset" => T<int>?index * Enum?pname ^-> T<int>
                "hint" => Enum?target * Enum?mode ^-> T<unit>
                "isBuffer" => Buffer?buffer ^-> T<bool>
                "isEnabled" => Enum?cap ^-> T<bool>
                "isFramebuffer" => Framebuffer?framebuffer ^-> T<bool>
                "isProgram" => Program?program ^-> T<bool>
                "isRenderbuffer" => Renderbuffer?renderbuffer ^-> T<bool>
                "isShader" => Shader?shader ^-> T<bool>
                "isTexture" => Texture?texture ^-> T<bool>
                "lineWidth" => T<float>?width ^-> T<unit>
                "linkProgram" => Program?program ^-> T<unit>
                "pixelStorei" => Enum?pname * T<int>?param ^-> T<unit>
                "polygonOffset" => T<float>?factor * T<float>?units ^-> T<unit>
                "readPixels" => T<int>?x * T<int>?y * T<int>?width * T<int>?height * Enum?format * Enum?typ * TypedArrays.ArrayBufferView?pixels ^-> T<unit>
                "renderbufferStorage" => Enum?target * Enum?internalformat * T<int>?width * T<int>?height ^-> T<unit>
                "sampleCoverage" => T<float>?value * T<bool>?invert ^-> T<unit>
                "scissor" => T<int>?x * T<int>?y * T<int>?width * T<int>?height ^-> T<unit>
                "shaderSource" => Shader?shader * T<string>?source ^-> T<unit>
                "stencilFunc" => Enum?func * T<int>?ref * T<int>?mask ^-> T<unit>
                "stencilFuncSeparate" => Enum?face * Enum?func * T<int>?ref * T<int>?mask ^-> T<unit>
                "stencilMask" => T<int>?mask ^-> T<unit>
                "stencilMaskSeparate" => Enum?face * T<int>?mask ^-> T<unit>
                "stencilOp" => Enum?fail * Enum?zfail * Enum?zpass ^-> T<unit>
                "stencilOpSeparate" => Enum?face * Enum?fail * Enum?zfail * Enum?zpass ^-> T<unit>
                "texImage2D" => Enum?target * T<int>?level * Enum?internalformat * T<int>?width * T<int>?height * T<int>?border * Enum?format * Enum?typ * TypedArrays.ArrayBufferView?pixels ^-> T<unit>
                "texImage2D" => Enum?target * T<int>?level * Enum?internalformat * Enum?format * Enum?typ * Canvas.ImageData?pixels ^-> T<unit>
                "texImage2D" => Enum?target * T<int>?level * Enum?internalformat * Enum?format * Enum?typ * T<Element>?image ^-> T<unit>
                "texParameterf" => Enum?target * Enum?pname * T<float>?param ^-> T<unit>
                "texParameteri" => Enum?target * Enum?pname * T<int>?param ^-> T<unit>
                "texParameteri" => Enum?target * Enum?pname * Enum?param ^-> T<unit>
                "texSubImage2D" => Enum?target * T<int>?level * T<int>?xoffset * T<int>?yoffset * T<int>?width * T<int>?height * Enum?format * Enum?typ * TypedArrays.ArrayBufferView?pixels ^-> T<unit>
                "texSubImage2D" => Enum?target * T<int>?level * T<int>?xoffset * T<int>?yoffset * Enum?format * Enum?typ * Canvas.ImageData?pixels ^-> T<unit>
                "texSubImage2D" => Enum?target * T<int>?level * T<int>?xoffset * T<int>?yoffset * Enum?format * Enum?typ * T<Element>?image ^-> T<unit>
                "uniform1f" => UniformLocation?location * T<float>?x ^-> T<unit>
                "uniform1fv" => UniformLocation?location * TypedArrays.Float32Array?v ^-> T<unit>
                "uniform1fv" => UniformLocation?location * (Type.ArrayOf T<float>)?v ^-> T<unit>
                "uniform1i" => UniformLocation?location * T<int>?x ^-> T<unit>
                "uniform1iv" => UniformLocation?location * TypedArrays.Int32Array?v ^-> T<unit>
                "uniform1iv" => UniformLocation?location * (Type.ArrayOf T<int>)?v ^-> T<unit>
                "uniform2f" => UniformLocation?location * T<float>?x * T<float>?y ^-> T<unit>
                "uniform2fv" => UniformLocation?location * TypedArrays.Float32Array?v ^-> T<unit>
                "uniform2fv" => UniformLocation?location * (Type.ArrayOf T<float>)?v ^-> T<unit>
                "uniform2i" => UniformLocation?location * T<int>?x * T<int>?y ^-> T<unit>
                "uniform2iv" => UniformLocation?location * TypedArrays.Int32Array?v ^-> T<unit>
                "uniform2iv" => UniformLocation?location * (Type.ArrayOf T<int>)?v ^-> T<unit>
                "uniform3f" => UniformLocation?location * T<float>?x * T<float>?y * T<float>?z ^-> T<unit>
                "uniform3fv" => UniformLocation?location * TypedArrays.Float32Array?v ^-> T<unit>
                "uniform3fv" => UniformLocation?location * (Type.ArrayOf T<float>)?v ^-> T<unit>
                "uniform3i" => UniformLocation?location * T<int>?x * T<int>?y * T<int>?z ^-> T<unit>
                "uniform3iv" => UniformLocation?location * TypedArrays.Int32Array?v ^-> T<unit>
                "uniform3iv" => UniformLocation?location * (Type.ArrayOf T<int>)?v ^-> T<unit>
                "uniform4f" => UniformLocation?location * T<float>?x * T<float>?y * T<float>?z * T<float>?w ^-> T<unit>
                "uniform4fv" => UniformLocation?location * TypedArrays.Float32Array?v ^-> T<unit>
                "uniform4fv" => UniformLocation?location * (Type.ArrayOf T<float>)?v ^-> T<unit>
                "uniform4i" => UniformLocation?location * T<int>?x * T<int>?y * T<int>?z * T<int>?w ^-> T<unit>
                "uniform4iv" => UniformLocation?location * TypedArrays.Int32Array?v ^-> T<unit>
                "uniform4iv" => UniformLocation?location * (Type.ArrayOf T<int>)?v ^-> T<unit>
                "uniformMatrix2fv" => UniformLocation?location * T<bool>?transpose * TypedArrays.Float32Array?value ^-> T<unit>
                "uniformMatrix2fv" => UniformLocation?location * T<bool>?transpose * (Type.ArrayOf T<float>)?value ^-> T<unit>
                "uniformMatrix3fv" => UniformLocation?location * T<bool>?transpose * TypedArrays.Float32Array?value ^-> T<unit>
                "uniformMatrix3fv" => UniformLocation?location * T<bool>?transpose * (Type.ArrayOf T<float>)?value ^-> T<unit>
                "uniformMatrix4fv" => UniformLocation?location * T<bool>?transpose * TypedArrays.Float32Array?value ^-> T<unit>
                "uniformMatrix4fv" => UniformLocation?location * T<bool>?transpose * (Type.ArrayOf T<float>)?value ^-> T<unit>
                "useProgram" => Program?program ^-> T<unit>
                "validateProgram" => Program?program ^-> T<unit>
                "vertexAttrib1f" => T<int>?indx * T<float>?x ^-> T<unit>
                "vertexAttrib1fv" => T<int>?indx * TypedArrays.Float32Array?values ^-> T<unit>
                "vertexAttrib1fv" => T<int>?indx * (Type.ArrayOf T<float>)?values ^-> T<unit>
                "vertexAttrib2f" => T<int>?indx * T<float>?x * T<float>?y ^-> T<unit>
                "vertexAttrib2fv" => T<int>?indx * TypedArrays.Float32Array?values ^-> T<unit>
                "vertexAttrib2fv" => T<int>?indx * (Type.ArrayOf T<float>)?values ^-> T<unit>
                "vertexAttrib3f" => T<int>?indx * T<float>?x * T<float>?y * T<float>?z ^-> T<unit>
                "vertexAttrib3fv" => T<int>?indx * TypedArrays.Float32Array?values ^-> T<unit>
                "vertexAttrib3fv" => T<int>?indx * (Type.ArrayOf T<float>)?values ^-> T<unit>
                "vertexAttrib4f" => T<int>?indx * T<float>?x * T<float>?y * T<float>?z * T<float>?w ^-> T<unit>
                "vertexAttrib4fv" => T<int>?indx * TypedArrays.Float32Array?values ^-> T<unit>
                "vertexAttrib4fv" => T<int>?indx * (Type.ArrayOf T<float>)?values ^-> T<unit>
                "vertexAttribPointer" => T<int>?indx * T<int>?size * Enum?typ * T<bool>?normalized * T<int>?stride * T<int>?offset ^-> T<unit>
                "viewport" => T<int>?x * T<int>?y * T<int>?width * T<int>?height ^-> T<unit>
            ]

    let ContextAttributesClass =
        Pattern.Config "WebGLContextAttributes" {
            Required = []
            Optional =
                [
                    "alpha", T<bool>
                    "depth", T<bool>
                    "stencil", T<bool>
                    "antialias", T<bool>
                    "premultipliedAlpha", T<bool>
                    "preserveDrawingBuffer", T<bool>
                ]
        }
        |=> ContextAttributes

    let ObjectClass =
        Interface "WebGLObject"
        |=> Object

    let BufferClass =
        Interface "WebGLBuffer"
        |=> Buffer

    let FramebufferClass =
        Interface "WebGLFramebuffer"
        |=> Framebuffer

    let ProgramClass =
        Interface "WebGLProgram"
        |=> Program

    let RenderbufferClass =
        Interface "WebGLRenderbuffer"
        |=> Renderbuffer

    let ShaderClass =
        Interface "WebGLShader"
        |=> Shader

    let TextureClass =
        Interface "WebGLTexture"
        |=> Texture

    let UniformLocationClass =
        Interface "WebGLUniformLocation"
        |=> UniformLocation

    let ActiveInfoClass =
        Class "WebGLActiveInfo"
        |=> ActiveInfo
        |+> Protocol
            [
                "size" =? T<int>
                "type" =? Enum
                "name" =? T<string>
            ]

module WebSockets =

    let WebSocketReadyState =
        Pattern.EnumInlines "WebSocketReadyState" [
            "Connecting", "0"
            "Open", "1"
            "Closing", "2"
            "Closed", "3"
        ]

    let WebSocket =
        Class "WebSocket"
        |+> Protocol
            [
                "readyState" =? WebSocketReadyState
                "bufferedAmount" =? T<int>
                "onopen" =@ T<unit->unit>
                "onclose" =@ T<unit->unit>
                "onerror" =@ T<unit->unit>
                "extensions" =? T<string>
                "protocol" =? T<string>
                "close" => T<unit->unit>
                "close" => T<int> ^-> T<unit>
                "close" => T<int> * T<string> ^-> T<unit>
                "onmessage" =@ (General.MessageEvent ^-> T<unit>)
                "binaryType" =@ T<string>
                "send" => T<string->unit>
                "send" => TypedArrays.ArrayBuffer ^-> T<unit>
                (* TODO: describe FILE Api: http://dev.w3.org/2006/webapi/FileAPI *)
                // "send" => Blob ^-> T<unit>
            ]
        |+> [
                Constructor T<string>
                Constructor (T<string> * T<string[]>)
            ]

module Definition =

    let Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.Html5" [
                AudioVideoCommon.MediaError
                AudioVideoCommon.MutableTimedTrack
                AudioVideoCommon.TimeRanges
                AudioVideoCommon.TimedTrack
                AudioVideoCommon.TimedTrackCue
                AudioVideoCommon.TimedTrackCueList
                AudioVideoCommon.TrackType
                AppCache.ApplicationCache
                Canvas.CanvasGradient
                Canvas.CanvasPattern
                Canvas.CanvasPixelArray
                Canvas.CanvasRenderingContext2D
                Canvas.GlobalCompositeOperation
                Canvas.ImageData
                Canvas.LineCap
                Canvas.LineJoin
                Canvas.Repetition
                Canvas.TextAlign
                Canvas.TextBaseline
                Canvas.TextMetrics
                Elements.CanvasElement
                Elements.HTMLAudioElement
                Elements.HTMLVideoElement
                Elements.HTMLMediaElement
                File.BinaryFileReader
                File.Blob
                File.BlobPropertyBag
                File.File
                File.FileList
                Generic - File.FileReader
                File.FileReaderReadyState
                File.ProgressEvent
                File.TextFileReader
                Geolocation.Coordinates
                Geolocation.Geolocation
                Geolocation.NavigatorGeolocation
                Geolocation.Position
                Geolocation.PositionError
                Geolocation.PositionOptions
                General.BarProp
                General.History
                General.Location
                General.MessageEvent
                General.MessagePort
                General.UndoManager
                General.Window
                TypedArrays.DataView.Class
                TypedArrays.ArrayBuffer
                TypedArrays.ArrayBufferView
                TypedArrays.Float32Array
                TypedArrays.Float64Array
                TypedArrays.Int16Array
                TypedArrays.Int32Array
                TypedArrays.Int8Array
                TypedArrays.Uint16Array
                TypedArrays.Uint32Array
                TypedArrays.Uint8Array
                TypedArrays.Uint8ClampedArray
                WebSockets.WebSocket
                WebSockets.WebSocketReadyState
                WebStorage.Storage
                WebStorage.StorageEvent
            ]
            Namespace "IntelliFactory.WebSharper.Html5.WebGL" [
                WebGL.RenderingContextClass
                WebGL.ContextAttributesClass
                WebGL.ObjectClass
                WebGL.BufferClass
                WebGL.FramebufferClass
                WebGL.ProgramClass
                WebGL.RenderbufferClass
                WebGL.ShaderClass
                WebGL.TextureClass
                WebGL.UniformLocationClass
                WebGL.ActiveInfoClass
                WebGL.EnumClass
                WebGL.DataViewClass
            ]
        ]

[<Sealed>]
type Html5Extension() =
    interface IExtension with
        member x.Assembly = Definition.Assembly

[<assembly: Extension(typeof<Html5Extension>)>]
do ()
