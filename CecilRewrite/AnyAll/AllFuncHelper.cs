using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace CecilRewrite
{
    using static Program;
    static class AllFuncHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AllFuncHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                @static.All(type);
            }
        }

        private static void All(this TypeDefinition @static, TypeDefinition type)
        {
            var method = new MethodDefinition(nameof(All), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            method.Parameters.Capacity = 1;
            var argumentsFromTypeToMethodParam = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericType(argumentsFromTypeToMethodParam);
            FillParameter(@this, method);
            FillBody(@this, method);
            @static.Methods.Add(method);
        }

        private static void FillParameter(GenericInstanceType @this, MethodDefinition method)
        {
            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);
            var funcReference = MainModule.ImportReference(typeof(Func<,>)).MakeGenericType(new[]
            {
                @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters),
                MainModule.TypeSystem.Boolean
            });
            var predicateParameterDefinition = new ParameterDefinition("predicate", ParameterAttributes.None, funcReference);
            method.Parameters.Add(predicateParameterDefinition);
        }

        private static void FillBody(GenericInstanceType @this, MethodDefinition method)
        {
            var body = method.Body;

            var typeReferenceEnumerator = (GenericInstanceType)@this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);
            var typeReferenceElement = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var typeReferencePredicate = method.Parameters[1].ParameterType;

            body.Variables.Add(new VariableDefinition(typeReferenceEnumerator));
            body.Variables.Add(new VariableDefinition(typeReferenceElement.MakeByReferenceType()));
            body.Variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

            var il001D = Instruction.Create(OpCodes.Ldarg_1);
            var il0034 = Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]);
            var il004A = Instruction.Create(OpCodes.Ldarg_1);

            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", Helper.NoParameter));
            processor.Do(OpCodes.Stloc_0);
            processor.LoadLocalAddress(0);
            processor.LoadLocalAddress(2);
            var TryGetNext = typeReferenceEnumerator.FindMethod("TryGetNext");
            processor.Call(TryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.True(il001D);
            processor.LoadLocalAddress(0);
            var Dispose = typeReferenceEnumerator.FindMethod("Dispose");
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
            processor.Append(il001D);
            processor.Do(OpCodes.Ldloc_1);
            var methodReferenceFuncInvoke = typeof(Func<,>).FindMethodImportGenericType(MainModule, "Invoke", new[] { typeReferenceElement, MainModule.TypeSystem.Boolean });
            processor.Append(Instruction.Create(OpCodes.Ldobj, typeReferenceElement));
            processor.CallVirtual(methodReferenceFuncInvoke);
            processor.True(il0034);
            processor.LoadLocalAddress(0);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ret);
            processor.Append(il0034);
            processor.LoadLocalAddress(2);
            processor.Call(TryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.True(il004A);
            processor.LoadLocalAddress(0);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
            processor.Append(il004A);
            processor.Do(OpCodes.Ldloc_1);
            processor.Append(Instruction.Create(OpCodes.Ldobj, typeReferenceElement));
            processor.CallVirtual(methodReferenceFuncInvoke);
            processor.True(il0034);
            processor.LoadLocalAddress(0);
            processor.Call(Dispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ret);
        }
    }
}
