using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    using static Program;
    internal static class AnyRefFuncHelper
    {
        private const string NameSpace = "UniNativeLinq";

        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(AnyRefFuncHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                @static.Any(type);
            }
        }

        private static void Any(this TypeDefinition @static, TypeDefinition type)
        {
            var method = new MethodDefinition(nameof(Any), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);
            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);
            FillParameter(@this, method);
            FillBody(@this, method);
            @static.Methods.Add(method);
        }

        private static void FillParameter(GenericInstanceType @this, MethodDefinition method)
        {
            var thisParameterDefinition = new ParameterDefinition("this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParameterDefinition.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParameterDefinition);
            var funcReference = MainModule.GetType(NameSpace, "RefFunc`2").MakeGenericInstanceType(new[]
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
            var typeReferenceEnumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var variables = body.Variables;
            variables.Add(new VariableDefinition(typeReferenceEnumerator));
            var typeReferenceElement = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            variables.Add(new VariableDefinition(typeReferenceElement.MakeByReferenceType()));
            variables.Add(new VariableDefinition(MainModule.TypeSystem.Boolean));

            var processor = body.GetILProcessor();
            if (!(typeReferenceEnumerator is GenericInstanceType genericInstanceEnumerator))
                throw new Exception();
            var methodReferenceTryGetNext = genericInstanceEnumerator.FindMethod("TryGetNext");
            var methodReferenceDispose = genericInstanceEnumerator.FindMethod("Dispose");

            var il0007 = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var il001D = Instruction.Create(OpCodes.Ldarg_1);

            processor.Do(OpCodes.Ldarg_0);
            processor.Call(@this.FindMethod("GetEnumerator", x => !x.HasParameters));
            processor.Do(OpCodes.Stloc_0);
            processor.Append(il0007);
            processor.LdLocaS(2);
            processor.Call(methodReferenceTryGetNext);
            processor.Do(OpCodes.Stloc_1);
            processor.Do(OpCodes.Ldloc_2);
            processor.Append(Instruction.Create(OpCodes.Brtrue_S, il001D));
            processor.LdLocaS(0);
            processor.Call(methodReferenceDispose);
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ret);
            processor.Append(il001D);
            processor.Do(OpCodes.Ldloc_1);
            var funcReference = MainModule.GetType(NameSpace, "RefFunc`2").MakeGenericInstanceType(new[]
            {
                @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters),
                MainModule.TypeSystem.Boolean
            });
            var methodReferenceFuncInvoke = funcReference.FindMethod("Invoke");
            processor.Append(Instruction.Create(OpCodes.Callvirt, methodReferenceFuncInvoke));
            processor.Append(Instruction.Create(OpCodes.Brfalse_S, il0007));
            processor.LdLocaS(0);
            processor.Call(methodReferenceDispose);
            processor.Do(OpCodes.Ldc_I4_1);
            processor.Do(OpCodes.Ret);
        }
    }
}
