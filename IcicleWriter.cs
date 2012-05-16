namespace StackVis {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class IcicleWriter {
        private class TreeElement {
            public long Total;
            public Dictionary<string, TreeElement> Children = new Dictionary<string, TreeElement>();
        }

        public void Write(TextWriter writer, IEnumerable<Stack> stacks) {
            var root = new TreeElement();

            var filtered = stacks
                .OrderByDescending(s => s.Count);

            foreach (var stack in filtered) {
                AddStacks(root, stack, 0);
            }

            var total = root.Total;
            var normMax = (double)(int.MaxValue / 2);
            var scaleFactor = Math.Min(normMax / total, 1);

            var lowerBound = total * 0.0001;

            RescaleAndTrim(root, scaleFactor, lowerBound);

            using (var jWriter = new JsonTextWriter(writer)) {
                jWriter.Formatting = Formatting.None;
                //jWriter.Formatting = Formatting.Indented;
                WriteTree(root, jWriter);
            }
        }

        private void RescaleAndTrim(TreeElement root, double scaleFactor, double lowerBound) {
            root.Total = (long)(root.Total * scaleFactor);

            if (root.Total == 0) {
                Console.WriteLine("Zero encountered.");
                root.Total = 1;
            }

            var toRemove = root.Children
                .Where(c => c.Value.Total < lowerBound)
                .Select(c => c.Key)
                .ToArray();

            foreach (var remove in toRemove) {
                root.Children.Remove(remove);
            }

            foreach (var child in root.Children) {
                RescaleAndTrim(child.Value, scaleFactor, lowerBound);
            }
        }

        private void WriteTree(TreeElement root, JsonTextWriter writer) {
            writer.WriteStartObject();
            writer.WritePropertyName("All");
            WriteFrame(root, writer);
            writer.WriteEnd();
        }

        private void WriteChildren(Dictionary<string, TreeElement> children, JsonTextWriter writer) {
            writer.WriteStartObject();
            foreach (var child in children) {
                writer.WritePropertyName(child.Key);
                WriteFrame(child.Value, writer);
            }
            writer.WriteEnd();
        }

        private void WriteFrame(TreeElement frame, JsonTextWriter writer) {
            writer.WriteStartObject();
            writer.WritePropertyName("T");
            writer.WriteValue(frame.Total);
            writer.WritePropertyName("C");
            WriteChildren(frame.Children, writer);
            writer.WriteEnd();
        }

        private void AddStacks(TreeElement root, Stack stack, int depth) {
            root.Total += stack.Count;

            var limit = stack.Stacks.Length;
            //var limit = Math.Min(stack.Stacks.Length, 4);
            if (depth < limit) {
                var frame = stack.Stacks[depth];
                if (!root.Children.ContainsKey(frame)) {
                    root.Children.Add(frame, new TreeElement());
                }

                AddStacks(root.Children[frame], stack, depth + 1);
            }
        }
    }
}
