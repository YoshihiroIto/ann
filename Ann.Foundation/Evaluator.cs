using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Ann.Foundation
{
    // http://odetocode.com/articles/80.aspx を参考にしました
    public static class Evaluator
    {
        public static object Eval(string statement)
        {
            return EvaluatorType.InvokeMember(
                "Eval",
                BindingFlags.InvokeMethod,
                null,
                EvaluatorInstance,
                new object[] {statement}
            ).ToString();
        }

        static Evaluator()
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

            EvaluatorType = assembly.GetType("Evaluator.Evaluator");
            EvaluatorInstance = Activator.CreateInstance(EvaluatorType);
        }

        private static readonly Type EvaluatorType;
        private static readonly object EvaluatorInstance;
    }
}