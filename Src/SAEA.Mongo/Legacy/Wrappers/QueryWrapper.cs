/* Copyright 2010-present MongoDB Inc.
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

using SAEA.Mongo.Bson.Serialization;
using SAEA.Mongo.Bson.Serialization.Attributes;
using SAEA.Mongo.Bson.Serialization.Serializers;

namespace SAEA.Mongo.Driver.Wrappers
{
    /// <summary>
    /// Represents a wrapped object that can be used where an IMongoQuery is expected (the wrapped object is expected to serialize properly).
    /// </summary>
    [BsonSerializer(typeof(QueryWrapper.Serializer))]
    public class QueryWrapper : BaseWrapper, IMongoQuery
    {
        // constructors
        /// <summary>
        /// Initializes a new instance of the QueryWrapper class.
        /// </summary>
        /// <param name="query">The wrapped object.</param>
        public QueryWrapper(object query)
            : base(query)
        {
        }

        // public static methods
        /// <summary>
        /// Creates a new instance of the QueryWrapper class.
        /// </summary>
        /// <param name="query">The wrapped object.</param>
        /// <returns>A new instance of QueryWrapper or null.</returns>
        public static QueryWrapper Create(object query)
        {
            if (query == null)
            {
                return null;
            }
            else
            {
                return new QueryWrapper(query);
            }
        }

        // nested classes
        new internal class Serializer : SerializerBase<QueryWrapper>
        {
            public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, QueryWrapper value)
            {
                value.SerializeWrappedObject(context);
            }
        }
    }
}
