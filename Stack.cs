namespace StackVis {
    public class Stack {
        public string[] Stacks;
        public long Count;
        public string Key;

        public Stack(string[] stack, long count) {
            Stacks = stack;
            Count = count;
            Key = string.Join(",", stack);
        }
    }
}
