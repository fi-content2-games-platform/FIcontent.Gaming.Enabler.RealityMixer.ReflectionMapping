using UnityEngine;
using System.Collections;

/// <summary>
/// Screen debug allows lines and rectangles to be drawn in screen space.
/// Doesn't seem to work in the editor but works on Android device (currently untested on iOS)
/// </summary>
public class ScreenDebug{
    
    static Texture2D lineTex;
    
    static void GenSinglePixelTexture()
    {
        lineTex = new Texture2D(1, 1);
        lineTex.SetPixel(0,0,Color.white);
        lineTex.Apply();
    }
 
    public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
    public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
    public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
    public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        pointA = ScreenToGUIPoint(pointA);
        pointB = ScreenToGUIPoint(pointB);
        
        // Save the current GUI matrix, since we're going to make changes to it.
        Matrix4x4 matrix = GUI.matrix;
 
        // Generate a single pixel texture if it doesn't exist
        if (!lineTex)
        {
            GenSinglePixelTexture();
        }
 
        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        Color savedColor = GUI.color;
        GUI.color = color;
 
        // Determine the angle of the line.
        float angle = Vector3.Angle(pointB - pointA, Vector2.right);
 
        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (pointA.y > pointB.y) { angle = -angle; }
 
        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
 
        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, pointA);
 
        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);
 
        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
    
    static Vector2 ScreenToGUIPoint(Vector2 point)
    {
        return new Vector2((int)point.x,(int)(Screen.height - point.y));
    }
    
    
    public static void DrawRectangle(Rect rect, float width)
    {
        Vector2 a = new Vector2((int)rect.xMin,(int)rect.yMin);
        Vector2 b = new Vector2((int)rect.xMin,(int)rect.yMax);
        Vector2 c = new Vector2((int)rect.xMax,(int)rect.yMin);
        Vector2 d = new Vector2((int)rect.xMax,(int)rect.yMax);
        
        DrawLine(a,b,width);
        DrawLine(b,d,width);
        DrawLine(d,c,width);
        DrawLine(c,a,width);
    }

}
