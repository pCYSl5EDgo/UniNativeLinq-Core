using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class TryGetElementAtHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(TryGetElementAtHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                TryGetElementAt(@static, type);
        }

        private static void TryGetElementAt(TypeDefinition @static, TypeDefinition type)
        {
            var MainModule = @static.Module;
            var Boolean = MainModule.TypeSystem.Boolean;
            var method = new MethodDefinition(nameof(TryGetElementAt), StaticMethodAttributes, Boolean)
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

            ParameterDefinition index;
            index = new ParameterDefinition(nameof(index), ParameterAttributes.None, MainModule.TypeSystem.Int64);
            method.Parameters.Add(index);

            ParameterDefinition value;
            value = new ParameterDefinition(nameof(value), ParameterAttributes.Out, Element.MakeByReferenceType());
            method.Parameters.Add(value);

            var body = method.Body;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(Enumerator));
            variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));
            variables.Add(new VariableDefinition(MainModule.TypeSystem.Int64));

            var processor = body.GetILProcessor();

            MethodReference LongCount;
            LongCount = @this.FindMethod(nameof(LongCount), Helper.NoParameter);
            MethodReference GetEnumerator;
            GetEnumerator = @this.FindMethod(nameof(GetEnumerator), Helper.NoParameter);
            MethodReference MoveNext;
            MoveNext = Enumerator.FindMethod(nameof(MoveNext));
            MethodReference TryGetNext;
            TryGetNext = Enumerator.FindMethod(nameof(TryGetNext));
            MethodReference Dispose;
            Dispose = Enumerator.FindMethod(nameof(Dispose), Helper.NoParameter);

            if (!(type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "FastCountAttribute") is null))
            {
                MethodReference get_Item;
                get_Item = @this.FindMethod(nameof(get_Item));

                var il0012 = Instruction.Create(OpCodes.Ldarg_2);

                processor.Do(OpCodes.Ldarg_0);
                processor.Call(@this.FindMethod("LongCount"));
                processor.Do(OpCodes.Ldarg_1);
                processor.Append(Instruction.Create(OpCodes.Bgt_S, il0012));
                processor.Do(OpCodes.Ldarg_2);
                processor.InitObj(Element);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il0012);
                processor.Do(OpCodes.Ldarg_0);
                processor.Do(OpCodes.Ldarg_1);
                processor.Call(@this.FindMethod("get_Item"));
                if (get_Item.ReturnType.IsByReference)
                    processor.LdObj(Element);
                processor.StObj(Element);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Ret();
            }
            else if (!(type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "SlowCountAttribute") is null))
            {
                var il000E = Instruction.Create(OpCodes.Ldarg_0);
                var il0038 = Instruction.Create(OpCodes.Ldloc_2);
                var il001A = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
                var il0033 = Instruction.Create(OpCodes.Ldloc_2);

                processor.Do(OpCodes.Ldarg_1);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Append(Instruction.Create(OpCodes.Bge_S, il000E));
                processor.Do(OpCodes.Ldarg_2);
                processor.Append(Instruction.Create(OpCodes.Initobj, Element));
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il000E);
                processor.Call(GetEnumerator);
                processor.Do(OpCodes.Stloc_0);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Stloc_2);
                processor.Jump(il0038);
                processor.Append(il001A);
                processor.Call(MoveNext);
                processor.True(il0033);
                processor.Do(OpCodes.Ldarg_2);
                processor.Append(Instruction.Create(OpCodes.Initobj, Element));
                processor.LdLocaS(0);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il0033);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Add);
                processor.Do(OpCodes.Stloc_2);
                processor.Append(il0038);
                processor.Do(OpCodes.Ldarg_1);
                processor.Append(Instruction.Create(OpCodes.Blt_S, il001A));
                processor.Do(OpCodes.Ldarg_2);
                processor.LdLocaS(0);
                processor.LdLocaS(1);
                processor.Call(TryGetNext);
                processor.LdObj(Element);
                processor.Append(Instruction.Create(OpCodes.Stobj, Element));
                processor.LdLocaS(0);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldloc_1);
                processor.Ret();
            }
            else
            {
                variables.Add(new VariableDefinition(MainModule.TypeSystem.Int64));

                var il000E = Instruction.Create(OpCodes.Ldarg_0);
                var il0028 = Instruction.Create(OpCodes.Ldarg_0);
                var il0034 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
                var il0041 = Instruction.Create(OpCodes.Ldloc_2);
                var il0047 = Instruction.Create(OpCodes.Ldarg_0);

                processor.Do(OpCodes.Ldarg_1);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Append(Instruction.Create(OpCodes.Bge_S, il000E));
                processor.Do(OpCodes.Ldarg_2);
                processor.Append(Instruction.Create(OpCodes.Initobj, Element));
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il000E);
                processor.Call(@this.FindMethod("CanFastCount"));
                processor.False(il0047);
                processor.Do(OpCodes.Ldarg_1);
                processor.Do(OpCodes.Ldarg_0);
                processor.Call(@this.FindMethod("LongCount"));
                processor.Append(Instruction.Create(OpCodes.Blt_S, il0028));
                processor.Do(OpCodes.Ldarg_2);
                processor.Append(Instruction.Create(OpCodes.Initobj, Element));
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il0028);
                processor.Call(GetEnumerator);
                processor.Do(OpCodes.Stloc_0);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Stloc_2);
                processor.Jump(il0041);
                processor.Append(il0034);
                processor.Call(MoveNext);
                processor.Do(OpCodes.Pop);
                processor.Do(OpCodes.Ldloc_2);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Add);
                processor.Do(OpCodes.Stloc_2);
                processor.Append(il0041);
                processor.Do(OpCodes.Ldarg_1);
                processor.Append(Instruction.Create(OpCodes.Blt_S, il0034));

                var il0053 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
                var il006C = Instruction.Create(OpCodes.Ldloc_3);
                var il0071 = Instruction.Create(OpCodes.Ldloc_3);
                var il0075 = Instruction.Create(OpCodes.Ldarg_2);
                processor.Jump(il0075);
                processor.Append(il0047);
                processor.Call(GetEnumerator);
                processor.Do(OpCodes.Stloc_0);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Stloc_3);
                processor.Jump(il0071);
                processor.Append(il0053);
                processor.Call(MoveNext);
                processor.True(il006C);
                processor.Do(OpCodes.Ldarg_2);
                processor.Append(Instruction.Create(OpCodes.Initobj, Element));
                processor.LdLocaS(0);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldc_I4_0);
                processor.Ret();
                processor.Append(il006C);
                processor.Do(OpCodes.Ldc_I4_1);
                processor.Do(OpCodes.Conv_I8);
                processor.Do(OpCodes.Add);
                processor.Do(OpCodes.Stloc_3);
                processor.Append(il0071);
                processor.Do(OpCodes.Ldarg_1);
                processor.Append(Instruction.Create(OpCodes.Blt_S, il0053));
                processor.Append(il0075);
                processor.LdLocaS(0);
                processor.LdLocaS(1);
                processor.Call(TryGetNext);
                processor.LdObj(Element);
                processor.Append(Instruction.Create(OpCodes.Stobj, Element));
                processor.LdLocaS(0);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldloc_1);
                processor.Ret();
            }

            @static.Methods.Add(method);
        }
    }
}
