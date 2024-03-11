namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fabulous.AST.StackAllocatedCollections
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak

module Expr =
    let Value = Attributes.defineWidget "Value"

    let WidgetKey =
        Widgets.register "Expr" (fun widget ->
            let value = Widgets.getNodeFromWidget<Constant> widget Value
            Expr.Constant(value))

    let WidgetNullKey =
        Widgets.register "ExprNull" (fun _ -> Expr.Null(SingleTextNode.``null``))

[<AutoOpen>]
module ExprBuilders =
    type Ast with
        static member ConstantExpr(value: WidgetBuilder<Constant>) =
            WidgetBuilder<Expr>(
                Expr.WidgetKey,
                AttributesBundle(StackList.empty(), ValueSome [| Expr.Value.WithValue(value.Compile()) |], ValueNone)
            )

        static member ConstantExpr(value: string, ?hasQuotes: bool) =
            match hasQuotes with
            | None
            | Some true ->
                WidgetBuilder<Expr>(
                    Expr.WidgetKey,
                    AttributesBundle(
                        StackList.empty(),
                        ValueSome [| Expr.Value.WithValue(Ast.Constant(value, true).Compile()) |],
                        ValueNone
                    )
                )
            | _ ->
                WidgetBuilder<Expr>(
                    Expr.WidgetKey,
                    AttributesBundle(
                        StackList.empty(),
                        ValueSome [| Expr.Value.WithValue(Ast.Constant(value, false).Compile()) |],
                        ValueNone
                    )
                )


        static member NullExpr() =
            WidgetBuilder<Expr>(Expr.WidgetNullKey, AttributesBundle(StackList.empty(), ValueNone, ValueNone))

[<Extension>]
type ExprYieldExtensions =
    [<Extension>]
    static member inline Yield(_: CollectionBuilder<'parent, Expr>, x: WidgetBuilder<Expr>) : CollectionContent =
        let node = Gen.mkOak x
        let widget = Ast.EscapeHatch(node).Compile()
        { Widgets = MutStackArray1.One(widget) }
