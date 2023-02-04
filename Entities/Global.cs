namespace WinISOEditor.Entities
{
    internal class Global
    {
        public static int PID { get; set; } = 1;
        public static Ps? ps { get; set; }
        public static string Argument { get; set; } = string.Empty;
        public static bool RedirectStandardInput { get; set; } = true;
        public static bool RedirectStandardOutput { get; set; } = false;
        public static bool RedirectStandardError { get; set; } = false;
    }
}
