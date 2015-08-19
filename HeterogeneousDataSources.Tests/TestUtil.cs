using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.Tests
{
    public static class TestUtil
    {
        public static readonly ReferenceTypeConfig<Image, string> ImageReferenceTypeConfig = new ReferenceTypeConfig<Image, string>(
            reference => reference.Id,
            ids => new ImageRepository().GetByIds(ids)
            );

        public static readonly ReferenceTypeConfig<Person, int> PersonReferenceTypeConfig = new ReferenceTypeConfig<Person, int>(
            reference => reference.Id,
            ids => new PersonRepository().GetByIds(ids)
        );

        public static readonly List<IReferenceTypeConfig> ReferenceTypeConfigs = new List<IReferenceTypeConfig>
        {
            ImageReferenceTypeConfig,
            PersonReferenceTypeConfig
        };
    }
}