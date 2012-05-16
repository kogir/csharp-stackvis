namespace StackVis {
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    public class CollapsedStreamReader : IEnumerable<Stack> {
        public long LineNumber { get; private set; }

        private static readonly Regex _format = new Regex(@"^(.*)\s+(\d+)$", RegexOptions.Compiled | RegexOptions.ECMAScript);
        private TextReader _in;
        private TextWriter _log;

        public CollapsedStreamReader(TextReader input, TextWriter log) {
            _in = input;
            _log = log;
        }

        public IEnumerator<Stack> GetEnumerator() {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private IEnumerable<Stack> Enumerate() {
            string line;
            while ((line = _in.ReadLine()) != null) {
                ++LineNumber;
                var match = _format.Match(line);
                if (!match.Success) {
                    _log.WriteLine("line {0}: garbled", LineNumber);
                    continue;
                }

                var stack = match.Groups[1].Value.Split(',');
                var count = long.Parse(match.Groups[2].Value);
                yield return new Stack(stack, count);
            }
        }
    }
}
