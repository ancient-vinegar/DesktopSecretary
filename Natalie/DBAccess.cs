using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DKSY.Natalie
{
    class DBAccess
    {
        string databaseName;
        MongoClient mongoDB;
        IMongoDatabase dataPad;
        CancellationTokenSource _cancelToken;

        public DBAccess(string connectionString, string databaseName)
        {
            mongoDB = new MongoClient(connectionString);
            dataPad = mongoDB.GetDatabase(databaseName);
            _cancelToken = new CancellationTokenSource();
        }

        public void InsertDocument(string collection, BsonDocument document)
        {
            IMongoCollection<BsonDocument> doc = dataPad.GetCollection<BsonDocument>(collection, new MongoCollectionSettings() { AssignIdOnInsert = true });
            doc.InsertOne(document);
        }

        public async void InsertDocumentAsync(string collection, BsonDocument document)
        {
            IMongoCollection<BsonDocument> doc = dataPad.GetCollection<BsonDocument>(collection, new MongoCollectionSettings() { AssignIdOnInsert = true });
            await doc.InsertOneAsync(document, null, _cancelToken.Token);
        }
    }
}
