﻿/* Copyright 2016-present MongoDB Inc.
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
using SAEA.Mongo.Bson;
using SAEA.Mongo.Driver.Core.Misc;

namespace SAEA.Mongo.Driver.GridFS
{
    /// <summary>
    /// Represents options for a GridFS Find operation.
    /// </summary>
    public class GridFSFindOptions : GridFSFindOptions<ObjectId>
    {
        // fields
        private SortDefinition<GridFSFileInfo> _sort;

        // properties
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public new SortDefinition<GridFSFileInfo> Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }
    }
}