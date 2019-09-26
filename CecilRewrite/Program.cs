using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

// ReSharper disable VariableHidesOuterVariable

namespace CecilRewrite
{
    public static class Program
    {
        internal const string NameSpace = "UniNativeLinq";

        struct Data
        {
            public readonly ModuleDefinition MainModule;
            public readonly CustomAttribute IsReadOnlyAttribute;
            public readonly AssemblyDefinition Assembly;

            public Data(string folderPath, string pathUniNativeLinqDll)
            {
                var resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(folderPath);
                Assembly = AssemblyDefinition.ReadAssembly(pathUniNativeLinqDll, new ReaderParameters(readingMode: ReadingMode.Deferred) { AssemblyResolver = resolver });
                MainModule = Assembly.MainModule;

                if(MainModule is null) throw new ArgumentNullException();

                var zipValueTuple = MainModule.GetType("UniNativeLinq", "ZipValueTuple`2"); 
                
                if (zipValueTuple is null)
                {
                    foreach (var typeDefinition in MainModule.Types)
                    {
                        Console.WriteLine(typeDefinition.FullName);
                    }
                    throw new ArgumentNullException();
                }

                var customAttributes = zipValueTuple.CustomAttributes;
                foreach (var customAttribute in customAttributes)
                {
                    Console.WriteLine(customAttribute.AttributeType.FullName);
                }
                IsReadOnlyAttribute = customAttributes[1];
            }
        }

        static Data data;

        public static int Main(string[] args)
        {
            if (args.Length != 3) return 1;

            data = new Data(args[0], args[1]);

            RewriteThrow(data.MainModule);
            ReWritePseudoIsReadOnly(data.MainModule);
            ReWritePseudoUtility(data.MainModule);

            data.Assembly.Write(args[2]);
            return 0;
        }

        private static void ReWritePseudoUtility(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes()
                .Where(x => x.IsValueType))
            {
                foreach (var method in type.Methods)
                {
                    using (ScopedProcessor processor = method.Body.GetILProcessor())
                    {
                        var instructions = method.Body.Instructions;
                        for (var i = instructions.Count; --i >= 0;)
                        {
                            var instruction = instructions[i];
                            if (instruction.OpCode != OpCodes.Call || !(instruction.Operand is MethodReference methodReference) || methodReference.DeclaringType.Name != "Pseudo") continue;
                            if (methodReference.Name == "AsRefNull")
                            {
                                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
                                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Conv_U));
                            }
                            processor.Remove(instruction);
                        }
                    }
                }
            }
            var pseudo = module.GetType(NameSpace, "Pseudo");
            module.Types.Remove(pseudo);
        }

        private static void ReWritePseudoIsReadOnly(this ParameterDefinition param)
        {
            var memberCustomAttributes = param.CustomAttributes;
            var pseudo = memberCustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "PseudoIsReadOnlyAttribute");
            if (pseudo is null) return;
            memberCustomAttributes.Remove(pseudo);
            memberCustomAttributes.Add(data.IsReadOnlyAttribute);
            param.Attributes = ParameterAttributes.In;
        }

        private static void ReWritePseudoIsReadOnly(ModuleDefinition module)
        {
            foreach (var type in module.GetTypes().Where(x => x.HasNestedTypes))
            {
                ReWritePseudoIsReadOnly(type);
            }
            var pseudo = data.MainModule.GetType(NameSpace, "PseudoIsReadOnlyAttribute");
            data.MainModule.Types.Remove(pseudo);
        }

        private static void ReWritePseudoIsReadOnly(TypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                foreach (var parameter in method.Parameters)
                {
                    parameter.ReWritePseudoIsReadOnly();
                }
            }
            foreach (var nestedType in type.NestedTypes)
                ReWritePseudoIsReadOnly(nestedType);
        }

        internal static void RewriteThrow(ModuleDefinition module)
        {
            const string local = "LocalRefReturnAttribute";
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Concat(module.GetTypes().Where(x => x.IsValueType));
            foreach (var enumeratorType in enumeratorTypes)
            {
                ReWriteRefReturn(module, enumeratorType);
                var remove = enumeratorType.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == local);
                if (remove is null) continue;
                enumeratorType.CustomAttributes.Remove(remove);
            }
            {
                var remove = module.GetType(NameSpace, local);
                module.Types.Remove(remove);
            }
        }

        internal static void ReWriteRefReturn(ModuleDefinition module, TypeDefinition type)
        {
            var firstOrDefault = type.Fields.FirstOrDefault(x => x.Name == "element");
            if (firstOrDefault is null) return;
            var element = firstOrDefault.FillGenericParams();
            foreach (var methodDefinition in type.Methods)
            {
                switch (methodDefinition.Name)
                {
                    case "TryGetNext":
                    case "get_Item":
                        ReWrite(methodDefinition, element);
                        break;
                }
            }
            static bool Predicate(PropertyDefinition x)
                => x.Name == "Current" && x.PropertyType.IsByReference;

            var method = type.Properties.FirstOrDefault(Predicate)?.GetMethod;
            if (method is null) return;
            ReWrite(method, element);
        }

        internal static FieldReference FillGenericParams(this FieldDefinition definition)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var param in definition.DeclaringType.GenericParameters)
                declaringType.GenericArguments.Add(param);
            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        internal static void ReWrite(MethodDefinition method, FieldReference field)
        {
            var body = method.Body;
            using (ScopedProcessor processor = body.GetILProcessor())
            {
                for (var i = body.Instructions.Count; --i >= 0;)
                {
                    var throwInstruction = body.Instructions[i];
                    if (throwInstruction.OpCode != OpCodes.Throw) continue;
                    var newObjInstruction = throwInstruction.Previous;
                    if (newObjInstruction.OpCode != OpCodes.Newobj) continue;
                    var NotImplementedExceptionConstructor = ((MethodReference)newObjInstruction.Operand);
                    Console.WriteLine(NotImplementedExceptionConstructor.DeclaringType.FullName);
                    if (NotImplementedExceptionConstructor.DeclaringType.FullName != "System.NotImplementedException") continue;

                    var ldarg0 = Instruction.Create(OpCodes.Ldarg_0);
                    var ldflda = Instruction.Create(OpCodes.Ldflda, field);
                    var ret = Instruction.Create(OpCodes.Ret);

                    processor
                        .Remove(newObjInstruction)
                        .Replace(throwInstruction, ldarg0)
                        .InsertAfter(ldarg0, ldflda)
                        .InsertAfter(ldflda, ret);
                }
            }
        }
    }
}
