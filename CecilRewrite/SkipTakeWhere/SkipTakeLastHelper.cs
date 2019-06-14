using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
// ReSharper disable InconsistentNaming
// ReSharper disable LocalNameCapturedOnly

namespace CecilRewrite
{
    using static Program;
    static class SkipTakeLastHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            TypeDefinition Take;
            Take = new TypeDefinition(NameSpace,
                nameof(Take) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Take.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Take);

            TypeDefinition Skip;
            Skip = new TypeDefinition(NameSpace,
                nameof(Skip) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            Skip.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(Skip);

            TypeDefinition SkipLast;
            SkipLast = new TypeDefinition(NameSpace,
                nameof(SkipLast) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            SkipLast.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(SkipLast);

            TypeDefinition TakeLast;
            TakeLast = new TypeDefinition(NameSpace,
                nameof(TakeLast) + "Helper",
                StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            TakeLast.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(TakeLast);

            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                MakeMethod(Take, type, nameof(Take), false);
                MakeMethod(Skip, type, nameof(Skip), false);
                MakeMethod(TakeLast, type, nameof(TakeLast), true);
                MakeMethod(SkipLast, type, nameof(SkipLast), true);
            }
        }

        private static void MakeMethod(TypeDefinition @static, TypeDefinition type, string name, bool hasAllocator)
        {
            var MainModule = @static.Module;
            var method = new MethodDefinition(name, StaticMethodAttributes, MainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(ExtensionAttribute);

            var added = method.FromTypeToMethodParam(type.GenericParameters);
            var @this = type.MakeGenericInstanceType(added);

            var Element = @this.GetElementTypeOfCollectionType().Replace(method.GenericParameters);
            var Enumerator = @this.GetEnumeratorTypeOfCollectionType().Replace(method.GenericParameters);

            var @return = MainModule.GetType(NameSpace, name + "Enumerable`3").MakeGenericInstanceType(new[]
            {
                @this,
                Enumerator,
                Element
            });

            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, @this.MakeByReferenceType());
            thisParam.CustomAttributes.Add(IsReadOnlyAttribute);
            method.Parameters.Add(thisParam);

            method.Parameters.Add(new ParameterDefinition("count", ParameterAttributes.None, MainModule.TypeSystem.Int64));

            var processor = method.Body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            processor.Do(OpCodes.Ldarg_1);

            if (hasAllocator)
            {
                method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
                {
                    Constant = 2,
                });
                processor.Do(OpCodes.Ldarg_2);
            }

            processor.NewObj(@return.FindMethod(".ctor"));
            processor.Ret();

            @static.Methods.Add(method);
        }
    }
}
