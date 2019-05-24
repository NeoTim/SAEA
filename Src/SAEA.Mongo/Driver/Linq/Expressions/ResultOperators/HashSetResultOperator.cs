﻿/* Copyright 2015-present MongoDB Inc.
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
using SAEA.Mongo.Bson.Serialization;
using SAEA.Mongo.Driver.Core.Misc;

namespace SAEA.Mongo.Driver.Linq.Expressions.ResultOperators
{
    internal sealed class HashSetResultOperator : ResultOperator
    {
        private readonly IBsonSerializer _serializer;
        private readonly Type _type;

        public HashSetResultOperator(Type type, IBsonSerializer serializer)
        {
            _type = Ensure.IsNotNull(type, nameof(type));
            _serializer = Ensure.IsNotNull(serializer, nameof(serializer));
        }

        public override string Name
        {
            get { return "HashSet"; }
        }

        public override IBsonSerializer Serializer
        {
            get { return _serializer; }
        }

        public override Type Type
        {
            get { return _type; }
        }
    }
}
