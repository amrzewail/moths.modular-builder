
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class InputController : IInputController
    {
        private readonly ILogger _logger;
        private bool _isLeftClickDown = false;
        private bool _isLeftClickHighlightDown = false;

        public KeyCommand Command { get; private set; } = KeyCommand.None;

        public int ScrollWheel { get; private set; }
        public Vector2 MousePosition { get; private set; }

        public InputController(ILogger logger)
        {
            _logger = logger;
        }

        public void Update()
        {
            Clear();

            MousePosition = Event.current.mousePosition;

            if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyDown)
            {
                Command = KeyCommand.UnselectedPrefab;
                return;
            }

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


            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Backspace)
            {
                Command = KeyCommand.Delete;
                return;
            }

            //if (Event.current.control && !Event.current.alt && !Event.current.control)
            //{
            //    Command = KeyCommand.PrepareDelete;

            //    if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDrag)
            //    {
            //        if (Event.current.button == 0)
            //        {
            //            Command = KeyCommand.Delete;
            //        }
            //    }


            //    return;
            //}

            if (Event.current.control && !Event.current.alt)
            {
                if (Event.current.button == 0)
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        _isLeftClickHighlightDown = true;
                    }

                    if (_isLeftClickHighlightDown)
                    {
                        if (Event.current.type == EventType.MouseUp)
                        {
                            Command = KeyCommand.HighlightClick;
                            return;
                        }
                        else if (Event.current.type == EventType.MouseDrag)
                        {
                            Command = KeyCommand.HighlightDrag;
                            return;
                        }
                    }
                }


                Command = KeyCommand.PrepareHighlight;
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
                        Command = KeyCommand.LeftMouseButtonUp;
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _isLeftClickDown = false;
                _isLeftClickHighlightDown = false;
            } 
            if (Event.current.alt || Event.current.control) _isLeftClickDown = false;
            if (Event.current.alt) _isLeftClickHighlightDown = false;

        }

        public void Clear()
        {
            Command = KeyCommand.None;
            ScrollWheel = 0;
        }

    }
}