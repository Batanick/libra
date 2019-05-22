using System;
using System.Collections.Generic;
using libra.core;

namespace libraTest.libretto
{
    public class ArrayResource : Resource
    {
        public List<int> IntList { get; set; }
        public List<ResourcePart> PartList { get; set; }
        public List<ResourceRef<BaseTypesResource>> ResourceList { get; set; }

        public float[] FloatArray { get; set; }
        public ResourcePart[] PartArray { get; set; }
        public ResourceRef<BaseTypesResource>[] ResourceArray { get; set; }
    }
}