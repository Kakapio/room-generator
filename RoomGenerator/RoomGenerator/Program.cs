using System;

namespace RoomGenerator
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new RoomGenerator())
                game.Run();
        }
    }
}
