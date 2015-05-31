using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Windows.Forms;
using System.Threading;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FlashDebugger.Controls
{
    public partial class DataVisualizer : Form
    {
        private GLControl mControl;
        private float mAngle;

        public DataVisualizer(GLControl control)
        {
            mControl = control;
            mAngle = 0.0f;

            Application.Idle += Application_Idle;
        }

        #region Events

        public void HandleLoad(EventArgs e)
        {
            OnLoad(e);
        }

        public void Application_Idle(object sender, EventArgs e)
        {
            while (mControl.IsIdle)
            {
                Render();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Idle -= Application_Idle;

            base.OnClosing(e);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);

            Application.Idle += Application_Idle;

            // Ensure that the viewport and projection matrix are set correctly.
            Control_Resize(mControl, EventArgs.Empty);

        }

        public void Control_Paint(object sender, PaintEventArgs e)
        {
            Render();
        }

        private void Render()
        {
            int i;
            Matrix4 lookat = Matrix4.LookAt(0, 0, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float posX;
            float posY;
            float w = 0.2f;
            float h = 0.2f;
            int col = 100;
            int row = 100;
            Color color;

            Color[] colors = new Color[] { Color.Red, Color.Green, Color.Blue };

            Rectangle r = new Rectangle();

            GL.Begin(PrimitiveType.Quads);

            for (i = 0; i < 1024; ++i)
            {
                color = colors[i % colors.Length];
                posX = -12.0f + (float) (i % col) * (w + 0.1f);
                posY = (float) (i/row) * (h+0.1f);

                DrawQuad(posX, posY, w, h, color);
            }

            GL.End();

            mControl.SwapBuffers();
        }

        private void DrawQuad(float posX, float posY, float width, float height, Color color)
        {
            float width2 = width / 2.0f;
            float height2 = height / 2.0f;

            GL.Color3(color);

            GL.Vertex3(posX + -width2, posY + -height2, -1.0f);
            GL.Vertex3(posX + -width2, posY + height2, -1.0f);
            GL.Vertex3(posX + width2, posY + height2, -1.0f);
            GL.Vertex3(posX + width2, posY + -height2, -1.0f);
        }


        private void DrawCube()
        {
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(Color.Silver);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GL.Color3(Color.Honeydew);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GL.Color3(Color.Moccasin);

            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GL.Color3(Color.IndianRed);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GL.Color3(Color.PaleVioletRed);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GL.Color3(Color.ForestGreen);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();
        }


        public void Control_Resize(object sender, EventArgs e)
        {
            OpenTK.GLControl c = sender as OpenTK.GLControl;

            if (c.ClientSize.Height == 0)
                c.ClientSize = new System.Drawing.Size(c.ClientSize.Width, 1);

            GL.Viewport(0, 0, c.ClientSize.Width, c.ClientSize.Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perspective = Matrix4.CreateOrthographic(c.ClientSize.Width/16, c.ClientSize.Height/16, 0, 64);
            //Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        #endregion
    }
}
