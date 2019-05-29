using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable InconsistentNaming

namespace CecilRewrite
{
    using static Program;
    static class AggregateOperatorSmallHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AggregateOperatorSmallHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
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
            var @this = type.MakeGenericType(added);
            GenericParameter Aggregate0, Func0;
            Aggregate0 = new GenericParameter(nameof(Aggregate0), method);
            Func0 = new GenericParameter(nameof(Func0), method);
            Func0.Constraints.Add(MainModule.GetType(NameSpace, "IRefAction`2").MakeGenericType(new[]
            {
                Aggregate0,
                @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters),
            }));
            method.GenericParameters.Add(Aggregate0);
            method.GenericParameters.Add(Func0);

            var thisParam = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            ParameterDefinition seed;
            seed = new ParameterDefinition(nameof(seed), ParameterAttributes.None, Aggregate0.MakeByReferenceType());
            method.Parameters.Add(seed);

            ParameterDefinition func;
            func = new ParameterDefinition(nameof(func), ParameterAttributes.In, Func0.MakeByReferenceType());
            func.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(func);

            FillBody(@this, method, Aggregate0, Func0);
            @static.Methods.Add(method);
        }

        private static void FillBody(GenericInstanceType @this, MethodDefinition method, GenericParameter aggregate0, GenericParameter func0)
        {
            var body = method.Body;

            var EnumeratorType = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(EnumeratorType));
            var ElementType = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            body.Variables.Add(new VariableDefinition(ElementType.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

            var il0007 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
            var il0025 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
            processor.Do(OpCodes.Stloc_0);
            processor.Append(il0007);
            processor.LoadLocalAddress(2);
            processor.Call(EnumeratorType.FindMethod("TryGetNext"));
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.False(il0025);
            processor.Do(OpCodes.Ldarg_2);
            processor.Do(OpCodes.Ldarg_1);
            processor.Do(OpCodes.Ldloc_1);
            var Execute = MainModule.GetType(NameSpace, "IRefAction`2").MakeGenericType(new[] { aggregate0, ElementType }).FindMethod("Execute");
            processor.Constrained(func0);
            processor.CallVirtual(Execute);
            processor.Jump(il0007);
            processor.Append(il0025);
            processor.Call(EnumeratorType.FindMethod("Dispose", Helper.NoParameter));
            processor.Ret();
        }
    }
}
