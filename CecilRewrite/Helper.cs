using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace CecilRewrite
{
    internal static class Helper
    {
        public static GenericInstanceType MakeGenericType(this TypeReference self, IEnumerable<TypeReference> arguments)
        {
            var instance = new GenericInstanceType(self);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);
            return instance;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, IEnumerable<TypeReference> arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };
            foreach (var parameter in self.Parameters)
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            foreach (var genericParameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));
            return reference;
        }

        public static MethodReference FindMethod(this GenericInstanceType type, string name)
            => type.Resolve().Methods.Single(x => x.Name == name).MakeHostInstanceGeneric(type.GenericArguments);

        public static MethodReference FindMethodAndImport(this GenericInstanceType type, string name, ModuleDefinition module)
            => module.ImportReference(type.Resolve().Methods.Single(x => x.Name == name)).MakeHostInstanceGeneric(type.GenericArguments);

        public static MethodReference FindMethod(this GenericInstanceType type, string name, Func<MethodDefinition, bool> predicate)
            => type.Resolve().Methods.Single(x => x.Name == name && predicate(x)).MakeHostInstanceGeneric(type.GenericArguments);

        public static GenericInstanceType FindNested(this GenericInstanceType type, string name)
        {
            var nestedType = new GenericInstanceType(((TypeDefinition)type.ElementType).NestedTypes.First(x => x.Name.EndsWith(name)));
            foreach (var argument in type.GenericArguments)
                nestedType.GenericArguments.Add(argument);
            return nestedType;
        }

        public static List<GenericParameter> FromTypeToMethodParam(this MethodDefinition method, Collection<GenericParameter> typeGenericParameters)
        {
            var initialLength = method.GenericParameters.Count;
            var answer = new List<GenericParameter>(typeGenericParameters.Count);
            foreach (var typeGenericParameter in typeGenericParameters)
            {
                var methodGenericParameter = new GenericParameter(typeGenericParameter.Name, method)
                {
                    HasNotNullableValueTypeConstraint = typeGenericParameter.HasNotNullableValueTypeConstraint,
                    HasDefaultConstructorConstraint = typeGenericParameter.HasDefaultConstructorConstraint,
                    IsContravariant = typeGenericParameter.IsContravariant,
                    IsValueType = typeGenericParameter.IsValueType,
                };
                foreach (var customAttribute in typeGenericParameter.CustomAttributes)
                    methodGenericParameter.CustomAttributes.Add(customAttribute);
                method.GenericParameters.Add(methodGenericParameter);
                answer.Add(methodGenericParameter);
            }
            for (int j = 0; j < answer.Count; j++)
            {
                var methodGenericParameter = method.GenericParameters[j + initialLength];
                foreach (var constraint in typeGenericParameters[j].Constraints)
                    methodGenericParameter.Constraints.Add(constraint.Replace(answer));
            }
            return answer;
        }

        public static List<GenericParameter> FromTypeToMethodParam(this MethodDefinition method, Collection<GenericParameter> typeGenericParameters, string specialReplaceName, TypeReference specialReplaceType)
        {
            var initialLength = method.GenericParameters.Count;
            var answer = new List<GenericParameter>(typeGenericParameters.Count);
            var skipIndex = -1;
            foreach (var (typeGenericParameter, i) in typeGenericParameters.Select((x, i) => (x, i)))
            {
                if (typeGenericParameter.Name == specialReplaceName)
                {
                    skipIndex = i;
                    continue;
                }
                var methodGenericParameter = new GenericParameter(typeGenericParameter.Name, method)
                {
                    HasNotNullableValueTypeConstraint = typeGenericParameter.HasNotNullableValueTypeConstraint,
                    HasDefaultConstructorConstraint = typeGenericParameter.HasDefaultConstructorConstraint,
                    IsContravariant = typeGenericParameter.IsContravariant,
                    IsValueType = typeGenericParameter.IsValueType,
                };
                foreach (var customAttribute in typeGenericParameter.CustomAttributes)
                    methodGenericParameter.CustomAttributes.Add(customAttribute);
                method.GenericParameters.Add(methodGenericParameter);
                answer.Add(methodGenericParameter);
            }
            for (int j = 0; j < answer.Count; j++)
            {
                var methodGenericParameter = method.GenericParameters[j + initialLength];
                foreach (var constraint in typeGenericParameters[j < skipIndex ? j : j + 1].Constraints)
                    methodGenericParameter.Constraints.Add(constraint.Replace(answer, specialReplaceName, specialReplaceType));
            }
            return answer;
        }

        private static TypeReference ReplaceDefault(this IEnumerable<GenericParameter> methodGenericParameters, TypeReference constraint)
            => constraint.IsGenericParameter ? methodGenericParameters.SingleOrDefault(x => x.Name == constraint.Name) ?? constraint : constraint;

        private static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string specialName, TypeReference specialType)
            => Replace(constraint, methodGenericParameters, y => y.Name == specialName ? specialType : y.IsGenericParameter ? methodGenericParameters.SingleOrDefault(x => x.Name == y.Name) ?? y : y);

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters)
        {
            if (!(constraint is GenericInstanceType genericConstraint))
                return constraint.IsGenericParameter ? methodGenericParameters.SingleOrDefault(x => x.Name == constraint.Name) ?? constraint : constraint;
            var newConstraint = new GenericInstanceType(constraint.Resolve());
            foreach (var argument in genericConstraint.GenericArguments)
                newConstraint.GenericArguments.Add(argument.Replace(methodGenericParameters));
            return newConstraint;
        }

        static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, Func<TypeReference, TypeReference> replace)
        {
            if (!(constraint is GenericInstanceType genericConstraint))
                return replace(constraint);
            var newConstraint = new GenericInstanceType(constraint.Resolve());
            foreach (var argument in genericConstraint.GenericArguments)
                newConstraint.GenericArguments.Add(argument.Replace(methodGenericParameters, replace));
            return newConstraint;
        }

        public static TypeReference GetElementTypeOfCollectionType(this TypeReference @this)
        {
            var enumeratorType = @this.GetEnumeratorTypeOfCollectionType().Resolve();
            var propertyDefinitions = enumeratorType.Properties;
            var propertyDefinition = propertyDefinitions.Single(x => x.Name == "Current");
            switch (propertyDefinition.PropertyType)
            {
                case ByReferenceType byref:
                    return byref.ElementType;
                default:
                    return propertyDefinition.PropertyType;
            }
        }

        public static TypeReference GetEnumeratorTypeOfCollectionType(this TypeReference @this)
        {
            var methodDefinitions = @this.Resolve().Methods;
            var methodDefinition = methodDefinitions.Single(x => x.Name == "GetEnumerator" && x.Parameters.Count == 0);
            var enumeratorType = methodDefinition.ReturnType;
            return enumeratorType;
        }

        public static void Do(this ILProcessor processor, OpCode code)
            => processor.Append(Instruction.Create(code));

        public static void Do(this ILProcessor processor, OpCode code, VariableDefinition variable)
            => processor.Append(Instruction.Create(code, variable));

        public static void Call(this ILProcessor processor, MethodReference method)
            => processor.Append(Instruction.Create(OpCodes.Call, method));
    }
}
