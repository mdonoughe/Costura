using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public static class Extensions
{
    public static IEnumerable<string> NonEmpty(this IEnumerable<string> list)
    {
        return list.Select(x => x.Trim()).Where(x => x != string.Empty);
    }

    public static Collection<TypeReference> GetGenericInstanceArguments(this TypeReference type)
    {
        return ((GenericInstanceType)type).GenericArguments;
    }

    public static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] args)
    {
        var declaringType = self.DeclaringType;
        if (!(self.DeclaringType is GenericInstanceType))
            declaringType = self.DeclaringType.MakeGenericInstanceType(args);

        var reference = new MethodReference(
            self.Name,
            self.ReturnType,
            declaringType);

        reference.HasThis = self.HasThis;
        reference.ExplicitThis = self.ExplicitThis;
        reference.CallingConvention = self.CallingConvention;

        foreach (var parameter in self.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        foreach (var genericParam in self.GenericParameters)
        {
            reference.GenericParameters.Add(new GenericParameter(genericParam.Name, reference));
        }

        return reference;
    }

    public static void InsertBefore(this Collection<Instruction> instructions, int index, params Instruction[] newInstructions)
    {
        foreach (var item in newInstructions)
        {
            instructions.Insert(index, item);
            index++;
        }
    }

    public static byte[] FixedGetResourceData(this EmbeddedResource resource)
    {
        // There's a bug in Mono.Cecil so when you access a resources data
        // the stream is not reset after use.
        var data = resource.GetResourceData();
        resource.GetResourceStream().Position = 0;
        return data;
    }
}