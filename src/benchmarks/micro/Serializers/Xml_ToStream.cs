﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Filters;

namespace MicroBenchmarks.Serializers
{
    [GenericTypeArguments(typeof(LoginViewModel))]
    [GenericTypeArguments(typeof(Location))]
    [GenericTypeArguments(typeof(IndexViewModel))]
    [GenericTypeArguments(typeof(MyEventsListerViewModel))]
    [GenericTypeArguments(typeof(CollectionsOfPrimitives))]
    [GenericTypeArguments(typeof(XmlElement))]
    [GenericTypeArguments(typeof(SimpleStructWithProperties))]
    [GenericTypeArguments(typeof(ClassImplementingIXmlSerialiable))]
    [AotFilter("Currently not supported due to missing metadata.")]
    public class Xml_ToStream<T>
    {
        private T value;
        private XmlSerializer xmlSerializer;
        private DataContractSerializer dataContractSerializer;
        private MemoryStream memoryStream;

        [GlobalSetup]
        public void Setup()
        {
            value = DataGenerator.Generate<T>();
            memoryStream = new MemoryStream(capacity: short.MaxValue);
            xmlSerializer = new XmlSerializer(typeof(T));
            dataContractSerializer = new DataContractSerializer(typeof(T));
        }

        [BenchmarkCategory(Categories.Libraries, Categories.Runtime)]
        [Benchmark(Description = nameof(XmlSerializer))]
        public void XmlSerializer_()
        {
            memoryStream.Position = 0;
            xmlSerializer.Serialize(memoryStream, value);
        }

        [BenchmarkCategory(Categories.Libraries)]
        [Benchmark(Description = nameof(DataContractSerializer))]
        public void DataContractSerializer_()
        {
            memoryStream.Position = 0;
            dataContractSerializer.WriteObject(memoryStream, value);
        }

        // YAXSerializer is not included in the benchmarks because it does not allow to serialize to stream (only to file and string)

        [GlobalCleanup]
        public void Dispose() => memoryStream.Dispose();
    }
}
