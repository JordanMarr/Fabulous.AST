namespace Fabulous.AST

open System.Runtime.CompilerServices
open Fantomas.FCS.Text
open Fabulous.AST.StackAllocatedCollections
open Fabulous.AST.StackAllocatedCollections.StackList
open Fantomas.Core.SyntaxOak

module IfThenElse =
    let IfExpr = Attributes.defineWidget "IfExpr"
    let ThenExpr = Attributes.defineWidget "ThenExpr"
    let ElseExpr = Attributes.defineWidget "ElseExpr"

    let WidgetKey =
        Widgets.register "IfThenElse" (fun widget ->
            let ifExpr = Helpers.getNodeFromWidget<Expr> widget IfExpr
            let thenExpr = Helpers.getNodeFromWidget<Expr> widget ThenExpr
            let elseExpr = Helpers.getNodeFromWidget<Expr> widget ElseExpr

            ExprIfThenElseNode(
                IfKeywordNode.SingleWord(SingleTextNode.``if``),
                ifExpr,
                SingleTextNode.``then``,
                thenExpr,
                SingleTextNode.``else``,
                elseExpr,
                Range.Zero
            ))

[<AutoOpen>]
module IfThenElseBuilders =
    type Ast with

        static member inline IfThenElse
            (
                ifExpr: WidgetBuilder<Expr>,
                thenExpr: WidgetBuilder<Expr>,
                elseExpr: WidgetBuilder<Expr>
            ) =
            WidgetBuilder<ExprIfThenElseNode>(
                IfThenElse.WidgetKey,
                AttributesBundle(
                    StackList.empty(),
                    ValueSome
                        [| IfThenElse.IfExpr.WithValue(ifExpr.Compile())
                           IfThenElse.ThenExpr.WithValue(thenExpr.Compile())
                           IfThenElse.ElseExpr.WithValue(elseExpr.Compile()) |],
                    ValueNone
                )
            )

[<Extension>]
type IfThenElseYieldExtensions =
    [<Extension>]
    static member inline Yield
        (
            _: CollectionBuilder<'parent, ModuleDecl>,
            x: WidgetBuilder<ExprIfThenElseNode>
        ) : CollectionContent =
        let node = Gen.mkOak x
        let expIfThen = Expr.IfThenElse(node)
        let moduleDecl = ModuleDecl.DeclExpr expIfThen
        let widget = Ast.EscapeHatch(moduleDecl).Compile()
        { Widgets = MutStackArray1.One(widget) }
