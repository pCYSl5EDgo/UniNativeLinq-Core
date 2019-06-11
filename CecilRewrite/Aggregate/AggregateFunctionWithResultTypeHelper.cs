using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CecilRewrite
{
    using static Program;
    static class AggregateFunctionWithResultTypeHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AggregateFunctionWithResultTypeHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
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
            var TResult0 = new GenericParameter("TResult0", method);
            method.ReturnType = TResult0;
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            GenericParameter TAccumulate0;
            TAccumulate0 = new GenericParameter(nameof(TAccumulate0), method);
            method.GenericParameters.Add(TAccumulate0);

            method.GenericParameters.Add(TResult0);


            var thisParam = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            ParameterDefinition seed;
            seed = new ParameterDefinition(nameof(seed), ParameterAttributes.None, TAccumulate0.MakeByReferenceType());
            method.Parameters.Add(seed);

            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);

            var TFunc0 = MainModule.ImportReference(SystemModule.GetType("System", "Func`3")).MakeGenericInstanceType(TAccumulate0, Element, TAccumulate0);
            var TResultFunc0 = MainModule.ImportReference(SystemModule.GetType("System", "Func`2")).MakeGenericInstanceType(TAccumulate0, TResult0);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc0));
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.None, TResultFunc0));

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(Enumerator));
            body.Variables.Add(new VariableDefinition(Element.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(@this.Module.TypeSystem.Boolean));

            var il0007 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
            var il0025 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.GetEnumerator(@this);
            processor.Do(OpCodes.Stloc_0);
            processor.Append(il0007);
            processor.LdLocaS(2);
            processor.Call(Enumerator.FindMethod("TryGetNext"));
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.False(il0025);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_1);
            processor.LdObj(TAccumulate0);
            processor.Do(OpCodes.Ldloc_1);
            processor.LdObj(Element);
            processor.CallVirtual(TFunc0.FindMethod("Invoke"));
            processor.Append(Instruction.Create(OpCodes.Stobj, TAccumulate0));
            processor.Jump(il0007);
            processor.Append(il0025);
            processor.Call(Enumerator.FindMethod("Dispose", Helper.NoParameter));
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Ldarg_1);
            processor.LdObj(TAccumulate0);
            processor.CallVirtual(TResultFunc0.FindMethod("Invoke"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
