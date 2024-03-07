namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fantomas.FCS.Text
open Fabulous.AST.StackAllocatedCollections
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak

module IfThen =
    let IfExpr = Attributes.defineWidget "IfExpr"
    let ThenExpr = Attributes.defineWidget "ThenExpr"

    let WidgetIfThenKey =
        Widgets.register "IfThen" (fun widget ->
            let ifExpr = Helpers.getNodeFromWidget<Expr> widget IfExpr
            let thenExpr = Helpers.getNodeFromWidget<Expr> widget ThenExpr

            ExprIfThenNode(
                IfKeywordNode.SingleWord(SingleTextNode.``if``),
                ifExpr,
                SingleTextNode.``then``,
                thenExpr,
                Range.Zero
            ))

    let WidgetElIfThenKey =
        Widgets.register "ElIfThen" (fun widget ->
            let ifExpr = Helpers.getNodeFromWidget<Expr> widget IfExpr
            let thenExpr = Helpers.getNodeFromWidget<Expr> widget ThenExpr

            ExprIfThenNode(
                IfKeywordNode.SingleWord(SingleTextNode.``elif``),
                ifExpr,
                SingleTextNode.``then``,
                thenExpr,
                Range.Zero
            ))

    let WidgetElseIfThenKey =
        Widgets.register "ElIfThen" (fun widget ->
            let elseIfExpr = Helpers.getNodeFromWidget<Expr> widget IfExpr
            let thenExpr = Helpers.getNodeFromWidget<Expr> widget ThenExpr

            ExprIfThenNode(
                IfKeywordNode.ElseIf(ElseIfNode(Range.Zero, Range.Zero, Unchecked.defaultof<Node>, Range.Zero)),
                elseIfExpr,
                SingleTextNode.``then``,
                thenExpr,
                Range.Zero
            ))

[<AutoOpen>]
module IfThenBuilders =
    type Ast with

        static member inline IfThen(ifExpr: WidgetBuilder<Expr>, thenExpr: WidgetBuilder<Expr>) =
            WidgetBuilder<ExprIfThenNode>(
                IfThen.WidgetIfThenKey,
                AttributesBundle(
                    StackList.empty(),
                    ValueSome
                        [| IfThen.IfExpr.WithValue(ifExpr.Compile())
                           IfThen.ThenExpr.WithValue(thenExpr.Compile()) |],
                    ValueNone
                )
            )

        static member inline ElIfThen(elIfExpr: WidgetBuilder<Expr>, thenExpr: WidgetBuilder<Expr>) =
            WidgetBuilder<ExprIfThenNode>(
                IfThen.WidgetElIfThenKey,
                AttributesBundle(
                    StackList.empty(),
                    ValueSome
                        [| IfThen.IfExpr.WithValue(elIfExpr.Compile())
                           IfThen.ThenExpr.WithValue(thenExpr.Compile()) |],
                    ValueNone
                )
            )

        static member inline ElseIfThen(elseIfExpr: WidgetBuilder<Expr>, thenExpr: WidgetBuilder<Expr>) =
            WidgetBuilder<ExprIfThenNode>(
                IfThen.WidgetElseIfThenKey,
                AttributesBundle(
                    StackList.empty(),
                    ValueSome
                        [| IfThen.IfExpr.WithValue(elseIfExpr.Compile())
                           IfThen.ThenExpr.WithValue(thenExpr.Compile()) |],
                    ValueNone
                )
            )

[<Extension>]
type IfThenYieldExtensions =
    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<'parent, ModuleDecl>,
            x: WidgetBuilder<ExprIfThenNode>
        ) : CollectionContent =
        let node = Gen.ast x
        let expIfThen = Expr.IfThen(node)
        let moduleDecl = ModuleDecl.DeclExpr expIfThen
        let widget = Ast.EscapeHatch(moduleDecl).Compile()
        { Widgets = MutStackArray1.One(widget) }
