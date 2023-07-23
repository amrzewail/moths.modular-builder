using HouseBuilder.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class InputController : IInputController
    {
        private class MouseClick
        {
            private int _button = 0;
            private bool _isClickDown = false;
            private bool _isDraggable = false;

            public MouseClick(int button)
            {
                _button = button;
            }

            public bool IsClick(Event ev)
            {
                if (ev.button == _button && ev.type == EventType.MouseUp)
                {
                    _isDraggable = false;
                    if (_isClickDown)
                    {
                        _isClickDown = false;
                        return true;
                    }
                }

                if (ev.button == _button && ev.type == EventType.MouseDown)
                {
                    _isClickDown = true;
                    _isDraggable = true;
                }

                if (_isClickDown)
                {
                    if (ev.type == EventType.MouseDrag)
                    {
                        _isClickDown = false;
                    }
                }
                return false;
            }

            public bool IsDrag(Event ev)
            {
                if (ev.type == EventType.MouseDrag && ev.button == _button)
                {
                    _isClickDown = false;
                    return _isDraggable;
                }
                return false;
            }

            public void Cancel()
            {
                _isClickDown = false;
                _isDraggable = false;
            }
        }


        private readonly ILogger _logger;

        private MouseClick _leftClickPlacement = new MouseClick(0);
        private MouseClick _leftClickHighlight = new MouseClick(0);
        private Keymap _keymap;

        private InputContext _context;

        public KeyCommand Command { get; private set; } = KeyCommand.None;

        public int ScrollWheel { get; private set; }
        public Vector2 MousePosition { get; private set; }
        public bool IsDragging => Event.current.type == EventType.MouseDrag;


        public InputController(ILogger logger)
        {
            _logger = logger;
            _keymap = Resources.LoadAll<Keymap>("HouseBuilder/")[0];
        }

        public void SetContext(InputContext context)
        {
            _context = context;
        }

        public void Update()
        {

        }


        public void UpdateFF()
        {
            Clear();

            //if (Event.current.alt) _leftClickHighlight.Cancel();
            //if (Event.current.alt || Event.current.control) _leftClickPlacement.Cancel();

            //MousePosition = Event.current.mousePosition;

            //if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E && Event.current.control && !Event.current.alt)
            //{
            //    Command = KeyCommand.Extrude;
            //    return;
            //}

            //if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyDown)
            //{
            //    Command = KeyCommand.UnselectPrefab;
            //    return;
            //}

            //if (Event.current.keyCode == KeyCode.F && Event.current.type == EventType.KeyDown)
            //{
            //    Command = KeyCommand.Frame;
            //    Event.current.Use();
            //    return;
            //}

            //if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && Event.current.control && Event.current.shift && !Event.current.alt)
            //{
            //    Command = KeyCommand.HighlightAll;
            //    return;
            //}


            //if (Event.current.alt && Event.current.type == EventType.ScrollWheel)
            //{
            //    Vector2 delta = Event.current.delta;
            //    ScrollWheel = Mathf.RoundToInt(Mathf.Sign(delta.y));
            //    Command = KeyCommand.Rotate;
            //    Event.current.Use();
            //    return;
            //}

            //if (Event.current.alt && Event.current.type == EventType.MouseDown && Event.current.button == 1)
            //{
            //    Command = KeyCommand.Flip;
            //    return;
            //}

            //if (Event.current.control && Event.current.type == EventType.ScrollWheel)
            //{
            //    Vector2 delta = Event.current.delta;
            //    ScrollWheel = -Mathf.RoundToInt(Mathf.Sign(delta.y));
            //    Command = KeyCommand.AdjustHeight;
            //    Event.current.Use();
            //    return;
            //}


            //if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Backspace)
            //{
            //    Command = KeyCommand.Delete;
            //    return;
            //}


            //if (Event.current.control && !Event.current.alt)
            //{
            //    if (_leftClickHighlight.IsClick(Event.current))
            //    {
            //        Command = KeyCommand.HighlightClick;
            //    }
            //    else if (_leftClickHighlight.IsDrag(Event.current))
            //    {
            //        Command = KeyCommand.HighlightDrag;
            //    }
            //    else
            //    {
            //        Command = KeyCommand.HidePreview;
            //    }
            //    return;
            //}

            //if (Event.current.alt == false && Event.current.control == false)
            //{
            //    if (_leftClickPlacement.IsClick(Event.current))
            //    {
            //        Command = KeyCommand.LeftMouseButtonUp;
            //    }
            //    else if (_leftClickPlacement.IsDrag(Event.current))
            //    {
            //        Command = KeyCommand.LeftMouseButtonUp;
            //    }
            //}

        }

        public void Clear()
        {
            Command = KeyCommand.None;
            ScrollWheel = 0;
        }

    }
}