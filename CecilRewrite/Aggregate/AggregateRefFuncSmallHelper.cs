using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming

namespace CecilRewrite
{
    using static Program;
    static class AggregateRefFuncSmallHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AggregateRefFuncSmallHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                @static.Aggregate(type);
            }
        }

        static void Aggregate(this TypeDefinition @static, TypeDefinition type)
        {
            var method = new MethodDefinition(nameof(Aggregate), StaticMethodAttributes, MainModule.TypeSystem.Void)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);
            GenericParameter TAccumulate0;
            TAccumulate0 = new GenericParameter(nameof(TAccumulate0), method);
            method.GenericParameters.Add(TAccumulate0);

            var thisParam = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            ParameterDefinition seed;
            seed = new ParameterDefinition(nameof(seed), ParameterAttributes.None, TAccumulate0.MakeByReferenceType());
            method.Parameters.Add(seed);

            var funcReference = MainModule.GetType(NameSpace, "RefFunc`3").MakeGenericInstanceType(new[]
            {
                TAccumulate0,
                @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters),
                TAccumulate0,
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, funcReference));

            FillBody(@this, method, TAccumulate0, funcReference);
            @static.Methods.Add(method);
        }

        private static void FillBody(GenericInstanceType @this, MethodDefinition method, GenericParameter accumulate0, GenericInstanceType func)
        {
            var body = method.Body;

            var EnumeratorType = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var ElementType = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            var variables = body.Variables;
            variables.Add(new VariableDefinition(EnumeratorType));
            variables.Add(new VariableDefinition(ElementType.MakeByReferenceType()));
            variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

            var il0007 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var il002E = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
            processor.Do(OpCodes.Stloc_0);
            processor.Append(il0007);
            processor.LdLocaS(2);
            processor.Call(EnumeratorType.FindMethod("TryGetNext"));
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.False(il002E);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldloc_1);
            var Func3 = MainModule.GetType(NameSpace, "RefFunc`3").MakeGenericInstanceType(accumulate0, ElementType, accumulate0);
            processor.CallVirtual(Func3.FindMethod("Invoke"));
            processor.Append(Instruction.Create(OpCodes.Stobj, accumulate0));
            processor.Jump(il0007);
            processor.Append(il002E);
            processor.Call(EnumeratorType.FindMethod("Dispose", Helper.NoParameter));
            processor.Ret();
        }
    }
}
