using MainPart.ForGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MainPart.ForScript
{
    public class TestScripter
    {
        readonly private List<string> _namesOfFiles= new List<string>();
        readonly private string _writerPath = "";
        
        private TransformBlock<string, Tuple<string, string>> _readingBlock;
        private TransformBlock<Tuple<string, string>, Dictionary<string, string>> _generatingBlock;
        private ActionBlock<Dictionary<string, string>> _writingBlock;

        public Dictionary<string, string> GeneratedFiles { get; set; } = new Dictionary<string, string>();


        public TestScripter(List<string> namesOfFiles, string writerPath, int maxDegreeOfParallelismRead, int maxDegreeOfParallelismWrite, int maxDegreeOfParallelismTranformation)
        {
            _namesOfFiles = namesOfFiles;
            _writerPath = writerPath;
            
            _readingBlock = new TransformBlock<string, Tuple<string, string>>(async s =>
            {
                using (var reader = File.OpenText(s))
                {
                    var fileString = await reader.ReadToEndAsync();
                    return new Tuple<string, string >(fileString, s);
                }
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelismRead
            });

            _generatingBlock = new TransformBlock<Tuple<string, string>, Dictionary<string, string>>(turpleS => new TestGenerator(turpleS.Item1, turpleS.Item2.Replace(".cs", "")).GenerateFiles()
            ,
                 new ExecutionDataflowBlockOptions
                 {
                     MaxDegreeOfParallelism = maxDegreeOfParallelismTranformation
                 });

            _writingBlock = new ActionBlock<Dictionary<string, string>>(async dictS =>
            {
                
                foreach (var s in dictS.Keys)
                {
                    GeneratedFiles.Add(s, dictS[s]);
                    await using var writer = new StreamWriter(_writerPath + "\\" + s + ".cs");
                    await writer.WriteAsync(dictS[s]);
                }

            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelismWrite });

            _readingBlock.LinkTo(_generatingBlock, new DataflowLinkOptions { PropagateCompletion = true });
            _generatingBlock.LinkTo(_writingBlock, new DataflowLinkOptions { PropagateCompletion = true });

        }

        public async Task Generate()
        {
            foreach (var fileName in _namesOfFiles)
            {
                _readingBlock.Post(fileName);
            }

            _readingBlock.Complete();
            await _writingBlock.Completion;
        }
    }
}
