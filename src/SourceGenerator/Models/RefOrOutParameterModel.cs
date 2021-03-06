using GenSubstitute.SourceGenerator.Utilities;

namespace GenSubstitute.SourceGenerator.Models
{
    internal readonly struct RefOrOutParameterModel
    {
        public readonly string LocalVariableName;
        
        // E.g. var someArg_wrapped = new RefArg<int>(someArg);
        public readonly string LocalVariableDeclaration;

        // E:g. var someArg = someArg_wrapped;
        public readonly string ResultAssignment; 

        public RefOrOutParameterModel(EnrichedParameterModel parameter)
        {
            LocalVariableName = InternalName.Make(parameter.Name);

            var initializerValue = parameter.IsRef ? parameter.Name : "";
            LocalVariableDeclaration =
                $"var {LocalVariableName} = {parameter.CallObjectType}.CreateForExecutionContext({initializerValue});";
            
            ResultAssignment = $"{parameter.Name} = {LocalVariableName};";
        }
    }
}
