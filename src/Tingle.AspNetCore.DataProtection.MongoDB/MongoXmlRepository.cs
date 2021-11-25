using Microsoft.AspNetCore.DataProtection.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Tingle.AspNetCore.DataProtection.MongoDB;

/// <summary>
/// An <see cref="IXmlRepository"/> which is backed by Mongo.
/// </summary>
public class MongoXmlRepository : IXmlRepository
{
    private readonly Func<IMongoCollection<DataProtectionKey>> _collectionFactory;

    /// <summary>
    /// Creates a <see cref="MongoXmlRepository"/> with keys stored at the given collection.
    /// </summary>
    /// <param name="databaseFactory">The delegate used to create <see cref="IMongoCollection{TDocument}"/> instances.</param>
    public MongoXmlRepository(Func<IMongoCollection<DataProtectionKey>> databaseFactory)
    {
        _collectionFactory = databaseFactory;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        var collection = _collectionFactory();
        return collection.Find(Builders<DataProtectionKey>.Filter.Empty)
                         .ToList()
                         .Select(key => XElement.Parse(key.Xml))
                         .ToList()
                         .AsReadOnly();
    }

    /// <inheritdoc />
    public void StoreElement(XElement element, string friendlyName)
    {
        var newKey = new DataProtectionKey()
        {
            FriendlyName = friendlyName,
            Xml = element.ToString(SaveOptions.DisableFormatting)
        };

        var collection = _collectionFactory();
        collection.InsertOne(newKey);
    }
}
