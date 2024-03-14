namespace Fabulous.AST.Tests.Expressions

open Xunit
open Fabulous.AST.Tests

open Fabulous.AST

open type Ast

module StructTuple =

    [<Fact>]
    let ``let value with a StructTuple expression``() =
        Oak() {
            AnonymousModule() {
                Value(
                    "x",
                    StructTupleExpr() {
                        ConstantExpr("1").hasQuotes(false)
                        ConstantExpr("2").hasQuotes(false)
                        ConstantExpr("3").hasQuotes(false)
                    }
                )
            }
        }
        |> produces
            """

let x = struct (1, 2, 3)
"""

    [<Fact>]
    let ``let value with a StructTuple expression with parenthesis``() =
        Oak() {
            AnonymousModule() {
                Value(
                    "x",
                    ParenExpr(
                        StructTupleExpr() {
                            ConstantExpr("1").hasQuotes(false)
                            ConstantExpr("2").hasQuotes(false)
                            ConstantExpr("3").hasQuotes(false)
                        }
                    )
                )
            }
        }
        |> produces
            """

let x = (struct (1, 2, 3))
"""
