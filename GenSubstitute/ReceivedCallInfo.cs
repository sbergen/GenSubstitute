using System.Linq;

namespace GenSubstitute
{
    public class ReceivedCallInfo
    {
        private readonly string _methodName;
        private readonly IReceivedCall _call;

        public ReceivedCallInfo(string methodName, IReceivedCall call)
        {
            _methodName = methodName;
            _call = call;
        }

        public override string ToString()
        {
            var args = string.Join(", ", _call
                .GetArguments()
                .Select(a => a?.ToString() ?? "null"));
            return $"{_call.ReturnType} {_methodName}({args})";
        }
            
    }
}
