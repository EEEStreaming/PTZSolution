using System;

namespace PTZPadController.BusinessLayer
{
    enum AxisPosition
    {
        NEGATIVE_FAR,
        NEGATIVE_MEDIUM,
        NEGATIVE_CLOSE,
        CENTER,
        POSITIVE_CLOSE,
        POSITIVE_MEDIUM,
        POSITIVE_FAR
    }

    class JoystickStateMachine
    {
        // Change these values if you like to change the detection zones (maybe these could be put in config)
        const double POSITIVE_FAR = 0.9;
        const double POSITIVE_MEDIUM = 0.8;
        const double POSITIVE_CLOSE = 0.575;
        
        const double NEGATIVE_CLOSE = 0.425;
        const double NEGATIVE_MEDIUM = 0.2;
        const double NEGATIVE_FAR = 0.1;

        public (AxisPosition, AxisPosition) CurrentState { get; private set; }

        public JoystickStateMachine()
        {
            CurrentState = (AxisPosition.CENTER, AxisPosition.CENTER);
        }

        public (AxisPosition, AxisPosition) GetNext(double x, double y, bool inverse_y)
        {
            // Inverse Y Axis if needed
            double y_converted = y;
            if (!inverse_y)
                y_converted = 1 - y;
            AxisPosition x_axis = ConvertValueToState(x);
            AxisPosition y_axis = ConvertValueToState(y_converted);
            return (x_axis, y_axis);
        }

        public (AxisPosition, AxisPosition) MoveNext(double x, double y, bool inverse_y)
        {
            CurrentState = GetNext(x, y, inverse_y);
            return CurrentState;
        }

        private AxisPosition ConvertValueToState(double value)
        {
            var result = value switch
            {
                double v when v <= NEGATIVE_FAR => AxisPosition.NEGATIVE_FAR,
                double v when v <= NEGATIVE_MEDIUM => AxisPosition.NEGATIVE_MEDIUM,
                double v when v <= NEGATIVE_CLOSE => AxisPosition.NEGATIVE_CLOSE,
                double v when v <= POSITIVE_CLOSE => AxisPosition.CENTER,
                double v when v <= POSITIVE_MEDIUM => AxisPosition.POSITIVE_CLOSE,
                double v when v <= POSITIVE_FAR => AxisPosition.POSITIVE_MEDIUM,
                double v when v > POSITIVE_FAR => AxisPosition.POSITIVE_FAR,
                _ => AxisPosition.CENTER,
            };
            return result;
        }
    }
}
