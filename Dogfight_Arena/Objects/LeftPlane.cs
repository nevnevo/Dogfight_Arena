﻿using Dogfight_Arena.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Objects
{
    public class LeftPlane : Plane
    {
        public LeftPlane(double x, double y, string Image, Canvas field, int size) : base(x, y, Image, field, size)
        {

        }

        protected override void Move(VirtualKey key)
        {
            if (key == Keys.AccelerateLeftPlayer)
                base.Accelerate();
            if (key == Keys.DeccelerateleftPlayer)
                base.Decelerate();
            if (key == Keys.RotateClockWiseLeftPlayer)
                base.Turn(1);
            if (key == Keys.RotateCounterClockWiseLeftPlayer)
                base.Turn(-1);
        }

    }
}
