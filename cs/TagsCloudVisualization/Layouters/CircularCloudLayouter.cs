﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualization.Extensions;

namespace TagsCloudVisualization.Layouters
{
    public class CircularCloudLayouter : ILayouter
    {
        private readonly Point center;
        private readonly CircularPositioner positioner;
        private readonly List<Rectangle> rectangles = new List<Rectangle>();

        public CircularCloudLayouter(Point center) : this(center, 0)
        {
        }

        public CircularCloudLayouter(Point center,
            double startRadius,
            double searchAngleStep = Math.PI / 2,
            double iterationOffsetAngle = Math.PI / 180)
        {
            this.center = center;
            positioner = new CircularPositioner(
                center,
                startRadius,
                searchAngleStep,
                iterationOffsetAngle);
        }

        public Rectangle PutNextRectangle(Size size)
        {
            if (size.Width <= 0 || size.Height <= 0)
                throw new ArgumentException($"Some side was negative in size: {size.Width}x{size.Height}");

            positioner.RadiusStep = LinearMath.GetDiagonal(size) / 2;
            var bestPoint = new Point(int.MaxValue, int.MaxValue);
            var bestDistance = double.PositiveInfinity;
            foreach (var point in positioner.Iteration(point => CanBePlaced(new Rectangle(point, size))))
            {
                var pointDistance = center.DistanceBetween(point.CenterWith(size));
                if (pointDistance > bestDistance)
                    continue;

                bestPoint = point;
                bestDistance = pointDistance;
            }

            var rectangle = new Rectangle(bestPoint, size);
            rectangles.Add(rectangle);
            return rectangle;
        }

        private bool CanBePlaced(Rectangle targetRectangle)
        {
            // TODO: Not best algorithm
            return rectangles.All(rectangle => !targetRectangle.IntersectsWith(rectangle));
        }
    }
}
