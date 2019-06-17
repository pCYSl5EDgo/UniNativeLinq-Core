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
                foreach (var type1 in Enumerables)
                    Concat(@static, type0, type1);
            }
        }

        private static void Concat(TypeDefinition @static, TypeDefinition type0, TypeDefinition type1)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(nameof(Concat), StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            GenericParameter T;
            T = new GenericParameter(nameof(T), method);
            T.CustomAttributes.Add(UnManagedAttribute);

            const string suffix0 = "0";


            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, nameof(T), T, suffix0);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable0 = type0.MakeGenericInstanceType(added0.Append(T));

            var Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            var Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);

            const string suffix1 = "1";

            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, nameof(T), T, suffix1);
            foreach (var parameter in added1)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable1 = type1.MakeGenericInstanceType(added1.Append(T));

            var Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);
            var Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);

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
    }
}
