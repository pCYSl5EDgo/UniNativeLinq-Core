using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming


namespace CecilRewrite
{
    using static Program;

    static class AggregateOperatorWithResultTypeHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AggregateOperatorWithResultTypeHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
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
            var @this = type.MakeGenericType(added);

            GenericParameter TAccumulate0;
            TAccumulate0 = new GenericParameter(nameof(TAccumulate0), method);
            method.GenericParameters.Add(TAccumulate0);

            GenericParameter TFunc0;
            TFunc0 = new GenericParameter(nameof(TFunc0), method);
            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            TFunc0.Constraints.Add(MainModule.GetType(NameSpace, "IRefAction`2").MakeGenericType(new[]
            {
                TAccumulate0,
                Element,
            }));
            method.GenericParameters.Add(TFunc0);

            method.GenericParameters.Add(TResult0);

            GenericParameter TResultFunc0;
            TResultFunc0 = new GenericParameter(nameof(TResultFunc0), method);
            TFunc0.Constraints.Add(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericType(new[]
            {
                TAccumulate0,
                TResult0,
            }));
            method.GenericParameters.Add(TResultFunc0);

            var thisParam = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            ParameterDefinition seed;
            seed = new ParameterDefinition(nameof(seed), ParameterAttributes.None, TAccumulate0.MakeByReferenceType());
            method.Parameters.Add(seed);

            var funcParameterDefinition = new ParameterDefinition("func", ParameterAttributes.In, TFunc0.MakeByReferenceType());
            funcParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(funcParameterDefinition);
            var resultFuncParameterDefinition = new ParameterDefinition("resultFunc", ParameterAttributes.In, TResultFunc0.MakeByReferenceType());
            resultFuncParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(resultFuncParameterDefinition);

            var body = method.Body;

            var Enumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
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
            processor.LoadLocalAddress(2);
            processor.Call(Enumerator.FindMethod("TryGetNext"));
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.False(il0025);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldloc_1);
            processor.Constrained(TFunc0);
            processor.CallVirtual(MainModule.GetType(NameSpace, "IRefAction`2").MakeGenericType(new[] { TAccumulate0, Element }).FindMethod("Execute"));
            processor.Jump(il0007);
            processor.Append(il0025);
            processor.Call(Enumerator.FindMethod("Dispose", Helper.NoParameter));
            processor.Do(OpCodes.Ldarg_3);
            processor.Do(OpCodes.Ldarg_1);
            processor.Constrained(TResultFunc0);
            processor.CallVirtual(MainModule.GetType(NameSpace, "IRefFunc`2").MakeGenericType(new[] { TAccumulate0, TResultFunc0 }).FindMethod("Calc"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
