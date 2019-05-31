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

namespace SAEA.Mongo.Driver
{
    /// <summary>
    /// Represents the document validation action.
    /// </summary>
    public enum DocumentValidationAction
    {
        /// <summary>
        /// Validation failures result in an error.
        /// </summary>
        Error = 0, // the default
        /// <summary>
        /// Validation failures result in a warning.
        /// </summary>
        Warn
    }
}