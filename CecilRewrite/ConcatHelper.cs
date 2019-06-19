using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;

    static class ConcatHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace,
                nameof(ConcatHelper),
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type0 in Enumerables)
            {
                Concat(@static, type0, "Array", 0, CalcArrayType, AsRefEnumerableArray);
                Concat(@static, type0, "Array", 1, CalcArrayType, AsRefEnumerableArray);
                Concat(@static, type0, "Native", 0, NativeArray.MakeGenericInstanceType, AsRefEnumerableNative);
                Concat(@static, type0, "Native", 1, NativeArray.MakeGenericInstanceType, AsRefEnumerableNative);
                foreach (var type1 in Enumerables)
                    Concat(@static, type0, type1);
            }
        }

        private static void Concat(TypeDefinition @static, TypeDefinition type, string name, int index, Func<IEnumerable<TypeReference>, TypeReference> InputFunc, MethodReference constructor)
        {
            var MainModule = Prologue(@static, type, out var method, out var T, out var Enumerable0, out var Enumerator0, out var Element0);
            if (Element0.Name != nameof(T)) return;
            var Input = InputFunc(new[] { T });

            var Enumerable = MainModule.GetType(NameSpace, name + "Enumerable`1").MakeGenericInstanceType(T);
            var Enumerator = MainModule.GetType(NameSpace, name + "Enumerable`1").NestedTypes.First(x => x.Name == "Enumerator").MakeGenericInstanceType(T);

            method.GenericParameters.Add(T);

            var returnGenericParameters = new TypeReference[5];
            returnGenericParameters[4] = T;
            returnGenericParameters[index << 1] = Enumerable;
            returnGenericParameters[1 + (index << 1)] = Enumerator;
            returnGenericParameters[(1 - index) << 1] = Enumerable0;
            returnGenericParameters[1 + ((1 - index) << 1)] = Enumerator0;
            var @return = MainModule.GetType("UniNativeLinq", "ConcatEnumerable`5").MakeGenericInstanceType(returnGenericParameters);
            method.ReturnType = @return;

            ParameterDefinition firstParam, secondParam;
            if (index == 0)
            {
                firstParam = new ParameterDefinition("@this", ParameterAttributes.None, Input);

                secondParam = new ParameterDefinition("second", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
                secondParam.CustomAttributes.Add(IsReadOnlyAttribute);
            }
            else
            {
                firstParam = new ParameterDefinition("@this", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
                firstParam.CustomAttributes.Add(IsReadOnlyAttribute);

                secondParam = new ParameterDefinition("second", ParameterAttributes.None, Input);
            }

            method.Parameters.Add(firstParam);
            method.Parameters.Add(secondParam);

            method.Body.Variables.Add(new VariableDefinition(Enumerable));

            var genericInstanceMethod = new GenericInstanceMethod(constructor);
            genericInstanceMethod.GenericArguments.Add(T);
            constructor = genericInstanceMethod;
            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            if (index == 0)
            {
                processor.Call(constructor);
                processor.Do(OpCodes.Stloc_0);
                processor.LdLocaS(0);
            }
            processor.Do(OpCodes.Ldarg_1);
            if (index != 0)
            {
                processor.Call(constructor);
                processor.Do(OpCodes.Stloc_0);
                processor.LdLocaS(0);
            }
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static void Concat(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var MainModule = Prologue(@static, type0, out var method, out var T, out var Enumerable0, out var Enumerator0, out var Element0);

            const string suffix1 = "1";

            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, nameof(T), T, suffix1);
            foreach (var parameter in added1)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable1 = type1.MakeGenericInstanceType(added1.Append(T));

            var Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);
            var Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);

            if (!Element0.Equals(Element1))
                return;

            method.GenericParameters.Add(T);

            var @return = MainModule.GetType("UniNativeLinq", "ConcatEnumerable`5").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable1,
                Enumerator1,
                T
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            var secondParam = new ParameterDefinition("second", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            secondParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(secondParam);

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);
            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }

        private static ModuleDefinition Prologue(TypeDefinition @static, TypeDefinition type0, out MethodDefinition method, out GenericParameter T, out GenericInstanceType Enumerable0, out TypeReference Enumerator0, out TypeReference Element0)
        {
            var MainModule = @static.Module;
            method = new MethodDefinition(nameof(Concat), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(UnManagedAttribute);

            const string suffix0 = "0";

            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, nameof(T), T, suffix0);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            Enumerable0 = type0.MakeGenericInstanceType(added0.Append(T));

            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            return MainModule;
        }
    }
}
