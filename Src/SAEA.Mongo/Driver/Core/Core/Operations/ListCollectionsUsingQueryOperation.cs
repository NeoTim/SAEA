﻿/* Copyright 2013-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SAEA.Mongo.Bson;
using SAEA.Mongo.Bson.Serialization.Serializers;
using SAEA.Mongo.Driver.Core.Bindings;
using SAEA.Mongo.Driver.Core.Events;
using SAEA.Mongo.Driver.Core.Misc;
using SAEA.Mongo.Driver.Core.WireProtocol.Messages.Encoders;

namespace SAEA.Mongo.Driver.Core.Operations
{
    /// <summary>
    /// Represents a list collections operation.
    /// </summary>
    public class ListCollectionsUsingQueryOperation : IReadOperation<IAsyncCursor<BsonDocument>>, IExecutableInRetryableReadContext<IAsyncCursor<BsonDocument>>
    {
        // fields
        private BsonDocument _filter;
        private readonly DatabaseNamespace _databaseNamespace;
        private readonly MessageEncoderSettings _messageEncoderSettings;
        private bool _retryRequested;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListCollectionsOperation"/> class.
        /// </summary>
        /// <param name="databaseNamespace">The database namespace.</param>
        /// <param name="messageEncoderSettings">The message encoder settings.</param>
        public ListCollectionsUsingQueryOperation(
            DatabaseNamespace databaseNamespace,
            MessageEncoderSettings messageEncoderSettings)
        {
            _databaseNamespace = Ensure.IsNotNull(databaseNamespace, nameof(databaseNamespace));
            _messageEncoderSettings = Ensure.IsNotNull(messageEncoderSettings, nameof(messageEncoderSettings));
        }

        // properties
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public BsonDocument Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        /// <summary>
        /// Gets the database namespace.
        /// </summary>
        /// <value>
        /// The database namespace.
        /// </value>
        public DatabaseNamespace DatabaseNamespace
        {
            get { return _databaseNamespace; }
        }

        /// <summary>
        /// Gets the message encoder settings.
        /// </summary>
        /// <value>
        /// The message encoder settings.
        /// </value>
        public MessageEncoderSettings MessageEncoderSettings
        {
            get { return _messageEncoderSettings; }
        }

        /// <summary>
        /// Gets or sets whether or not retry was requested.
        /// </summary>
        /// <value>
        /// Whether retry was requested.
        /// </value>
        public bool RetryRequested
        {
            get { return _retryRequested; }
            set { _retryRequested = value; }
        }

        // public methods
        /// <inheritdoc/>
        public IAsyncCursor<BsonDocument> Execute(IReadBinding binding, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(binding, nameof(binding));

            using (var context = RetryableReadContext.Create(binding, retryRequested: false, cancellationToken))
            {
                return Execute(context, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public IAsyncCursor<BsonDocument> Execute(RetryableReadContext context, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(context, nameof(context));

            using (EventContext.BeginOperation())
            {
                var operation = CreateOperation();
                var cursor = operation.Execute(context, cancellationToken);
                return new BatchTransformingAsyncCursor<BsonDocument, BsonDocument>(cursor, NormalizeQueryResponse);
            }
        }

        /// <inheritdoc/>
        public async Task<IAsyncCursor<BsonDocument>> ExecuteAsync(IReadBinding binding, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(binding, nameof(binding));

            using (var context = await RetryableReadContext.CreateAsync(binding, retryRequested: false, cancellationToken).ConfigureAwait(false))
            {
                return await ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<IAsyncCursor<BsonDocument>> ExecuteAsync(RetryableReadContext context, CancellationToken cancellationToken)
        {
            Ensure.IsNotNull(context, nameof(context));

            using (EventContext.BeginOperation())
            {
                var operation = CreateOperation();
                var cursor = await operation.ExecuteAsync(context, cancellationToken).ConfigureAwait(false);
                return new BatchTransformingAsyncCursor<BsonDocument, BsonDocument>(cursor, NormalizeQueryResponse);
            }
        }

        // private methods
        private FindOperation<BsonDocument> CreateOperation()
        {
            // if the filter includes a comparison to the "name" we must convert the value to a full namespace
            var filter = _filter;
            if (filter != null && filter.Contains("name"))
            {
                var value = filter["name"];
                if (!value.IsString)
                {
                    throw new NotSupportedException("Name filter must be a plain string when connected to a server version less than 2.8.");
                }
                filter = (BsonDocument)filter.Clone(); // shallow clone
                filter["name"] = _databaseNamespace.DatabaseName + "." + value;
            }

            return new FindOperation<BsonDocument>(_databaseNamespace.SystemNamespacesCollection, BsonDocumentSerializer.Instance, _messageEncoderSettings)
            {
                Filter = filter,
                RetryRequested = _retryRequested
            };
        }

        private IEnumerable<BsonDocument> NormalizeQueryResponse(IEnumerable<BsonDocument> collections)
        {
            var prefix = _databaseNamespace + ".";
            foreach (var collection in collections)
            {
                var name = (string)collection["name"];
                if (name.StartsWith(prefix))
                {
                    var collectionName = name.Substring(prefix.Length);
                    if (!collectionName.Contains('$'))
                    {
                        collection["name"] = collectionName; // replace the full namespace with just the collection name
                        yield return collection;
                    }
                }
            }
        }
    }
}