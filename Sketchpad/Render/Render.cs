﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Sketchpad.Settings;
using Sketchpad.Data;
using System;

namespace Sketchpad.Render
{
    static class Render
    {
        public static void PaintCanvas(PaintEventArgs e, CanvasData canvasData)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Color.White);
            
            DrawPolygon(e, canvasData.polygon.vertices);
            DrawBoundingBox(e, canvasData.polygon.vertices);
            DrawConstraintsIcons(e, canvasData);
        }
        public static void DrawPolygon(PaintEventArgs e, List<Point> polygon)
        {

            for (int i = 0; i < polygon.Count; i++)
            {
                DrawVertex(e, polygon[i]);
                MyDrawLine(e, new Pen(ProgramSettings.lineColor), polygon[i], polygon[(i + 1) % polygon.Count]);
            }
        }

        public static void DrawBoundingBox(PaintEventArgs e, List<Point> polygon)
        {
            Rectangle boundingBox = Polygon.GetBoundingBox(polygon);
            e.Graphics.DrawRectangle(new Pen(ProgramSettings.boundingBoxColor), boundingBox);

        }

        private static void DrawConstraintsIcons(PaintEventArgs e, CanvasData canvasData)
        {
            foreach(Constraint constraint in canvasData.constraints)
            {


                int point1 = constraint.constrainedEdges[0].Item1, point2 = constraint.constrainedEdges[0].Item2;
                int x1 = canvasData.polygon.vertices[point1].X, x2 = canvasData.polygon.vertices[point2].X,
                    y1 = canvasData.polygon.vertices[point1].Y, y2 = canvasData.polygon.vertices[point2].Y;
                Point drawPoint = new Point((x1 + x2) / 2, (y1 + y2) / 2);



                if (constraint.constraintMode == Utils.ConstraintMode.HorizontalEdge)
                    DrawIcon(e, new Point(drawPoint.X + 5, drawPoint.Y - 10), new Point(drawPoint.X + 15, drawPoint.Y - 10));
                if (constraint.constraintMode == Utils.ConstraintMode.VerticalEdge)
                    DrawIcon(e, new Point(drawPoint.X + 10, drawPoint.Y - 5), new Point(drawPoint.X + 10, drawPoint.Y + 5));
                if (constraint.constraintMode == Utils.ConstraintMode.FixedAngle)
                {
                    DrawIcon(e, new Point(x1, y1 - 15), new Point(x1, y1 - 5));
                    DrawIcon(e, new Point(x1 - 5, y1 - 10), new Point(x1 + 5, y1 - 10));
                }

            }
        }
        
        private static void DrawIcon(PaintEventArgs e, Point startPoint, Point endPoint)
        {
            MyDrawLine(e, new Pen(ProgramSettings.iconColor), startPoint, endPoint);
        }

        public static void DrawVertex(PaintEventArgs e, Point vertexCoordinates)
        {
            Rectangle vertex = new Rectangle(vertexCoordinates, ProgramSettings.vertexSize);
            e.Graphics.FillRectangle(ProgramSettings.vertexColor, vertex);
        }

        public static void MyDrawLine(PaintEventArgs e, Pen pen,Point p1, Point p2)
        {

            line(e, p1.X, p1.Y, p2.X, p2.Y, pen.Brush);
            //graphics.DrawLine(pen, p1, p2);
        }


        public static void line(PaintEventArgs e, int x, int y, int x2, int y2, Brush brush)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                e.Graphics.FillRectangle(brush, x, y, 1, 1);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }
    }
}
