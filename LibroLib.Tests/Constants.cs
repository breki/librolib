namespace LibroLib.Tests
{
    public static class Constants
    {
#if NCRUNCH
        public const string DataPath = @"D:\hg\Maperitive\current\Data";
        public const string DataSamplesPath = @"D:\hg\Maperitive\current\Data/Samples/";
#else
        public const string DataPath = "../../Data";
        public const string DataSamplesPath = "../../Data/Samples/";
#endif
    }
}
