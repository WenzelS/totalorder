﻿// some .cs file included in your project
using System.Runtime.CompilerServices;
using to.contracts.data.domain;

[assembly: InternalsVisibleTo("to.backlogrepo.test")]

namespace to.backlogrepo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using to.contracts;
    using Newtonsoft.Json;

    public class BacklogRepo : IBacklogRepo
    {
        private readonly string rootpath;

        private readonly Func<int, int> rnd;
        private readonly Func<Guid> guidGenerator;

        public BacklogRepo()
        {
            this.rootpath = Environment.CurrentDirectory;
            var x = new Random();
            this.rnd = g => x.Next(0, g);
            this.guidGenerator = Guid.NewGuid;
        }

        public BacklogRepo(string rootpath)
        {
            this.rootpath = rootpath;
            var x = new Random();
            this.rnd = g => x.Next(0, g);
            this.guidGenerator = Guid.NewGuid;
        }

        internal BacklogRepo(string rootpath, Func<int, int> rnd)
        {
            this.rootpath = rootpath;
            this.rnd = rnd;
        }

        internal BacklogRepo(string rootpath, Func<Guid> guidGenerator)
        {
            this.rootpath = rootpath;
            this.guidGenerator = guidGenerator;
        }

        public string CreateBacklog(Backlog backlog)
        {
            var id = GenerateBacklogId();
            SaveBacklog(backlog, id);
            return id;
        }

        internal void SaveBacklog(Backlog backlog, string id)
        {
            backlog.Id = id;
            var jsonString = JsonConvert.SerializeObject(backlog);
            var backlogDirectory = CreateBacklogDirectory(id);
            File.WriteAllText(Path.Combine(backlogDirectory, "Backlog.json"), jsonString);
        }

        private string CreateBacklogDirectory(string id)
        {
            var backlogPath = Path.Combine(this.rootpath, id);
            Directory.CreateDirectory(backlogPath);
            return backlogPath;
        }

        internal string GenerateBacklogId()
        {
            var backlogId = new StringBuilder();
            string fullPath;

            do
            {
                for (var i = 0; i < 3; i++)
                {
                    backlogId.Append((char) (this.rnd(25) + 65));
                }

                for (var i = 0; i < 3; i++)
                {
                    backlogId.Append(this.rnd(9));
                }

                fullPath = Path.Combine(this.rootpath, backlogId.ToString());
            }
            while (Directory.Exists(fullPath));

            Directory.CreateDirectory(fullPath);

            return backlogId.ToString();
        }

        public Submission[] ReadSubmissions(string id)
        {
            var filePaths = Directory.GetFiles(Path.Combine(this.rootpath, id));
            var tempList = new List<Submission>();

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                if (!fileName.StartsWith("submission", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var jsonString = File.ReadAllText(filePath);
                var currentSubmission = JsonConvert.DeserializeObject<Submission>(jsonString);
                tempList.Add(currentSubmission);
            }

            return tempList.ToArray();
        }

        public Backlog ReadBacklog(string id)
        {
            var backlogDirectory = Path.Combine(this.rootpath, id);
            var jsonString = File.ReadAllText(Path.Combine(backlogDirectory,"Backlog.json"));
            return JsonConvert.DeserializeObject<Backlog>(jsonString);
        }

        public void WriteSubmission(string id, Submission submission)
        {
            var submissionPath = Path.Combine(this.rootpath, id);
            var fileName = new StringBuilder();
            fileName.Append("Submission-");
            fileName.Append(guidGenerator().ToString());
            fileName.Append(".json");
            var jsonString = JsonConvert.SerializeObject(submission);
            File.WriteAllText(Path.Combine(submissionPath, fileName.ToString()), jsonString);
        }

        public List<Backlog> GetAll()
        {
            var backlogs = new List<Backlog>();

            var backlogDirs = Directory.GetDirectories(this.rootpath);
            foreach (var backlogDir in backlogDirs)
            {
                var jsonString = File.ReadAllText(Path.Combine(backlogDir, "Backlog.json"));
                backlogs.Add(JsonConvert.DeserializeObject<Backlog>(jsonString));
            }

            return backlogs;
        }

        public void DeleteBacklog(string id)
        {
            string path = Path.Combine(this.rootpath, id);
            Directory.Delete(path, true);
        }
    }
}
