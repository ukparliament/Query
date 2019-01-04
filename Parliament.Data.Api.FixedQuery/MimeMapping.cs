// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.Collections.Generic;
    using VDS.RDF;


    public class MimeMapping
    {
        public IEnumerable<string> MimeTypes { get; set; }

        public IEnumerable<string> Extensions { get; set; }

        public Func<IRdfWriter> RdfWriter { get; set; }

        public Func<IStoreWriter> StoreWriter { get; set; }

        public Func<ISparqlResultsWriter> SparqlWriter { get; set; }

        public bool CanWriteRdf
        {
            get
            {
                return this.RdfWriter != null;
            }
        }

        public bool CanWriteStore
        {
            get
            {
                return this.StoreWriter != null;
            }
        }

        public bool CanWriteSparql
        {
            get
            {
                return this.SparqlWriter != null;
            }
        }
    }
}