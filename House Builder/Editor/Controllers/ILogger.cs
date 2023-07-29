using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public interface ILogger
    {
        void Log(string owner, string message);

        void Error(string owner, string message);
    }

    public class Logger : ILogger
    {
        public void Log(string owner, string message)
        {
            Debug.Log($"<color=green><b>ModularBuilder</b></color>::{owner}: {message}");
        }

        public void Error(string owner, string message)
        {
            Debug.Log($"<color=red><b>ModularBuilder</b></color>::{owner}: {message}");
        }
    }
}