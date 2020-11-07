using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marble.Generator
{
    public class ClassFileFinder
    {
        private readonly string projectFile;

        public ClassFileFinder(string projectFile)
        {
            this.projectFile = projectFile;
        }

        public IEnumerable<string> Find()
        {
            var rootPath = new FileInfo(projectFile).Directory!.FullName;
            var fileList = new List<string>();
            index(rootPath, ref fileList);
            return fileList;
        }

        private void index(string path, ref List<string> list)
        {
            list.AddRange(Directory
                .GetFiles(path)
                .Where(filePath => filePath.EndsWith(".cs")));

            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                list.AddRange(Directory
                    .GetFiles(directoryPath)
                    .Where(filePath => filePath.EndsWith(".cs")));

                index(directoryPath, ref list);
            }
        }
    }
}