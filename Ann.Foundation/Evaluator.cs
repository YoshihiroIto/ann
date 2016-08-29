using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Ann.Foundation
{
    // http://odetocode.com/articles/80.aspx を参考にしました
    public class Evaluator
    {
        public string Eval(string statement)
        {
            try
            {
                return _EvaluatorType.InvokeMember(
                    "Eval",
                    BindingFlags.InvokeMethod,
                    null,
                    _EvaluatorInstance,
                    new object[] {statement}
                ).ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public Evaluator()
        {
            const string source =
                @"package Evaluator
                {
                   class Evaluator
                   {
                      public function Eval(expr : String) : String 
                      { 
                         return eval(expr); 
                      }
                   }
                }";

            var provider = CodeDomProvider.CreateProvider("JScript");
            var parameters = new CompilerParameters {GenerateInMemory = true};
            var results = provider.CompileAssemblyFromSource(parameters, source);
            var assembly = results.CompiledAssembly;

            _EvaluatorType = assembly.GetType("Evaluator.Evaluator");
            _EvaluatorInstance = Activator.CreateInstance(_EvaluatorType);
        }

        private readonly Type _EvaluatorType;
        private readonly object _EvaluatorInstance;
    }
}