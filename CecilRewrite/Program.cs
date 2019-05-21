#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CecilRewriteAndAddExtensions
{
    static class Program
    {
        private const string NameSpace = "UniNativeLinq";

        private const TypeAttributes StaticExtensionClassTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Abstract;
        private const MethodAttributes StaticMethodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        private static ModuleDefinition MainModule;
        private static ModuleDefinition UnsafeModule;
        private static CustomAttribute ExtensionAttribute;
        private static ModuleDefinition UnityModule;

        private static void Main(string[] args)
        {
            var directory = args.Length != 0 && !string.IsNullOrEmpty(args[0]) ? args[0] : @"C:\Users\conve\source\repos\pcysl5edgo\UniNativeLinq\bin\Release\netstandard2.0";
            using var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(directory);
            using var asm = AssemblyDefinition.ReadAssembly(
                Path.Combine(directory, "UniNativeLinq.dll"),
                new ReaderParameters { AssemblyResolver = resolver }
            );
            MainModule = asm.MainModule;
            InitializeModule(MainModule);
            RewriteThrow(MainModule);
            MinMaxHelper(MainModule);
            asm.Write(Path.Combine(directory, "../UniNativeLinq.dll"));
            var s = Console.ReadLine();
        }

        private static void InitializeModule(ModuleDefinition module)
        {
            var nativeEnumerable = module.Types.First(x => x.Name == "NativeEnumerable");
            ExtensionAttribute = nativeEnumerable.CustomAttributes.Single();
            UnsafeModule = ExtensionAttribute.AttributeType.Module;
            UnityModule = nativeEnumerable.Methods.First(x => x.Parameters.First().ParameterType.IsValueType).Parameters.First().ParameterType.Module;
        }

        private static void MinMaxHelper(ModuleDefinition module)
        {
            var @static = new TypeDefinition(NameSpace, nameof(MinMaxHelper), StaticExtensionClassTypeAttributes, module.TypeSystem.Object);
            @static.CustomAttributes.Add(ExtensionAttribute);
            module.Types.Add(@static);
            foreach (var type in module.Types.Where(x => x.IsValueType && x.HasInterfaces && x.Interfaces.Any(y => y.InterfaceType.Name == "IRefEnumerable`2")))
            {
                if (type.Name == "GroupByEnumerable`8") continue;
                @static.TryGetMin(type);
                //Console.WriteLine(type.Name);
            }
        }

        private static void TryGetMin(this TypeDefinition @static, TypeDefinition type)
        {
            Console.WriteLine(type.FullName);
            foreach (var genericParameter in type.GenericParameters)
            {
                Console.WriteLine("\t" + genericParameter.FullName);
                foreach (var genericParameterConstraint in genericParameter.Constraints)
                {
                    Console.WriteLine("\t\t" + genericParameterConstraint.ToString());
                }
            }

            var function = new MethodDefinition(nameof(TryGetMin), StaticMethodAttributes, MainModule.TypeSystem.Boolean);
            function.CustomAttributes.Add(ExtensionAttribute);
        }

        private static void RewriteThrow(ModuleDefinition module)
        {
            var enumeratorTypes = module.GetTypes()
                .Where(x => x.IsPublic && x.IsValueType && x.HasNestedTypes)
                .SelectMany(x => x.NestedTypes)
                .Where(x => x.HasCustomAttributes && x.CustomAttributes.Any(y => y.AttributeType.Name == "LocalRefReturnAttribute"));
            Console.WriteLine("rewrite 'throw new NotImplemented' to return ref element;");
            foreach (var enumeratorType in enumeratorTypes)
            {
                //Console.WriteLine(enumeratorType.FullName);
                ReWriteRefReturn(module, enumeratorType);
            }
        }

        private static void ReWriteRefReturn(ModuleDefinition module, TypeDefinition type)
        {
            var element = type.Fields.First(x => x.Name == "element").FillGenericParams();
            ReWrite(type.Methods.First(x => x.Name == "TryGetNext"), element);
            static bool predicate(PropertyDefinition x)
                => x.Name == "Current" && x.PropertyType.IsByReference;
            ReWrite(type.Properties.First(predicate).GetMethod, element);
        }

        private static FieldReference FillGenericParams(this FieldDefinition definition)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var param in definition.DeclaringType.GenericParameters)
                declaringType.GenericArguments.Add(param);
            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        private static void ReWrite(MethodDefinition method, FieldReference field)
        {
            var body = method.Body;
            var processor = body.GetILProcessor();
            var ldflda = processor.Create(OpCodes.Ldflda, field);
            var ret = processor.Create(OpCodes.Ret);
            var ldarg0 = processor.Create(OpCodes.Ldarg_0);
            for (var i = body.Instructions.Count; --i >= 0;)
            {
                var instruction = body.Instructions[i];
                if (instruction.OpCode == OpCodes.Throw)
                    processor.Replace(instruction, ret);
                else if (instruction.OpCode == OpCodes.Newobj)
                {
                    processor.InsertBefore(instruction, ldarg0);
                    processor.Replace(instruction, ldflda);
                }
            }
        }
    }
}
