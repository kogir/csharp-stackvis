namespace StackVis {
    using System;
    using System.IO;
    using System.Linq;

    public static class Program {
        static void Main(string[] args) {
            var sources = new[] {
                @"F:\20120515_105712_condensed_mem.txt",
                @"F:\20120515_105712_condensed_time.txt",
            };
            foreach (var source in sources) {
                var dest = string.Format(
                    "{0}_{1}.json",
                    source,
                    DateTime.UtcNow.ToString("yyyyMMdd_hhmmss"));

                Console.WriteLine("Processing {0}", source);
                using (var reader = new StreamReader(source))
                using (var writer = new StreamWriter(dest, false)) {
                    var input = new CollapsedStreamReader(reader, Console.Error);

                    // De-deupe just in case
                    //var stacks = input
                    //    .GroupBy(frame => frame.Key)
                    //    .Select((group) => {
                    //        var ret = group.First();
                    //        ret.Count = group.Sum(s => s.Count);
                    //        return ret;
                    //    }).ToList();

                    new IcicleWriter().Write(writer, input);
                }
                Console.WriteLine("Wrote {0}", dest);
            }
            Console.WriteLine("Done!");
        }
    }
}
