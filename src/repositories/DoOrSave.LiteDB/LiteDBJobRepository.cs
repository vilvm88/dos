using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DoOrSave.Core;

using LiteDB;

namespace DoOrSave.LiteDB
{
    public sealed class LiteDBJobRepository : IJobRepository
    {
        private readonly string _connectionString;
        private IJobLogger _logger;

        public IJobLogger Logger { get; set; }

        public LiteDBJobRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("message", nameof(connectionString));

            _connectionString = connectionString;

            Init();
        }

        public void SetLogger(IJobLogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<Job> Get()
        {
            using (var db = new LiteDatabase(_connectionString))
            {
                var list = new List<Job>();

                foreach (var name in db.GetCollectionNames())
                {
                    var collection = db.GetCollection(name);

                    list.AddRange(collection.FindAll()
                        .Select(x => BsonMapper.Global.ToObject<Job>(x)));
                }

                return list;
            }
        }

        public TJob Get<TJob>(string jobName) where TJob : Job
        {
            if (string.IsNullOrWhiteSpace(jobName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(jobName));

            try
            {
                using (var db = new LiteDatabase(_connectionString))
                {
                    var fullName = typeof(TJob).FullName;

                    if (fullName == null)
                        return null;

                    var collection = db.GetCollection<TJob>(fullName.Replace(".", "_"));

                    return collection.FindOne(x => x.JobName == jobName);
                }
            }
            catch (Exception exception)
            {
                _logger?.Error(exception);

                throw;
            }
        }

        public void Insert(Job job)
        {
            if (job is null)
                throw new ArgumentNullException(nameof(job));

            try
            {
                using (var db = new LiteDatabase(_connectionString))
                {
                    var fullName = job.GetType().FullName;

                    if (fullName == null)
                        return;

                    var collection = db.GetCollection(fullName.Replace(".", "_"));

                    collection.Insert(BsonMapper.Global.ToDocument(job));
                }

                _logger?.Information($"Job has inserted to repository: {job}");
            }
            catch (Exception exception)
            {
                _logger?.Error(exception);

                throw;
            }
        }

        public void Remove(Job job)
        {
            if (job is null)
                throw new ArgumentNullException(nameof(job));

            try
            {
                using (var db = new LiteDatabase(_connectionString))
                {
                    var fullName = job.GetType().FullName;

                    if (fullName == null)
                        return;

                    var collection = db.GetCollection(fullName.Replace(".", "_"));

                    var bson = BsonMapper.Global.ToDocument(job);

                    collection.Delete(bson);
                }

                _logger?.Information($"Job has removed from repository: {job}");
            }
            catch (Exception exception)
            {
                _logger?.Error(exception);

                throw;
            }
        }

        public void Update(Job job)
        {
            if (job is null)
                throw new ArgumentNullException(nameof(job));

            try
            {
                using (var db = new LiteDatabase(_connectionString))
                {
                    string fullName = job.GetType().FullName;

                    if (fullName == null)
                        return;

                    var collection = db.GetCollection(fullName.Replace(".", "_"));

                    collection.Update(BsonMapper.Global.ToDocument(job));
                }

                _logger?.Information($"Job has updated in repository: {job}");
            }
            catch (Exception exception)
            {
                _logger?.Error(exception);

                throw;
            }
        }

        private void Init()
        {
            try
            {
                var directory = Directory.GetParent(_connectionString);

                if (!directory.Exists)
                    directory.Create();
            }
            catch (Exception exception)
            {
                _logger?.Error(exception);

                throw;
            }
        }
    }
}