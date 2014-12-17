﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Text = Program.Title;
            lblProductName.Text = Program.Title;

            rtbShareXInfo.AddContextMenu();
            rtbCredits.AddContextMenu();

            uclUpdate.CheckUpdate(TaskHelpers.CheckUpdate);
        }

        private void AboutForm_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
            cLogo.Start(50);
        }

        private void lblProductName_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_VERSION_HISTORY);
        }

        private void pbBerkURL_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_BERK);
        }

        private void pbBerkSteamURL_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_BERK_STEAM);
        }

        private void pbMikeURL_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_MIKE);
        }

        private void pbMikeGooglePlus_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_MIKE_GOOGLE_PLUS);
        }

        private void rtb_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        #region Animation

        private const int w = 200;
        private const int h = w;
        private const int mX = w / 2;
        private const int mY = h / 2;
        private const int minStep = 3;
        private const int maxStep = 30;
        private const int speed = 1;
        private int step = 10;
        private int direction = speed;
        private Color lineColor = new HSB(0d, 1d, 0.9d);
        private bool isPaused;
        private int clickCount;

        private void cLogo_Draw(Graphics g)
        {
            g.SetHighQuality();

            using (Matrix m = new Matrix())
            {
                m.RotateAt(45, new PointF(mX, mY));
                g.Transform = m;
            }

            using (Pen pen = new Pen(lineColor, 2))
            {
                for (int i = 0; i <= mX; i += step)
                {
                    g.DrawLine(pen, i, mY, mX, mY + i); // Left top
                    g.DrawLine(pen, i, mY, mX, mY - i); // Left bottom
                    g.DrawLine(pen, w - i, mY, mX, mY - i); // Right top
                    g.DrawLine(pen, w - i, mY, mX, mY + i); // Right bottom
                }

                g.DrawLine(pen, mX, mY, mX, mY + mX); // Left top
                g.DrawLine(pen, mX, mY, mX, mY - mX); // Left bottom
                g.DrawLine(pen, w - mX, mY, mX, mY - mX); // Right top
                g.DrawLine(pen, w - mX, mY, mX, mY + mX); // Right bottom
            }

            if (!isPaused)
            {
                if (step + speed > maxStep)
                {
                    direction = -speed;
                }
                else if (step - speed < minStep)
                {
                    direction = speed;
                }

                step += direction;

                HSB hsb = lineColor;

                if (hsb.Hue >= 1)
                {
                    hsb.Hue = 0;
                }
                else
                {
                    hsb.Hue += 0.01;
                }

                lineColor = hsb;
            }
        }

        private void cLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isEasterEggStarted)
            {
                isPaused = !isPaused;

                clickCount++;

                if (clickCount >= 10)
                {
                    isEasterEggStarted = true;
                    cLogo.Stop();
                    RunEasterEgg();
                }
            }
            else
            {
                if (bounceTimer != null)
                {
                    bounceTimer.Stop();
                }

                isEasterEggStarted = false;
            }
        }

        #endregion Animation

        #region Easter egg

        private bool isEasterEggStarted;
        private Rectangle screenRect;
        private Timer bounceTimer;
        private const int windowGravityPower = 3;
        private const int windowBouncePower = -50;
        private const int windowSpeed = 20;
        private Point windowVelocity = new Point(windowSpeed, windowGravityPower);

        private void RunEasterEgg()
        {
            screenRect = CaptureHelpers.GetScreenWorkingArea();

            bounceTimer = new Timer();
            bounceTimer.Interval = 20;
            bounceTimer.Tick += bounceTimer_Tick;
            bounceTimer.Start();
        }

        private void bounceTimer_Tick(object sender, EventArgs e)
        {
            if (!IsDisposed)
            {
                int x = Left + windowVelocity.X;
                int windowRight = screenRect.X + screenRect.Width - 1 - Width;

                if (x <= screenRect.X)
                {
                    x = screenRect.X;
                    windowVelocity.X = windowSpeed;
                }
                else if (x >= windowRight)
                {
                    x = windowRight;
                    windowVelocity.X = -windowSpeed;
                }

                int y = Top + windowVelocity.Y;
                int windowBottom = screenRect.Y + screenRect.Height - 1 - Height;

                if (y >= windowBottom)
                {
                    y = windowBottom;
                    windowVelocity.Y = windowBouncePower.RandomAdd(-10, 10);
                }
                else
                {
                    windowVelocity.Y += windowGravityPower;
                }

                Location = new Point(x, y);
            }
        }

        #endregion Easter egg
    }
}