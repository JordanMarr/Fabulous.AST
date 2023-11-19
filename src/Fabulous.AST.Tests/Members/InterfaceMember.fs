namespace Fabulous.AST.Tests.MethodDefinitions

open Fantomas.FCS.Text
open Fabulous.AST
open Fabulous.AST.Tests
open Fantomas.Core.SyntaxOak
open type Ast
open NUnit.Framework

module InterfaceMembers =
    [<Test>]
    let ``Produces a record with TypeParams and interface member`` () =
        AnonymousModule() {
            Interface("IMyInterface") {
                let parameters = [ CommonType.Unit ]
                AbstractCurriedMethodMember("GetValue", parameters, CommonType.String)
            }

            (GenericRecord("Colors", [ "'other" ]) {
                Field("Green", CommonType.String)
                Field("Blue", CommonType.mkType("'other"))
                Field("Yellow", CommonType.Int32)
            })
                .members() {
                let expr =
                    Expr.Constant(Constant.FromText(SingleTextNode("x.MyField2", Range.Zero)))

                InterfaceMember(CommonType.mkType("IMyInterface")) { MethodMember("x.GetValue") { EscapeHatch(expr) } }
            }
        }

        |> produces
            """
type IMyInterface =
    abstract member GetValue: unit -> string

type Colors<'other> =
    { Green: string
      Blue: 'other
      Yellow: int }

    interface IMyInterface with
        member x.GetValue() = x.MyField2

"""

    [<Test>]
    let ``Produces a record with interface member`` () =

        AnonymousModule() {
            Interface("IMyInterface") {
                let parameters = [ CommonType.Unit ]
                AbstractCurriedMethodMember("GetValue", parameters, CommonType.String)
            }

            (Record("MyRecord") {
                Field("MyField1", CommonType.Int32)
                Field("MyField2", CommonType.String)
            })
                .members() {
                let expr =
                    Expr.Constant(Constant.FromText(SingleTextNode("x.MyField2", Range.Zero)))

                InterfaceMember(CommonType.mkType("IMyInterface")) { MethodMember("x.GetValue") { EscapeHatch(expr) } }
            }
        }
        |> produces
            """

type IMyInterface =
    abstract member GetValue: unit -> string

type MyRecord =
    { MyField1: int
      MyField2: string }

    interface IMyInterface with
        member x.GetValue() = x.MyField2
"""

    [<Test>]
    let ``Produces a class with a interface member`` () =
        let expr = Expr.Constant(Constant.FromText(SingleTextNode("\"23\"", Range.Zero)))

        AnonymousModule() {
            Interface("Meh") { AbstractPropertyMember("Name", CommonType.String) }

            (Class("Person") {
                InterfaceMember(CommonType.mkType("Meh")) { PropertyMember("this.Name") { EscapeHatch(expr) } }
            })
                .implicitConstructorParameters([])
        }
        |> produces
            """
type Meh =
    abstract member Name: string

type Person () =
    interface Meh with
        member this.Name = "23"
"""