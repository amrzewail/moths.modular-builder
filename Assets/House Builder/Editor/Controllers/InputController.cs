using HouseBuilder.Editor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HouseBuilder.Editor.Controllers
{
    public class InputController : IInputController
    {
        private class MouseClick
        {
            private ILogger _logger;
            private int _button = 0;

            private long _pressTimestampTicks;
            private long _lastClickTimestampTicks;

            private bool _isClickDown = false;
            private bool _isDraggable = false;
            private bool _isDoubleClick = false;

            private const long CLICK_TIME_TICKS = 200 * TimeSpan.TicksPerMillisecond;
            private const long DOUBLE_CLICK_TIME_TICKS = 400 * TimeSpan.TicksPerMillisecond;

            public MouseClick(int button, ILogger logger)
            {
                _button = button;
                _logger = logger;
            }

            public void Update(Event ev)
            {
                if (ev.button == _button && ev.type == EventType.MouseDown)
                {
                    _isClickDown = true;
                    _isDraggable = true;
                    _pressTimestampTicks = DateTime.UtcNow.Ticks;
                    _logger.Log(nameof(MouseClick), $"Mouse: {_button} is down.");
                }

                if (_isClickDown)
                {
                    if (ev.type == EventType.MouseDrag)
                    {
                        _isClickDown = false;
                        _logger.Log(nameof(MouseClick), $"Mouse: {_button} click cancelled for drag.");
                    }
                    if (DateTime.UtcNow.Ticks - _pressTimestampTicks > CLICK_TIME_TICKS)
                    {
                        _isClickDown = false;
                        _logger.Log(nameof(MouseClick), $"Mouse: {_button} click cancelled exceeded time.");
                    }
                }
            }

            public bool IsClick(Event ev)
            {
                long ticks = DateTime.UtcNow.Ticks;
                _isDoubleClick = false;

                if (_isClickDown && ticks - _pressTimestampTicks > CLICK_TIME_TICKS)
                {
                    _isClickDown = false;
                    _logger.Log(nameof(MouseClick), $"Mouse: {_button} click cancelled exceeded time.");
                }
                if (ev.button == _button && ev.type == EventType.MouseUp)
                {
                    _isDraggable = false;
                    if (_isClickDown)
                    {
                        _logger.Log(nameof(MouseClick), $"Mouse: {_button} is clicked, isClickActive:{_isClickDown} holdTime:{(ticks - _pressTimestampTicks) / TimeSpan.TicksPerMillisecond}");

                        _isClickDown = false;

                        if (ticks - _lastClickTimestampTicks < DOUBLE_CLICK_TIME_TICKS)
                        {
                            _logger.Log(nameof(MouseClick), $"Mouse: {_button} is double clicking, lastClickTime:{(ticks - _lastClickTimestampTicks) / TimeSpan.TicksPerMillisecond}");
                            _isDoubleClick = true;
                        }
                        else
                        {
                            _logger.Log(nameof(MouseClick), $"Mouse: {_button} not double clicking, lastClickTime:{(ticks - _lastClickTimestampTicks) / TimeSpan.TicksPerMillisecond}");
                        }

                        _lastClickTimestampTicks = ticks;

                        return true;
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

            public bool IsDoubleClick()
            {
                return _isDoubleClick;
            }

            public void Cancel()
            {
                _isClickDown = false;
                _isDraggable = false;
            }
        }

        private class KeyClick
        {
            private ILogger _logger;
            private KeyCode _downKey;
            private KeyCode _lastClickedKey;

            private long _pressTimestampTicks;
            private bool _isClickDown = false;
            private bool _isDoubleClick = false;

            private long _lastClickTimestampTicks;

            private const long CLICK_TIME_TICKS = 200 * TimeSpan.TicksPerMillisecond;
            private const long DOUBLE_CLICK_TIME_TICKS = 400 * TimeSpan.TicksPerMillisecond;

            public KeyClick(ILogger logger)
            {
                _logger = logger;
            }

            public void Update(Event ev)
            {

                if (ev.type == EventType.KeyDown && _downKey != ev.keyCode)
                {
                    _downKey = ev.keyCode;
                    _isClickDown = true;
                    _pressTimestampTicks = DateTime.UtcNow.Ticks;
                    _logger.Log(nameof(KeyClick), $"{ev.keyCode} is down.");
                    
                }
                if (_isClickDown)
                {
                    if (DateTime.UtcNow.Ticks - _pressTimestampTicks > CLICK_TIME_TICKS)
                    {
                        _isClickDown = false;
                        _logger.Log(nameof(KeyClick), $"{_downKey} click cancelled.");
                    }
                }
            }

            public bool IsClick(Event ev, KeyCode key)
            {
                long ticks = DateTime.UtcNow.Ticks;
                _isDoubleClick = false;

                if (_isClickDown)
                {
                    if (ticks - _pressTimestampTicks > CLICK_TIME_TICKS)
                    {
                        _isClickDown = false;
                        _logger.Log(nameof(KeyClick), $"{_downKey} click cancelled.");
                        _downKey = KeyCode.None;
                    }
                }
                if (ev.keyCode == _downKey && ev.type == EventType.KeyUp)
                {
                    bool istrue = false;
                    _logger.Log(nameof(KeyClick), $"{ev.keyCode} is up, isClickActive:{_isClickDown} holdTime:{(ticks - _pressTimestampTicks) / TimeSpan.TicksPerMillisecond}");
                    if (_downKey == key && _isClickDown)
                    {
                        istrue = true;

                        if (ticks - _lastClickTimestampTicks < DOUBLE_CLICK_TIME_TICKS && _lastClickedKey == _downKey)
                        {
                            _logger.Log(nameof(MouseClick), $"{ev.keyCode} is double clicking, lastClickTime:{(ticks - _lastClickTimestampTicks) / TimeSpan.TicksPerMillisecond}");
                            _isDoubleClick = true;
                        }
                        else
                        {
                            _logger.Log(nameof(MouseClick), $"{ev.keyCode} not double clicking, lastClickedKey:{_lastClickedKey} lastClickTime:{(ticks - _lastClickTimestampTicks) / TimeSpan.TicksPerMillisecond}");
                        }

                        _lastClickedKey = _downKey;
                        _lastClickTimestampTicks = ticks;
                    }

                    _isClickDown = false;
                    _downKey = KeyCode.None;

                    return istrue;
                }
                return false;
            }

            public bool IsDoubleClick()
            {
                return _isDoubleClick;
            }
        }


        private readonly ILogger _logger;

        private KeyClick _keyClick;
        private Dictionary<KeyCommand, MouseClick[]> _mouseClicks = new Dictionary<KeyCommand, MouseClick[]>();
        private Dictionary<InputContext, Keymap> _keymaps;
        private KeyCommand _lastCommand;

        private InputContext _context;

        public KeyCommand Command { get; private set; } = KeyCommand.None;

        public int ScrollWheel { get; private set; }
        public Vector2 MousePosition { get; private set; }
        public bool IsDragging => Event.current.type == EventType.MouseDrag;


        public InputController(ILogger logger)
        {
            _logger = logger;
            _keyClick = new KeyClick(_logger);

            LoadKeymaps();
        }

        public void LoadKeymaps()
        {
            var maps = Resources.LoadAll<Keymap>("HouseBuilder/");

            _keymaps = new Dictionary<InputContext, Keymap>();
            Dictionary<InputContext, int> currentPriorities = new Dictionary<InputContext, int>();
            for (int i = 0; i < maps.Length; i++)
            {
                if (!currentPriorities.ContainsKey(maps[i].context)) currentPriorities[maps[i].context] = -1;
                if (maps[i].priority > currentPriorities[maps[i].context])
                {
                    _keymaps[maps[i].context] = maps[i];
                    currentPriorities[maps[i].context] = maps[i].priority;
                }
            }

            _logger.Log(nameof(InputController), "Loaded keymaps.");
        }

        public void SetContext(InputContext context)
        {
            _context = context;
        }

        public Keymap GetKeymap(InputContext context)
        {
            if (_keymaps.ContainsKey(context))
            {
                return _keymaps[context];
            }
            return null;
        }

        public void Update()
        {
            Clear();

            MousePosition = Event.current.mousePosition;

            _keyClick.Update(Event.current);
            foreach(var mouse in _mouseClicks)
            {
                foreach (var click in mouse.Value) click.Update(Event.current);
            }
            
            if (!_keymaps.ContainsKey(_context)) return;

            var keymap = _keymaps[_context];
            foreach (var binding in keymap.bindings)
            {
                if (!_mouseClicks.ContainsKey(binding.Command))
                {
                    _mouseClicks[binding.Command] = new MouseClick[3];
                    _mouseClicks[binding.Command][0] = new MouseClick(0, _logger);
                    _mouseClicks[binding.Command][1] = new MouseClick(1, _logger);
                    _mouseClicks[binding.Command][2] = new MouseClick(2, _logger);
                }

                if (ProcessBinding(binding))
                {
                    Command = binding.Command;
                    if (_lastCommand != Command)
                    {
                        _logger.Log(nameof(InputController), $"Run command {Command}");
                        _lastCommand = Command;
                    }
                    break;
                }
            }
        }

        private bool ProcessBinding(KeyBinding binding)
        {
            if (binding.Behaviour == KeyBehaviour.Nothing) return false;

            bool isAlt = Event.current.alt == binding.Alt;
            bool isShift = Event.current.shift == binding.Shift;
            bool isCtrl = Event.current.control == binding.Ctrl;

            if (!isAlt || !isShift || !isCtrl)
            {
                _mouseClicks[binding.Command][0].Cancel();
                _mouseClicks[binding.Command][1].Cancel();
                _mouseClicks[binding.Command][2].Cancel();
                return false;
            }
            bool isKey = true;

            if (binding.Key != KeyCode.None)
            {
                isKey = binding.Key == Event.current.keyCode;
            }

            if (!isKey) return false;

            bool isMouseBtn = true;

            if (binding.Mouse != MouseButton.None)
            {
                isMouseBtn = (binding.Mouse == MouseButton.Left && Event.current.button == 0) || (binding.Mouse == MouseButton.Right && Event.current.button == 1);
            }

            if (!isMouseBtn) return false;

            bool isMouseDrag = false;
            bool isMouseClick = false;
            bool isMouseDoubleClick = false;

            if (binding.Mouse != MouseButton.None)
            {
                switch (Event.current.button)
                {
                    case 0: isMouseDrag = _mouseClicks[binding.Command][0].IsDrag(Event.current); break;
                    case 1: isMouseDrag = _mouseClicks[binding.Command][1].IsDrag(Event.current); break;
                }

                switch (Event.current.button)
                {
                    case 0: isMouseClick = _mouseClicks[binding.Command][0].IsClick(Event.current); break;
                    case 1: isMouseClick = _mouseClicks[binding.Command][1].IsClick(Event.current); break;
                }

                switch (Event.current.button)
                {
                    case 0: isMouseDoubleClick = _mouseClicks[binding.Command][0].IsDoubleClick(); break;
                    case 1: isMouseDoubleClick = _mouseClicks[binding.Command][1].IsDoubleClick(); break;
                }
            }

            bool isKeyClick = false;
            bool isKeyDoubleClick = false;

            if (binding.Key != KeyCode.None)
            {
                isKeyClick = _keyClick.IsClick(Event.current, binding.Key);
                isKeyDoubleClick = _keyClick.IsDoubleClick();
            }

            bool isBindingTrue = true;

            switch (binding.Behaviour)
            {
                case KeyBehaviour.Press:
                    if (binding.Key != KeyCode.None && binding.Mouse == MouseButton.None)
                    {
                        isBindingTrue = Event.current.type == EventType.KeyDown;
                    }else if (binding.Key == KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = Event.current.type == EventType.MouseDown;
                    }else if (binding.Key != KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = (Event.current.type == EventType.KeyDown) && (Event.current.type == EventType.MouseDown);
                    }
                    break;

                case KeyBehaviour.Release:

                    if (binding.Key != KeyCode.None && binding.Mouse == MouseButton.None)
                    {
                        isBindingTrue = Event.current.type == EventType.KeyUp;
                    }
                    else if (binding.Key == KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = Event.current.type == EventType.MouseUp;
                    }
                    else if (binding.Key != KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = (Event.current.type == EventType.KeyUp) && (Event.current.type == EventType.MouseUp);
                    }
                    break;

                case KeyBehaviour.Click:

                    //keyboard key only
                    if (binding.Key != KeyCode.None && binding.Mouse == MouseButton.None)
                    {
                        isBindingTrue = isKeyClick;
                    }
                    //mouse button only
                    else if (binding.Key == KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = isMouseClick;
                    }
                    //keyboard and mouse
                    else if (binding.Key != KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = isKeyClick && isMouseClick;
                    }
                    break;

                case KeyBehaviour.DoubleClick:

                    //keyboard key only
                    if (binding.Key != KeyCode.None && binding.Mouse == MouseButton.None)
                    {
                        isBindingTrue = isKeyDoubleClick;
                    }
                    //mouse button only
                    else if (binding.Key == KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = isMouseDoubleClick;
                    }
                    //keyboard and mouse
                    else if (binding.Key != KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = isKeyDoubleClick && isMouseDoubleClick;
                    }

                    break;

                case KeyBehaviour.ClickOrDrag:

                    //keyboard key only
                    if (binding.Key != KeyCode.None && binding.Mouse == MouseButton.None)
                    {
                        isBindingTrue = isKeyClick;
                    }
                    //mouse button only
                    else if (binding.Key == KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = isMouseClick || isMouseDrag;
                    }
                    //keyboard and mouse
                    else if (binding.Key != KeyCode.None && binding.Mouse != MouseButton.None)
                    {
                        isBindingTrue = isKeyClick && (isMouseClick || isMouseDrag);
                    }

                    break;

                case KeyBehaviour.ScrollWheel:
                    if (Event.current.isScrollWheel)
                    {
                        Vector2 delta = Event.current.delta;
                        ScrollWheel = -Mathf.RoundToInt(Mathf.Sign(delta.y));
                    }
                    else
                    {
                        isBindingTrue = false;
                    }
                    break;

            }

            if (isBindingTrue && binding.Consume)
            {
                Event.current.Use();
            }

            return isBindingTrue;

        }


        public void Clear()
        {
            Command = KeyCommand.None;
            ScrollWheel = 0;
        }

    }
}