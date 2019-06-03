using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming

namespace CecilRewrite
{
    using static Program;
    static class TryGetLastHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetLastHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                TryGetLast(@static, type);
        }

        private static void TryGetLast(TypeDefinition @static, TypeDefinition type)
        {
            if (type.Methods.Any(x => x.Name == nameof(TryGetLast)))
                return;
            var MainModule = @static.Module;
            var Boolean = MainModule.TypeSystem.Boolean;
            var method = new MethodDefinition(nameof(TryGetLast), StaticMethodAttributes, Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(argumentsFromTypeToMethodParam);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);

            ParameterDefinition value;
            value = new ParameterDefinition(nameof(value), ParameterAttributes.Out, Element.MakeByReferenceType());
            method.Parameters.Add(value);

            var body = method.Body;
            var variables = body.Variables;
            var processor = body.GetILProcessor();

            MethodReference LongCount;
            LongCount = @this.FindMethod(nameof(LongCount), Helper.NoParameter);

            if (!(type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "FastCountAttribute") is null))
            {
                variables.Add(new VariableDefinition(MainModule.TypeSystem.Int64));

                var il0013 = Instruction.Create(OpCodes.Ldarg_1);

                processor.Do(OpCodes.Ldarg_0);
                processor.Call(LongCount);
                processor.Do(OpCodes.Stloc_0);
                processor.Do(OpCodes.Ldloc_0);
                processor.True(il0013);
                processor.Do(OpCodes.Ldarg_1);
                processor.InitObj(Element);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il0013);
                processor.Do(OpCodes.Ldarg_0);
                processor.Do(OpCodes.Ldloc_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Sub);
                MethodReference get_Item;
                get_Item = @this.FindMethod(nameof(get_Item));
                processor.Call(get_Item);
                if (get_Item.ReturnType.IsByReference)
                    processor.LdObj(Element);
                processor.StObj(Element);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Ret();
            }
            else
            {
                variables.Add(new VariableDefinition(Enumerator));
                variables.Add(new VariableDefinition(Element));

                var il001A = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
                var il002E = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

                processor.Do(OpCodes.Ldarg_0);
                processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
                processor.Do(OpCodes.Stloc_0);
                processor.LdLocaS(0);
                processor.Do(OpCodes.Ldarg_1);
                MethodReference TryMoveNext;
                TryMoveNext = Enumerator.FindMethod(nameof(TryMoveNext));
                processor.Call(TryMoveNext);
                processor.True(il001A);
                processor.LdLocaS(0);
                MethodReference Dispose;
                Dispose = Enumerator.FindMethod(nameof(Dispose), Helper.NoParameter);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il001A);
                processor.LdLocaS(1);
                processor.Call(TryMoveNext);
                processor.False(il002E);
                processor.Do(OpCodes.Ldarg_1);
                processor.Do(OpCodes.Ldloc_1);
                processor.StObj(Element);
                processor.Jump(il001A);
                processor.Append(il002E);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Ret();
            }
            @static.Methods.Add(method);
        }
    }
}
