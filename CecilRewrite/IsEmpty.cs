using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CecilRewrite
{
    using static Program;
    // ReSharper disable once InconsistentNaming
    static class IsEmptyHelper
    {
        internal static void Create(ModuleDefinition module)
        {
            foreach (var type in module.Types.Where(x => x.IsValueType && x.IsPublic && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
                IsEmpty(type);
        }

        // ReSharper disable once InconsistentNaming
        private static void IsEmpty(TypeDefinition type)
        {
            var module = type.Module;
            var method = new MethodDefinition(nameof(IsEmpty), MethodAttributes.Public | MethodAttributes.HideBySig, module.TypeSystem.Boolean)
            {
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(IsReadOnlyAttribute);
            var body = method.Body;
            var processor = body.GetILProcessor();
            processor.Do(OpCodes.Ldarg_0);
            var instance = new GenericInstanceType(type);
            foreach (var genericParameter in type.GenericParameters)
                instance.GenericArguments.Add(genericParameter);
            processor.Call(instance.FindMethod("Any"));
            processor.Do(OpCodes.Ldc_I4_0);
            processor.Do(OpCodes.Ceq);
            processor.Do(OpCodes.Ret);
            type.Methods.Add(method);
        }
    }
}
