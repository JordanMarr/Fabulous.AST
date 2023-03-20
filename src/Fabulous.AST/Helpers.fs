namespace Fabulous.AST

open Fabulous.AST.ScalarAttributeDefinitions
open Fabulous.AST.WidgetAttributeDefinitions
open Fabulous.AST.WidgetCollectionAttributeDefinitions

module Helpers =
    let createValueForWidget<'T> (widget: Widget) =
        let definition = WidgetDefinitionStore.get widget.Key
        let value = definition.CreateView widget
        unbox<'T> value
    
    let tryGetScalarValue (widget: Widget) (def: SimpleScalarAttributeDefinition<'data>) =
        match widget.ScalarAttributes with
        | ValueNone -> ValueNone
        | ValueSome scalarAttrs ->
            match Array.tryFind (fun (attr: ScalarAttribute) -> attr.Key = def.Key) scalarAttrs with
            | None -> ValueNone
            | Some attr -> ValueSome(unbox<'data> attr.Value)
            
    let getScalarValue (widget: Widget) (def: SimpleScalarAttributeDefinition<'data>) =
        match tryGetScalarValue widget def with
        | ValueNone -> failwith $"Could not find scalar attribute {def.Name} on widget {widget.DebugName}"
        | ValueSome value -> value
        
    let tryGetWidgetValue (widget: Widget) (def: WidgetAttributeDefinition) =
        match widget.WidgetAttributes with
        | ValueNone -> ValueNone
        | ValueSome widgetAttrs ->
            match Array.tryFind (fun (attr: WidgetAttribute) -> attr.Key = def.Key) widgetAttrs with
            | None -> ValueNone
            | Some attr -> ValueSome attr.Value
            
    let getWidgetValue (widget: Widget) (def: WidgetAttributeDefinition) =
        match tryGetWidgetValue widget def with
        | ValueNone -> failwith $"Could not find widget attribute {def.Name} on widget {widget.DebugName}"
        | ValueSome value -> value
        
    let tryGetWidgetCollectionValue (widget: Widget) (def: WidgetCollectionAttributeDefinition) =
        match widget.WidgetCollectionAttributes with
        | ValueNone -> ValueNone
        | ValueSome widgetCollectionAttrs ->
            match Array.tryFind (fun (attr: WidgetCollectionAttribute) -> attr.Key = def.Key) widgetCollectionAttrs with
            | None -> ValueNone
            | Some attr -> ValueSome attr.Value
        
    let getWidgetCollectionValue (widget: Widget) (def: WidgetCollectionAttributeDefinition) =
        match tryGetWidgetCollectionValue widget def with
        | ValueNone -> failwith $"Could not find widget collection attribute {def.Name} on widget {widget.DebugName}"
        | ValueSome value ->
            let struct (count, elements) = value
            
            elements
            |> Array.take (int count)
            |> Array.map createValueForWidget
            |> List.ofArray