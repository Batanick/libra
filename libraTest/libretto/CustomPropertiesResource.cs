using libra.core;
using libra.core.attribs;

namespace libraTest.libretto
{
    public class CustomPropertiesResource : Resource
    {
        public int JustProperty { get; set; }
        [Ignore] public float IgnoredProperty { get; set; }
        [Title("CustomTitle")] public bool CustomTitleProperty { get; set; }
    }
}