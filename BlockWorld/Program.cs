namespace BlockWorld
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (BlockWorld gw = new BlockWorld(800, 600, "Test"))
            {
                gw.Run(60.0);
            }
        }
    }
}
