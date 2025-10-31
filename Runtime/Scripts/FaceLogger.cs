using UnityEngine;

namespace FaceLib
{
    public static class FaceLogger
    {
        public static void Log(string message)
        {
            Debug.Log($"[FaceLib] {message}");
        }
    }
}


