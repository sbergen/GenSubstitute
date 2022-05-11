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
            // TODO: ensure uniqueness!
            LocalVariableName = $"{parameter.Name}_local";

            var initializerValues = parameter.IsRef ? $"{parameter.Name}, isImmutable: false" : "";
            LocalVariableDeclaration = $"var {LocalVariableName} = new {parameter.WrappedType}({initializerValues});";
            ResultAssignment = $"{parameter.Name} = {LocalVariableName};";
        }
    }
}
