using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming

namespace CecilRewrite.Contains
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

            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Conv_I8);
            if (!(type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "FastCountAttribute") is null))
            {
                var il000E = Instruction.Create(OpCodes.Ldarg_0);
                var il001A = Instruction.Create(OpCodes.Ldloca_S, variables[0]); 
                var il0033 = Instruction.Create(OpCodes.Ldloc_2);
                var il0038 = Instruction.Create(OpCodes.Ldloc_2);

                processor.Append(Instruction.Create(OpCodes.Bge_S, il000E));
                processor.Do(OpCodes.Ldarg_2);
                processor.InitObj(Element);
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
                processor.InitObj(Element);
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
                processor.StObj(Element);
                processor.LdLocaS(0);
                processor.Call(Dispose);
                processor.Do(OpCodes.Ldloc_1);
                processor.Ret();
            }
            else if (!(type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "SlowCountAttribute") is null))
            {
                var il000E = Instruction.Create(OpCodes.Ldarg_0);
                var il0038 = Instruction.Create(OpCodes.Ldloc_2);
                var il001A = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
                var il0033 = Instruction.Create(OpCodes.Ldloc_2);

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
            /*
	// if (index < 0)
	IL_0000: ldarg.1
	IL_0001: ldc.i4.0
	IL_0002: conv.i8
	IL_0003: bge.s IL_000e

	// value = default(T);
	IL_0005: ldarg.2
	IL_0006: initobj !!T
	// return false;
	IL_000c: ldc.i4.0
	// (no C# code)
	IL_000d: ret

	// WhereEnumerable<TEnumerable, TEnumerator, T, TPredicate0>.Enumerator enumerator = @this.GetEnumerator();
	IL_000e: ldarg.0
	IL_000f: call instance valuetype UniNativeLinq.WhereEnumerable`4/Enumerator<!0, !1, !2, !3> valuetype UniNativeLinq.WhereEnumerable`4<!!TEnumerable, !!TEnumerator, !!T, !!TPredicate0>::GetEnumerator()
	IL_0014: stloc.0
	// for (long num = 0L; num < index; num++)
	IL_0015: ldc.i4.0
	IL_0016: conv.i8
	IL_0017: stloc.2
	// (no C# code)
	IL_0018: br.s IL_0038
	// loop start (head: IL_0038)
		// if (!enumerator.MoveNext())
		IL_001a: ldloca.s 0
		IL_001c: call instance bool valuetype UniNativeLinq.WhereEnumerable`4/Enumerator<!!TEnumerable, !!TEnumerator, !!T, !!TPredicate0>::MoveNext()
		// (no C# code)
		IL_0021: brtrue.s IL_0033

		// value = default(T);
		IL_0023: ldarg.2
		IL_0024: initobj !!T
		// enumerator.Dispose();
		IL_002a: ldloca.s 0
		IL_002c: call instance void valuetype UniNativeLinq.WhereEnumerable`4/Enumerator<!!TEnumerable, !!TEnumerator, !!T, !!TPredicate0>::Dispose()
		// return false;
		IL_0031: ldc.i4.0
		// (no C# code)
		IL_0032: ret

		// for (long num = 0L; num < index; num++)
		IL_0033: ldloc.2
		IL_0034: ldc.i4.1
		IL_0035: conv.i8
		IL_0036: add
		IL_0037: stloc.2

		// for (long num = 0L; num < index; num++)
		IL_0038: ldloc.2
		IL_0039: ldarg.1
		IL_003a: blt.s IL_001a
	// end loop

	// value = enumerator.TryGetNext(out bool success);
	IL_003c: ldarg.2
	IL_003d: ldloca.s 0
	IL_003f: ldloca.s 1
	IL_0041: call instance !2& valuetype UniNativeLinq.WhereEnumerable`4/Enumerator<!!TEnumerable, !!TEnumerator, !!T, !!TPredicate0>::TryGetNext(bool&)
	IL_0046: ldobj !!T
	IL_004b: stobj !!T
	// enumerator.Dispose();
	IL_0050: ldloca.s 0
	IL_0052: call instance void valuetype UniNativeLinq.WhereEnumerable`4/Enumerator<!!TEnumerable, !!TEnumerator, !!T, !!TPredicate0>::Dispose()
	// return success;
	IL_0057: ldloc.1
	// (no C# code)
	IL_0058: ret
             */
            else
            {
                variables.Add(new VariableDefinition(MainModule.TypeSystem.Int64));

                var il000E = Instruction.Create(OpCodes.Ldarg_0);
                var il0028 = Instruction.Create(OpCodes.Ldarg_0);
                var il0034 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
                var il0041 = Instruction.Create(OpCodes.Ldloc_2);
                var il0047 = Instruction.Create(OpCodes.Ldarg_0);

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
