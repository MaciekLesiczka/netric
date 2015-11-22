using System.Collections.Generic;
using Netric.Shared.Clr;

namespace Netric.EventConsumer.Flamegraphs
{
    public class ClrCallStackFolder
    {
        private struct StackItem
        {
            public string Value { get; set; }
            public int Level { get; set; }            
        }
        private readonly Stack<StackItem> _stack = new Stack<StackItem>();

        public string Fold(Method input)
        {
            var newItem = new StackItem
            {
                Level = input.MethodStats.StackLevel,                
            };
            string prefix = null;

            while (_stack.Count > 0)
            {
                var current = _stack.Peek();
                
                if (current.Level < newItem.Level)
                {
                    prefix = string.Format("{0};", current.Value);
                    break;
                }
                _stack.Pop();
            }
            newItem.Value = string.Format("{0}{1}", prefix, input.Name);

            _stack.Push(newItem);
            return string.Format("{0} {1}", newItem.Value, input.MethodStats.ElapsedExclusive/10);
        }
    }
}