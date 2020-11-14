﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marble.Generator.Workers
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
            var rootPath = new FileInfo(this.projectFile).Directory!.FullName;
            var fileList = new List<string>();
            this.index(rootPath, ref fileList);
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

                this.index(directoryPath, ref list);
            }
        }
    }
}