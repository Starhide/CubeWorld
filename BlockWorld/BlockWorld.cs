using BlockWorld.render;
using BlockWorld.world;
using BlockWorld.world.entities;
using OTKUtilities;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using QuickFont;
using System;
using System.Drawing;

namespace BlockWorld
{
    internal class BlockWorld : ManagedGameWindow
    {

        public static TextureAtlas Atlas;
        private Point oldMousePosition;
        public bool Pause = false;
        public bool Wireframe = false;
        public World World;

        private QFont droidSans;
        private QFontDrawing fontDrawer;

        public BlockWorld(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {

            World = new World(128, 16, 128);
            Player player = new Player(World, Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(45.0), 1.0f * Width / Height, 0.1f, 300.0f),
                     new Vector3(10, 200, 10), 0, 0);
            World.Player = player;

            CursorVisible = false;

            KeyDown += new EventHandler<KeyboardKeyEventArgs>(KeyDownAction);

            droidSans = new QFont("assets/fonts/DroidSansMono.ttf", 18, new QuickFont.Configuration.QFontBuilderConfiguration(false));
            fontDrawer = new QFontDrawing();

            GL.ClearColor(0.2f, 0.4f, 1.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line); // Wire Frame Mode
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point); // Point Mode
            //GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill); // Default Fill Mode

            World.PreLoad();

            Atlas = new TextureAtlas(32, 32, 16);

            Atlas.AddImage("starhide.dirt", "assets/dirt.png");
            Atlas.AddImage("starhide.grass", "assets/grass.png");



            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            

            if (Wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            World.Render(e.Time);

            if (Wireframe)
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            fontDrawer.ProjectionMatrix = Matrix4.CreateOrthographic(Width, Height, -1.0f, 1.0f);
            fontDrawer.DrawingPrimitives.Clear();
            fontDrawer.Print(droidSans,
                $"<X = {World.Player.Position.X}, Y = {World.Player.Position.Y}, Z = {World.Player.Position.Z}>\n" +
                $"FPS: {Math.Floor(1 / e.Time)}\n" +
                $"Chunk: {World.Player.ChunkPosition}",
                new Vector3(-900, 500, 0), QFontAlignment.Left);
            fontDrawer.Print(droidSans,
                World.ChunkManager.GetDebugString(),
                new Vector3(600, 500, 0), QFontAlignment.Left);
            fontDrawer.RefreshBuffers();
            fontDrawer.Draw();
            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (!Pause)
            {
                float s = 1.34f;
                if (input.IsKeyDown(Key.LShift))
                {
                    s = 40.0f;
                    World.Player.UseGravity = false;
                }
                else
                    World.Player.UseGravity = true;


                if (input.IsKeyDown(Key.LControl))
                    s = 5.364f;

                if (input.IsKeyDown(Key.W))
                    World.Player.MoveBy(World, World.Player.WForward * (float)(s * e.Time));

                if (input.IsKeyDown(Key.S))
                    World.Player.MoveBy(World, -World.Player.WForward * (float)(s * e.Time));

                if (input.IsKeyDown(Key.D))
                    World.Player.MoveBy(World, World.Player.WRight * (float)(s * e.Time));

                if (input.IsKeyDown(Key.A))
                    World.Player.MoveBy(World, -World.Player.WRight * (float)(s * e.Time));


                int deltaX = Mouse.GetState().X - oldMousePosition.X;
                int deltaY = Mouse.GetState().Y - oldMousePosition.Y;

                World.Player.Yaw += (float)deltaX / Width * 90;
                World.Player.Pitch -= (float)deltaY / Height * 90;

                Mouse.SetPosition(Location.X + Width / 2, Location.Y + Height / 2);
                oldMousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                World.Update((float)e.Time);

            }

            base.OnUpdateFrame(e);
        }

        private void KeyDownAction(object o, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                World.Player.Velocity += new Vector3(0.0f, 4.9f, 0.0f);
            }
            if (e.Key == Key.P)
            {
                if (Pause)
                {
                    Pause = false;
                    CursorVisible = false;
                }
                else
                {
                    Pause = true;
                    CursorVisible = true;
                }
            }
            if (e.Key == Key.O)
            {
                Wireframe = !Wireframe;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        //Now, for cleanup. This isn't technically necessary since C# will clean up all resources automatically when the program closes, but it's very
        //important to know how anyway.
        protected override void OnUnload(EventArgs e)
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            base.OnUnload(e);
        }

    }
}
