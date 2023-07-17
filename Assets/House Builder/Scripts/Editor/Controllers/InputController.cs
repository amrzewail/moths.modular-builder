
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class InputController : IInputController
    {
        private readonly ILogger _logger;
        private bool _isLeftClickDown = false;

        public KeyCommand Command { get; private set; } = KeyCommand.None;

        public int ScrollWheel { get; private set; }

        public InputController(ILogger logger)
        {
            _logger = logger;
        }

        public void Update()
        {


            if (Event.current.keyCode == KeyCode.F && Event.current.type == EventType.KeyDown)
            {
                Command = KeyCommand.Frame;
                Event.current.Use();
                return;
            }


            if (Event.current.alt && Event.current.type == EventType.ScrollWheel)
            {
                Vector2 delta = Event.current.delta;
                ScrollWheel = Mathf.RoundToInt(Mathf.Sign(delta.y));
                Command = KeyCommand.Rotate;
                Event.current.Use();
                return;
            }

            if (Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Command = KeyCommand.Flip;
                return;
            }

            if (Event.current.control && Event.current.type == EventType.ScrollWheel)
            {
                Vector2 delta = Event.current.delta;
                ScrollWheel = -Mathf.RoundToInt(Mathf.Sign(delta.y));
                Command = KeyCommand.ChangeHeight;
                Event.current.Use();
                return;
            }

            if (Event.current.shift && !Event.current.alt && !Event.current.control)
            {
                Command = KeyCommand.PrepareDelete;

                if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDrag)
                {
                    if (Event.current.button == 0)
                    {
                        Command = KeyCommand.Delete;
                    }
                }


                return;
            }

            if (Event.current.alt == false && Event.current.control == false)
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                {
                    _isLeftClickDown = true;
                }
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDrag)
                {
                    if (_isLeftClickDown)
                    {
                        Command = KeyCommand.Instantiate;
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0) _isLeftClickDown = false;
            if (Event.current.alt || Event.current.control) _isLeftClickDown = false;

        }

        public void Clear()
        {
            Command = KeyCommand.None;
            ScrollWheel = 0;
        }

    }
}