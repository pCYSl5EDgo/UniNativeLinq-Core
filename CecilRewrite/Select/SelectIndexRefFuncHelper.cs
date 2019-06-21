using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class SelectIndexRefFuncHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(SelectIndexRefFuncHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                SelectIndex(@static, type);
            }
        }

        private static void SelectIndex(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(SelectIndex), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            const string suffix = "00";
            var added = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            var @this = type.MakeGenericInstanceType(added);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters, suffix);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters, suffix);

            var T = new GenericParameter("T", method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);
            method.GenericParameters.Add(T);

            var TAction = MainModule.GetType(NameSpace, "DelegateRefFuncToSelectIndexStructOperator`2").MakeGenericInstanceType(new[]
            {
                Element,
                T,
            });

            var @return = MainModule.GetType(NameSpace, "SelectIndexEnumerable`5").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element,
                T,
                TAction,
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var Func = MainModule.GetType(NameSpace, "RefSelectIndex`2").MakeGenericInstanceType(new[]
            {
                Element,
                T,
            });

            var funcParam = new ParameterDefinition("func", ParameterAttributes.None, Func);
            method.Parameters.Add(funcParam);

            method.Body.Variables.Add(new VariableDefinition(TAction));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.LdLocaS(0);
            processor.Do(OpCodes.Ldarg_1);
            processor.Append(Instruction.Create(OpCodes.Stloc_0));
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
