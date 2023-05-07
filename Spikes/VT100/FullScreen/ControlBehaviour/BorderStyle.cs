using System;

namespace VT100.FullScreen.ControlBehaviour
{
    public enum BorderType
    {
        None,
        Normal
    }
    
    public class BorderBorderStyle : IBorderStyle
    {
        private IBorderStyle _impl;

        public BorderBorderStyle()
        {
            _impl = new StandardBorderStyleImpl();
        }

        public event EventHandler<StyleChanged> StyleChanged;

        public BorderType TopBorder
        {
            get => _impl.TopBorder;
            set => _impl.TopBorder = value;
        }

        public BorderType BottomBorder
        {
            get => _impl.BottomBorder;
            set => _impl.BottomBorder = value;
        }

        public BorderType LeftBorder
        {
            get => _impl.LeftBorder;
            set => _impl.LeftBorder = value;
        }

        public BorderType RightBorder
        {
            get => _impl.RightBorder;
            set => _impl.RightBorder = value;
        }

    }

    public interface IBorderStyle
    {
        event EventHandler<StyleChanged> StyleChanged;
        
        BorderType TopBorder { get; set; }
        BorderType BottomBorder { get; set; }
        BorderType LeftBorder { get; set; }
        BorderType RightBorder { get; set; }
        
    }

    public class StyleChanged
    {
    }

    internal class StandardBorderStyleImpl : IBorderStyle
    {
        #region Implementation of IStyle

        public event EventHandler<StyleChanged> StyleChanged;

        #region TopBorder

        private BorderType _topBorder;
        public BorderType TopBorder
        {
            get => _topBorder;
            set
            {
                _topBorder = value;
                RaiseChanged();
            }
        }

        #endregion

        #region BottomBorder

        private BorderType _bottomBorder;
        public BorderType BottomBorder
        {
            get => _bottomBorder;
            set
            {
                _bottomBorder = value;
                RaiseChanged();
            }
        }

        #endregion

        #region LeftBorder

        private BorderType _leftBorder;

        public BorderType LeftBorder
        {
            get => _leftBorder;
            set
            {
                _leftBorder = value;
                RaiseChanged();
            }
        }

        #endregion

        #region RightBorder

        private BorderType _rightBorder;

        public BorderType RightBorder
        {
            get => _rightBorder;
            set
            {
                _rightBorder = value;
                RaiseChanged();
            }
        }

        #endregion

        private void RaiseChanged()
        {
            StyleChanged?.Invoke(this, new StyleChanged());
        }

        #endregion
    }
}